using ConfigServices;
using LogServices;
using MailServices;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppMailSender;

class Program
{
    static void Main(string[] args)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddScoped<IConfigService, EnvVarConfigService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<ILogProvider, ConsoleLogProvider>();
        using (var sp = services.BuildServiceProvider())
        {
            var mailService = sp.GetRequiredService<IMailService>();
            mailService.Send("Hello", "abc@gmail.com", "body language");
        }


    }
}

