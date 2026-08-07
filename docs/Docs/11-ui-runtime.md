# UI 运行时

## 组成

```text
UIService
├─ UILayerData             # 层级名称和顺序
├─ PanelCollection         # path -> layer/fullScreen/load metadata
├─ IUIDelegate             # 资源加载、销毁和全局通知
├─ IViewBridge             # path -> UIView 生命周期
├─ LoadPart                # 已加载 UIPanel 和加载计数
└─ LayerPart               # 层级节点、排序、可见性、射线遮罩
```

UIService 不绑定具体资源系统。业务通过 `IUIDelegate.LoadPanelAsync` 加载 Prefab，可以接 Resources、Addressables、WooAsset 或任意自定义实现。

## 1. UILayerData

在 Project 窗口创建：

```text
Create > IFramework > UILayerData
```

默认层级从低到高：

```text
Background
Mid
Pop
Guide
Toast
Top
```

列表索引就是 `PanelCollection.Data.layer`。运行时会按该顺序创建全屏 RectTransform 节点。

API：

```csharp
List<string> names = layerData.GetLayerNames();
int index = layerData.LayerNameToIndex("Pop");
string name = layerData.GetLayerName(index);
```

修改层级后要重新生成或校正 PanelCollection 中的 index。

## 2. PanelCollection

```csharp
[Serializable]
public class PanelCollection
{
    public List<Data> datas;
}
```

每条 Data：

| 字段 | 说明 |
| --- | --- |
| `path` | 面板唯一标识，同时传给资源委托 |
| `isResourcePath` | 编辑器收集时是否来自 Resources；Runtime 不自动加载 |
| `layer` | UILayerData 索引 |
| `fullScreen` | 显示时是否遮挡更低 UI |

手工创建：

```csharp
var collection = new PanelCollection();
collection.datas.Add(new PanelCollection.Data
{
    path = "UI/LoginPanel",
    isResourcePath = true,
    layer = layerData.LayerNameToIndex("Pop"),
    fullScreen = true
});
```

`UIService` 构造时调用 `ListToMap()`。path 必须唯一；重复 path 会在字典构建时抛异常。

通常由编辑器工具生成 JSON：

```csharp
PanelCollection collection =
    JsonUtility.FromJson<PanelCollection>(collectionJson.text);
```

## 3. UIPanel Prefab

每个面板根节点挂 `IFramework.UI.UIPanel`：

- `Prefabs`：供生成 View 的 `FindPrefab(name)` 使用。
- `Adapt Type`：安全区适配方向。
- `Adapt Rect`：实际应用安全区 offset 的 RectTransform。

运行时状态：

```text
None -> OnLoad -> OnShow -> OnHide -> OnShow -> OnClose
```

`UIPanel.visible` 由 CanvasGroup 的 `blocksRaycasts` 表示。UIService 同步设置 alpha、blocksRaycasts 和 interactable。

## 4. 实现 IUIDelegate

最小 Resources 示例：

```csharp
using IFramework;
using IFramework.UI;
using UnityEngine;

public sealed class GameUIDelegate : IUIDelegate
{
    async AsyncTask<UIPanel> IUIDelegate.LoadPanelAsync(
        RectTransform parent,
        PanelCollection.Data data)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>(data.path);
        while (!request.isDone)
            await AsyncTask.NextFrame();

        var prefab = request.asset as GameObject;
        if (prefab == null)
            return null;

        GameObject instance = Object.Instantiate(prefab, parent);
        return instance.GetComponent<UIPanel>();
    }

    void IUIDelegate.DestroyPanel(GameObject gameObject)
    {
        Object.Destroy(gameObject);
    }

    // 其余回调可用于埋点、音效、顶部资源栏和全局 UI 协调。
    public void OnFullScreenCount(bool hide, int count) { }
    public void OnLayerTopChange(int layer, string path) { }
    public void OnTopShowChange(int layer, string path) { }
    public void OnLayerTopShowChange(int layer, string path) { }
    public void OnVisibleChange(string path, bool visible) { }
    public void OnPanelClose(string path) { }
    public void OnPanelHide(string path) { }
    public void OnPanelLoad(string path) { }
    public void OnPanelShow(string path) { }
    public void OnClosePanelAsync(string path) { }
    public void OnHidePanelAsync(string path) { }
    public void OnShowPanelRequest(string path) { }
}
```

重要约束：

- 成功加载必须返回带 `UIPanel` 的实例。
- path 不在 PanelCollection 或加载返回 null 时，当前 Show operation 不会自动成功完成。应在接入期校验全部配置和资源。
- DestroyPanel 必须匹配资源系统的实例释放方式；Addressables 通常不能简单替换成 `Object.Destroy`。

## 5. ViewBridge 与 UIView

定义 View：

```csharp
public sealed class LoginView : UIView
{
    private Button loginButton;

    protected override void InitComponents()
    {
        loginButton = GetComponent<Button>("LoginButton@sm");
    }

    protected override void OnLoad()
    {
        this.BindButton(loginButton, Login);
    }

    protected override void OnShow() { }
    protected override void OnHide() { }
    protected override void OnClose() { }
}
```

path/type 映射：

```csharp
var viewMap = new Dictionary<string, Type>
{
    ["UI/LoginPanel"] = typeof(LoginView)
};

IViewBridge bridge = new ViewBridge(viewMap);
```

编辑器代码生成器会生成等价 map。

`ViewBridge.Subscribe` 在面板首次加载时创建 UIView、绑定 UIPanel 并执行 `InitComponents()`。Close 后 View 被移出 Bridge，`UIView.OnClose` 后自动 ClearFields。

多个 View 技术栈可以组合：

```csharp
var bridge = new MixedViewBridge(new IViewBridge[]
{
    new ViewBridge(aotMap),
    customLuaBridge
});
```

MixedViewBridge 会依次尝试 Subscribe，并记住每个 path 由哪个 Bridge 接管。

## 6. 创建 UIService

```csharp
public sealed class UIGame : Game
{
    public UILayerData layers;
    public TextAsset panelCollectionJson;

    protected override void Startup()
    {
        this.UseValues();

        var collection = JsonUtility.FromJson<PanelCollection>(
            panelCollectionJson.text);

        UIService ui = this.UseUI(
            layers,
            collection,
            new ViewBridge(GeneratedPanelNames.map),
            new GameUIDelegate());
    }
}
```

`UseUI` 会：

1. 构造并注册命名 UIService，默认名 `UI`。
2. 设置 Delegate 和 Bridge。
3. 创建或使用 Canvas。
4. 创建层级。
5. 根据屏幕比例调整 CanvasScaler。

获取：

```csharp
UIService ui = game.UI();
UIService secondary = game.UI("secondary-ui");
```

使用已有 Canvas：

```csharp
game.UseUI(layers, collection, bridge, del, existingCanvas);
```

Canvas 没有 BaseRaycaster 时，框架创建 `RayCast` 层和 `Empty4Raycast`；已有 GraphicRaycaster 时，通过启停它控制输入。

## 7. Show

```csharp
await ui.Show(PanelNames.Login);
```

首次显示：

```text
OnShowPanelRequest
-> 拒绝 UI Raycast（加载计数 > 0）
-> IUIDelegate.LoadPanelAsync
-> UIPanel.SetPath / OnLoad state
-> 放入 layer
-> ViewBridge.Subscribe
-> UIView.OnLoad
-> IUIDelegate.OnPanelLoad
-> UIView.OnBecameVisible
-> UIView.OnShow
-> IUIDelegate.OnPanelShow
-> 更新 Top/FullScreen/Visible 通知
-> Show task 完成
-> 加载计数归零后恢复 Raycast
```

已加载但隐藏的面板再次 Show：

- 移到当前层末尾，即同层最上方。
- 不再执行 OnLoad。
- 执行 OnShow 和可见性重新计算。

同 path 在加载尚未完成时再次 Show，当前实现没有请求合并。业务应在按钮层防重复点击，或根据 `ui.IsLoading` 控制入口。

## 8. Hide 与 Close

```csharp
ui.Hide(path);
ui.Close(path);
```

两者都是 `async void`，先等待下一帧再执行：

- Hide：保留实例和 View，执行 OnHide，可再次 Show。
- Close：执行 OnClose、解绑 View、从层级和已加载表移除、调用 DestroyPanel。

关闭后不要继续持有 UIPanel 或 UIView 内部组件引用。

批量操作：

```csharp
ui.CloseAll();
ui.CloseWithout(PanelNames.PersistentHUD);
ui.CloseByLayer("Pop");
```

这些方法会为每个 path 调用延迟一帧的 Close。

## 9. 动画式 HideAsync/CloseAsync

```csharp
await ui.HideAsync(path);
await ui.CloseAsync(path);
```

UIService 创建 operation 并传给 View：

```csharp
protected override void OnHideAsync(AsyncTask operation)
{
    PlayHideAnimation(() => operation.SetResult());
}

protected override void OnCloseAsync(AsyncTask operation)
{
    PlayCloseAnimation(() => operation.SetResult());
}
```

operation 完成后 UIService 才调用实际 Hide/Close。

`UIView` 默认实现会立即完成 operation，因此没有动画时无需 override。自定义动画一旦 override，就必须在所有结束分支调用 `operation.SetResult()`：

```csharp
protected override void OnCloseAsync(AsyncTask operation) => operation.SetResult();
```

不存在该 path 时返回 `CompletedTask`。

## 10. 层级、全屏和可见性

同层最后 Show 的面板是 layer top。`GetTopShow()` 从最高层向下找第一个处于 OnShow 的面板。

当可见面板标记 `fullScreen` 后，更低层和同层更早的面板会被设置为不可见：

- CanvasGroup alpha = 0
- blocksRaycasts = false
- interactable = false
- View 收到 OnBecameInvisible

重新可见时收到 OnBecameVisible。

查询：

```csharp
bool open = ui.GetIsPanelOpen(path);
UIPanel panel = ui.FindPanel(path);
UIPanel top = ui.GetTopShow();
UIPanel layerTop = ui.GetLayerTop(layerIndex);
UIPanel layerTopShow = ui.GetLayerTopShow(layerIndex);
List<string> visible = ui.GetVisibleList();
```

返回的 List 是内部复用列表，不要长期保存或修改。

## 11. Raycast 控制

```csharp
ui.RefuseRayCast();
ui.AcceptRayCast();
```

加载中调用 Accept 不生效。

强制遮罩：

```csharp
ui.ForceRefuseRayCast();
// 完成不可交互流程后
ui.ForceAcceptRayCast();
```

ForceRefuse 会维持阻止状态，即使普通加载计数归零。必须成对调用。

## 12. View 字段和 Prefab

`GameObjectView` 提供：

```csharp
Transform GetTransform(string path);
GameObject GetGameObject(string path);
T GetComponent<T>(string path);
GameObject FindPrefab(string name);
```

生成代码把标记节点路径写入 `GetComponent`，把 UIPanel/ScriptCreatorContext 的 Prefabs 列表写入 `FindPrefab`。

FindPrefab 首次按 name 扫描，随后缓存。重新 SetGameObject/ClearFields 时清缓存。

## 13. WidgetView 与 WidgetPool

Widget 是面板内部可复用 View：

```csharp
public sealed class ItemWidget : WidgetView, IPoolAbleWidget
{
    protected override void InitComponents() { }

    public void OnSet()
    {
        // 归还前清理业务状态。
    }
}
```

创建池：

```csharp
WidgetPool<ItemWidget> pool =
    CreateWidgetPool<ItemWidget>(itemPrefab, itemParent);
```

获取和归还：

```csharp
ItemWidget item = pool.Get();
pool.Set(item);
```

Widget 层级：

```csharp
item.parent
item.root
item.FindViewInParent<T>()
item.FindViewInChildren<T>()
item.FindViewsInChildren(result)
```

归还时：

1. 调用 `IPoolAbleWidget.OnSet()`。
2. 隐藏 GameObject。
3. wrapper 和 GameObject 分别入队。
4. `ClearFields()` 清子 View、IDisposable、子 WidgetPool 和 Prefab 缓存。

UIView Close 时也会 ClearFields，自动清理其创建的 WidgetPool。

## 14. UnityEvent 生命周期

```csharp
this.BindButton(button, OnClick)
    .BindToggle(toggle, OnToggle)
    .BindSlider(slider, OnSlider)
    .BindInputField(input, OnInput);
```

`UnityEventHelper` 把 RemoveListener 封装成 IDisposable 并 `AddTo(this)`。UIView/Widget ClearFields 时统一解绑，避免 View 复用后重复监听。

通用绑定：

```csharp
owner.Bind(
    add: () => customEvent += Callback,
    remove: () => customEvent -= Callback);
```

## 15. 刘海屏适配

UIPanel 启用时，如果 `adaptRect` 存在，按 `AdaptType` 将 Screen.safeArea 转换成当前 Rect 尺寸的 offset。

测试时可以覆盖静态值：

```csharp
UIPanel.ScreenWidth = 1170;
UIPanel.ScreenHeight = 2532;
UIPanel.SafeArea = simulatedSafeArea;
```

屏幕旋转或分辨率变化时应更新这些值并重新调用 `AdaptNotchScreen()`。

## 16. 退出

UIService OnQuit：

- Dispose Bridge。
- 销毁 Canvas GameObject。

自定义 IUIDelegate 若持有资源句柄，应由 Game 或自定义 Bridge/Service 生命周期负责释放。

## 检查清单

- [ ] 每个 path 唯一且能被 Delegate 加载。
- [ ] 每个 Prefab 根有 UIPanel。
- [ ] 生成 map 的 path 与 PanelCollection 完全一致。
- [ ] 使用 Async Hide/Close 的 View 会完成 operation。
- [ ] 所有 UnityEvent 用 Bind 或在 Close 时解绑。
- [ ] Widget 归还时清业务状态。
- [ ] ForceRefuseRayCast 总能走到 ForceAcceptRayCast。
- [ ] 资源系统的实例销毁与 Release 语义匹配。
