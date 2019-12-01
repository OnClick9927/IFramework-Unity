/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace IFramework.Net
{
#if !UNITY_5_3_OR_NEWER
    public class Email
    {
        private SmtpException error;
        private string senderEmail;
        private string name;
        private SmtpClient smtpClient;
        public SmtpException SendError { get { return error; } }
        public Email() { }
        public Email(string host, string senderEmail, string password, string name = null)
        {
            Init(host, senderEmail, password, name);
        }
        public void Init(string host, string senderEmail, string password, string name = null)
        {
            this.senderEmail = senderEmail;
            smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式 
            smtpClient.Host = host;//邮件服务器
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new NetworkCredential(senderEmail, password);//用户名、密码
            if (name == null) return;
            this.name = name;
        }
       
        public bool Send(string title, string content, string recieverEmail, string attachFilePath = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            if (!string.IsNullOrEmpty(attachFilePath))
            {
                mailMessage.Attachments.Add(new Attachment(attachFilePath));
            }
            return Send(mailMessage);
        }
        public bool Send(string title, string content, string recieverEmail, List<string> attachFilePaths = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            if (attachFilePaths != null)
            {
                for (int i = 0; i < attachFilePaths.Count; i++)
                {
                    mailMessage.Attachments.Add(new Attachment(attachFilePaths[i]));
                }
            }
            return Send(mailMessage);
        }
        public bool Send(string title, string content, string recieverEmail, string copyReciever, string attachFilePath = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            mailMessage.CC.Add(copyReciever);
            if (attachFilePath != null)
            {
                mailMessage.Attachments.Add(new Attachment(attachFilePath));
            }
            return Send(mailMessage);
        }
        public bool Send(string title, string content, string recieverEmail, string copyReciever, List<string> attachFilePaths = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            mailMessage.CC.Add(copyReciever);
            if (attachFilePaths != null)
            {
                for (int i = 0; i < attachFilePaths.Count; i++)
                {
                    mailMessage.Attachments.Add(new Attachment(attachFilePaths[i]));
                }
            }
            return Send(mailMessage);
        }
        public bool Send(string title, string content, string recieverEmail, List<string> copyRecievers, string attachFilePath = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            for (int i = 0; i < copyRecievers.Count; i++)
            {
                mailMessage.CC.Add(copyRecievers[i]);
            }
            if (attachFilePath != null)
            {
                mailMessage.Attachments.Add(new Attachment(attachFilePath));
            }
            return Send(mailMessage);
        }
        public bool Send(string title, string content, string recieverEmail, List<string> copyRecievers, List<string> attachFilePaths = null)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail, name);
            mailMessage.To.Add(recieverEmail);
            mailMessage.Subject = title;//邮件标题   
            mailMessage.Body = content;//邮件内容   
            mailMessage.BodyEncoding = Encoding.UTF8;//邮件内容编码   
            mailMessage.IsBodyHtml = true;//是否是HTML邮件   
            mailMessage.Priority = MailPriority.High;//邮件优先级  
            for (int i = 0; i < copyRecievers.Count; i++)
            {
                mailMessage.CC.Add(copyRecievers[i]);
            }
            if (attachFilePaths != null)
            {
                for (int i = 0; i < attachFilePaths.Count; i++)
                {
                    mailMessage.Attachments.Add(new Attachment(attachFilePaths[i]));
                }
            }
            return Send(mailMessage);
        }
        private bool Send(MailMessage mailMessage)
        {
            try
            {
                smtpClient.Send(mailMessage);
                error = null;
                return true;
            }
            catch (SmtpException ex)
            {
                error = ex;
                return false;
            }
        }


        public void Example()
        {
            string host = "smtp.163.com";// 邮件服务器smtp.163.com表示网易邮箱服务器    
            string userName = "wulei9927@163.com";// 发送端账号   
            string password = "******";// 发送端密码(这个客户端重置后的密码)
            string MyName = "OnClick";
            string title = "代码发的标题";//邮件的主题             
            string content = "代码发的内容";//发送的邮件正文  
            string strto = "2809399366@qq.com";
            string attachPath = @"C:\Users\User\Desktop\17654682_215920199818_2.png";
            string strcc = "2809399366@qq.com";//抄送

            Email email = new Email();
            email.Init(host, userName, password, MyName);
            email.Send(title, content, strto, strcc,attachPath);
        }
    }



#endif


}
