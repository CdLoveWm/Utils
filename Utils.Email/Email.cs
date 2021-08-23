using System;

namespace Utils.Email
{
    /// <summary>
    /// 邮件基础信息类
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Desc:SMTP服务帐号
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string smtp_account { get; set; }
        /// <summary>
        /// Desc:SMTP服务密码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string smtp_pwd { get; set; }

        /// <summary>
        /// Desc:发送方邮件地址
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string sender_email { get; set; }

        /// <summary>
        /// Desc:设置指示邮件正文是否为 Html 格式的值。true / false
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string body_html { get; set; }

        /// <summary>
        /// Desc:0：（正常），1（低优先级），2（高优先级）
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int priority { get; set; }
        /// <summary>
        /// Desc:SMTP服务器地址
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string smtp_server { get; set; }

    }
}
