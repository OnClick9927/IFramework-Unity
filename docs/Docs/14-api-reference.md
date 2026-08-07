# API 速查

本章提供运行时公开 API 的导航。它不是源码签名的机械复制，而是说明每组 API 的职责、所有权和常见调用位置。完整行为以当前版本源码为准。

## 命名空间与程序集

| 模块 | 主要命名空间 | 程序集 |
| --- | --- | --- |
| Core | `IFramework` | `IFramework` |
| Services | `IFramework` | `IFramework` |
| UI | `IFramework.UI` | `IFramework` |
| Editor | `IFramework`、`IFramework.UI` | Editor 程序集 |

业务程序集引用 IFramework 后，通常只需要：

```csharp
using IFramework;
using IFramework.UI;
```

## 1. Game 与生命周期

### Game

`Game` 是框架运行时入口，也是服务容器。业务通常继承它并挂到场景对象上。

| 成员 | 用途 |
| --- | --- |
| `protected abstract void Startup()` | 注册、进入服务并启动业务；由框架 `Awake` 调用 |
| `protected virtual void OnQuit()` | Game 退出前的业务清理扩展点 |
| `Quit()` | 幂等退出；随后逆序退出服务并释放绑定 |
| `Use<T>(service, string)` | 按接口类型和可选名称注册服务 |
| `GetService<T>(string)` | 查找服务；不存在时返回 `null` |
| `GetServices<T>()` | 获取同一接口下的所有实例 |
| `EnterService<T>(string)` | 进入已经注册的服务 |
| `Current` | 当前 Launcher 持有的 Game |
| `BindUpdate/UnBindUpdate` | 绑定或解绑每帧更新 |
| `BindFixedUpdate/UnBindFixedUpdate` | 绑定或解绑固定帧更新 |
| `BindLateUpdate/UnBindLateUpdate` | 绑定或解绑 LateUpdate |
| `BindOnApplicationFocus` | 监听应用焦点变化 |
| `BindOnApplicationPause` | 监听暂停变化 |
| `BindDisable` | 监听 Launcher 禁用 |

绑定使用委托身份解绑。不要用两个内容相同但实例不同的匿名委托进行绑定和解绑。

```csharp
private void OnFrame() { }

private void OnEnable() => Game.BindUpdate(OnFrame);
private void OnDisable() => Game.UnBindUpdate(OnFrame);
```

### Launcher

`Launcher` 在运行时初始化阶段自动建立，并把 Unity PlayerLoop 转换成静态事件。一般由 `Game` 的静态方法间接使用，不需要业务手动创建。

## 2. 服务容器

### IServiceProvider

| API | 说明 |
| --- | --- |
| `GetService(Type, string)` | 按运行时类型和名称查找 |
| `GetService<T>(string)` | 泛型查找 |
| `GetRequiredService<T>(string)` | 必需服务查找 |
| `EnterService<T>(string)` | 进入已经注册的服务 |

### IServiceCollection

在 `IServiceProvider` 基础上增加注册能力。大多数 `UseXxx` 扩展都返回 `IServiceCollection`，因此可以链式配置。

### IService / ServiceBase

自定义服务推荐继承 `ServiceBase`：

```csharp
public sealed class InventoryService : ServiceBase
{
    protected override void OnUse(IServiceCollection services)
    {
        // 注册时建立覆盖整个服务生命周期的资源。
    }

    protected override void OnEnter(IServiceCollection services)
    {
        // 全部依赖注册后开始运行。
    }

    protected override void OnQuit(IServiceCollection services)
    {
        // 解绑事件、释放池或外部句柄
    }
}
```

注册实例不等于进入服务。约定顺序是先完成全部 `UseXxx`，再执行 `EnterXxx` 或 `EnterService<T>`。

`GetRequiredService<T>(name)` 支持命名查询。它在缺失时记录错误并返回 `null`，不会像部分 DI 容器一样抛出异常。

## 3. AsyncTask

### 创建和完成

| API | 说明 |
| --- | --- |
| `new AsyncTask()` | 创建手动控制的任务 |
| `AsyncTask.CreateFromPool()` | 保留的兼容入口；当前返回独立任务对象 |
| `SetResult()` | 成功完成 |
| `AsyncTask<T>.SetResult(T)` | 带结果完成 |
| `IsCompleted` | 是否已经结束 |
| `IsCanceled` | 是否因框架取消异常结束 |
| `Exception` | 当前异常 |

任务只能完成一次。完成后的重复 `SetResult` 或 `SetException` 不会改变第一次完成结果。

### 组合

| API | 结果 |
| --- | --- |
| `WhenAll(tasks)` | 所有任务完成后结束 |
| `WhenAny(tasks)` | 任意任务完成后结束 |
| `WhenAny<T>(tasks)` | 返回最先完成任务的结果 |
| `Sequence(calls)` | 按顺序调用任务工厂 |

把真正的工作包装为 `Func<AsyncTask>` 传给 `Sequence`。若提前创建所有任务，它们可能在进入序列前已经开始执行。

### 帧与时间

| API | 说明 |
| --- | --- |
| `NextFrame(token)` | 下一次框架 Update 完成 |
| `Delay(seconds, token)` | 等待秒数；使用框架更新时间源 |
| `Repeat(interval, count, action, token)` | 按间隔重复；`count == -1` 为无限 |
| `While(condition, interval, token)` | 条件为真时持续等待 |
| `Util(condition, interval, token)` | 等待条件变为真 |

边界语义：

- `Delay` 的负数和零会立即完成；`NaN` 会产生参数异常。
- `Repeat(..., 0, ...)` 不调用回调并立即完成。
- `Repeat` 的有效次数是 `-1` 或大于等于 `0`。
- 已取消 Token 优先产生取消结果。

### CancellationTokenSource

这是 IFramework 自有类型，不是 `System.Threading.CancellationTokenSource`。

| API | 说明 |
| --- | --- |
| `Token` | 获取轻量 Token |
| `Cancel()` | 通知已注册任务取消 |
| `Dispose()` | 释放注册集合 |
| `token.IsCancellationRequested` | 查询取消状态 |
| `token.Register(task)` | 任务随 Token 取消 |

框架异步系统面向 Unity 主线程调度，不应把它当作线程同步原语。

## 4. Events

| API | 说明 |
| --- | --- |
| `Subscribe(string, Action<IEventArgs>)` | 订阅同步消息 |
| `Subscribe<T>(IEventHandler)` | 订阅类型消息 |
| `Subscribe(string, Func<T, AsyncTask>)` | 订阅异步处理器 |
| `Publish(string, args)` | 同步发布 |
| `Publish<T>(args)` | 以类型名发布 |
| `PublishAsync(...)` | 等待异步处理器 |
| `Wait(message, token)` | 等待一次通知 |
| `Wait<T>(...)` | 等待并取得事件参数 |
| `Notify(message)` | 唤醒无参数等待者 |
| `Notify(message, arg)` | 唤醒带参数等待者 |

`Subscribe` 返回 `IDisposable`，它就是订阅所有权。调用方必须在生命周期结束时释放。

## 5. Pool

### ObjectPool<T>

| 成员 | 说明 |
| --- | --- |
| `Get()` | 取出对象；池空时调用 `Create()` |
| `Set(T)` | 归还对象；成功返回 `true` |
| `Clear()` | 清空池内对象 |
| `count` | 当前缓存数量 |
| `CreateNew()` | 子类创建对象的扩展点 |
| `OnGet(T)` | 取出后回调 |
| `OnSet(T)` | 入池前验证/重置；返回 `false` 拒绝入池 |
| `OnClear(T)` | 清理单个缓存项 |

重复归还同一实例会被拒绝。引用类型的 `null` 也不会进入池。

### SimpleObjectPool<T>

约束为 `class, new()`。类型实现 `IPoolObject` 后，可通过 `OnGet` 和 `OnSet` 重置状态。

### StaticPool

| API | 说明 |
| --- | --- |
| `Get<T>()` / `Set<T>(value)` | 按编译期类型使用静态对象池 |
| `SetByRealType(value)` | 按对象真实运行时类型归还 |
| `GetArray<T>(length)` | 获取精确长度数组 |
| `Set<T>(array)` | 归还数组 |
| `CreateDisposable<T>()` | 取得随 `Dispose` 自动归还的值包装 |
| `CreateDisposableArray<T>(length)` | 取得数组包装 |

数组池按精确长度匹配。引用类型或包含引用字段的数组在归还时会清空元素，避免延长对象生命周期。

## 6. Value 与注入

| API | 说明 |
| --- | --- |
| `UseValues()` | 注册 Value 服务 |
| `Values()` | 取得 Value 服务 |
| `Register(Type, value, name)` | 注册值 |
| `Get(Type, name)` | 读取值 |
| `Register<T>(value, name)` | 泛型注册 |
| `Register<T>(factory)` | 注册按需创建工厂 |
| `Get<T>(name)` | 泛型读取 |
| `Inject(object)` | 给标记成员注入 |

注入目标实现 `IInjectAble`，字段使用 `[Inject]`。当前 `InjectAttribute` 只允许标记字段，不支持属性；名称为空时按类型匹配，使用命名值时注册端和注入端必须一致。

## 7. MVC 与 State

### MVC

| API | 说明 |
| --- | --- |
| `UseMvc(models, ctrls)` | 注册模型和控制器集合 |
| `EnterMvc()` | 注入并进入 MVC 服务 |
| `Mvc()` | 获取 MVC 服务 |
| `GetModel<T>()` | 读取模型 |
| `GetCtrl<T>()` | 读取控制器 |

`Mvc()` 按 `IMvcService` 查询，与 `UseMvc` 的注册类型一致。

### State

| API | 说明 |
| --- | --- |
| `UseState(states)` | 注册状态集合 |
| `EnterState<T>()` | 进入状态服务并切到初始状态 |
| `State()` | 获取状态服务 |
| `SwitchState<T>()` | 切换状态 |
| `FindState<T>()` | 查找状态实例 |
| `GetCurrentState()` | 当前状态 |

状态实例实现 `IGameState`，负责自己的进入、更新和退出行为。不要在同一次状态切换回调中递归触发无边界切换。

## 8. Pref 与 GameObjectPool

### Pref

| API | 说明 |
| --- | --- |
| `UsePref(converter, loader)` | 注册序列化转换器与存储加载器 |
| `Pref()` | 获取服务 |
| `Load<T>(key)` | 加载对象 |
| `Save<T>(key, value)` | 保存对象 |
| `PrefContext<T>` | 把 key 与值绑定为上下文 |

`IPrefConverter` 负责对象与存储格式转换，`IPrefLoader` 负责实际读写。密钥、版本迁移和原子写入由具体实现负责。

### GameObjectPool

| API | 说明 |
| --- | --- |
| `UseGameObjectPool(asset)` | 注册池和资源适配器 |
| `GameObjectPool()` | 获取服务 |
| `Prepare(key)` | 准备指定 key 的资源和内部池 |
| `Get<T>(key)` | 获取 `GameObjectView` 实例 |
| `Set(view)` | 归还实现 `IPoolAbleGameObjectView` 的 View |
| `Clear(path)` | 清理指定资源池 |
| `ClearAll()` | 清理全部缓存 |

`IGameObjectPoolAsset` 隔离资源加载和销毁。池化对象可实现 `IPoolAbleGameObjectView` 接收取出/归还回调。

## 9. RedDot 与 Undo

### RedDot

| API | 说明 |
| --- | --- |
| `UseRedTree()` | 注册红点树 |
| `RedTree()` | 获取服务 |
| `CreateRedDot<T>(path, init)` | 创建逻辑节点 |
| `CreateRedActiveDot(path, gameObject)` | 创建控制 GameObject 激活状态的节点 |
| `FreshDots()` | 批量刷新脏节点 |

路径表示层级关系。修改叶子状态后应在合适的批处理边界执行刷新。

### Undo

| API | 说明 |
| --- | --- |
| `UseUndo(name)` | 注册命名 Undo 服务 |
| `Undo(name)` | 获取服务 |
| `Subscribe<T>(init, redo)` | 从静态池取得记录、初始化并写入历史 |
| `Subscribe(record, redo)` | 直接写入记录；可控制是否立即 Redo |
| `Undo()` | 撤销 |
| `Redo()` | 重做 |

Undo 记录可实现池重置协议。记录内容必须包含恢复前后状态所需的全部信息，不能依赖后来会变化的临时引用。

## 10. UI

### 注册与查找

| API | 说明 |
| --- | --- |
| `game.UseUI(layerData, collection, bridge, delegate, canvas, name)` | 注册 UI 服务 |
| `game.UI(name)` | 获取命名 UI 服务 |
| `Show(path)` | 显示面板；必要时加载 |
| `Hide(path)` / `Close(path)` | 发起隐藏/关闭；实际操作延迟到下一帧 |
| `HideAsync(path)` / `CloseAsync(path)` | 等待 View 完成过渡 |
| `GetIsPanelOpen(path)` | 是否已经加载该面板 |
| `FindPanel(path)` | 获取已加载 `UIPanel` |
| `GetVisibleList()` | 获取当前可见 path 列表 |

### IUIDelegate

`LoadPanelAsync` 和 `DestroyPanel` 负责资源边界，其余回调用于记录请求、显示状态、层顶变化、全屏数量和生命周期。适配器不应在回调里再次无条件调用同一 UI 操作，否则容易递归。

### IViewBridge / UIView

Bridge 把字符串 path 映射到具体 View：

- `Subscribe/UnSubscribe` 建立或移除映射。
- `OnLoad/OnShow/OnHide/OnClose` 转发生命周期。
- `OnBecameVisible/OnBecameInvisible` 表示因层级遮挡造成的可见性变化。
- `OnHideAsync/OnCloseAsync` 把等待操作交给 View。

`UIView` 默认会立即完成异步隐藏/关闭 operation。自定义 View 覆盖动画方法后，必须在所有结束分支完成传入的 operation，否则调用方会一直等待。

### WidgetPool<T>

用于一个面板内大量重复 Item。创建池时传入 Prefab、父节点和 View 工厂；使用结束必须释放池或回收所有活动项。

### UI 工具

| 类型 | 用途 |
| --- | --- |
| `UnityEventHelper` | 绑定 UnityEvent，并把解绑挂到对象生命周期 |
| `Empty4Raycast` | 无渲染射线接收区域 |
| `PolygonRaycastImage` | 多边形射线命中 |
| `ImagePolygonMeshEffect` | 减少透明区域网格开销 |
| `UIPanel.AdaptNotchScreen` | 安全区/异形屏适配 |

## 11. Log 与通用工具

| API | 说明 |
| --- | --- |
| `Log.L` | 普通日志 |
| `Log.W` | 警告 |
| `Log.E` | 错误 |
| `Log.Exception` | 异常 |
| `Log.A` | 条件断言 |
| `Log.logger` | 替换日志适配器 |

发行构建是否输出日志由项目配置与日志实现共同决定。高频循环不要构造仅用于关闭日志的字符串。

`Singleton<T>` 提供纯 C# 单例，`MonoSingleton<T>` 提供 MonoBehaviour 单例。二者都代表全局所有权，应明确初始化、场景切换和销毁行为，避免用单例隐藏模块依赖。
