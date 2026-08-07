# 服务容器与依赖注入

## 服务容器解决什么问题

`Game` 内部持有一个 `ServiceCollection`，用于：

- 按接口注册服务。
- 支持同接口多个命名实例。
- 分离“注册”和“进入”阶段。
- 在退出时按逆序清理。
- 将服务同步注册到 ValueService，供依赖注入使用。

## 核心接口

### IServiceProvider

```csharp
public interface IServiceProvider
{
    IReadOnlyList<IService> GetServices<T>() where T : class, IService;
    T GetService<T>(string name = "") where T : class, IService;
    IServiceProvider EnterService<T>(string name = "") where T : class, IService;
}
```

### IServiceCollection

在 Provider 基础上增加注册：

```csharp
T Use<T>(T service, string name = "") where T : class, IService;
```

### IService

服务有名称，并接收 Use、Enter、Quit 生命周期。业务实现建议继承 `ServiceBase`，不要直接实现接口中的 internal 生命周期成员。

## 定义服务

```csharp
public interface IInventoryService : IService
{
    int GetCount(int itemId);
}

public sealed class InventoryService : ServiceBase, IInventoryService
{
    private readonly Dictionary<int, int> items = new();

    public int GetCount(int itemId) =>
        items.TryGetValue(itemId, out var count) ? count : 0;

    protected override void OnUse(IServiceCollection services)
    {
        // 此时服务已加入容器。
    }

    protected override void OnEnter(IServiceCollection services)
    {
        // ServiceBase 已先对 this 执行 ValueService.Inject。
    }

    protected override void OnQuit(IServiceCollection services)
    {
        items.Clear();
    }
}
```

## 注册、进入与获取

```csharp
protected override void Startup()
{
    this.UseValues();

    this.Use<IInventoryService>(new InventoryService());
    this.EnterService<IInventoryService>();

    var inventory = this.GetService<IInventoryService>();
}
```

需要记录缺失错误时：

```csharp
var inventory = this.GetRequiredService<IInventoryService>();
```

`GetRequiredService` 记录错误后仍返回 `null`，不是抛异常。调用方仍应保证注册顺序或判空。

## 多实例与名称

```csharp
this.Use<IUndoService>(battleUndo, "battle");
this.Use<IUndoService>(editorUndo, "editor");

IUndoService battle = this.GetService<IUndoService>("battle");
IReadOnlyList<IService> all = this.GetServices<IUndoService>();
```

同一接口下名称必须唯一。重复名称会记录错误，第二个服务不会进入该接口的映射。

空名称查询返回第一个注册服务。

## ValueService

启用：

```csharp
this.UseValues();
```

获取：

```csharp
IValueService values = this.Values();
```

ValueService 提供三类注册：

- 实例：按基类型和名称保存对象。
- 类型映射：需要时通过无参构造延迟创建。
- 工厂：首次按类型和名称获取时调用自定义工厂。

## 注册实例

```csharp
values.Register<IInventoryService>(inventory);
values.Register<IInventoryService>(secondaryInventory, "secondary");
```

底层形式：

```csharp
values.Register(typeof(IInventoryService), inventory, "");
```

获取：

```csharp
var inventory = values.Get<IInventoryService>();
var secondary = values.Get<IInventoryService>("secondary");
```

当服务容器执行：

```csharp
services.Use<IInventoryService>(service, name);
```

且 ValueService 已注册时，容器会自动把 service 按 `IInventoryService + name` 注册进 ValueService。

因此 `UseValues()` 必须早于需要自动注册的服务。

## 类型映射和延迟创建

```csharp
values.Register<IClock, SystemClock>();
IClock clock = values.Get(typeof(IClock), "") as IClock;
```

扩展方法为同类型注册：

```csharp
values.RegisterType<PlayerRepository>();
var repository = values.Get<PlayerRepository>();
```

延迟创建要求实现有公开无参构造。创建后 ValueService 会对新对象执行 `Inject`。

自定义工厂可以接收当前 ValueService：

```csharp
values.Register<IClock>(container =>
    new NetworkClock(container.Get<INetworkTime>()));
```

类型映射和工厂以请求类型为键。首次 `Get(type, name)` 创建实例后，会按该次请求的名称缓存，因此同一工厂可为不同名称各创建一个实例；工厂参数本身不包含请求名称。创建结果随后执行字段注入。传入 `null` 工厂会抛出 `ArgumentNullException`。

## 字段注入

目标必须实现标记接口：

```csharp
public sealed class BattleController : IInjectAble
{
    [Inject] private IInventoryService inventory;
    [Inject("battle")] private IUndoService undo;
}
```

执行：

```csharp
values.Inject(controller);
```

框架扫描实例的 public/private 字段，并缓存每个目标类型的注入元数据。

字段会被忽略的情况：

- 目标未实现 `IInjectAble`。
- 字段没有 `[Inject]`。
- 字段是 `readonly`。
- 字段类型是值类型。

找不到值时记录字段类型、字段名和注入名称，不会抛异常。

## 自动注入点

以下位置会自动注入：

- `ServiceBase.OnEnter` 之前注入服务自身。
- `MvcService.OnEnter` 注入 Model 和 Ctrl。
- `GameStateService.OnEnter` 注入每个 State。
- `GameObjectView` 构造时通过 `Game.Current.Values().Inject(this)` 注入。

最后一项意味着创建 View 前必须已有有效 `Game.Current` 和 ValueService。

## 初始化模板

```csharp
protected override void Startup()
{
    // 1. 基础容器
    this.UseValues();

    // 2. 注册全部服务
    this.Use<IInventoryService>(new InventoryService());
    this.Use<IQuestService>(new QuestService());
    this.UseUndo();
    this.UseRedTree();

    // 3. 进入需要显式初始化的服务
    this.EnterService<IInventoryService>();
    this.EnterService<IQuestService>();

    // 4. 注入 Game 或业务根对象
    this.Values().Inject(this);

    // 5. 开始状态/UI/场景流程
}
```

## 自定义服务依赖

```csharp
public sealed class QuestService : ServiceBase, IQuestService, IInjectAble
{
    [Inject] private IInventoryService inventory;

    protected override void OnEnter(IServiceCollection services)
    {
        // inventory 已在 ServiceBase 中注入。
    }
}
```

接口服务已经通过 `Use` 自动进入 ValueService 时，也可以不加名称直接注入。

## 退出

服务按 Use 的逆序退出。ValueService 的 `OnQuit` 会清空实例映射和类型映射；类型字段元数据是静态缓存，会跨服务实例保留。

自定义服务退出时应：

- 解绑 `Game.BindXxx`。
- Dispose 事件订阅。
- 取消异步循环。
- 释放资源适配器。
- 清空对 UnityEngine.Object 的引用。

## 常见问题

### Values() 返回 null

检查是否先调用 `UseValues()`，以及是否已经退出 Game。

### 字段没有注入

按顺序检查：目标实现 `IInjectAble`、字段有 `[Inject]`、不是 readonly/value type、类型和名称已注册。

### 同接口多实例拿错

不要依赖空名称的“第一个服务”，显式传 name。

### 在构造函数中访问服务

普通 Service 构造时可能还未加入容器。把容器相关逻辑放到 OnUse/OnEnter。

### 后注册 ValueService

已经 Use 的旧服务不会自动回填注册。应把 `UseValues()` 放在第一位，或手工 `values.Register`。
