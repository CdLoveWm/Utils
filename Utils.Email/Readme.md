> 这是邮件发送的帮助类， Email为邮件服务器信息类

`调用方式`

```c#
// 1、配置email信息
Email email = new Email();
......
// 2、将配置信息放入helper
EmailHelper helper = new EmailHelper();
helper.SetEmailInfo(email)
// 3、调用
helper.Sendmail(...);   
```

