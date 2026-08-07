# IFramework 简介

IFramework 是一个面向 Unity 项目的轻量游戏框架。它把项目中反复出现的基础能力集中到三个区域：

- `Core`：异步任务、事件、等待/通知、对象池、单例、日志和生命周期绑定。
- `Services`：可组合的游戏服务容器，以及 DI、MVC、状态机、配置存储、GameObject 池、红点和 Undo。
- `UI`：可替换资源加载方式的界面管理、层级与全屏遮挡、View/Widget 生命周期、代码生成和编辑器工作流。

框架的目标不是接管整个项目，而是提供一组可以按需启用的基础设施。业务入口继承 `Game`，在 `Startup()` 中选择服务，然后通过接口、扩展方法或 `[Inject]` 使用它们。

## 适用场景

IFramework 适合以下项目：

- 需要统一游戏启动、退出和服务生命周期的 Unity 项目。
- 希望用同一套代码组织 UI、状态、红点、配置和 Undo 的中小型团队。
- 资源系统需要可替换，UI 框架不能依赖固定的 `Resources`、Addressables 或 AssetBundle 实现。
- 需要低成本的事件系统、主线程异步工具和对象池。
- 希望生成 UI 字段绑定代码，减少 `transform.Find` 和手工拖引用。

以下情况需要先评估：

- 多线程任务调度。IFramework 的 Runtime 工具主要围绕 Unity 主线程设计，不是 `System.Threading.Tasks` 的替代品。
- 需要标准 .NET `Task` 的异常传播、调度器和线程同步语义。
- 需要完整 ECS、网络层、资源热更新或序列化解决方案。这些不属于本仓库职责。

## 包结构

```text
Assets/IFramework/
├─ Runtime/
│  ├─ Core/                 # 基础能力
│  ├─ Services/             # 游戏服务
│  ├─ UI/                   # UI 运行时
│  └─ IFramework.asmdef
├─ Editor/
│  ├─ Core/                 # 通用编辑器工具和 RootWindow
│  ├─ UI/                   # UI 配置、代码生成、红点查看器
│  └─ IFramework.Editor.asmdef
├─ package.json
└─ README.md
```

程序集：

| 程序集 | 平台 | 用途 |
| --- | --- | --- |
| `IFramework` | 全平台 | Core、Services、UI Runtime |
| `IFramework.Editor` | Editor | 编辑器窗口、代码生成和调试工具 |

## 功能地图

| 需求 | 入口 | 文档 |
| --- | --- | --- |
| 创建游戏入口 | `Game` | [架构与生命周期](3-architecture.md) |
| 主线程异步 | `AsyncTask`、`CancellationTokenSource` | [异步任务](4-async-task.md) |
| 发布订阅 | `Events` | [事件与等待](5-events.md) |
| 普通对象/数组复用 | `ObjectPool<T>`、`StaticPool`、`ArrayPool<T>` | [对象池](6-pool.md) |
| 注册与获取服务 | `IServiceCollection`、`ServiceBase` | [服务与依赖注入](7-services-di.md) |
| 字段注入 | `IValueService`、`[Inject]` | [服务与依赖注入](7-services-di.md) |
| MVC | `UseMvc`、`ModelBase`、`CtrlBase` | [MVC 与状态机](8-mvc-state.md) |
| 游戏状态 | `UseState`、`IGameState` | [MVC 与状态机](8-mvc-state.md) |
| 配置存储 | `UsePref`、`IPrefLoader`、`IPrefConverter` | [Pref 与 GameObject 池](9-pref-gameobject-pool.md) |
| GameObject 复用 | `UseGameObjectPool` | [Pref 与 GameObject 池](9-pref-gameobject-pool.md) |
| 红点树 | `UseRedTree` | [红点与 Undo](10-reddot-undo.md) |
| 撤销/重做 | `UseUndo` | [红点与 Undo](10-reddot-undo.md) |
| UI 管理 | `UseUI`、`UIService` | [UI 运行时](11-ui-runtime.md) |
| UI 代码生成 | `Tools/IFramework/Window` | [UI 编辑器工作流](12-ui-editor.md) |

## 设计原则

### 服务按需启用

除 `Launcher` 外，功能不会自动塞进游戏。服务在 `Game.Startup()` 中通过 `UseXxx` 注册，通过 `EnterXxx` 完成需要的初始化。

### 接口隔离实现

大多数内置服务实现类是内部类型，业务代码面向 `IValueService`、`IGameStateService`、`IRedTreeService` 等接口。这样可以替换实现，也减少业务对内部结构的依赖。

### Unity 主线程优先

`AsyncTask.Delay`、`NextFrame`、状态更新和 UI 操作都绑定 Unity PlayerLoop。除明确由业务保证外，不应从后台线程调用框架容器、事件、对象池或 UI API。

### 生命周期显式

- `Game.Startup()`：注册服务、绑定业务。
- `EnterService<T>()`：进入服务，执行注入和初始化。
- `Game.Quit()`：按注册逆序退出服务并释放绑定到 Game 的 `IDisposable`。
- `UIView.OnClose()`：清理 View 字段、子 Widget、事件绑定和 WidgetPool。

## 版本与兼容性

当前 UPM 包信息位于 `Assets/IFramework/package.json`：

- 包名：`com.woo.iframework`
- Unity 最低版本声明：`2019.4`
- 仓库当前开发工程：Unity `2021.3.33f1c1`

建议在项目采用的 Unity LTS 版本中完整验证 UI、编辑器脚本和目标平台构建。包声明表示最低目标，不等于所有较新 Unity 版本都无需回归。

## 阅读顺序

第一次接入建议按以下顺序阅读：

1. [安装与升级](1-installation.md)
2. [快速开始](2-quick-start.md)
3. [架构与生命周期](3-architecture.md)
4. 根据项目选择 Core、Services 或 UI 专题
5. 开发遇到问题时查阅 [API 速查](14-api-reference.md) 和 [性能与排错](15-performance-troubleshooting.md)

## 仓库示例

`Assets/Project/Examples` 包含四类示例：

- `Pool`：GameObject 池。
- `Record`：Undo/Redo。
- `RedPoint`：红点树和编辑器查看器。
- `UI`：UIService、ViewBridge、代码生成字段、WidgetPool 和事件绑定。

示例是理解实际初始化顺序的首选材料，专题文档会注明对应目录。
