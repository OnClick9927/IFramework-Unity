# IFramework 文档

IFramework 为 Unity 项目提供 Core、Services 和 UI 三组可组合能力。文档以当前仓库源码为准，包含安装、运行时语义、编辑器工作流、API 查询、性能建议和故障排查。

## 从这里开始

- 初次了解：[框架简介](Docs/0-overview.md)
- 安装或升级：[安装与升级](Docs/1-installation.md)
- 建立第一个入口：[快速开始](Docs/2-quick-start.md)
- 查询方法和边界：[API 速查](Docs/14-api-reference.md)
- 定位 GC、CPU 或运行故障：[性能与故障排查](Docs/15-performance-troubleshooting.md)

## 模块地图

```text
IFramework
├─ Core
│  ├─ Game / Launcher
│  ├─ AsyncTask / CancellationToken
│  ├─ Events / Wait / Notify
│  └─ ObjectPool / ArrayPool / Log / Singleton
├─ Services
│  ├─ ServiceCollection / Value / Inject
│  ├─ MVC / State
│  ├─ Pref / GameObjectPool
│  └─ RedDot / Undo
└─ UI
   ├─ UIService / Layer / PanelCollection
   ├─ IUIDelegate / ViewBridge / UIView
   ├─ Widget / WidgetPool
   └─ Editor 配置、代码生成与调试工具
```

## 全部章节

| 序号 | 章节 | 内容 |
| --- | --- | --- |
| 0 | [框架简介](Docs/0-overview.md) | 定位、适用范围、目录、程序集、设计原则 |
| 1 | [安装与升级](Docs/1-installation.md) | Git UPM、本地包、版本、升级和卸载 |
| 2 | [快速开始](Docs/2-quick-start.md) | Game、自定义服务、DI、Events、AsyncTask、Pool |
| 3 | [架构与生命周期](Docs/3-architecture.md) | Launcher、Game、Use/Enter/Quit、命名服务、所有权 |
| 4 | [异步任务](Docs/4-async-task.md) | Awaiter、完成、异常、取消、组合、时间和边界 |
| 5 | [事件与等待](Docs/5-events.md) | 同步/异步发布、订阅、Wait/Notify、释放 |
| 6 | [对象池](Docs/6-pool.md) | ObjectPool、StaticPool、ArrayPool、重置协议 |
| 7 | [服务与依赖注入](Docs/7-services-di.md) | 服务容器、Value、字段注入、初始化顺序 |
| 8 | [MVC 与状态机](Docs/8-mvc-state.md) | Model、Ctrl、State、切换和生命周期 |
| 9 | [Pref 与 GameObject 池](Docs/9-pref-gameobject-pool.md) | 转换/存储适配器、Prefab 实例复用 |
| 10 | [红点与 Undo](Docs/10-reddot-undo.md) | 路径树、批量刷新、记录、撤销和重做 |
| 11 | [UI 运行时](Docs/11-ui-runtime.md) | 层级、加载、View、同步/异步显隐、Widget |
| 12 | [UI 编辑器工作流](Docs/12-ui-editor.md) | 配置、收集、标记、生成、优化和调试 |
| 13 | [编辑器工具](Docs/13-editor-tools.md) | RootWindow、ProjectConfig、菜单和扩展 |
| 14 | [API 速查](Docs/14-api-reference.md) | 各模块公开入口、参数语义和所有权 |
| 15 | [性能与故障排查](Docs/15-performance-troubleshooting.md) | GC、内存、CPU、缓存局部性、Profiler、问题矩阵 |
| 16 | [示例与贡献](Docs/16-examples-contributing.md) | 四类示例、自定义扩展、测试和提交检查 |

## 推荐阅读路径

### 只使用 Core

```text
简介 -> 安装 -> 快速开始 -> 架构 -> AsyncTask -> Events -> Pool
```

### 使用游戏服务

```text
架构 -> 服务与 DI -> MVC/State -> Pref/GameObjectPool -> RedDot/Undo
```

### 接入 UI

```text
架构 -> AsyncTask -> UI 运行时 -> UI 编辑器工作流 -> 示例
```

## 版本说明

- UPM 包：`com.woo.iframework`
- 当前包版本：`1.3.40`
- 最低 Unity 声明：`2019.4`
- 仓库开发工程：Unity `2021.3.33f1c1`

不同 Unity LTS、Mono/IL2CPP 和目标平台可能影响反射、资源、序列化与 Editor API。升级后应重新执行编译、示例场景和目标平台测试。

## 获取帮助

先查看 [性能与故障排查](Docs/15-performance-troubleshooting.md)。提交问题时请提供框架 commit、Unity 完整版本、平台、脚本后端、最小复现和首个有效调用栈。

- [GitHub 仓库](https://github.com/OnClick9927/IFramework-Unity)
- QQ Group：782290296
