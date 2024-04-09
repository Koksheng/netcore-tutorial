# Caching

A process of storing frequently accessed data in a temporary storage location.

To accelerate data delivery to clients, as it eliminates the need to repeatedly fetch the same data from the original source.

## Cache Control

Cache-control is an HTTP header used to specfiy browser caching policies in both client requests and server responses. 

Policies include how a resource is cached, where it's cached and its maximum age before expiring.

![abc](https://www.imperva.com/learn/wp-content/uploads/sites/13/2019/01/response-headers.jpg.webp)

## Cache Control: Max-Age

cache-control: max-age=120 means that the returned resource is valid for 120 seconds, afther which the browser has to request a newer version.


[Link to Reference Cache](https://www.imperva.com/learn/performance/cache-control/)


# Response Caching Middleware 
[Link to Response Caching Middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-8.0&tabs=aspnetcore2x)

[Easy to understand cache](https://medium.com/simform-engineering/caching-in-net-core-7c759a5bc3c6)

# Server Side Response Caching
[YangZhongKe ASP.NET Core Server Side Response Caching](https://www.bilibili.com/video/BV1pK41137He?p=121&vd_source=0db8da3630570b65a3001fb44134ff14)

1. If the ASP.NET Core install **Response Caching Middleware**, then ASP.NET Core not only will follow **[ResponseCache]** configuration to generate cache-control response header for client side caching, and the server side will follow **[ResponseCache]**'s configuration to generate server side caching. What's the different between client and server side caching? (Server Side)When the request coming from diff client but with same type of request.

2. **Response Caching Middleware**'s benefit: 
   > When the request coming from different Client side by with exactly same type of request, or the client side cannot support client side caching, then can reduce server pressure.

3. How to use: before `app.MapControllers()` add the `app.UseResponseCaching()`. Then make sure `app.UseCors()` is added before `app.UseResponseCaching`.

4. `cache-control: no-cache` will disable the server side caching

    ## Problem of Server Side Cahing
    1. Cant resolve malicious(恶意) request.
    2. Limitation of server side caching include, status code 200 with `GET` only can be cache. The request header cannt have `Authorization`, `Set-Cookie` etc.
    3. How? use In-memory cache, distributed caching etc.


# In-memory cache
[YangZhongKe In-memory cache](https://www.youtube.com/watch?v=Drf-InwDxWA&list=PL9sJKk6XPMxehYCui7OysUV6trlBbJ4T_&index=122&ab_channel=%E6%9D%A8%E4%B8%AD%E7%A7%91)

1. Put the caching data into application memory (RAM). In-memory caching use the key-value method like Dictionay.
2. Different Website have different process, so they wont interupt each other. After restart the website, it will clear all the in-memory cache data.

### IMemoryCache Interface
```
public interface IMemoryCache : IDisposable
{
    ICacheEntry CreateEntry(object key);
    void Remove(object key);
    bool TryGetValue(object key, out object value);
}
```

How to use
1. add `builder.Services.AddMemoryCache()`
2. Insert `IMemoryCache` interface, example of interface (Cache Extension): TryGetValue, Remove, Set, GetOrCreate, GetOrCreateAsync
3. Demo with `GetOrCreateAsync`
```
private readonly IMemoryCache memCache;

public TestController(IMemoryCache memCache)
{
    this.memCache = memCache;
}

public async Task<ActionResult<Book?>> GetBookById(long id)
{
    Console.WriteLine($"Start to Execute GetBookById,id={id}");
    Book? b = await memCache.GetOrCreateAsync("Book"+id, async(e) => {
        Console.WriteLine($"Cant find in cache, search in DB, id={id}");
        return await MyDbContext.GetByIdAsync(id);
    });
    Console.WriteLine($"GetOrCreateAsync result{b}");
}
``` 

# Caching Expired
1. The demo caching above wont be expired, except restart the server.
2. Solution: 
   1. when somewhere update or save the data, trigger `Remove` or `Set` to delete or edit the caching (Advantage: Immediately); 
   2. set expiring date, if the expiring date not very long, the inconsistent of caching data wont last so long.
   3. **AbsoluteExpiration** and **SlidingExpiration**, both can mix to use.

`AbsoluteExpiration`
```
public async Task<ActionResult<Book?>> GetBookById(long id)
{
    Console.WriteLine($"Start to Execute GetBookById,id={id}");
    Book? b = await memCache.GetOrCreateAsync("Book"+id, async(e) => {
        Console.WriteLine($"Cant find in cache, search in DB, id={id}");
        e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
        return await MyDbContext.GetByIdAsync(id);
    });
    Console.WriteLine($"GetOrCreateAsync result{b}");
}
``` 


`SlidingExpiration`

When the caching hasn't expired yet, a new request will extend the expiration time again.
```
public async Task<ActionResult<Book?>> GetBookById(long id)
{
    Console.WriteLine($"Start to Execute GetBookById,id={id}");
    Book? b = await memCache.GetOrCreateAsync("Book"+id, async(e) => {
        Console.WriteLine($"Cant find in cache, search in DB, id={id}");
        e.SlidingExpiration = TimeSpan.FromSeconds(10);
        return await MyDbContext.GetByIdAsync(id);
    });
    Console.WriteLine($"GetOrCreateAsync result{b}");
}
``` 

Mix `AbsoluteExpiration` and `SlidingExpiration`

AbsoluteExpiration will set the max Expiration time, even SlidingExpiration keep extend, but still will expire.
```
public async Task<ActionResult<Book?>> GetBookById(long id)
{
    Console.WriteLine($"Start to Execute GetBookById,id={id}");
    Book? b = await memCache.GetOrCreateAsync("Book"+id, async(e) => {
        Console.WriteLine($"Cant find in cache, search in DB, id={id}");
        e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        e.SlidingExpiration = TimeSpan.FromSeconds(10);
        return await MyDbContext.GetByIdAsync(id);
    });
    Console.WriteLine($"GetOrCreateAsync result{b}");
}
``` 

# Cache Penetration 缓存穿透
This happen when the key doesn't exist in the cache or the database. This problem creates a lot of pressuere on both the cache and the database

Example:

 code below will keep executing by malicious(恶意) request, if the cacheKey is a fake key, doesnt exist in both cache and db.

```
string cacheKey = "Book"+id; // 缓存键
Book? b = memCache.Get<Book?>(cacheKey);
if(b==null) //如果缓存中没有数据
{
    // 查数据库，然后写入缓存
    b = await.dbCtx.Books.FindAsync(id);
    memCache.Set(cacheKey,b);
}
```

Solution:

`GetOrCreateAsyns` will store `null` value as legal value in cache. So that cache will return null when next query without going to DB again.

```
string cacheKey = "Book"+id; // 缓存键
Book? b = memCache.Get<Book?>(cacheKey);
var book = await memCache.GetOrCreateAsync(cacheKey, async(e) =>{
    var b = await.dbCtx.Books.FindAsync(id);
    logger.LogInformation("数据库查询：{0}"， b==null? "为空"："不为空")；
    return b;
});
logger.LogInformation("Demo5执行结束：{0}"， book==null? "为空"："不为空")；
```

# Cache Avalanche 缓存雪崩
This happen when a lots of cached data expire at the same time or the cache service is down and all suddenlly search these data at DB, cause high load to the DB layer and impact the performance.

Solution:

set a random expiring date
```
public async Task<ActionResult<Book?>> GetBookById(long id)
{
    Console.WriteLine($"Start to Execute GetBookById,id={id}");
    Book? b = await memCache.GetOrCreateAsync("Book"+id, async(e) => {
        Console.WriteLine($"Cant find in cache, search in DB, id={id}");
        e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Random.Shared.Next(10,15)); // set a random expiring second, 10 is min second, 15 is max second
        return await MyDbContext.GetByIdAsync(id);
    });
    Console.WriteLine($"GetOrCreateAsync result{b}");
}
``` 

# Extension for Cache Helper by YangZhongKe

1. `IQueryable`, `IEnumerable` might encounter delay issues. For example, when `IQueryable`, `IEnumerable` object is initialized and point to a cache value, it doesn't immediately execute the query. Instead, it executes the query when it begins to loop. However, if the object pointed to in the cache has been released by that time, it will lead to a failed execution. Therefore, these two must be banned in the cache.

Solution:

Nuget package: Zack.ASPNETCore 

[Link to Github MemoryCacheHelper](https://github.com/yangzhongke/NETBookMaterials/blob/main/%E6%9C%80%E5%90%8E%E5%A4%A7%E9%A1%B9%E7%9B%AE%E4%BB%A3%E7%A0%81/YouZack-VNext/Zack.ASPNETCore/MemoryCacheHelper.cs)

[Link to Video Explanation](https://www.bilibili.com/video/BV1pK41137He?p=126&vd_source=0db8da3630570b65a3001fb44134ff14)

`IMemoryCacheHelper`
```
public interface IMemoryCacheHelper
{
    TResult? GetOrCreate<Result>(string cacheKey, Func<ICacheEntry, TResult?>valueFactory, int expireSeconds);
    Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactury, int expireSeconds);
    void Remove(string cacheKey);
}
```

```
public async Task<ActionResult<Book?>> Test2(long id)
{
    var b = await memCacheHelper.GetOrCreateAsync("Book"+id, async(e) => {
        return await MyDbContext.GetByIdAsync(id);
    }, 10);
    if(b==null){
        return NotFound("Not Exists");
    }
    else{
        return b;
    }
}
```

# Distributed Cache
1. Frequently use by distributed cache server: `Redis`, `Memcached`.
2. .Net Core provide interface `IDistributedCache` similar to `IMemoryCache`.
3. Difference between **Distributed Cache** and **Memory Cache**: 
   > **Distributed Cache** store their value as byte[], so we need to convert into byte, and also provide some string type storing extension method.

[Link to Video](https://www.bilibili.com/video/BV1pK41137He?p=127&spm_id_from=pageDriver&vd_source=0db8da3630570b65a3001fb44134ff14)

## Redis (How to use)
1. Nuget install Microsoft.Extensions.Caching.StackExchangeRedis
2. 
```
builder.Services.AddStackExchangeRedisCache(options=>{
    options.Configuration = "localhost";
    options.InstanceName = "yzk_";
})
```

```
private readonly IDistributedcache distCache;
public TestController(IDistributedcache distCache)
{
    this.distCache = distCache;
}
```

```
[HttpGet]
public async Task<ActionResult<Book?>> Test3(long id)
{
    Book? book;
    string? s = await disCache.GetStringAsync("Book"+id);
    if(s==null)
    {
        Console.WriteLine($"Take from DB id={id}");
        book = await MyDbContext.GetByIdAsync(id);
        await disCache.SetStringAsync("Book"+id, JsonSerializer.Serialize(book));
    }
    else
    {
        Console.WriteLine($"Take from Cache id={id}");
        book = JsonSerialize.Deserialze<Book?>(s);
    }
    
    if(b==null){
        return NotFound("Not Exists");
    }
    else{
        return b;
    }
}
```