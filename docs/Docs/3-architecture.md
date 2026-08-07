# 架构与生命周期

## 总体关系

```text
Unity Runtime
    |
    v
Launcher (自动创建、转发 PlayerLoop)
    |
    v
Game.Current
    |
    v
ServiceCollection
    |-- IValueService
    |-- IMvcService
    |-- IGameStateService
    |-- IPrefService
    |-- IGameObjectPool
    |-- IRedTreeService
    |-- IUndoService
    `-- UIService / 自定义 IService
```

`Launcher` 是框架的 Unity 生命周期桥。`Game` 是业务入口和服务容器外观。业务服务通过 `ServiceCollection` 管理，服务之间通过接口查询或 `IValueService` 注入。

## Launcher

`Launcher` 是带 `[DynamicMonoSingleton]` 的内部 MonoSingleton，并通过：

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
```

在运行前初始化。

它负责转发：

- `Update`
- `FixedUpdate`
- `LateUpdate`
- `OnApplicationFocus`
- `OnApplicationPause`
- `OnDisable`

公开入口由 `Game` 的静态方法暴露：

```csharp
Game.BindUpdate(OnUpdate);
Game.UnBindUpdate(OnUpdate);

Game.BindFixedUpdate(OnFixedUpdate);
Game.BindLateUpdate(OnLateUpdate);
Game.BindOnApplicationFocus(OnFocus);
Game.BindOnApplicationPause(OnPause);
Game.BindDisable(OnLauncherDisable);
```

每次绑定都必须有对应解绑，或把解绑动作封装为 `IDisposable` 并绑定到合适 owner。

## Game 生命周期

### Awake

`Game.Awake()` 的顺序：

1. 创建 `ServiceCollection`，名称默认取 Game 类型名。
2. 将 Game Transform 放到 Launcher 下。
3. 设置 `Launcher.Instance.game = this`。
4. 重置退出标志。
5. 调用业务 `Startup()`。

`Startup()` 可以是普通方法，也可以写成 `async override void`，仓库 UI 示例采用后者以等待首屏。

### Quit

`Quit()` 只执行一次：

1. 标记 `quited = true`。
2. 调用 `OnQuit()`。
3. 调用容器 `services.Quit()`。
4. 调用 `this.ClearDisposable()`。

`OnDestroy()` 自动转发到 `Quit()`。

### 当前 Game

```csharp
Game current = Game.Current;
```

`Game.Current` 实际读取 `Launcher.Instance.game`。设置新的 Game 会销毁之前 Game 的 GameObject，因此不要并行维护多个 Game 根实例。

## 服务生命周期

每个服务实现 `IService`。推荐继承 `ServiceBase`，只实现三个阶段：

```csharp
public sealed class InventoryService : ServiceBase, IInventoryService
{
    protected override void OnUse(IServiceCollection services) { }
    protected override void OnEnter(IServiceCollection services) { }
    protected override void OnQuit(IServiceCollection services) { }
}
```

### OnUse

调用 `Use<T>(service, name)` 时立即执行。适合：

- 保存容器引用。
- 将自身注册到 ValueService（容器会为服务接口自动注册）。
- 绑定需要覆盖整个服务生存期的 Update/事件。
- 创建内部数据结构。

### OnEnter

调用 `EnterService<T>(name)` 时执行。`ServiceBase` 会先尝试使用 `IValueService` 注入服务自身，然后调用自定义 `OnEnter`。

适合：

- 在全部依赖已经 `Use` 后开始业务初始化。
- 注入依赖对象。
- 初始化状态、Model 和 Controller。

### OnQuit

Game 退出时按服务注册的逆序调用。适合：

- 解绑 PlayerLoop 和事件。
- 保存数据。
- 销毁对象池和资源引用。
- 清空回调，避免 Domain Reload 关闭时保留状态。

## 为什么分 Use 和 Enter

分阶段可以先注册全部服务，再统一初始化，避免 A 初始化时 B 尚未注册：

```csharp
protected override void Startup()
{
    this.UseValues();
    this.Use<IInventoryService>(new InventoryService());
    this.Use<IQuestService>(new QuestService());

    this.EnterService<IInventoryService>();
    this.EnterService<IQuestService>();
}
```

如果服务之间有依赖，顺序建议：

```text
UseValues
-> Use 所有基础服务
-> Use 所有业务服务
-> Enter 基础服务
-> Enter 业务服务
-> 开始场景/UI 流程
```

## 服务命名与多实例

同一个接口可以注册多个命名服务：

```csharp
this.Use<IUndoService>(firstUndo, "battle");
this.Use<IUndoService>(secondUndo, "editor");

var battleUndo = this.GetService<IUndoService>("battle");
```

名称为空时，`GetService<T>()` 返回该接口注册列表中的第一个服务。

`GetRequiredService<T>(name)` 会把名称传给容器查询；服务不存在时记录错误并返回 `null`，不会抛出异常。需要自行恢复的可选依赖可直接使用 `GetService<T>(name)`。

## 服务退出顺序

容器用栈记录服务：

```text
Use A
Use B
Use C

Quit: C -> B -> A
```

因此依赖项应先注册，被依赖方后退出。例如 ValueService 通常最先 `Use`，最后退出。

## IDisposable 生命周期绑定

任意 `IDisposable` 可以绑定到 owner：

```csharp
subscription.AddTo(owner);
cancellationSource.AddTo(owner);
```

清理：

```csharp
owner.RemoveDisposable(subscription); // 移除并 Dispose 单项
owner.ClearDisposable();               // Dispose owner 下全部项
```

适合绑定：

- `Events.Subscribe` 返回值。
- `CancellationTokenSource`。
- 自定义资源句柄。
- UI 事件绑定实体（`UnityEventHelper` 已自动使用此机制）。

owner 使用对象引用作为字典键。生命周期结束必须调用 `ClearDisposable()`，否则静态字典会继续引用 owner。

## 单例

### 纯 C# Singleton

```csharp
public sealed class ConfigRegistry : Singleton<ConfigRegistry>
{
    protected override void OnSingletonInit() { }
}
```

首次访问 `Instance` 时创建，双重检查锁保护初始化。

### MonoSingleton

```csharp
[DynamicMonoSingleton("AudioRoot")]
public sealed class AudioRoot : MonoSingleton<AudioRoot>
{
    protected override void OnSingletonInit() { }
}
```

要允许自动创建，类型必须带 `[DynamicMonoSingleton]`。没有特性且场景中没有实例时，`Instance` 返回 `null`。

`DestroyOnLoad` 默认是 `true`，代码会调用 `DontDestroyOnLoad`。该字段名表示是否跨场景保留，而不是“加载时销毁”。

## 线程模型

框架的大多数容器使用普通 `Dictionary`、`List`、`Queue` 和 Unity 对象，不提供通用线程安全保证。建议：

- 在 Unity 主线程注册/查询服务。
- 在主线程发布事件和操作对象池。
- 在主线程调用 UI、状态机、红点和 Undo。
- 后台任务结束后先切回主线程，再访问框架和 Unity API。

## 推荐项目布局

```text
Assets/Game/
├─ Runtime/
│  ├─ AppGame.cs
│  ├─ Services/
│  ├─ States/
│  ├─ UI/
│  └─ Events/
├─ Editor/
└─ Resources or Addressable assets
```

将资源加载接口实现放在业务层，避免框架程序集反向依赖具体资源系统。
