# 事件与等待

`Events` 提供两套能力：

- Publish/Subscribe：一个消息可以有多个订阅者。
- Wait/Notify：一个流程暂停，直到另一个流程发送完成信号。

两者都使用静态容器，主要面向 Unity 主线程。

## 类型消息

消息类型实现 `IEventArgs`：

```csharp
public sealed class PlayerLevelChanged : IEventArgs
{
    public int oldLevel;
    public int newLevel;
}
```

同步处理器：

```csharp
public sealed class LevelPresenter : IEventHandler<PlayerLevelChanged>
{
    public void OnEvent(PlayerLevelChanged message)
    {
        Refresh(message.newLevel);
    }
}
```

订阅与发布：

```csharp
IDisposable subscription = Events.Subscribe<PlayerLevelChanged>(new LevelPresenter());

Events.Publish(new PlayerLevelChanged
{
    oldLevel = 9,
    newLevel = 10
});

subscription.Dispose();
```

类型消息默认通道名是 `typeof(T).Name`。不同命名空间中同名消息会落入同一字符串通道，项目应避免重复短类型名，或显式使用字符串通道。

## 异步处理器

```csharp
public sealed class SaveHandler : IAsyncEventHandler<SaveRequested>
{
    public async AsyncTask OnEvent(SaveRequested message)
    {
        await SaveAsync(message.slot);
    }
}
```

```csharp
await Events.PublishAsync(new SaveRequested { slot = 1 });
```

`PublishAsync` 会调用全部订阅者，并在存在未完成任务时使用 `AsyncTask.WhenAll` 等待。

没有订阅者时，`PublishAsync` 的空条件调用可能返回 `null`。需要无订阅者也可安全等待时：

```csharp
await (Events.PublishAsync(message) ?? AsyncTask.CompletedTask);
```

## 字符串通道

同步：

```csharp
var subscription = Events.Subscribe("network.connected", args =>
{
    Log.L("connected");
});

Events.Publish("network.connected", null);
```

异步：

```csharp
var subscription = Events.Subscribe<LoadEvent>("scene.load", async args =>
{
    await LoadScene(args.sceneName);
});

await Events.PublishAsync("scene.load", new LoadEvent { sceneName = "Lobby" });
```

字符串通道适合跨模块协议，但建议集中定义常量：

```csharp
public static class EventNames
{
    public const string NetworkConnected = "network.connected";
    public const string SceneReady = "scene.ready";
}
```

## 生命周期绑定

订阅返回 `IDisposable`。直接绑定 owner：

```csharp
Events.Subscribe("inventory.changed", OnInventoryChanged)
    .AddTo(this);
```

退出时：

```csharp
this.ClearDisposable();
```

扩展写法：

```csharp
this.SubscribeEvent("inventory.changed", OnInventoryChanged);
this.SubscribeEvent<PlayerLevelChanged>(handler);
```

这些扩展内部会自动 `AddTo(self)`。

## 取消订阅

```csharp
private IDisposable subscription;

void Enable()
{
    subscription = Events.Subscribe("message", OnMessage);
}

void Disable()
{
    subscription?.Dispose();
    subscription = null;
}
```

Dispose 后，如果该通道没有其他订阅者，内部 `MessageContext` 会回到对象池，字典条目被移除。

## Publish 的调用顺序

订阅者保存在 List 中，按订阅顺序调用。发布期间修改订阅列表可能改变遍历结果，因此建议：

- 不在处理器中批量新增/删除同通道订阅。
- 需要延迟解绑时放到当前发布完成后。
- 处理器保持短小，避免阻塞后续订阅者。

## Wait/Notify

### 无数据

等待：

```csharp
async AsyncTask EnterBattle()
{
    StartLoading();
    await Events.Wait("battle.scene.ready");
    StartBattleLogic();
}
```

通知：

```csharp
Events.Notify("battle.scene.ready");
```

### 携带数据

```csharp
public sealed class LoginResult : IEventArgs
{
    public int userId;
}
```

```csharp
async AsyncTask WaitLogin()
{
    LoginResult result = await Events.Wait<LoginResult>();
    Log.L("User: {0}", result.userId);
}

Events.Notify(new LoginResult { userId = 1001 });
```

显式 message：

```csharp
var task = Events.Wait<LoginResult>("login.secondary");
Events.Notify("login.secondary", new LoginResult());
```

## Wait 的约束

每个 message 同时只能有一个等待者：

```csharp
var first = Events.Wait("ready");
var second = Events.Wait("ready"); // 记录 Already Exist Wait，返回 null
```

原因是内部 `wait_map` 的值是单个 `AsyncTask`，不是列表。

建议把 Wait/Notify 用于“一次性握手”，例如：

- 场景加载完成。
- SDK 初始化完成。
- 首次配置准备完成。
- 引导步骤确认。

广播状态变化应使用 Publish/Subscribe。

## Wait 取消

```csharp
var source = new CancellationTokenSource();
var wait = Events.Wait("ready", source.Token);

source.Cancel();
```

任务完成后 continuation 会从 `wait_map` 删除 message。预先已取消的 token 会由 `ThrowIfCancellationRequested()` 直接抛出 `AsyncTaskCanceledException`。

## Notify 错误

- message 没有 Wait：记录 `Not Exist Wait`。
- Notify 数据类型和 Wait 泛型不匹配：记录 `Not Fit Wait`。
- `Notify<T>(T arg)` 使用 `typeof(T).Name` 作为 message。

## 选择指南

| 场景 | 建议 |
| --- | --- |
| 多个观察者监听状态变化 | Publish/Subscribe |
| 需要等待全部异步监听器 | PublishAsync |
| 一个请求等待一个完成信号 | Wait/Notify |
| 请求/响应且可能并发多次 | 自定义 requestId 通道，或业务层任务映射 |
| 长期保存的最新状态 | 服务/Model，不要只依赖事件 |

## 常见错误

### 忘记 Dispose

静态事件会持有处理器和 owner。用 `AddTo` 或在退出时显式 Dispose。

### 用类型短名做跨程序集协议

同名类型会冲突。重要协议使用唯一字符串常量。

### 直接 await 无订阅者的 PublishAsync

先用 `?? AsyncTask.CompletedTask` 兜底。

### 同一 message 创建多个 Wait

为每个并发请求生成唯一 message，例如 `login:{requestId}`。
