using Microsoft.Extensions.DependencyInjection;

namespace ioc1;

class Program
{
    static void Main(string[] args)
    {
        ServiceCollection services = new ServiceCollection();
        //services.AddScoped<ITestService, TestServiceImpl>();
        //services.AddScoped(typeof(ITestService), typeof(TestServiceImpl));
        //services.AddSingleton(typeof(ITestService), new TestServiceImpl());
        services.AddSingleton(typeof(ITestService), new TestServiceImpl());

        using (ServiceProvider sp = services.BuildServiceProvider())
        {
            ITestService ts1 = sp.GetService<ITestService>();
            ts1.Name = "tom";
            ts1.SayHi();
            Console.WriteLine(ts1.GetType());
        }
        Console.ReadLine();
    }


    static void Main1(string[] args)
    {
        //ITestService t = new TestServiceImpl();
        //t.Name = "tom";
        //t.SayHi();

        ServiceCollection services = new ServiceCollection();
        //services.AddTransient<TestServiceImpl>();
        //services.AddSingleton<TestServiceImpl>();
        services.AddScoped<TestServiceImpl>();
        using (ServiceProvider sp = services.BuildServiceProvider())  //---> Service Locator
        {
            //TestServiceImpl t = sp.GetService<TestServiceImpl>();
            //t.Name = "lily";
            //t.SayHi();

            //TestServiceImpl t1 = sp.GetService<TestServiceImpl>();
            //t1.Name = "tom";
            //t1.SayHi();

            //Console.WriteLine(ReferenceEquals(t, t1));

            //t.SayHi();
            TestServiceImpl tt1;
            using (IServiceScope scope1 = sp.CreateScope())
            {
                //在scope中获取Scope相关的对象，scope1.ServiceProvider而不是sp
                TestServiceImpl t = scope1.ServiceProvider.GetService<TestServiceImpl>();
                t.Name = "lily";
                t.SayHi();

                TestServiceImpl t1 = scope1.ServiceProvider.GetService<TestServiceImpl>();
                Console.WriteLine(ReferenceEquals(t, t1));
                tt1 = t1;
            }

            using (IServiceScope scope2 = sp.CreateScope())
            {
                //在scope中获取Scope相关的对象，scope1.ServiceProvider而不是sp
                TestServiceImpl t = scope2.ServiceProvider.GetService<TestServiceImpl>();
                t.Name = "lily";
                t.SayHi();

                TestServiceImpl t1 = scope2.ServiceProvider.GetService<TestServiceImpl>();
                Console.WriteLine(ReferenceEquals(t, t1));
                Console.WriteLine(ReferenceEquals(tt1, t1));
            }
        }





        Console.Read();
    }


}

public interface ITestService
{
    public string Name { get; set; }
    public void SayHi();
}

public class TestServiceImpl : ITestService, IDisposable
{
    public string Name { get; set; }

    public void Dispose()
    {
        Console.WriteLine("Dispose........");
    }

    public void SayHi()
    {
        Console.WriteLine($"Hi, I'm {Name}");
    }
}

public class TestServiceImpl2 : ITestService
{
    public string Name { get; set; }
    public void SayHi()
    {
        Console.WriteLine($"你好，我是{Name}");
    }
}




