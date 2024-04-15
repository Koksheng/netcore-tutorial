using System;
namespace LogServices
{
	public class ConsoleLogProvider : ILogProvider
	{
		public ConsoleLogProvider()
		{
		}

        public void LogError(string msg)
        {
            Console.WriteLine($"ERROR:{msg}");
        }

        public void LogInfo(string msg)
        {
            Console.WriteLine($"Info:{msg}");
        }
    }
}

