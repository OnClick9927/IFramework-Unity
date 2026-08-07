# MVC 与游戏状态

## MVC 服务

IFramework 的 MVC 是轻量生命周期容器：

- `ModelBase`：数据和领域状态。
- `CtrlBase`：业务操作和协调。
- `MvcService`：注册、注入、Init 和 Quit。

它不强制 View 实现；UI View 可以通过 DI 获取 Model/Ctrl。

## 定义 Model

```csharp
public sealed class PlayerModel : ModelBase
{
    public int Level { get; private set; }

    protected override void Init()
    {
        Level = 1;
    }

    protected override void Quit()
    {
        Level = 0;
    }
}
```

`FreshRedPoints()` 是可选虚方法，适合 Model 在数据变化后刷新关联红点；框架不会自动调用。

## 定义 Controller

```csharp
public sealed class PlayerCtrl : CtrlBase, IInjectAble
{
    [Inject] private PlayerModel model;

    protected override void Init()
    {
        // model 已注入。
    }

    protected override void Quit()
    {
    }
}
```

`ModelBase` 和 `CtrlBase` 已通过内部 `IMCBase` 实现 `IInjectAble`。

## 注册和进入

```csharp
private readonly List<ModelBase> models = new()
{
    new PlayerModel()
};

private readonly List<CtrlBase> ctrls = new()
{
    new PlayerCtrl()
};

protected override void Startup()
{
    this.UseValues();
    this.UseMvc(models, ctrls);
    this.EnterMvc();
}
```

生命周期：

```text
UseMvc
  -> 把每个 Model/Ctrl 按具体类型注册到 ValueService

EnterMvc
  -> 先注入全部 Model
  -> 再注入全部 Ctrl
  -> Init 全部 Model
  -> Init 全部 Ctrl

Game.Quit
  -> Quit 全部 Ctrl
  -> Quit 全部 Model
```

## 获取 Model/Ctrl

获取服务：

```csharp
IMvcService mvc = this.Mvc();
PlayerModel model = mvc.Values().Get<PlayerModel>();
PlayerCtrl ctrl = mvc.Values().Get<PlayerCtrl>();
```

也可以使用：

```csharp
PlayerModel model = mvc.GetModel<PlayerModel>();
PlayerCtrl ctrl = mvc.GetCtrl<PlayerCtrl>();
```

`MvcServiceEx.Mvc()` 按 `IMvcService` 查询，与 `UseMvc` 的注册类型一致。也可以直接调用 `GetService<IMvcService>()` 获取可选服务。

## MVC 注意事项

- 必须先 `UseValues()`。
- Model/Ctrl 列表中的元素不要为 null。
- 同一具体类型注册多次会覆盖 ValueService 空名称实例，但列表生命周期仍会全部执行。
- Init 中不要假设 UI 已加载；用事件或状态流程连接。
- Ctrl Quit 先于 Model Quit，适合 Ctrl 解除对 Model 的监听。

## 游戏状态服务

状态机由 `IGameStateService` 管理当前 `IGameState`，每帧调用当前状态的 `Update()`。

## 定义状态

```csharp
public sealed class LobbyState : IGameState
{
    [Inject] private IInventoryService inventory;

    public void Init()
    {
        // EnterState 前统一初始化一次。
    }

    public void OnEnter(IGameState exit)
    {
        // 从 exit 进入当前状态。
    }

    public void Update()
    {
    }

    public void OnExit(IGameState enter)
    {
        // 即将进入 enter。
    }

    public void Quit()
    {
    }
}
```

`IGameState` 继承 `IInjectAble`，可直接使用 `[Inject]`。

## 注册并进入初始状态

```csharp
private readonly List<IGameState> states = new()
{
    new BootState(),
    new LobbyState(),
    new BattleState()
};

protected override void Startup()
{
    this.UseValues();
    this.UseState(states);
    this.EnterState<BootState>();
}
```

`UseState`：

- 注册 `IGameStateService`。
- 把每个状态按具体类型注册到 ValueService。
- 在 OnUse 时绑定 Game Update。

`EnterState<T>`：

- 先 Enter `IGameStateService`。
- 注入并 Init 全部状态。
- 切换到 T。

初始 Enter 只应执行一次。后续切换用 `SwitchState<T>()`。

## 状态切换顺序

```text
old.OnExit(new)
old.ClearDisposable()
current = new
new.OnEnter(old)
OnGameStateChange(old, new)
```

绑定到旧状态的事件和取消源可以通过 `AddTo(oldState)` 自动在退出时清理。

## 切换和查询

```csharp
IGameStateService stateService = this.State();

stateService.SwitchState<LobbyState>();
stateService.SwitchState(typeof(BattleState));

LobbyState lobby = stateService.FindState<LobbyState>() as LobbyState;
IGameState current = stateService.GetCurrentState();
```

传入状态实例的重载实际按实例类型查找已注册状态，不会直接把任意新实例设为当前状态。

## 监听切换

```csharp
void OnStateChanged(IGameState exit, IGameState enter)
{
}

stateService.ListenStateChange(OnStateChanged);
stateService.RemoveListenStateChange(OnStateChanged);
```

服务退出时会清空切换事件。

## 状态退出

Game 退出时：

1. 解绑 Game Update。
2. 当前状态置空。
3. 清空状态变化事件。
4. 按列表顺序调用每个状态 `Quit()`。

## 状态机实践

### 状态负责流程，不持有全局数据

长期数据放 Service/Model；State 负责进入、退出、UI 和场景编排。

### OnEnter 创建临时绑定

```csharp
public void OnEnter(IGameState exit)
{
    Events.Subscribe("battle.end", _ => ExitBattle()).AddTo(this);
    new CancellationTokenSource().AddTo(this);
}
```

切换时 `ClearDisposable()` 自动清理。

### 不在 Update 中频繁查服务

用 `[Inject]` 缓存接口，减少字典查询和代码噪音。

### 防止重入切换

`state` setter 会同步执行 OnExit/OnEnter。不要在同一个切换回调中无条件再次切换，避免递归流程。
