# 红点与 Undo

## 红点树

红点服务把路径组织为树，叶节点设置数量，父节点聚合所有直接子节点数量，再把变化推送到绑定的 `RedDot` 视图。

默认路径分隔符：

```csharp
RedTreeService.separator = '/';
```

业务通常只通过 `IRedTreeService` 和扩展方法访问。

## 注册

```csharp
protected override void Startup()
{
    this.UseRedTree();
}
```

`UseRedTree` 的 OnUse 会清空旧树。红点服务没有必须执行的 Enter 初始化。

## 建立路径

```csharp
IRedTreeService redTree = this.RedTree();

redTree.ReadPath("mail");
redTree.ReadPath("mail/system");
redTree.ReadPath("mail/friend");
redTree.ReadPath("quest/daily");
```

路径键是累计完整路径：

```text
mail
mail/system
mail/friend
quest
quest/daily
```

先 `ReadPath`，后 `SetCount`。不存在的 key 会被忽略。

## 设置和刷新数量

```csharp
redTree.SetCount("mail/system", 3);
redTree.SetCount("mail/friend", 2);
redTree.FreshDots();
```

`SetCount` 只把叶节点放入 dirty 队列，`FreshDots` 才执行：

1. 应用叶节点新数量。
2. 向上递归计算父节点总数。
3. 刷新发生变化的 RedDot 视图。

因此应选择统一刷新点：

```csharp
void LateUpdate()
{
    Game.Current.RedTree().FreshDots();
}
```

或数据批量变更后手动刷新一次。不要每次 SetCount 后都刷新，批处理能减少重复父链计算。

查询：

```csharp
int mailCount = redTree.GetCount("mail");
int viewCount = redTree.GetDotCount("mail");
```

`GetDotCount` 返回绑定在该路径上的 RedDot 视图数量，不是业务红点数。

## 绑定 GameObject 显隐

```csharp
RedActiveDot dot = redTree.CreateRedActiveDot("mail", redDotGameObject);
```

数量大于 0 时激活 GameObject，否则隐藏。

销毁绑定：

```csharp
dot.Dispose();
```

Dispose 会从树移除、把视图刷新为 0，并尝试按真实类型归还 StaticPool。

## 自定义红点 View

```csharp
public sealed class NumberDot : RedDot
{
    public TMPro.TMP_Text label;

    public override void FreshView(int count)
    {
        label.text = count.ToString();
        label.gameObject.SetActive(count > 0);
    }
}
```

```csharp
NumberDot dot = redTree.CreateRedDot<NumberDot>("mail", value =>
{
    value.label = label;
});
```

初始化回调发生在 SetPath/首次刷新之前。

## 清理路径

```csharp
redTree.ClearPath("mail/system");
redTree.FreshDots();
```

ClearPath 删除节点和子树，并把父节点加入重新计算队列。

全部清理：

```csharp
redTree.ClearAll();
```

ClearAll 会清 dirty、绑定映射和节点映射。若业务仍持有 RedDot 对象，应先 Dispose，避免后续对已清空树的引用操作。

## 红点实践

- 路径集中定义常量，避免拼写差异。
- 只给叶节点写业务数量。
- 一帧批量 SetCount，帧末 FreshDots。
- UI View 关闭时 Dispose RedDot。
- 大树避免每次重新 ReadPath；初始化一次后只更新数量。

## Undo/Redo 服务

UndoService 维护双向记录链：

```text
head <-> record1 <-> record2 <-> record3
                           ^ current
```

新记录插入在 current 后面；如果 current 后还有 Redo 分支，会先回收后续记录。

## 注册

```csharp
protected override void Startup()
{
    this.UseValues();
    this.UseUndo();
}
```

支持命名实例：

```csharp
this.UseUndo("level-editor");
IUndoService undo = this.Undo("level-editor");
```

再次 Use 同名 Undo 时会 Clear 已有记录。

## 使用 UndoRecord

```csharp
float value = 10;

undo.Subscribe<UndoRecord>(record =>
{
    record.SetName("Add 5");
    record.SetValue(
        redo: () => value += 5,
        undo: () => value -= 5);
});
```

默认订阅后立即执行 Redo。

```csharp
undo.Undo();
undo.Redo();

bool canUndo = undo.CouldUndo();
bool canRedo = undo.CouldRedo();
```

## 自定义记录

```csharp
public sealed class PositionRecord : BaseUndoRecord
{
    public Transform target;
    public Vector3 before;
    public Vector3 after;

    protected override void OnRedo() => target.position = after;
    protected override void OnUndo() => target.position = before;

    protected override void OnReset()
    {
        base.OnReset();
        target = null;
        before = default;
        after = default;
    }
}
```

```csharp
undo.Subscribe<PositionRecord>(record =>
{
    record.SetName("Move Object");
    record.target = target;
    record.before = oldPosition;
    record.after = newPosition;
});
```

记录通过 StaticPool 获取和回收。`OnReset` 必须清除 Unity 对象、委托、集合等引用。

## 不立即 Redo

接口支持：

```csharp
undo.Subscribe(record, redo: false);
```

泛型扩展会把 `redo` 参数转发给服务。也可以直接创建记录并调用接口：

```csharp
var record = StaticPool.Get<PositionRecord>();
// 初始化 record
undo.Subscribe(record, false);
```

## 历史列表

```csharp
List<string> names = undo.GetRecordNames(out int currentIndex);
BaseUndoRecord current = undo.GetCurrent();
```

列表包含内部 `head` 记录。`currentIndex` 表示当前指针位置。

此方法每次创建新 List，编辑器历史面板可以按需调用，不建议在 Update 高频调用。

## Clear

```csharp
undo.Clear();
```

Clear 回收 head 之后的全部记录并把 current 重置到 head。服务退出时自动 Clear。

## Undo 实践

- 一个记录只描述一个原子操作。
- Redo/Undo 都应可重复执行，并互为逆操作。
- 新操作发生时记录 before/after，不要在 Undo 时再读取变化后的外部状态。
- OnReset 清空全部引用，防止对象池保留场景对象和闭包。
- UI 按钮状态用 CouldUndo/CouldRedo 刷新。
