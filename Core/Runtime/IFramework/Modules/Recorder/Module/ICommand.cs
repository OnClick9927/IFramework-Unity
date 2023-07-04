namespace IFramework.Recorder
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface ICommand:IEventArgs
    {
        /// <summary>
        /// 处理
        /// </summary>
        void Excute();
    }
}
