/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
    public interface IDownLoader:IDisposable
    {
        float Progress { get; }
        string Url { get; }
        /// 资源下载存放路径，不包含文件名
        string SaveDir { get; }
        /// 文件名,不包含后缀
        string FileNameWithoutExt { get; }
        /// 文件后缀
        string FileExt { get; }
        /// 下载文件全路径，路径+文件名+后缀
        string SaveFilePath { get; }
        /// 原文件大小
        long FileLength { get; }
        /// 当前下载好了的大小
        long CurrentLength { get; }
        /// 是否开始下载
        bool IsDownLoading { get; }
        void DownLoad();
       
    }
}