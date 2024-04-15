using System;
using ConfigServices;
using LogServices;

namespace MailServices
{
	public class MailService : IMailService
	{
        private readonly ILogProvider log;
        private readonly IConfigService config;
		public MailService(ILogProvider log, IConfigService config)
		{
            this.log = log;
            this.config = config;
		}

        public void Send(string title, string to, string body)
        {
            log.LogInfo("准备发送邮件");
            string smtpServer = config.GetValue("SmtpServer");
            string userName = config.GetValue("UserName");
            string password = config.GetValue("Password");
            Console.WriteLine($"邮件服务器地址{smtpServer}, {userName}, {password}");
            Console.WriteLine($"真发邮件啦！{title}, {to}, {body}");
            log.LogInfo("邮件发送完成");
        }
    }
}

