/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
namespace IFramework
{

    public enum MessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public MessageType messageType;
        public string message;
        public HelpBoxAttribute(string message, MessageType messageType =  MessageType.None)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }
}
