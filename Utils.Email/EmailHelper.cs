using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Email
{
    /// <summary>
    /// 邮件发送工具类
    /// </summary>
    public class EmailHelper
    {
        private Email email;
        /// <summary>
        /// 设置邮件基本信息
        /// </summary>
        /// <param name="email"></param>
        public void SetEmailInfo(Email _email)
        {
            this.email = _email;
        }
        /// <summary>
        /// 构造MailMessage对象基础信息
        /// </summary>
        /// <param name="displayName">邮件显示名称</param>
        /// <param name="suject">主题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        private MailMessage GenerateMailMessage(string displayName, string subject, string body)
        {
            if (email == null) throw new Exception("邮件设置参数为空，请先设置邮件配置信息");
            var mailMessage = new MailMessage();
            // 邮件标头   
            mailMessage.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
            // 邮件主题                   
            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            //邮件内容
            if (!string.IsNullOrWhiteSpace(body))
            {
                mailMessage.Body = body;
            }
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.From = new MailAddress(email.sender_email, displayName, Encoding.UTF8);
            mailMessage.IsBodyHtml = email.body_html == "true" ? true : false;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Priority = (MailPriority)email.priority;
            mailMessage.Headers.Add("X-MSMail-Priority", "Normal");
            mailMessage.Headers.Add("X-Mailer", "Microsoft Outlook Express 6.00.2900.2869");
            mailMessage.Headers.Add("X-MimeOLE", "Produced By Microsoft MimeOLE V6.00.2900.2869");
            mailMessage.Headers.Add("ReturnReceipt", "1");//不被当作垃圾邮件的关键代码
            return mailMessage;
        }
        /// <summary>
        /// 构造SmtpClient对象基础信息
        /// </summary>
        /// <returns></returns>
        private SmtpClient GenerateSmtpClient()
        {
            if (email == null) throw new Exception("邮件设置参数为空，请先设置邮件配置信息");
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = email.smtp_server;//指定SMTP服务器
            smtpClient.Port = 25;
            smtpClient.UseDefaultCredentials = false;// 是否使用默认证书 
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式 将smtp的出站方式设为 Network
            smtpClient.EnableSsl = false; // 使用套间字加密连接 
            smtpClient.Credentials = new System.Net.NetworkCredential(email.smtp_account, email.smtp_pwd);//用户名和密码
            smtpClient.Timeout = 60000;
            return smtpClient;
        }
        /// <summary>
        /// outlook生成会议带calendar
        /// </summary>
        /// <param name="toList">目标email</param>
        /// <param name="subject">主题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="displayName">邮件显示名称</param>
        /// <param name="emailUid">邮件ID</param>
        /// <param name="summary">摘要</param>
        /// <param name="bodyhtml">主体内容</param>
        /// <param name="senderEmail">发送者邮箱</param>
        /// <param name="senderName">发送者名称</param>
        /// <param name="description">描述、备注</param>
        /// <param name="place">地点</param>
        public void SendWithCalendar(List<string> toList, string subject, DateTime startTime, DateTime endTime, string displayName, string emailUid, string summary, string bodyhtml,
             string senderEmail, string senderName, string description, string place)
        {
            try
            {
                var _mailMessage = GenerateMailMessage(displayName, subject, bodyhtml);
                var _smtpClient = GenerateSmtpClient();
                toList.ForEach(to => {
                    _mailMessage.To.Add(to);
                });

                // 在邮件中设置不同的Mime类型             
                ContentType typeHtml = new ContentType("text/html");
                ContentType typeCalendar = new ContentType("text/calendar");
                //向calendar header添加参数                    
                typeCalendar.Parameters.Add("method", "REQUEST");
                typeCalendar.Parameters.Add("charset", "utf-8");
                typeCalendar.Parameters.Add("name", "meeting.ics");
                //string bodyhtml = "会议备注：<br>查看会议详情，请前往OfficeLink会议管理系统！";
                AlternateView viewHtml = AlternateView.CreateAlternateViewFromString(bodyhtml, typeHtml);
                _mailMessage.AlternateViews.Add(viewHtml);
                // 使用vcalendar格式创建邮件的body部分                
                var calendarContent = this.CalendarGenerate(emailUid, bodyhtml, senderEmail, toList, senderName, summary, description, place, startTime, endTime);
                AlternateView viewCalendar = AlternateView.CreateAlternateViewFromString(calendarContent, typeCalendar);
                viewCalendar.TransferEncoding = TransferEncoding.SevenBit;
                _mailMessage.AlternateViews.Add(viewCalendar);
                _smtpClient.Send(_mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("邮件发送失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="Path"></param>
        private MailMessage Attachments(List<string> paths, MailMessage mailMessage)
        {
            Attachment data;
            ContentDisposition disposition;
            foreach (var path in paths)
            {
                data = new Attachment(path, MediaTypeNames.Application.Octet);//实例化附件  
                disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(path);//获取附件的创建日期  
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(path);//获取附件的修改日期  
                disposition.ReadDate = System.IO.File.GetLastAccessTime(path);//获取附件的读取日期  
                mailMessage.Attachments.Add(data);//添加到附件中  
            }
            return mailMessage;
        }
        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="displayName">邮件显示名称</param>
        /// <param name="CompletedMethod">注册异步发送邮件完成时的事件</param>
        public void SendEmailAsync(List<string> toList, string subject, string content, string displayName, SendCompletedEventHandler CompletedMethod = null)
        {
            try
            {
                var _mailMessage = GenerateMailMessage(displayName, subject, content);
                var _smtpClient = GenerateSmtpClient();
                toList.ForEach(to => {
                    _mailMessage.To.Add(to);
                });
                if (CompletedMethod != null)
                {
                    _smtpClient.SendCompleted += new SendCompletedEventHandler(CompletedMethod);//注册异步发送邮件完成时的事件  
                }
                _smtpClient.SendAsync(_mailMessage, _mailMessage.Body);
            }
            catch (Exception ex)
            {
                throw new Exception("邮件发送失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">接收方邮件地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件正文内容</param>
        /// <param name="displayName">邮件显示名称</param>
        /// <param name="filePaths">附件地址</param>
        public bool Sendmail(List<string> toList, string subject, string content, string displayName, List<string> filePaths = null)
        {
            try
            {
                var _mailMessage = GenerateMailMessage(displayName, subject, content);
                var _smtpClient = GenerateSmtpClient();
                _smtpClient.Timeout = 30_000; // 默认30s超时
                toList.ForEach(to => {
                    _mailMessage.To.Add(to);
                });
                if (filePaths != null && filePaths.Count > 0)
                {
                    this.Attachments(filePaths, _mailMessage);
                }
                _smtpClient.Send(_mailMessage);
                _mailMessage.Attachments.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("邮件发送失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 生成ics文件（为以Outlook主要参考）     
        /// </summary>
        /// <param name="uid">邮件uid</param>
        /// <param name="strBodyHtml">主体内容</param>
        /// <param name="senderEmail">发送者邮箱</param>
        /// <param name="toList">邮件目标人员邮箱地址</param>
        /// <param name="sendername">发送人姓名</param>
        /// <param name="summary">摘要</param>
        /// <param name="description">描述/备注</param>
        /// <param name="place">地址</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private string CalendarGenerate(string uid, string strBodyHtml, string senderEmail, List<string> toList, string sendername, string summary
            , string description, string place, DateTime startTime, DateTime endTime)
        {
            string calDateFormat = "yyyyMMddTHHmmssZ";
            StringBuilder sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR").Append("\r\n");
            sb.Append("PRODID:").Append("-//Microsoft Corporation//Outlook 12.0 MIMEDIR//EN").Append("\r\n");
            sb.Append("VERSION:2.0").Append("\r\n");
            sb.Append("METHOD:REQUEST").Append("\r\n");
            sb.Append("X-MS-OLK-FORCEINSPECTOROPEN:TRUE").Append("\r\n");
            sb.Append("BEGIN:VEVENT").Append("\r\n");
            //发送者
            if (!string.IsNullOrEmpty(senderEmail))
            {
                sb.Append("ORGANIZER;CN=\"").Append(sendername).Append("\":MAILTO" + ":").Append(senderEmail).Append("\r\n");
            }
            foreach (string recipient in toList)
            {
                sb.Append("ATTENDEE;CN=\"" + recipient + "\";RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            }
            //传抄人
            //foreach (string recipient in this.CalendarModel.OptionalRecipients)
            //{
            //    sb.Append("ATTENDEE;CN=\"" + recipient + "\";ROLE=OPT-PARTICIPANT;RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            //}
            //秘密传抄人
            //foreach (string recipient in this.CalendarModel.ResourceRecipients)
            //{
            //    sb.Append("ATTENDEE;CN=\"" + recipient + "\";CUTYPE=RESOURCE;ROLE=NON-PARTICIPANT;RSVP=TRUE:mailto:" + recipient).Append("\r\n");
            //}
            sb.Append("CLASS:PUBLIC").Append("\r\n");
            sb.Append("CREATED:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DESCRIPTION:").Append(this.NotNull(description)).Append("\r\n");
            sb.Append("DTSTAMP:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DTSTART:").Append(startTime.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("DTEND:").Append(endTime.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("LAST-MODIFIED:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("LOCATION:").Append(this.NotNull(place)).Append("\r\n");
            sb.Append("PRIORITY:5").Append("\r\n");
            sb.Append("SEQUENCE:0").Append("\r\n");
            sb.Append("SUMMARY:").Append(this.NotNull(summary)).Append("\r\n");
            //sb.Append("UID:").Append(MeetingId.Replace("-", "")).Append("\r\n");
            sb.Append("UID:").Append(uid).Append("\r\n");
            sb.Append("X-ALT-DESC;FMTTYPE=text/html:").Append(strBodyHtml).Append("\r\n");
            sb.Append("STATUS:CONFIRMED").Append("\r\n");
            sb.Append("TRANSP:OPAQUE").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-BUSYSTATUS:BUSY").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-IMPORTANCE:1").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-INSTTYPE:0").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-ALLDAYEVENT:FALSE").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-OWNERAPPTID:-611620904").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-APPT-SEQUENCE:0").Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:").Append(DateTime.Now.ToUniversalTime().ToString(calDateFormat)).Append("\r\n");
            sb.Append("BEGIN:VALARM").Append("\r\n");
            sb.Append("ACTION:DISPLAY").Append("\r\n");
            sb.Append("DESCRIPTION:REMINDER").Append("\r\n");
            sb.Append("TRIGGER;RELATED=START:-PT00H15M00S").Append("\r\n");
            sb.Append("END:VALARM").Append("\r\n"); sb.Append("END:VEVENT").Append("\r\n");
            sb.Append("END:VCALENDAR").Append("\r\n");
            return sb.ToString();
        }
        /// <summary>           
        /// 设定不为null值           
        /// </summary>           
        /// <param name="str"></param>           
        /// <returns></returns>            
        private string NotNull(string str)
        {
            return str ?? String.Empty;
        }
    }
}
