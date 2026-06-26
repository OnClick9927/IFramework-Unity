# IFramework [Doc](https://onclick9927.github.io/IFramework-Unity/#/)


[![Stargazers over time](https://starchart.cc/OnClick9927/IFramework-Unity.svg?variant=adaptive)](https://starchart.cc/OnClick9927/IFramework-Unity)
``` csharp
while(true)
    Console.Write("Thanks For EveryOne Who Used It Once !")
```
QQ Group ：782290296 

 
#### 主体 https://github.com/OnClick9927/IFramework-Unity.git#src
* Core
  * 通用单例
  * 通用消息系统 
    * Publish/Subscribe   常见
    * Wait/Notify  方法A Wait 之后代码 会等到 某个地方 Notify 之后才会继续执行
      * 例子-》加载场景方法 有两个Class A（某个方法） B（场景加载器）
      * 1、A 通知B 开始加载
      * 2、A Wait
      * 3、B 加载
      * 4、B 加载完成
      * 5、B Notify
      * 6、A 开始业务逻辑
  * C# Async 扩展 AsyncTask
    * 主线程等待
    * Delay/WhenAll/WhenAny/Sequence/.....
  * Pool
  * Log
  * 编辑器工具  序列化/拖拽/窗口/模板代码/项目设置/GUI
  * 支持组件添加回调
* Services 游戏服务
  * 具体使用
    * （统一写法  Usexxx -》 Enterxxx-》  Game.xxx()/Inject->使用接口）
    * 一般过程
      * 1、所有的Usexxx
      * 2、所有的Enterxxx
      * 3、游戏逻辑
    * Usexxx 启用 某个服务
    * Enterxxx 服务初始化、通常是注入-》参考 EnterMVC/EnterState
    * Game.xxx 比如 Game.State()-> 就是游戏状态服务
    * Game.xxx 太麻烦 注入写法
      * 1、声明字段：[Inject] IGameStateService stateService
      * 2、注入：Game.Values.Inject(this)
      * 3、使用：stateService.xxx
  * 内置
    * MVC          （通用MVC框架）
    * GameObjectPool  （通用GameObject 对象池）
    * Pref（通用  Prefs 数据读写）
    * RedDot（通用M红点）
    * State（通用游戏状态框架）
    * Undo（通用Ctrl Z/Ctrl Shit Z）
    * Value（通用DI框架）
  * 扩展： 继承 IFramework.ServiceBase即可
* UI
  * 自定义加载
  * 支持 同步/异步（开启、隐藏、关闭）界面，可等待
  * 内部处理 ui 多次点击的问题
  * 提供全局遮罩
  * 提供 Item 池
  * 自定义层级
  * 配套代码生成
  * 支持多样化扩展（预定义了 MVC模式）
  * 极简生命周期（OnLoad、OnShow、OnHide、OnClose）
  * UI整体变化接口（用于顶部资源栏、全屏UI、触发各种事件等）
  * UI组件添加时 默认优化取消勾选Raycast 等
  * UI组件移除时候自动 移除CanvasRendererer
  * 支持异形屏幕适配





### 其他-> HotFix (基于XLUA、不想更新了😀)
  * 模板代码
  * lua 工具（ class、async、try、handler、using、EventSystem、ObservableObject、_G锁）
  * UI模块的 Lua扩展 （MVC）
  * 热重载



