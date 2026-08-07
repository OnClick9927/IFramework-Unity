# 示例、扩展与贡献

仓库内示例位于 `Assets/Project/Examples`。建议使用与项目一致的 Unity 版本打开仓库，等待脚本编译完成后逐个打开场景，而不是把示例脚本脱离场景单独复制。

## 1. 示例总览

| 示例 | 场景 | 关注点 |
| --- | --- | --- |
| Pool | `Assets/Project/Examples/Pool/New Scene.unity` | GameObject 池、取出和归还生命周期 |
| Record | `Assets/Project/Examples/Record/New Scene.unity` | Undo/Redo 记录 |
| RedPoint | `Assets/Project/Examples/RedPoint/New Scene.unity` | 红点树、节点刷新 |
| UI | `Assets/Project/Examples/UI/New Scene.unity` | UILayerData、PanelCollection、代码生成、Widget |

示例代码以展示框架连接方式为主，不代表生产项目的资源目录、错误处理和配置方案。

## 2. Pool 示例

相关文件：

```text
Assets/Project/Examples/Pool/
├─ New Scene.unity
├─ PoolTest.cs
├─ CubeView.cs
└─ Cube.prefab
```

阅读顺序：

1. 打开 `PoolTest.cs`，确认如何注册 `IGameObjectPoolAsset`。
2. 查看 Prefab 的 `CubeView`，理解对象取出和归还回调。
3. 运行场景并重复创建/回收。
4. 在 Profiler 中比较第一次实例化与后续复用。
5. 停止运行，检查场景退出时的池清理策略。

迁移到业务项目时，需要替换资源适配器。资源适配器至少负责：

- 根据 path 加载或实例化 Prefab。
- 正确设置父节点。
- 在池最终清理时销毁实例或释放资源句柄。
- 明确同步与异步加载边界。

## 3. Record 示例

相关文件：

```text
Assets/Project/Examples/Record/
├─ New Scene.unity
└─ RecordGame.cs
```

重点观察：

- `UseUndo` 的注册时机。
- Record 类型如何保存撤销前后的数据。
- 写入新记录后 Undo/Redo 栈如何变化。
- 记录对象如果进入池，何时清理旧引用。

生产实现中的记录应是一次操作的完整快照或可逆命令。不要只保存一个指向可变对象的引用，否则撤销时读取到的可能已经是新状态。

推荐测试序列：

```text
初始状态 A
-> 操作到 B
-> 操作到 C
-> Undo 回 B
-> Undo 回 A
-> Redo 到 B
-> 新操作到 D
-> 确认旧的 Redo 分支被正确处理
```

还要覆盖空栈 Undo、空栈 Redo、连续大量记录和场景退出。

## 4. RedPoint 示例

相关文件：

```text
Assets/Project/Examples/RedPoint/
├─ New Scene.unity
└─ redPointGame.cs
```

红点调试建议把路径画成树：

```text
Root
├─ Mail
│  ├─ System
│  └─ Friend
└─ Task
   ├─ Daily
   └─ Achievement
```

验证顺序：

1. 先建立全部节点。
2. 修改一个叶子的业务值。
3. 调用 `FreshDots()`。
4. 检查叶子、直接父节点和根节点。
5. 同时修改多个叶子，只刷新一次。
6. 释放节点后再次刷新，确认没有旧回调。

UI 显示节点可以使用 `RedActiveDot`，但业务状态仍应来自逻辑数据，不应把 GameObject 的 active 状态当作红点真值。

## 5. UI 示例

相关文件较多：

```text
Assets/Project/Examples/UI/
├─ New Scene.unity
├─ New UI Layer Data.asset
├─ UICollect.json
├─ UIGame.cs
├─ PanelNames_UIGame.cs
├─ PanelOne.prefab
├─ PanelOneView.cs
├─ PanelOneItem.prefab
├─ PanelOneItemWidget.cs
├─ PanelTwo.prefab
└─ PanelTwoView.cs
```

建议按以下链路阅读：

```text
UIGame.UseUI
-> PanelCollection 中的 path/layer/fullScreen
-> IUIDelegate.LoadPanelAsync
-> UIPanel 实例
-> ViewBridge path/type map
-> PanelOneView 生命周期
-> PanelOneItemWidget 的创建与回收
```

运行时验证：

1. 第一次 Show 是否触发加载、订阅和 `OnLoad`。
2. 再次 Show 是否复用已加载 Panel。
3. 上层全屏 Panel 出现时，下层可见性通知是否正确。
4. Hide 后重新 Show，View 状态是否重置。
5. Close 后是否解除 Bridge 映射并按策略销毁资源。
6. 快速连续点击 Show/Hide/Close 时是否保持一致状态。
7. 异步动画被跳过或对象禁用时，等待任务是否仍能完成。
8. Item 重复取出后是否残留上一个数据项的文字、图片和监听器。

代码生成工作流见 [UI 编辑器工作流](12-ui-editor.md)。运行时适配见 [UI 运行时](11-ui-runtime.md)。

## 6. 新建自定义服务

最小服务：

```csharp
using IFramework;

public interface IClockService : IService
{
    float Time { get; }
}

public sealed class ClockService : ServiceBase, IClockService
{
    public float Time { get; private set; }

    protected override void OnUse(IServiceCollection services)
    {
    }

    protected override void OnEnter(IServiceCollection services)
    {
        Game.BindUpdate(Tick);
    }

    private void Tick()
    {
        Time += UnityEngine.Time.deltaTime;
    }

    protected override void OnQuit(IServiceCollection services)
    {
        Game.UnBindUpdate(Tick);
    }
}

public static class ClockServiceExtensions
{
    public static IServiceCollection UseClock(this IServiceCollection services)
    {
        services.Use<IClockService>(new ClockService());
        return services;
    }

    public static IClockService Clock(this IServiceProvider services)
    {
        return services.GetRequiredService<IClockService>();
    }
}
```

接入 Game：

```csharp
protected override void Startup()
{
    this.UseClock();
    this.EnterService<IClockService>();
}
```

自定义服务评审清单：

- 接口只暴露业务需要的能力。
- `UseXxx` 只注册，不隐式启动复杂流程。
- `OnEnter` 可预测，依赖已经注册。
- `OnQuit` 解绑所有事件和 Update。
- 命名服务明确名称冲突策略。
- 热点 API 不做无必要反射和临时分配。

## 7. 接入自定义 UI 资源系统

业务实现 `IUIDelegate`，核心是两个资源方法：

```csharp
public sealed class ProjectUIDelegate : IUIDelegate
{
    public async AsyncTask<UIPanel> LoadPanelAsync(
        RectTransform parent,
        PanelCollection.Data data)
    {
        // 1. 使用 data.path 加载资源
        // 2. 实例化到 parent
        // 3. 取得并返回 UIPanel
        // 4. 失败时完成为异常，不要留下永久 pending 的任务
        throw new System.NotImplementedException();
    }

    public void DestroyPanel(GameObject gameObject)
    {
        // 销毁实例，并释放与该实例绑定的资源句柄。
        UnityEngine.Object.Destroy(gameObject);
    }

    // 其余方法接收 UI 全局状态和生命周期通知。
}
```

资源系统适配需要自行决定：

- 相同 path 的并发加载是否合并。
- 资源句柄由 Panel、Delegate 还是独立缓存拥有。
- Close 是只销毁实例，还是同时释放资源。
- 加载取消后如何回收迟到的实例。
- 场景切换时如何清空缓存。

这些所有权必须能画出唯一清晰的释放链，否则容易产生重复释放或资源泄漏。

## 8. 扩展 Editor Window

业务 Editor 工具可以沿用 RootWindow 的 Tab 发现机制。扩展代码应放在 Editor 程序集中或 `Editor` 目录，避免被打进 Player。

实现时注意：

- 类型发现发生在 Editor 域重载后。
- 菜单、窗口标题和路径应稳定，方便团队检索。
- ProjectSettings 与 EditorPrefs 分清项目级和用户级配置。
- 修改资产后正确调用 Undo、SetDirty、SaveAssets 或 AssetDatabase 刷新。
- 批量处理前给出预览或明确范围。
- 生成代码保持确定性，避免每次生成无意义改动。

## 9. 测试策略

### Core

异步至少覆盖：

- 完成前和完成后注册 continuation。
- 成功、异常、取消和重复完成。
- 空集合、单元素、多元素 `WhenAll/WhenAny/Sequence`。
- `Delay` 的负数、零、`NaN` 和预取消。
- `Repeat` 的 `-1`、`0`、正常次数和非法次数。

池至少覆盖：

- 空池创建、取出、归还、重复归还。
- `null` 和 `OnSet == false`。
- Clear 生命周期。
- 数组长度 `0`、常见长度、大长度和负长度。
- 引用数组及包含引用字段的结构体数组清理。
- 随机顺序取得和归还后的计数一致性。

### Services

- 注册顺序、命名服务和缺失依赖。
- 重复 Enter/Dispose 的预期行为。
- 注入字段、属性、继承成员和命名值。
- 状态切换的进入/退出顺序。
- Pref 转换失败与存储失败。
- Undo 空栈、分支覆盖和容量边界。
- 红点多级传播、批量刷新和节点释放。

### UI

- 资源加载成功、返回 null、异常、取消。
- 相同 path 的连续和并发 Show。
- Hide/Close 与异步过渡交错。
- 多层全屏面板的显示恢复。
- ViewBridge 无映射、重复订阅和取消订阅。
- Widget 取出/回收后监听器和数据重置。
- 横竖屏、安全区和极端分辨率。

### 执行层级

建议至少包含：

1. 纯 C# 快速回归，覆盖无 Unity 场景依赖的边界。
2. Unity EditMode 测试，覆盖序列化和 Editor 工具。
3. Unity PlayMode 测试，覆盖 PlayerLoop、GameObject 和 UI 生命周期。
4. 目标平台冒烟测试，覆盖 IL2CPP、资源和输入差异。

## 10. 提交前检查

```text
1. 查看 git status，确认没有临时文件或无关资产。
2. 查看完整 diff，确认公开行为和兼容性。
3. 运行格式、编译和自动化测试。
4. 在 Unity 中打开受影响示例场景。
5. 检查 Console 无新增 Error/Exception。
6. 修改公开 API 时同步文档和示例。
7. 修改序列化字段时验证旧资产升级。
8. 修改 Editor 生成器时验证重复生成没有漂移。
9. 提交信息说明行为变化，而不只写“优化”。
```

## 11. 报告问题

Issue 中建议提供：

- IFramework 版本或 commit。
- Unity 完整版本。
- 平台、脚本后端和构建模式。
- 最小复现工程或最小代码。
- 预期行为与实际行为。
- 完整异常和首个有效调用栈。
- 是否只在 Editor、Mono、IL2CPP 或特定设备发生。
- 性能问题的 Profiler 截图、采样区间和复现频率。

没有复现条件的耗时数字很难比较；请同时说明对象数量、循环次数、预热方式和采样环境。

## 12. 文档贡献

文档采用 Markdown，并通过 Docsify 展示。新增章节后：

1. 文件名使用稳定的数字前缀和英文短名。
2. 在 `docs/_sidebar.md` 添加入口。
3. 相对链接以当前 Markdown 文件为基准。
4. 代码块注明语言。
5. 示例必须对应仓库当前 API。
6. 对限制和失败路径给出明确说明。
7. 检查桌面和窄屏下表格、长路径与代码块的可读性。
