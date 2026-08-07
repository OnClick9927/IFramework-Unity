# Pref 与 GameObject 池

## PrefService 概览

PrefService 把“对象与字符串互转”和“字符串保存位置”拆成两个接口：

```csharp
public interface IPrefConverter
{
    object FromString(Type type, string value);
    string ToString(object value, Type type);
}

public interface IPrefLoader
{
    string Load(string key);
    void Save(string key, string value);
}
```

这样可以自由组合 JSON/二进制编码和 PlayerPrefs/文件/云存储。

## 实现转换器

```csharp
using System;
using IFramework;
using UnityEngine;

public sealed class JsonPrefConverter : IPrefConverter
{
    public object FromString(Type type, string value) =>
        JsonUtility.FromJson(value, type);

    public string ToString(object value, Type type) =>
        JsonUtility.ToJson(value);
}
```

## 实现加载器

```csharp
public sealed class PlayerPrefsLoader : IPrefLoader
{
    public string Load(string key) => PlayerPrefs.GetString(key, string.Empty);

    public void Save(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
}
```

## 注册

```csharp
protected override void Startup()
{
    this.UseValues();
    this.UsePref(new JsonPrefConverter(), new PlayerPrefsLoader());
}
```

PrefService 会使用 `IServiceCollection.Name` 作为 `baseKey`，最终真实 key 格式为：

```text
{baseKey}_{key}_{type}
```

## 保存和读取

```csharp
[Serializable]
public sealed class PlayerSettings
{
    public float musicVolume = 1f;
    public bool vibration = true;
}
```

```csharp
IPrefService pref = this.Pref();

var settings = pref.Load<PlayerSettings>("account-1001");
settings.musicVolume = 0.5f;
pref.Save("account-1001", settings);
```

省略 key 时使用：

```csharp
PrefServiceEx.defaultKey
```

默认值基于 `SystemInfo.deviceName`。多账号项目通常应显式传账号 key，避免设备名变化或冲突。

## 缓存行为

`Load` 首次读取后把对象放入内存字典，后续同真实 key 返回同一对象引用。没有存档内容时通过 `Activator.CreateInstance(type)` 创建默认对象。

`SaveAll()` 遍历缓存对象并写回。直接调用 `Save(key, obj)` 会立即保存传入对象，同时更新对应真实 key 的内存缓存；后续 Load 返回该缓存实例。

## PrefContext<T>

```csharp
var context = new PrefContext<PlayerSettings>();
pref.SetContext(context);

context.Value = pref.Load<PlayerSettings>("account-1001");
pref.Save(context);
```

SetContext 会：

- 按 T 保存 Context。
- 把 Context 注册到 ValueService。
- Load T 时同步设置 `context.Value` 和 `context.key`。

`FindContext<T>()` 获取已注册 Context。

## Editor 与 Player 的差异

在 `UNITY_EDITOR` 下，PrefService 忽略 `IPrefLoader`，直接读写：

```text
Assets/Editor/Pref/{baseKey}/{realKey}.json
```

保存会调用 `AssetDatabase.Refresh()`。

Player 构建中才使用传入的 `IPrefLoader`。因此必须分别验证 Editor 数据和目标平台数据。

## 清理与退出

```csharp
pref.ClearAll(); // 清内存缓存，重置 Context，不删除外部存储
pref.SaveAll();  // 保存当前缓存
```

Game 退出时 PrefService 会 SaveAll，然后清空缓存和 Context。

`ClearAll` 不是“删除存档”。要删除外部数据，应扩展 `IPrefLoader` 协议或在业务层实现。

## GameObjectPool 概览

GameObjectPool 把 Prefab 加载/释放交给业务适配器：

```csharp
public interface IGameObjectPoolAsset
{
    AsyncTask<GameObject> LoadAsset(string key);
    void ReleaseAsset(string key, GameObject asset);
}
```

框架负责：

- 为 key 准备 Prefab 和池根节点。
- 实例化/激活/隐藏 GameObject。
- 复用 `GameObjectView` wrapper。
- Clear 时通知资源适配器释放。

## 实现资源适配器

```csharp
public sealed class ResourcesPoolAsset : IGameObjectPoolAsset
{
    public AsyncTask<GameObject> LoadAsset(string key)
    {
        var task = AsyncTask<GameObject>.CreateFromPool();
        task.SetResult(Resources.Load<GameObject>(key));
        return task;
    }

    public void ReleaseAsset(string key, GameObject asset)
    {
        Resources.UnloadAsset(asset);
    }
}
```

Addressables/WooAsset/自定义 AssetBundle 可以实现同一接口。`ReleaseAsset` 收到的是池内部保存的 Prefab 实例，请按资源系统实际引用模型处理。

## 定义 View

```csharp
public sealed class EnemyView : GameObjectView, IPoolAbleGameObjectView
{
    public string PoolKey { get; set; }

    protected override void InitComponents()
    {
        // SetGameObject 后缓存组件。
    }
}
```

`GameObjectView` 已公开 `gameObject`，满足接口要求。

## 注册和使用

```csharp
protected override void Startup()
{
    this.UseValues();
    this.UseGameObjectPool(new ResourcesPoolAsset());
}
```

预热：

```csharp
bool ready = await this.GameObjectPool().Prepare("Enemies/Goblin");
```

获取：

```csharp
EnemyView enemy = await this.GameObjectPool().Get<EnemyView>("Enemies/Goblin");
if (enemy != null)
{
    enemy.transform.position = spawnPosition;
}
```

归还：

```csharp
this.GameObjectPool().Set(enemy);
```

归还时 GameObject 被隐藏并移到池节点下，View wrapper 通过真实类型回到 StaticPool。

## Prepare 流程

首次 key：

1. 调用 `asset.LoadAsset(key)`。
2. 创建 key 对应父节点。
3. 实例化一份隐藏 Prefab 模板。
4. 创建内部 ObjectPool。
5. 后续 `Get` 从池中取对象或实例化模板。

相同 key 已存在时直接返回 `true`。

`asset == null`、加载返回 null 时返回 `false`。

## 清理

```csharp
pool.Clear("Enemies/Goblin");
pool.ClearAll();
```

Clear 会调用资源适配器 `ReleaseAsset`、销毁池根节点并回收内部 Pool wrapper。

当前 GameObjectPool 的 `OnQuit` 不自动调用 `ClearAll()`。生产项目应在 Game/状态退出前显式清理，以确保资源适配器收到 Release 回调：

```csharp
protected override void OnQuit()
{
    this.GameObjectPool()?.ClearAll();
}
```

## 注意事项

- key 必须稳定且和资源系统一致。
- 不要重复 Set 同一个 View。
- View 归还前应自行重置业务状态、事件和异步流程。
- `GameObjectView` 构造会访问 `Game.Current.Values()`，必须先启用 ValueService。
- Clear 后旧 View/GameObject 引用失效，不要继续使用。
- 所有 UnityEngine.Object 操作在主线程执行。
