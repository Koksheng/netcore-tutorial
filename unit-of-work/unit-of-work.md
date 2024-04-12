# Unit of Work

1. Unit of work should use by application layer, other layers should not invoke `SaveChangesAsync` to save the dbContext.

2. When all create, update, delete actions have been conpleted in one request, only then trigger the **Unit of Work** to save the changes

3. So develop a function inside the container, then after the ending of function auto trigger `SaveChangesAsync`'s Filter: `UnitOfWorkAttribute`, `UnitOfWorkFilter`'

## UnitOfWorkFilter

[Link to Github](https://github.com/yangzhongke/NETBookMaterials/blob/main/%E6%9C%80%E5%90%8E%E5%A4%A7%E9%A1%B9%E7%9B%AE%E4%BB%A3%E7%A0%81/YouZack-VNext/Zack.ASPNETCore/UnitOfWorkFilter.cs)

From DI get the DB Context, then for loop DB Context, call the `SaveChangesAsync`


## UnitOfWorkAttribute

[Link to Github](https://github.com/yangzhongke/NETBookMaterials/blob/main/%E6%9C%80%E5%90%8E%E5%A4%A7%E9%A1%B9%E7%9B%AE%E4%BB%A3%E7%A0%81/YouZack-VNext/Zack.ASPNETCore/UnitOfWorkAttribute.cs)


## How to Use
`UnitOfWorkAttribute`

```
namespace UserMgr.WebAPI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute:Attribute
    {
        public Type[] DbContextTypes{ get; init; }
        public UnitOfWorkAttribute(params Type[] dbContextTypes)
        {
            this.DbContextTypes = dbContextTypes;
        }
    }
}
```

`UnitOfWorkFilter`

```
namespace UserMgr.WebAPI
{
    public class UnitOfWorkFilter:IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if(result.Exception!=null) // 只有Action执行成功，才自动调用SaveChanges
            {
                return;
            }
            var actionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if(actionDesc == null)
            {
                return;
            }
            var uowAttr = actionDesc.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if(uowAttr==null)
            {
                return;
            }
            foreach(var dbCtxType in uowAttr.DbContextTypes)
            {
                var dbCtx = context.HttpContext.RequestServices.GetService(dbCtxType) as DbContext; // 管DI要DbContext实例
                if(dbCtx!=null)
                {
                    await dbCtx.SaveChangesAsync();
                }
            }
        }
        
    }
}
```

`Program.cs` (register)
```
builder.Services.Configure<MvcOptions>(o=>{
    o.Filters.Add<UnitOfWorkFilter>();
});
```

`WeatherForecastController`
```
[HttpGet(Name="GetWeatherForecast")]
[UnitOfWork(typeof(UserDbContext))] // <----- add this into Controller
public IEnumerable<WeatherForecast> Get()
{

}
```