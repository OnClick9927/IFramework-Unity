# 对象池

IFramework 提供：

- `ObjectPool<T>`：可自定义创建、取出、归还和清理钩子。
- `SimpleObjectPool<T>`：对 `class, new()` 的默认实现。
- `StaticPool`：按泛型类型维护全局池。
- `ArrayPool<T>`：按数组精确长度复用。

这些池主要面向 Unity 主线程，不提供通用并发保证。

## ObjectPool<T>

```csharp
public sealed class BulletDataPool : ObjectPool<BulletData>
{
    protected override BulletData CreateNew() => new BulletData();

    protected override void OnCreate(BulletData value)
    {
        // 只在新建实例时执行。
    }

    protected override void OnGet(BulletData value)
    {
        // 每次 Get 后执行。
    }

    protected override bool OnSet(BulletData value)
    {
        value.Reset();
        return true; // false 表示拒绝进入池。
    }

    protected override void OnClear(BulletData value)
    {
        // Clear 移除池中对象时执行。
    }
}
```

使用：

```csharp
var pool = new BulletDataPool();
var value = pool.Get();

bool returned = pool.Set(value);
int available = pool.count;

pool.Clear();
```

## IPoolObject

对象实现 `IPoolObject` 后，ObjectPool 会额外管理状态：

```csharp
public sealed class Command : IPoolObject
{
    public bool valid { get; set; }

    public void OnGet()
    {
        // valid 已被设为 true。
    }

    public void OnSet()
    {
        // valid 已被设为 false。
        Reset();
    }
}
```

调用顺序：

```text
新对象: CreateNew -> OnCreate -> IPoolObject.OnGet -> ObjectPool.OnGet
复用对象: Dequeue -> IPoolObject.OnGet -> ObjectPool.OnGet
归还: ObjectPool.OnSet -> valid=false -> IPoolObject.OnSet -> Enqueue
```

`Set` 返回 `false` 的情况：

- 对象为 `null`。
- 同一对象已经在池中。
- `OnSet` 返回 `false`。

## Clear 和 IDisposable

`ObjectPool.Clear()` 对池中每个对象：

1. 调用 `OnClear`。
2. 如果对象实现 `IDisposable`，调用 `Dispose()`。

已借出的对象不在 Queue 中，不会被 Clear。业务必须跟踪并归还或自行释放。

## SimpleObjectPool<T>

```csharp
var pool = new SimpleObjectPool<List<int>>();
var list = pool.Get();
list.Clear();

pool.Set(list);
```

它用 `new T()` 创建对象，不会自动清理 T 的业务字段。归还前应自行 Reset/Clear，或改用自定义 ObjectPool。

`ISimpleObjectPool.SetObject(object)` 用于不知道泛型参数的运行时归还。类型不匹配时记录错误；`null` 不会进入池。

## StaticPool

### 普通对象

```csharp
var list = StaticPool.Get<List<string>>();
list.Clear();
try
{
    list.Add("A");
}
finally
{
    list.Clear();
    StaticPool.Set(list);
}
```

要求 `T : class, new()`。

### 按真实类型归还

当变量类型是基类或接口时：

```csharp
BaseUndoRecord record = GetRecord();
StaticPool.SetByRealType(record);
```

只有该具体类型的 StaticPool 已经初始化并存在于内部映射时才会归还。传 `null` 会直接忽略。

### IDisposable 包装

```csharp
using (var pooled = StaticPool.CreateDisposable<List<int>>())
{
    pooled.value.Clear();
    pooled.value.Add(1);
}
```

数组：

```csharp
using (var pooled = StaticPool.CreateDisposableArray<byte>(1024))
{
    byte[] buffer = pooled.value;
}
```

包装是 struct。避免复制后多次 Dispose，否则可能重复归还。

## ArrayPool<T>

```csharp
object[] array = StaticPool.GetArray<object>(64);
try
{
    array[0] = someObject;
}
finally
{
    StaticPool.Set(array);
}
```

### 精确长度

ArrayPool 只复用 `Length` 完全一致的数组，不会返回更大的桶：

```csharp
var pool = new ArrayPool<int>();
pool.SetLength(16);
int[] a = pool.Get(); // Length == 16
pool.Set(a);
```

`SetLength` 传负数会抛出 `ArgumentOutOfRangeException`。

### 扫描策略

实现用并行 Queue 保存数组和长度。Get 最多单次扫描当前可用数组，找到第一个精确长度后返回；跳过的项被顺序移到队尾。相较于先 `Contains` 再重新搬运，避免重复遍历和额外中转 Queue。

如果长度种类很多、池很大，精确长度扫描仍是 O(n)。对性能敏感的系统建议：

- 统一常用数组长度。
- 使用 2 的幂或固定协议长度。
- 避免把大量一次性特殊长度数组混入全局池。

### 引用清理

归还数组时，如果 `T` 是引用类型或包含引用的 struct，框架调用：

```csharp
Array.Clear(array, 0, array.Length);
```

这避免池长期持有业务对象。纯值类型数组不清理，以降低 CPU 成本。

### Clear

`ArrayPool.Clear()` 同时清空对象 Queue 和长度 Queue。两者必须同步清理，否则后续租借会读取错误的长度元数据。

## GameObject 和 Widget 不用 StaticPool 直接替代

UnityEngine.Object 有销毁、层级、激活和资源引用语义：

- GameObject 使用 [GameObjectPool 服务](9-pref-gameobject-pool.md)。
- UI Widget 使用 [WidgetPool](11-ui-runtime.md)。
- `StaticPool` 适合纯 C# wrapper、View 类和临时集合。

## 性能建议

### 值得池化

- 高频、短命且初始化成本明显的对象。
- UI Item wrapper 和 GameObject。
- 已知固定长度的临时数组。
- `List<T>`、`HashSet<T>` 等临时集合，但归还前必须 Clear。

### 不值得池化

- 很少创建的小对象。
- 生命周期长、数量稳定的服务。
- 容易忘记 Reset 的复杂状态对象。
- 归还成本高于重新创建的对象。

### 防止脏状态

每个池对象定义明确 Reset 协议：

```csharp
public void OnSet()
{
    callback = null;
    owner = null;
    items.Clear();
}
```

不要只依赖下次 `OnGet` 清理，因为归还后的引用会一直保存在池中。

## 常见错误

### 重复归还

`Set` 会返回 `false`。不要忽略所有返回值；在开发期可以断言：

```csharp
Log.A(pool.Set(value), "Duplicate pool return");
```

### 归还 null

框架返回 `false` 或忽略，不会加入池。

### 忘记 Clear 集合

SimpleObjectPool 不知道业务语义。`List<T>`、`Dictionary<TKey,TValue>` 必须手动清空。

### Clear 后仍使用旧引用

Clear 只清理池中对象。调用方继续持有的对象可能已 Dispose，不应继续使用。

### 跨线程访问

内部 Queue、Dictionary 不加锁。需要后台线程池时在业务层提供独立的并发实现。
