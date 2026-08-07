# 编辑器工具

## RootWindow

入口：

```text
Tools > IFramework > Window
Ctrl/Cmd + Shift + I
```

RootWindow 提供：

- 左侧树形工具导航。
- 搜索。
- 内置 UserOptionTab。
- 外部 EditorWindow 的统一打开/关闭。
- 可选 Dock 行为。

工具通过反射发现 UserOptionTab 子类和 `EditorWindowCache` 注册窗口，因此业务 Editor 程序集可以扩展窗口树。

## Project Config

RootWindow 的 Project 配置包含：

- UserName。
- Version。
- Namespace。
- 日志开关。
- Frame Log 开关。
- 编辑器窗口 Dock 设置。
- ScriptMarkFlag 等代码生成设置。

配置通过 EditorPrefs/PrefService 保存。团队共享的关键配置不要只依赖本机 EditorPrefs，应在项目配置资产或版本库中另行维护。

## 常用菜单

### 打开目录

```text
Tools > IFramework > Open Path > Persistent
Tools > IFramework > Open Path > Streaming
Tools > IFramework > Open Path > Assets
Tools > IFramework > Open Path > Temporary
Tools > IFramework > Open Path > Console
```

用于快速定位 `Application.*Path`。

### 复制层级路径

选中单个 Transform：

```text
GameObject > Copy Path
```

把层级路径写入系统剪贴板，可用于 `GetTransform(path)` 或生成调试信息。

### GitHub 与社区

```text
Tools > IFramework > Github
Tools > IFramework > Join us
```

## EditorTools 主要能力

`EditorTools` 是 partial class，按功能拆文件。

### DragAndDropTool

用于 IMGUI 区域接收 Project/Hierarchy 拖拽对象。编写自定义窗口时优先复用，而不是重复处理 DragUpdated/DragPerform。

### FolderField

绘制目录选择字段并规范化 Unity 项目路径。

### ProjectConfig

读取和保存项目级编辑器设置，包括命名空间、作者和窗口行为。

### RectEx / SplitView

提供 Rect 缩放、横向/纵向分割和可拖拽分栏，RootWindow 与 UI 编辑器使用这些工具构建 IMGUI 布局。

### ScriptCreator

模板代码生成和脚本标记区更新。生成代码时应保留标记注释，以便后续只替换自动区域。

### EditorWindowTool

用于：

- 注册/查找窗口。
- 统一创建窗口。
- 集成 RootWindow 导航。

仓库红点示例通过：

```csharp
EditorTools.EditorWindowTool.Create("RedPoint");
```

打开运行时红点查看器。

## 路径工具

```csharp
string regular = path.ToRegularPath();
string combined = path.CombinePath("Child/File.cs");
```

`ToRegularPath` 把反斜杠转换为 `/`，适合 AssetDatabase path。

创建多个目录：

```csharp
EditorTools.CreateDirectories(paths);
```

## 类型发现

```csharp
IEnumerable<Type> allTypes = EditorTools.GetTypes();
IEnumerable<Type> implementations = typeof(IMyTool).GetSubTypesInAssemblies();
```

支持接口、普通基类和泛型基类查找。

注意：遍历 `AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetTypes)` 遇到加载不完整的程序集时可能抛 ReflectionTypeLoadException。大型项目可在业务封装中增加容错和缓存。

## 定位脚本

```csharp
string scriptPath = EditorTools.LocateScript(typeof(MyType));
EditorTools.DrawPingScript("Script", typeof(MyType));
```

LocateScript 先按文件名查找，再扫描脚本文本中的 class/struct/enum 声明。项目脚本很多时这是昂贵操作，不应在每帧 OnGUI 无缓存调用。

## OnAddComponentAttribute

```csharp
[OnAddComponent(typeof(MyComponent))]
static void Configure(MyComponent component)
{
    // 添加组件后设置默认属性。
}
```

内置 UIOptimize 使用此机制调整 uGUI 默认值。

方法应：

- 是静态方法。
- 参数类型和 Attribute type 对应。
- 快速执行；需要等 Unity 初始化子对象时使用 delayCall。
- 使用 Undo/Dirty 标记时遵循自定义 Editor 工具规范。

## UI 组件上下文菜单

内置：

- Text/Image/RawImage/Empty4Raycast/PolygonRaycastImage：`Remove Component`。
- Image：`Replace To PolygonRaycastImage`。

Remove Component 会同时销毁 CanvasRenderer。只在确定节点不再承担 Graphic 渲染时使用。

## PolygonRaycast 编辑

`PolygonRaycastGraphicDrawTool` 在 Scene 视图编辑 Points。运行时：

- Points 为空：使用 RectTransform 矩形点击。
- Points 非空：转换点击到本地坐标并执行多边形相交测试。

适合非矩形按钮、空心点击区和减少透明区域误触。

## RedPoint Window

在 Play Mode 查看当前 RedTree：

- Path。
- 聚合 Count。
- 绑定 Dot 数量。
- 搜索和双击定位。

树在 `FreshDots` 触发 OnFresh 时刷新，也可以点击 Fresh。

## UI Runtime Inspector

GameObjectView Tab 在 Play Mode 读取所有 UIService：

- 选择命名 UI 模块。
- 查看 visible、top 和每层 top/topShow。
- Hierarchy 模式查看 Canvas 结构。

适合排查：

- FullScreen 为什么隐藏某个面板。
- Panel 实际进入了哪个层。
- Show 后 sibling order 是否正确。
- 多 UIService 是否取错 name。

## Editor 日志

`EditorTools` 初始化时设置 `Log.logger = new UnityLogger()`，将 ILogger 转发到 Unity Debug。

日志开关：

```csharp
Log.enable = true;
Log.enable_L = true;
Log.enable_W = true;
Log.enable_E = true;
Log.enable_F = true;
```

框架内部 `FL/FW/FE` 带 IFramework 前缀；业务使用 `L/W/E/Exception/A`。

## 自定义 Editor 扩展建议

- 业务扩展放在 `Editor` 目录和独立 Editor asmdef。
- 引用 `IFramework.Editor`，不要让 Runtime 反向依赖。
- 复杂窗口复用 RootWindow/EditorWindowTool，但保持模块独立。
- 频繁 OnGUI 操作缓存反射、AssetDatabase 和文件扫描结果。
- 修改资产后使用 SerializedObject、Undo.RecordObject、SetDirty 和 SaveAssets 的标准流程。
- 生成文件路径统一用 `/`，提交版本库。
