# IFramework

IFramework 是一个面向 Unity 的轻量游戏框架，提供主线程异步任务、事件、对象池、服务容器与依赖注入、MVC、状态机、配置存储、GameObject 池、红点、Undo，以及带代码生成工具的 UI 运行时。

[在线文档](https://onclick9927.github.io/IFramework-Unity/#/) · [仓库内完整文档](docs/README.md) · [安装与升级](docs/Docs/1-installation.md) · [快速开始](docs/Docs/2-quick-start.md) · [API 速查](docs/Docs/14-api-reference.md) · [性能与故障排查](docs/Docs/15-performance-troubleshooting.md)

## 安装

在 Unity Package Manager 中选择 `Add package from git URL...`，输入：

```text
https://github.com/OnClick9927/IFramework-Unity.git#src
```

包名为 `com.woo.iframework`，最低 Unity 版本声明为 `2019.4`。仓库开发工程使用 Unity `2021.3.33f1c1`，接入其他版本时请在目标平台完整回归。

## 功能

| 模块 | 能力 |
| --- | --- |
| Core | `Game` 生命周期、`AsyncTask`、Events、Wait/Notify、对象池、数组池、日志、单例 |
| Services | 服务容器、Value/DI、MVC、State、Pref、GameObjectPool、RedDot、Undo |
| UI | 自定义资源加载、同步/异步显隐、层级与全屏规则、View/Widget、Item 池、代码生成、安全区适配 |
| Editor | RootWindow、项目设置、类型/路径/脚本工具、UI 配置与生成、组件添加回调、红点调试 |

资源系统不与 UIService 绑定。`IUIDelegate` 可以接入 Resources、Addressables、WooAsset 或项目自有资源方案。

## 最小入口

```csharp
using IFramework;

public sealed class AppGame : Game
{
    protected override void Startup()
    {
        this.UseValues();
        this.UseRedTree();
        this.UseUndo();

        // 先 Use 全部服务，再 Enter 需要初始化的服务。
    }

    protected override void OnQuit()
    {
        // 业务退出逻辑；服务随后按注册逆序退出。
    }
}
```

将 `AppGame` 挂到启动场景中的启用 GameObject。框架会建立 `Launcher`、设置 `Game.Current` 并调用 `Startup()`。

## 文档导航

| 主题 | 文档 |
| --- | --- |
| 定位、模块和版本 | [框架简介](docs/Docs/0-overview.md) |
| UPM、本地包、升级 | [安装与升级](docs/Docs/1-installation.md) |
| 第一个 Game 与服务 | [快速开始](docs/Docs/2-quick-start.md) |
| Launcher、Game、服务生命周期 | [架构与生命周期](docs/Docs/3-architecture.md) |
| 异步、取消、组合和边界 | [AsyncTask](docs/Docs/4-async-task.md) |
| 发布订阅、等待通知 | [Events](docs/Docs/5-events.md) |
| 对象池和数组池 | [Pool](docs/Docs/6-pool.md) |
| 服务容器、Value 和注入 | [服务与依赖注入](docs/Docs/7-services-di.md) |
| Model/Ctrl 与游戏状态 | [MVC 与 State](docs/Docs/8-mvc-state.md) |
| 配置持久化与 GameObject 复用 | [Pref 与 GameObjectPool](docs/Docs/9-pref-gameobject-pool.md) |
| 红点树、撤销和重做 | [RedDot 与 Undo](docs/Docs/10-reddot-undo.md) |
| UIService、View、Widget、异步显隐 | [UI 运行时](docs/Docs/11-ui-runtime.md) |
| UI 配置与代码生成 | [UI 编辑器工作流](docs/Docs/12-ui-editor.md) |
| RootWindow 与编辑器工具 | [Editor 工具](docs/Docs/13-editor-tools.md) |
| 公开入口集中查询 | [API 速查](docs/Docs/14-api-reference.md) |
| GC、CPU、缓存局部性和排错 | [性能与故障排查](docs/Docs/15-performance-troubleshooting.md) |
| 仓库示例、自定义扩展、测试与贡献 | [示例与贡献](docs/Docs/16-examples-contributing.md) |

## 示例

`Assets/Project/Examples` 包含可直接打开的 Pool、Record、RedPoint 和 UI 场景。详细阅读顺序见[示例、扩展与贡献](docs/Docs/16-examples-contributing.md)。

## 使用边界

- Runtime 主要按 Unity 主线程设计，不提供通用线程安全保证。
- `AsyncTask` 是框架的 PlayerLoop 异步工具，不等同于完整的 .NET `Task` 调度模型。
- UI 资源加载、资源句柄与销毁策略由业务 `IUIDelegate` 负责。
- 池化对象必须完整重置；事件、Update、UnityEvent 和取消源必须有明确释放点。

## 交流

QQ Group：782290296

[![Stargazers over time](https://starchart.cc/OnClick9927/IFramework-Unity.svg?variant=adaptive)](https://starchart.cc/OnClick9927/IFramework-Unity)

```csharp
while (true)
    Console.Write("Thanks For EveryOne Who Used It Once!");
```
