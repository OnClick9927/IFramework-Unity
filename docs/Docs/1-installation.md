# 安装与升级

## 环境要求

- Unity `2019.4` 或更高版本。
- 项目启用 Unity UI（uGUI）；UI 模块使用 `Canvas`、`GraphicRaycaster`、`CanvasScaler` 等类型。
- 使用 Git URL 安装时，本机需要可执行的 Git。
- 自定义 asmdef 项目需要引用 `IFramework`；编辑器程序集只在 Editor 平台可用。

仓库使用 Unity `2021.3.33f1c1` 开发。对生产项目，建议优先使用同代或更高的 LTS，并在升级 Unity 后执行一次 UI 和构建回归。

## 方式一：Unity Package Manager

1. 打开 `Window > Package Manager`。
2. 点击左上角 `+`。
3. 选择 `Add package from git URL...`。
4. 输入：

```text
https://github.com/OnClick9927/IFramework-Unity.git#src
```

5. 点击 `Add`，等待 Unity 导入和编译。

也可以直接编辑项目 `Packages/manifest.json`：

```json
{
  "dependencies": {
    "com.woo.iframework": "https://github.com/OnClick9927/IFramework-Unity.git#src"
  }
}
```

> [!NOTE]
> Git URL 中的 `#src` 是仓库用于 UPM 发布的引用。若你固定到 tag 或 commit，请确认该引用下仍以 `package.json` 为包根目录。

## 方式二：源码导入

1. 克隆仓库。
2. 将 `Assets/IFramework` 复制或以 Git Submodule 方式放入目标工程的 `Assets`。
3. 保留 `.meta` 文件，避免 GUID 改变。
4. 等待 Unity 编译 `IFramework` 和 `IFramework.Editor` 两个程序集。

源码方式适合需要调试框架内部、维护私有补丁或和主仓库同步的团队。普通使用优先 UPM，升级边界更清晰。

## asmdef 引用

如果业务代码有自己的 asmdef，在 Inspector 的 `Assembly Definition References` 中添加：

```text
IFramework
```

业务的 Editor asmdef 如需调用 `EditorTools` 或编辑器扩展，再添加：

```text
IFramework.Editor
```

不要让 Runtime asmdef 引用 `IFramework.Editor`。后者的 `includePlatforms` 仅包含 `Editor`，进入 Player 构建时不可用。

## 安装验证

完成导入后检查：

1. Console 没有 `IFramework` 编译错误。
2. 菜单存在 `Tools > IFramework > Window`。
3. 可以创建一个继承 `Game` 的组件并编译：

```csharp
using IFramework;

public sealed class DemoGame : Game
{
    protected override void Startup()
    {
        Log.L("IFramework started");
    }
}
```

4. 将 `DemoGame` 挂到场景中的 GameObject，进入 Play Mode。

`Launcher` 会在运行前自动创建，并把当前 `Game` 放到自身节点下。一个运行时只应有一个有效的 `Game.Current`。

## 日志适配

`Log` 依赖 `ILogger`。编辑器工具初始化时会设置 Unity Logger，但独立 Runtime 使用建议显式提供：

```csharp
using System;
using IFramework;
using UnityEngine;

public sealed class UnityRuntimeLogger : ILogger
{
    public void Log(string message, params object[] args) => Debug.LogFormat(message, args);
    public void Warn(string message, params object[] args) => Debug.LogWarningFormat(message, args);
    public void Error(string message, params object[] args) => Debug.LogErrorFormat(message, args);
    public void Exception(Exception exception) => Debug.LogException(exception);
    public void Assert(bool condition, string message, params object[] args) =>
        Debug.AssertFormat(condition, message, args);
}
```

在游戏启动早期设置：

```csharp
Log.logger = new UnityRuntimeLogger();
```

## 升级

### UPM 安装

Package Manager 中选择 IFramework，点击更新；Git 引用没有版本变化时，Unity 可能继续使用缓存。可以固定 tag/commit，或在 `manifest.json` 中改变引用后重新解析。

### 源码安装

1. 提交或备份本地修改。
2. 比较 `Assets/IFramework`，不要覆盖业务目录。
3. 保留目标工程中的 `.meta` 或确保 GUID 迁移一致。
4. 更新后检查 API 变化、生成代码和 UI 配置 JSON。
5. 执行 Play Mode、目标平台构建和关键 UI 流程测试。

## 卸载

卸载前先搜索业务引用：

```text
using IFramework
Game
AsyncTask
Events
UseValues / UseUI / UseState / UsePref
```

然后：

- UPM：从 Package Manager 移除包，或删除 `manifest.json` 对应依赖。
- 源码：删除 `Assets/IFramework` 及其 `.meta`。

最后移除业务 asmdef 对 `IFramework` / `IFramework.Editor` 的引用。

## 常见安装问题

### 找不到 Git executable

安装 Git，并确保 Unity Hub/Unity Editor 进程可以读取系统 `PATH`，然后重启 Unity。

### asmdef 循环引用

业务 Runtime 只能引用 `IFramework`。不要在 `IFramework` 中反向引用业务程序集；通过接口、事件或资源适配器连接业务。

### 找不到 UnityEngine.UI

确认项目包含 uGUI 包，并且没有通过自定义 asmdef 排除 UI 引用。

### Editor 类型进入 Player

业务 Runtime 文件不要 `using UnityEditor`，也不要引用 `IFramework.Editor`。需要双环境逻辑时使用 Editor 目录或 `#if UNITY_EDITOR`。
