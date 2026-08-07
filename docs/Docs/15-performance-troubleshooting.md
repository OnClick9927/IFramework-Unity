# 性能与故障排查

本章从 GC、托管内存、CPU 和缓存局部性四个角度说明 IFramework 的使用边界。优化前先在目标设备、目标构建配置和真实业务负载下采样；Editor 数据只能用于定位趋势，不能替代 Player 数据。

## 1. 建立可复现基线

建议至少记录以下环境：

```text
Unity 版本：
目标平台与设备：
Scripting Backend：Mono / IL2CPP
Development Build：是 / 否
场景与操作路径：
预热帧数：
采样帧数：
平均 / P95 / 最大帧耗时：
GC Alloc / Frame：
托管堆大小与峰值：
```

一次可靠的对比应满足：

1. 优化前后使用相同场景、资源和输入序列。
2. 跳过资源首次加载、JIT 或 Shader 编译造成的预热波动。
3. 分开测量“首次创建”和“稳定复用”。
4. 除平均值外观察峰值和长尾。
5. 同时检查正确性，避免用少执行业务换取表面耗时下降。

## 2. GC 与托管内存

### AsyncTask

建议：

- 已有任务直接 `await`，不要为转发结果再包一层无意义的 `async` 方法。
- 高频流程优先复用业务状态对象，但不要复用一个仍可能被等待的任务实例。
- `Sequence` 的任务工厂可延迟创建任务，减少不必要的并发对象。
- 长生命周期取消源用完后调用 `Dispose`，释放注册引用。
- 事件和 UI View 中不要让完成任务继续持有大对象闭包。

避免：

```csharp
// 每次调用额外创建异步状态和闭包，没有附加语义。
async AsyncTask Forward(AsyncTask task)
{
    await task;
}
```

更直接的形式：

```csharp
AsyncTask Forward(AsyncTask task) => task;
```

框架 Awaiter 在注册 continuation 时不需要为每次等待创建临时队列。业务仍可能因为捕获局部变量的 lambda、LINQ、装箱或临时集合产生分配，应在 Profiler 的 `GC Alloc` 调用栈继续向上定位。

### Events

每个订阅都可能持有目标对象。忘记释放带来的问题通常不是单帧分配，而是对象长期无法回收。

```csharp
private IDisposable subscription;

void OnEnable()
{
    subscription = Events.Subscribe("InventoryChanged", OnInventoryChanged);
}

void OnDisable()
{
    subscription?.Dispose();
    subscription = null;
}
```

不要在 `Update` 中重复订阅。需要批量管理时，将返回的 `IDisposable` 放进明确的生命周期集合，并在退出时一次释放。

### ObjectPool

对象池降低创建频率，但会增加常驻内存。适合池化的对象通常满足：

- 创建或销毁成本明显。
- 生命周期短且反复出现。
- 峰值数量可估计。
- 状态能完整重置。

不适合池化：

- 很少创建的小对象。
- 数量无限增长且没有裁剪策略的唯一对象。
- 持有大型资源、事件订阅或外部句柄且难以重置的对象。

`IPoolObject.OnSet` 应清理：

- 指向场景对象、资源和上下文的引用。
- 集合中的元素。
- 委托、事件与回调。
- 取消注册和临时句柄。
- 可观察状态、计时器和索引。

### ArrayPool

`StaticPool.GetArray<T>(length)` 只返回精确长度数组，适合长度集合较稳定的热点路径。归还引用类型或内部包含引用字段的数组时，框架会清空元素，防止池长期保活旧对象。

需要注意：

- 清空大引用数组本身需要 CPU 时间。
- 长度高度离散会让池保存许多低复用数组。
- 取得数组后只能在所有消费者结束使用后归还。
- 归还后不得继续读取或写入，因为下一位使用者拥有它。
- 不要归还并非从该池取得、但仍被其他代码持有的数组。

### Value 注入

反射注入适合初始化阶段，不适合每帧调用。

```csharp
// 推荐：初始化一次
Game.Values().Inject(this);

// 不推荐：每帧反复扫描成员
void Update() => Game.Values().Inject(this);
```

类型成员元数据会被复用，但目标写入和查找仍有成本。热点代码应保存已注入接口引用。

### UI

UI 常见内存来源：

- 已关闭但未真正销毁的 Panel。
- WidgetPool 中过多的闲置 Item。
- View 没有解绑 UnityEvent。
- 资源适配器缓存仍持有 Prefab、纹理或句柄。
- 异步关闭没有完成，导致调用链与闭包一直存活。

关闭策略应按业务区分：高频面板可保留，低频大面板应销毁，Item 池应限制到实际峰值附近。

## 3. CPU

### 事件发布

消息系统适合跨模块通知，不应替代所有直接调用。每帧给大量实体广播同一细粒度事件，会增加字典查找、委托调用和间接分支。

建议：

- 高频局部逻辑使用直接调用或紧凑数据循环。
- 跨系统、低频状态变化使用 Events。
- 一次消息携带处理所需的完整数据，避免订阅者再次全局搜索。
- 不要在回调中修改正在遍历的业务集合，必要时延后操作。

### 红点树

多次改变叶子节点后集中调用 `FreshDots()`，让同一父链在一个业务批次中统一刷新。每改一个值立即刷新整棵树，会重复做祖先传播与 UI 更新。

### UI 层级和 Raycast

降低 UI CPU/GPU 开销的顺序：

1. 关闭不可交互 Graphic 的 `raycastTarget`。
2. 不需要渲染的点击区域使用 `Empty4Raycast`。
3. 透明区域很大的 Image 评估多边形网格和射线命中。
4. 控制 Canvas 重建范围，避免频繁修改大 Canvas 下的 Layout。
5. 列表使用 WidgetPool，并批量更新可见项。
6. 用 Profiler 验证 Mesh 优化收益；顶点过多也可能抵消收益。

### 日志

即使最终 Logger 不输出，字符串插值、`params object[]` 和装箱也可能在调用前发生。高频日志应由条件包围：

```csharp
#if DEVELOPMENT_BUILD || UNITY_EDITOR
Log.L("Actor count: {0}", actors.Count);
#endif
```

### Update 绑定

Launcher 让多个对象共享 PlayerLoop 入口，但每个委托仍会执行。对大量短生命周期对象：

- 只在活动期间绑定。
- 禁用或销毁时严格解绑。
- 相同类型的大量对象可由一个 Manager 紧凑遍历。
- 不要在回调内部每帧创建枚举器、LINQ 查询或闭包。

## 4. 提高内存命中率

托管环境无法直接控制 CPU Cache，但数据布局和访问顺序仍然重要。

### 连续数据优先

热点遍历优先使用数组或 `List<T>`，并按索引顺序访问：

```csharp
for (int i = 0; i < items.Count; i++)
{
    items[i].Tick();
}
```

链表、层层对象包装和随机字典访问会增加指针追踪。字典适合查找入口，取得数据后尽量在连续容器中完成批处理。

### 减少工作集

缓存命中率不仅取决于结构，还取决于同时活跃的数据量：

- 池只保留实际可能复用的容量。
- UI 只更新可见项。
- 状态切换时释放不再使用的大型临时集合。
- 把冷配置与每帧热状态分开存放。
- 循环内保存频繁访问的服务和集合引用，避免反复经过多层查询。

### 批量处理

把同类修改聚合后顺序执行：

- 多个红点变化后刷新一次。
- 列表数据准备完后统一刷新 UI。
- 同一类型池对象集中取出/归还。
- 事件中传递变更集合，而不是为每个元素各发布一次消息。

批处理不能牺牲响应语义。如果业务要求每个中间状态可观察，就不能简单合并。

### 结构体注意事项

小而稳定的纯值数据可以减少对象跳转；过大的结构体会增加复制成本。包含引用字段的结构体数组仍会持有对象引用，IFramework 数组池会在归还时正确清理此类数组。

## 5. Profiler 定位顺序

出现卡顿时建议按以下顺序：

1. 在 Timeline 找到异常帧和主线程耗时块。
2. 判断是脚本、渲染、资源加载、GC 还是等待同步。
3. 若有 GC，打开 Allocation Call Stacks 定位分配源。
4. 若是脚本 CPU，进入 Hierarchy，按 `Total` 与 `Self` 分别排序。
5. 在目标设备 Development Player 复现。
6. 做单一变量修改并重新采样。
7. 对比正确性、平均值、峰值和内存峰值。

Memory Profiler 排查长期增长时，比较同一流程执行前后的快照，重点查看：

- Managed Shell 和 Native Object 是否仍被持有。
- 静态字段、事件、池和资源缓存的引用链。
- 场景卸载后 GameObject、Texture、Mesh 是否仍存活。
- 大数组是否因为长度过于离散而堆积在池中。

## 6. 常见问题

### await 永远不返回

检查：

- 手动创建的任务是否在所有分支调用了 `SetResult` 或 `SetException`。
- `UIView.OnHideAsync` / `OnCloseAsync` 是否完成传入 operation。
- UI 资源加载失败时，适配器是否完成或失败任务。
- `Events.Wait` 对应的消息名和 `Notify` 是否完全相同。
- Token 是否已经取消，以及异常是否被上层吞掉。
- `Sequence` 中某个工厂是否返回了永不结束的任务。

### 面板 Show 后没有显示

检查：

- path 是否存在于 `PanelCollection`。
- `LoadPanelAsync` 是否返回非空 `UIPanel`。
- Prefab 上是否挂有 `UIPanel`。
- `IViewBridge` 的类型映射是否包含同一 path。
- layer 索引是否在 `UILayerData.layers` 范围内。
- Panel 是否被更高层全屏面板设为不可见。
- CanvasGroup、GameObject active 和动画是否仍处于隐藏状态。

### HideAsync / CloseAsync 卡住

异步关闭把完成权交给 View。动画、Tween 或自定义过渡结束后必须完成 operation；即使动画被跳过、对象被禁用或发生异常，也要有兜底分支。

### 服务为 null

检查：

- 是否先执行 `UseXxx`。
- 获取时的名称是否与注册名称一致。
- 是否在 Game 的初始化之前访问。
- `Game` 脚本是否处于启用对象上，以及 `Startup()` 是否已经执行。
- 服务是否已在 `OnDestroy` 后释放。

### 注入字段没有值

检查：

- 目标是否实现 `IInjectAble`。
- 字段/属性是否带 `[Inject]`。
- 类型与名称是否完全匹配。
- 是否先向 Values 注册依赖，再调用 `Inject`。
- 属性是否具有可写 setter。

### 池对象带着上次状态

这是对象重置协议不完整。把所有会影响下一次使用的状态放进 `OnGet`/`OnSet`，尤其是委托、父节点、激活状态、集合内容、动画和异步操作。

### 数组池内存没有下降

池的目的就是保留数组以供复用，因此 GC 后托管堆大小不一定立刻降低。确认：

- 是否真的需要缓存该峰值长度。
- 是否出现大量不同长度。
- `Clear()` 是否在场景或模式退出时执行。
- 是“保留容量”还是数组元素继续持有业务对象。

### 红点状态没有更新

确认节点路径存在、父子关系正确，并在状态变更后调用 `FreshDots()`。多个变更可以合并刷新，但不能永久省略刷新。

### Editor 正常、Player 异常

检查：

- 反射创建的 View 类型是否被代码裁剪。
- 资源路径大小写是否一致。
- Editor 专用 API 是否被运行时代码引用。
- IL2CPP 与 Mono 对异常、反射和 AOT 泛型的差异。
- StreamingAssets/PersistentDataPath 的平台读写限制。

## 7. 线程安全边界

IFramework 的 Game、Events、AsyncTask 调度、服务、UI 和大多数池按 Unity 主线程设计。除非具体实现明确保证，否则不要从后台线程：

- 操作 UnityEngine.Object。
- 发布会触发 Unity API 的事件。
- 完成将立即执行主线程 continuation 的任务。
- 读写同一个服务容器或对象池。

后台任务应只处理独立数据，在明确的主线程切换点交回结果。

## 8. 发布前性能清单

- [ ] 目标设备 Player 已完成采样。
- [ ] 稳态热点路径 `GC Alloc / Frame` 符合预算。
- [ ] 所有事件、Update、UnityEvent 绑定均有解绑点。
- [ ] 所有池对象都能完整重置，池容量有上限或清理时机。
- [ ] 数组归还后没有继续使用。
- [ ] UI 异步过渡的成功、跳过、取消和异常分支都能结束。
- [ ] Value 注入和反射扫描不在逐帧热点中。
- [ ] 红点与列表更新已经合理批处理。
- [ ] 场景切换后没有残留大资源和静态引用。
- [ ] Development 日志不会进入发行版高频路径。
