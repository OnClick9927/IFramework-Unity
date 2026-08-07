# 快速开始

本章建立一个最小可运行结构：`Game` 入口、Value/DI、一个自定义服务、事件和取消令牌。UI 有独立专题。

## 1. 创建 Game 入口

```csharp
using IFramework;

public sealed class AppGame : Game, IInjectAble
{
    [Inject] private IClockService clock;

    protected override void Startup()
    {
        // ValueService 必须先注册，后续服务才能使用注入。
        this.UseValues();

        this.Use<IClockService>(new ClockService());
        this.EnterService<IClockService>();

        this.Values().Inject(this);
        clock.StartClock();
    }

    protected override void OnQuit()
    {
        Log.L("Game quit");
    }
}
```

将 `AppGame` 挂到启动场景中的 GameObject。`Game.Awake()` 会创建服务容器并调用 `Startup()`。

## 2. 创建服务接口与实现

```csharp
using IFramework;

public interface IClockService : IService
{
    void StartClock();
}

public sealed class ClockService : ServiceBase, IClockService
{
    public void StartClock()
    {
        Log.L("Clock started");
    }

    protected override void OnUse(IServiceCollection services)
    {
        // 注册进容器时执行。适合保存依赖、绑定基础回调。
    }

    protected override void OnEnter(IServiceCollection services)
    {
        // EnterService<IClockService>() 时执行。
    }

    protected override void OnQuit(IServiceCollection services)
    {
        // Game.Quit() 时按服务注册逆序执行。
    }
}
```

服务使用流程是：

```text
Use -> Enter -> 业务运行 -> Quit
```

`Use` 负责注册，`Enter` 负责进入。不是每个服务都必须单独 Enter；内置扩展会根据自身设计提供 `EnterMvc`、`EnterState<T>` 等入口。

## 3. 使用依赖注入

只有实现 `IInjectAble` 的对象会被 `IValueService.Inject` 处理。

```csharp
public sealed class PlayerController : IInjectAble
{
    [Inject] private IClockService clock;
    [Inject("secondary")] private IClockService secondaryClock;
}
```

注册命名实例：

```csharp
this.Use<IClockService>(new ClockService());
this.Use<IClockService>(new ClockService(), "secondary");

var controller = new PlayerController();
this.Values().Inject(controller);
```

注入规则：

- 字段必须带 `[Inject]`。
- 字段不能是 `readonly`。
- 字段类型不能是值类型。
- 目标对象必须实现 `IInjectAble`。
- 名称必须和注册名称一致；默认名称是空字符串。

## 4. 发布和订阅事件

定义消息：

```csharp
public sealed class GoldChanged : IEventArgs
{
    public int value;
}
```

订阅并绑定生命周期：

```csharp
public sealed class GoldPresenter
{
    public void Enable()
    {
        Events.Subscribe<GoldChanged>(new GoldHandler()).AddTo(this);
    }

    public void Disable()
    {
        this.ClearDisposable();
    }
}

public sealed class GoldHandler : IEventHandler<GoldChanged>
{
    public void OnEvent(GoldChanged message)
    {
        Log.L("Gold: {0}", message.value);
    }
}
```

发布：

```csharp
Events.Publish(new GoldChanged { value = 100 });
```

也可以使用字符串通道：

```csharp
IDisposable subscription = Events.Subscribe("player.ready", _ => Log.L("ready"));
Events.Publish("player.ready", null);
subscription.Dispose();
```

## 5. 等待和通知

```csharp
async AsyncTask WaitForSceneReady()
{
    await Events.Wait("scene.ready");
    Log.L("Scene is ready");
}

void CompleteSceneLoading()
{
    Events.Notify("scene.ready");
}
```

同一个 message 同时只能存在一个 `Wait`。重复等待会记录错误并返回 `null`。

携带数据：

```csharp
async AsyncTask WaitForConfig()
{
    var config = await Events.Wait<ConfigReady>();
    Log.L("Version: {0}", config.version);
}

Events.Notify(new ConfigReady { version = "1.0" });
```

## 6. 使用 AsyncTask

```csharp
async AsyncTask RunFlow(CancellationToken token)
{
    await AsyncTask.NextFrame(token);
    await AsyncTask.Delay(0.5f, token);

    await AsyncTask.Sequence(token,
        () => LoadConfig(),
        () => LoadPlayer(),
        () => EnterLobby());
}
```

取消：

```csharp
private CancellationTokenSource source;

void Begin()
{
    source = new CancellationTokenSource();
    RunFlow(source.Token).Coroutine();
}

void Stop()
{
    source.Cancel();
}
```

`CancellationTokenSource.Dispose()` 在本框架中等价于触发取消。它可以通过 `AddTo(owner)` 绑定 owner 生命周期：

```csharp
var source = new CancellationTokenSource().AddTo(this);
```

## 7. 使用对象池

```csharp
var list = StaticPool.Get<List<int>>();
list.Clear();
try
{
    list.Add(1);
    list.Add(2);
}
finally
{
    list.Clear();
    StaticPool.Set(list);
}
```

数组：

```csharp
var buffer = StaticPool.GetArray<object>(32);
try
{
    // 使用 buffer
}
finally
{
    // 归还时框架会清理包含引用的元素。
    StaticPool.Set(buffer);
}
```

不要重复归还同一个对象或数组。Pool API 主要面向 Unity 主线程。

## 8. 退出与清理

主动退出：

```csharp
Game.Current.Quit();
```

`Quit()` 幂等，执行：

1. `Game.OnQuit()`。
2. 服务按注册逆序 `OnQuit()`。
3. `Game` 上通过 `AddTo(game)` 绑定的 `IDisposable` 全部释放。

GameObject 销毁时也会自动调用 `Quit()`。

## 下一步

- 理解注册/进入/退出顺序：[架构与生命周期](3-architecture.md)
- 完整异步语义：[异步任务](4-async-task.md)
- UI 接入：[UI 运行时](11-ui-runtime.md)
- 直接运行仓库示例：[示例与贡献](16-examples-contributing.md)
