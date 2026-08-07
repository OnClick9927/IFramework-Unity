# 异步任务

## 定位

`AsyncTask` 是 IFramework 的自定义 task-like 类型，通过 `[AsyncMethodBuilder]` 支持 `async/await`。它主要用于 Unity 主线程流程，例如等待下一帧、延时、UI 显示和服务初始化。

它不是 `System.Threading.Tasks.Task` 的完整替代：

- 不提供线程池调度器。
- 不提供 `ConfigureAwait`。
- 不保证框架容器和 UI API 的多线程安全。
- 异常观察方式和 .NET Task 不同。

## 声明异步方法

无返回值：

```csharp
async AsyncTask LoadPlayer()
{
    await AsyncTask.NextFrame();
    await AsyncTask.Delay(0.2f);
}
```

有返回值：

```csharp
async AsyncTask<int> LoadLevel()
{
    await AsyncTask.Delay(0.1f);
    return 10;
}
```

调用：

```csharp
var level = await LoadLevel();
```

不等待返回值时，可以调用空扩展入口：

```csharp
LoadPlayer().Coroutine();
```

`Coroutine()` 只是表达“有意忽略等待”，不会创建 Unity Coroutine。

## 状态和结果

| 成员 | 含义 |
| --- | --- |
| `IsCompleted` | 是否已经完成 |
| `IsCanceled` | `exception` 是否为 `AsyncTaskCanceledException` |
| `exception` | 保存的异常；可能被 `OnException` 标记处理后清空 |
| `AsyncTask<T>.result` | 泛型结果 |
| `CompletedTask` | 共享的已完成非泛型任务 |
| `CanceledTask` | 共享的已取消非泛型任务 |
| `CompletedTaskT` | 共享的已完成泛型任务，结果是 `default(T)` |
| `CanceledTaskT` | 共享的已取消泛型任务 |

共享任务不能被当作可变操作对象。调用 `SetResult` 对已完成任务没有效果。

## 创建和手动完成

```csharp
var task = AsyncTask.CreateFromPool();

// 某个异步回调完成时：
task.SetResult();
```

泛型：

```csharp
var task = AsyncTask<string>.CreateFromPool();
task.SetResult("ready");
```

API 保留了 `CreateFromPool` 名称以兼容既有调用，但公开任务可能长期被调用者持有，因此当前实现创建稳定的新实例，不会在完成瞬间复用同一个任务对象。

不要对同一个任务重复设置不同结果。顺序调用下，已完成泛型任务会保留第一次结果。

## 延时和帧等待

### NextFrame

```csharp
await AsyncTask.NextFrame();
```

`NextFrame` 把回调绑定到 `Launcher.Update`，下一次 Update 时解绑并完成。

### Delay

```csharp
await AsyncTask.Delay(1.5f);
```

规则：

- `second <= 0`：立即返回 `CompletedTask`。
- `float.NaN`：抛出 `ArgumentOutOfRangeException`，避免永不完成。
- 预先取消的 token：返回 `CanceledTask`，即使延时为 0。
- 正延时：基于 `Launcher.time` 在 Update 中检查。

Editor 非 Play Mode 下，`Launcher.time` 使用 `EditorApplication.timeSinceStartup`；Player 中使用 `Time.time`。

## 组合任务

### WhenAll

```csharp
await AsyncTask.WhenAll(
    LoadConfig(),
    LoadInventory(),
    LoadQuest());
```

行为：

- `null` 或空集合会得到已完成任务。
- 等待所有输入任务完成。
- 遇到非取消异常时，结果任务保存该异常并完成。
- 子任务的 `AsyncTaskCanceledException` 当前按“子任务已结束”计数；要取消聚合结果，应传外部 token。

带 token：

```csharp
await AsyncTask.WhenAll(token, taskA, taskB);
```

### WhenAny

```csharp
await AsyncTask.WhenAny(requestA, requestB);
```

泛型会返回第一个完成任务的结果：

```csharp
int value = await AsyncTask.WhenAny<int>(new[] { first, second });
```

空集合立即完成，泛型结果是 `default(T)`。

### Sequence

```csharp
await AsyncTask.Sequence(
    () => LoadConfig(),
    () => LoadPlayer(),
    () => EnterLobby());
```

每个委托在前一个任务完成后调用。`null` 集合和空集合立即完成；集合中的 `null` 委托或返回 `null` 的委托会被跳过。

带取消：

```csharp
await AsyncTask.Sequence(token,
    () => StepA(),
    () => StepB());
```

## Repeat、While 和 Util

### Repeat

```csharp
await AsyncTask.Repeat(0.5f, 10, Tick);
```

参数：

- `interval`：每次调用前等待秒数。
- `count > 0`：执行固定次数。
- `count == 0`：立即完成，不调用回调。
- `count == -1`：无限重复，直到外部结束。
- `count < -1`：抛出 `ArgumentOutOfRangeException`。

回调抛异常时，结果任务记录异常并结束。

取消是协作式的：Repeat 每轮延时后检查 token，并以普通完成结束。需要“取消状态”时，应在业务层明确处理，而不要假设它和 .NET `Task.Delay(token)` 完全一致。

### While

当条件保持 `true` 时重复：

```csharp
await AsyncTask.While(() => isLoading, 0.02f, token);
```

条件初始为 `false` 时立即完成。

### Util

等待条件变成 `true`：

```csharp
await AsyncTask.Util(() => dataReady, 0.02f, token);
```

命名保留自既有 API；语义相当于 `WaitUntil`。

## ContinueWith

```csharp
task.ContinueWith(completed =>
{
    Log.L("completed: {0}", completed.IsCompleted);
});
```

泛型派生任务：

```csharp
task.ContinueWith<AsyncTask<int>>(completed =>
{
    Log.L("result: {0}", completed.result);
});
```

已完成任务注册 continuation 时会立即调用。不要在 continuation 中执行长时间阻塞操作，因为它运行在完成任务的调用线程，Unity 流程通常就是主线程。

## 异常处理

自定义 awaiter 的 `GetResult()` 只检查任务是否完成，不会像标准 Task 一样自动重新抛出 `exception`。应使用以下方式之一观察：

```csharp
var task = SomeOperation();
await task;
if (task.exception != null)
{
    Log.Exception(task.exception);
}
```

或注册处理器：

```csharp
SomeOperation().OnException(task =>
{
    if (task.exception is RecoverableException)
    {
        Recover();
        return true; // 表示已处理，框架会清空 exception
    }
    return false;
});
```

返回 `false` 或没有处理器时，框架通过 `Log.Exception` 记录异常。

不要仅依赖 `try/catch` 包裹 `await AsyncTask` 来观察任务中保存的异常。

## 取消令牌

```csharp
var source = new CancellationTokenSource
{
    userData = "loading-screen"
};

Run(source.Token).Coroutine();
source.Cancel();
```

### CancellationToken

| 成员 | 说明 |
| --- | --- |
| `userData` | 从 Source 读取的业务标识 |
| `IsCancellationRequested` | 是否已请求取消 |
| `ThrowIfCancellationRequested()` | 抛出 `AsyncTaskCanceledException` |
| `Register(Action)` | 注册取消回调 |
| `Register(AsyncTask)` | 取消时调用任务内部取消 |

### CancellationTokenRegistration

```csharp
var registration = token.Register(StopLoading);
registration.Dispose();
```

Dispose 用于取消注册，不会触发回调。

### CancellationTokenSource.Dispose

本框架的 `CancellationTokenSource` 实现中，`Dispose()` 会调用 `Cancel()`：

```csharp
using (var source = new CancellationTokenSource())
{
    Run(source.Token).Coroutine();
} // 离开 using 时请求取消
```

这和 .NET CancellationTokenSource 的“Dispose 不等于 Cancel”习惯不同，迁移代码时要特别注意。

## 生命周期模式

```csharp
public sealed class LoadingView
{
    private CancellationTokenSource source;

    public void Open()
    {
        source = new CancellationTokenSource();
        RefreshLoop(source.Token).Coroutine();
    }

    public void Close()
    {
        source?.Cancel();
        source = null;
    }

    private async AsyncTask RefreshLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await AsyncTask.Delay(0.5f, token);
            if (token.IsCancellationRequested) break;
            Refresh();
        }
    }
}
```

## 注意事项

- `AsyncTask` 的帧/延时方法依赖 `Launcher`，不要在 Unity Runtime 尚未初始化时调用正延时。
- 默认令牌是惰性的；注册前可以直接传递，无需判空。
- 不要在后台线程调用 UI 或对象池，即使任务本身可以从别的线程完成。
- 无限 `Repeat`、`While` 和 `Util` 必须提供可到达的结束条件。
- 任务回调运行时发生异常会经过框架日志；回调应保持短小。
