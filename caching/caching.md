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

# Extension for Cache Helper by YangZhongKe (MemoryCacheHelper)

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
private readonly IDistributedCache distCache;
public TestController(IDistributedCache distCache)
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

## Cache Penetration 缓存穿透 (Distributed Cache)

The code above `GetStringAsync` already solved the **Cache Penetration 缓存穿透**, cause although value is **null**, it will also save into Redis with value null.

## Cache Avalanche 缓存雪崩 (Distributed Cache)

Can solve **Cache Avalanche 缓存雪崩** by using `DistributedCacheHelper` to set the expiration timming.


# Extension for Cache Helper by YangZhongKe (DistributedCacheHelper)


[Link to Github DistributedCacheHelper](https://github.com/yangzhongke/NETBookMaterials/blob/main/%E6%9C%80%E5%90%8E%E5%A4%A7%E9%A1%B9%E7%9B%AE%E4%BB%A3%E7%A0%81/YouZack-VNext/Zack.ASPNETCore/DistributedCacheHelper.cs)

[Link to Video Explanation](https://www.bilibili.com/video/BV1pK41137He?p=128&vd_source=0db8da3630570b65a3001fb44134ff14)

```
public interface IDistributedCacheHelper
{
    TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds);
    Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds);
    void Remove(string cacheKey);
    Task RemoveAsync(string cacheKey);
}
```

`Program.cs`
```
builder.Services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>(); 
```

`TestController.cs`
```
private readonly IDistributedCacheHelper distCacheHelper;
public TestController(IDistributedCacheHelper distCacheHelper)
{
    this.distCacheHelper = distCacheHelper;
}

[HttpGet]
public async Task<ActionResult<Book?>> Test6(long id)
{
    var book = await distCacheHelper.GetOrCreateAsync("Book"+id, async (e) => {
        e.SlidingExpiration = TimeSpan.FromSeconds(5);
        var book = await MyDbContext.GetByIdAsync(id);
        return book;
    },20);

    if(book==null){
        return NotFound("Not Exists");
    }
    else{
        return book;
    }
}

```


# valueFactory

**Definition**

is a delegate or a function that encapsulates the logic for creating or retrieving a value. It takes any required input parameters and returns the desired value.

**Deferred Execution 延迟执行**
One of the key features of a `valueFactory` is that it represents deferred execution. The logic inside the `valueFactory` is not executed until it's needed. This allows for efficient resource utilization and lazy loading of data.


`Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory` 

This delefate type represents a method that takes an `DistributedCacheEntryOptions` object as input and returns a nullable `TResult`. In other words, it's a function that can create a value of type `TResult` based on some logic and the information provided by the cache entry.

**Usage Explanation**
- When u call the `GetOrCreateAsync` method, u pass a function (represented by `valueFactory`) as an argument. This function will be invoked by the `GetOrCreateAsync` method if the value corresponding to the `cacheKey` doesnt exist in the cache.
- The `valueFactory` function's responsibility is to generate the value that needs to be cached. It can use the provided `DistributedCacheEntryOptions` to set various properties of the cache entry, such as expiration time, priority, etc.
- After the `valueFactory` function completes its execution, the value it generates (of type `TResult`) is stored in the cache entry and returned by the `GetOrCreateAsync` method.

**Example**

`1. Code from Zack.ASPNETCore DistributedCacheHelper.cs`
```
public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            string jsonStr = await distCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = await valueFactory(options);
                string jsonOfResult = JsonSerializer.Serialize(result,
                    typeof(TResult));
                await distCache.SetStringAsync(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                await distCache.RefreshAsync(cacheKey);
                return JsonSerializer.Deserialize<TResult>(jsonStr)!;
            }
        }
```

`2. TestController.cs` (call the `GetOrCreateAsync` method)
```
private readonly IDistributedCacheHelper distCacheHelper;
public TestController(IDistributedCacheHelper distCacheHelper)
{
    this.distCacheHelper = distCacheHelper;
}

[HttpGet]
public async Task<ActionResult<Book?>> Test6(long id)
{
    var book = await distCacheHelper.GetOrCreateAsync("Book"+id, async (e) => {
        e.SlidingExpiration = TimeSpan.FromSeconds(5);
        var book = await MyDbContext.GetByIdAsync(id);
        return book;
    },20);

    if(book==null){
        return NotFound("Not Exists");
    }
    else{
        return book;
    }
}

```

`3. valueFactory` example
```
async (e) => {
    return await MyDbContext.GetByIdAsync(id);
}
```
Code above is the `valueFactory` that will be executed when the `distCache` cannot get the value.

Lets break it down:

`async (e) => { ... }`: This is a lambda expression defining an asynchronous function that takes an `DistributedCacheEntryOptions` parameter named e.

`e.SlidingExpiration = TimeSpan.FromSeconds(5);`: This line sets the sliding expiration for the cache entry to 5 seconds.

`var book = await MyDbContext.GetByIdAsync(id);`: This line represents an asynchronous operation that retrieves a book from the database by its `id`. Since it's an asynchronous operation, it returns a `Task<Book?>`.

`return book`;: This line returns the retrieved book.

The entire lambda expression `async (e) => { ... }` serves as the `valueFactory` function passed to the GetOrCreateAsync method. When invoked, this function will execute asynchronously to fetch the book from the database if it's not already cached, and then cache the result using the provided `DistributedCacheEntryOptions`.