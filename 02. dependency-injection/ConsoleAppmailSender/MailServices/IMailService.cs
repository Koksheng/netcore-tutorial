using System;
namespace MailServices
{
	public interface IMailService
	{
		public void Send(string title, string to, string body);
	}
}

