# domain event 领域事件的实现选型

[Link to video](https://www.bilibili.com/video/BV1pK41137He?p=182&vd_source=0db8da3630570b65a3001fb44134ff14)

**Methond 1**

when using c# event handle
```
var bl = new ProcessBusinessLogic();
bl.ProcessCompleted += bl_ProcessCompleted;
bl.StartProcess();
```
缺点： 需要显示地注册


**Method 2**

Open source library: `MediatR`
support `1 poster to 1 handler` and `1 poster to multiple handler`


## How to use

1. NuGet: MediatR.Extensions.Microsoft.DependencyInjection
2. `Program.cs` call `AddMediatR()`
   ```
   builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
   ```
3. Create a class, to connect and handle between **Poster** and **Handler**, this class must implement `INotification` interface, then usually use `record`.
   
   `PostNotification.cs`
   ```
   using MediarR;
   namespace MediarRTest1
   {
        public record PostNotification(string body) : INotification;
   }
   ```
4. Inject `IMediator` into event poster class.
   - `Send()` one poster to one handler
   - `Publish()` one poster to multiple handler
  
    `WeatherForecastController.cs`
    ```
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IMediator mediator;
        public WeatherForecastController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // start the publish event
        [HttpGet(Name="GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            mediator.Publish(new PostNotification("你好呀"+DateTime.Now));
            return ... .ToArray();
        }
    }
    ```
5. Event Handler must implement `NotificationHandler<TNotification>` interface. Code below with multiple handler.
   `PostNotifHandler1.cs`
   ```
   using MediarR;
   namespace MediarRTest1
   {
        public class PostNotifHandler1 : NotificationHandler<PostNotification>
        {
            protected override void Handle(PostNotification notification)
            {
                Console.WriteLine("1111:",notification.Body);
            }
        }
   }
   ```
   `PostNotifHandler2.cs`
   ```
   using MediarR;
   namespace MediarRTest1
   {
        public class PostNotifHandler2 : NotificationHandler<PostNotification>
        {
            protected override void Handle(PostNotification notification)
            {
                Console.WriteLine("2222:",notification.Body);
            }
        }
   }
   ```

6. Result:
   ```
   1111:你好呀2022/2/10 14:33:34
   2222:你好呀2022/2/10 14:33:34
   ```



# Advance of using Mediator

[Link to video](https://www.bilibili.com/video/BV1pK41137He?p=183&vd_source=0db8da3630570b65a3001fb44134ff14)

**Backgound**: 
```

[HttpGet(Name="GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{

    User u1 = new ("qzk");
    u1.ChangePassword("123456");
    u1.ChangeUserName("abc");
    ctx.Users.Add(u1);
    await ctx.SaveChangesAsync();
    mediator.Publish(new PostNotification("你好呀"+DateTime.Now));


    return ... .ToArray();
}
```
If using method above, which trigger `mediator.Publish()` after SaveChangesAsync() in controller will cause to missing out of event （漏发）.

**Solution**:

Add the domain event (注册事件) into a domain event list, then foreach them when doing SaveChangesAsync()

1. `IDomainEvents` interface
```
public interface IDomainEvents
{
    IEnumerable<INotification> GetDomainEvents();
    void AddDomainEvent(INotification eventItem);
    void AddDomainEventIfAbsent(INotification eventItem);
    void ClearDomainEvents();
}
```

2. `BaseEntity` (parent class to simplify the `IDomainEvents`)
```
public abstract class BaseEntity : IDomainEvents
{
    private readonly IList<INotification> events = new List<INotification>();
    public void AddDomainEvent(INotification notif)
    {
        events.Add(notif);
    }
    public void ClearDomainEvents()
    {
        events.Clear();
    }
    public IEnumerable<INotification> GetDomainEvents()
    {
        return events;
    }
}
```

3. `User.cs` model inherit `BaseEntity`
```
public class User : BaseEntity
{
    public int Id { get; init; }
    public DateTime CreateDateTime { get; init; }
    public string UserName { get; private set; }
    public int Credits { get; init; }

    private User() // 给EFCore从DB加载data然后生成User对象返回用的
    {

    }

    public User(string yhm)
    {
        this.UserName = yhm;
        this.CreateDateTime = DateTime.Now;
        this.Credits = 10;
        // publish 新增用户
        AddDomainEvents(new NewUserNotification(yhm, this.CreateDateTime)); // 注册事件
    }

    public void ChangeUserName(string un)
    {
        if(un.Length > 5)
        {
            Console.WriteLine("用户名长度不能大于5")；
            return;
        }
        string oldUserName = this.UserName;
        this.UserName = un;
        AddDomainEvents(new UserNameChangeNotification(oldUserName, un)); // 注册事件
    }
}
```

4. `NewUserNotification` 
   
   a class, to connect and handle between **Poster** and **Handler**
    ```
    using MediarR;
    namespace MediarRTest2
    {
            public record NewUserNotification(string UserName, DateTime Time) : INotification;
    }
    ```

5. `UserNameChangeNotification` 
   
   a class, to connect and handle between **Poster** and **Handler**
    ```
    using MediarR;
    namespace MediarRTest2
    {
            public record UserNameChangeNotification(string OldUserName, string NewUserName) : INotification;
    }
    ```
6. After 注册事件, 在`DBContext`的`SaveChangeAsync`里发布事件 `Publish()`
    `MyDbConext.cs`

    By 拿到所有`domainEvents` `list`, 然后`foreach` 发布
    ```
    public class MyDbContext:DbContext
    {
        private readonly IMediator mediator;

        public MyDbContext(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // override SaveChanesAsync
        public override Task<int> SaveChanesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = this.ChangeTrackerEntries<IDomainEvents>().Where(x=>x.Entity.GetDomainEvents().Any()); // 获得所有含有未发布事件的实体对象
            var domainEvents = domainEntities.SelectMany(x=>x.Entity.GetDomainEvents()).ToList(); 、、获得所有待发布消息
            domainEntities.ToList().ForEach(entity=>entity.Entity.ClearDomainEvents());
            foreach(var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
            、、把消息的发布放到base.SaveChangesAsync()之前，可以保证领域事件响应代码中的事务操作和base.SaveChangeAsync中的代码在同一个事务中,实现强一致性事务
            return await base.SaveChangesAsync(acceptAllChangeOnSuccess, cancellationToken);
        }
    }
    
    ```

7. `NewUserHandlers`
