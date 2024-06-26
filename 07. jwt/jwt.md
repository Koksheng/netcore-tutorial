# JWT 

[Link to video](https://www.bilibili.com/video/BV1pK41137He?p=147&vd_source=0db8da3630570b65a3001fb44134ff14)

## Session 的缺点

store session id and user id 

1. In distributed server environment, **Session** not suitable to save data on to server's memory cache, should put it on to a status server. **ASP.NET Core** support **Session** in **Redis**, **Memcached**.

2. But using status server will have performance issue.

# JWT (Json Web Token)
1. JWT save login message (also known token) on the client side.
2. To prevent client side use the fake jwt, the token stored on client side already encoded by signature handling, and the key of signature only server side will know. So everytime the server receive token from client side, have to check their signature first.
3. How JWT achieve **Login**
   - Header
    ```
    {"alg":"hmac-sha256","typ":"JWT"}
    ```
   - Payload
    ```
    {"id":"6","name":"admin","exp":"1633842858"}
    ```
   - Signature
    ```
    HMACSHA256(header+"."+payload,secKEy)
    ```

4. Advantages of JWT
   1. status of user store on **client** side, but not **server** side, suitable for distributed server.
   2. Signature can prevent **client** side submit the fake JWT.
   3. Performance better, no need to connect with status server (which storing user's status), JWT is just pure in memory calculation. 

## How to use
1. Create a console app
2. NuGet: System.IdentityModel.Tokens.Jwt
3. `program.cs`
    ```
    using System.Security.Claims;

    List<Claim> claims = new List<Claim>(); 
    claims.Add(new Claim("Passport","123456"));         // freely string type
    claims.Add(new Claim(ClaimTypes.Name, "yzk"));      // with build in type
    claims.Add(new Claim(ClaimTypes.HomePhone, "999")); // with build in type
    claims.Add(new Claim(ClaimTypes.Role, "admin"));    // with build in type
    claims.Add(new Claim(ClaimTypes.Role, "manager"));  // with build in type
    string key = "abcdefghij12345546546" // min 16 character
    DateTime expires = DateTime.Now.AddDays(1);
    byte[] secBytes = Encoding.UTF8.GetBytes(key);
    var secKey = new SymmetricSecurityKey(secBytes);
    var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
    var tokenDescriptor = new JwtSecurityToken(claims:claims, expires:expires, signingCredentials:credentials);
    string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    Console.WriteLine(jwt);
    ```
4. Example

    **Encoded JWT**
    `yJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c`

    **Decoded**
    `Header` : Algorithm & Token Type
    ```
    {
        "alg": "HS256",
        "typ": "JWT"
    }
    ```
    `Payload` : Data
    ```
    {
        "sub": "1234567890",
        "name": "John Doe",
        "iat": 1516239022
    }
    ```
    `Verify Signature`
    ```
    HMACSHA256(
        base64UrlEncode(header) + "." +
        base64UrlEncode(payload),
        
        your-256-bit-secret

     ) secret base64 encoded
    ```

# JWT 的 Encapsulation

[Link to video](https://www.bilibili.com/video/BV1pK41137He?p=149&spm_id_from=pageDriver&vd_source=0db8da3630570b65a3001fb44134ff14)

1. Config JWT node, under node config `SigningKey`, `ExpireSeconds`.
2. Nuget: Microsoft.AspNetCore.Authentication.JwtBearer
3. `JWTSettings`
    ```
    namespace JWTWebApp1
    {
        public class JWTSettings
        {
            public string SecKey { get; set; }
            public int ExpireSeconds { get; set; }
        }
    }
    ```
4. `appsettings.json`
    ```
    {
        "JWT": {
            "SecKey": "dsf;sldjfkds32494302947@#@%#@#",
            "ExpireSeconds": 3600
        }
    }
    ```
5. `Program.cs`
    ```
    builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
        var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTSettings>();
        btye[] keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);
        var secKey = new SymmetricSecurityKey(keyBytes);
        opt.TokenValidationParameters = new ()
        {
            ValidateIssuer=false, 
            ValidateAudience=false, 
            ValidateLifetime=true,
            ValidateIssuerSigningKey=true, 
            IssuerSigningKey=secKey
        };
    });
    ```
6. `Program.cs`
    add `app.UseAuthentication()` before `app.UseAuthorization()`

7. `DemoController.cs` create a fake Login
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        public class DemoController : ControllerBase
        {
            private readonly IOptionsSnapshot<JWTSettings> jwtSettingsOpt;

            public DemoController(IOptionsSnapshot<JWTSettings> jwtSettingsOpt)
            {
                this.jwtSettingsOpt = jwtSettingsOpt;
            }

            public ActionResult<string> Login(string userName, string password)
            {
                if(userName=="yzk"&&password=="123456")
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));      // with build in type
                    claims.Add(new Claim(ClaimTypes.Name, userName));      // with build in type
                    
                    string key = jwtSettingsOpt.Value.SecKey;
                    DateTime expires = DateTime.Now.AddSeconds(jwtSettingsOpt.Value.ExpireSeconds);
                    
                    byte[] secBytes = Encoding.UTF8.GetBytes(key);
                    var secKey = new SymmetricSecurityKey(secBytes);
                    var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new JwtSecurityToken(claims:claims, expires:expires, signingCredentials:credentials);
                    string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                    return jwt;
                }
                else{
                    return BadRequest();
                }
            }
        }
    }
    ```

8. Add `[Authorize]` on to **Controller** or **Action** to force user login before using it.
   
    `Demo2Controller.cs` 
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        [Authorize]
        public class DemoController : ControllerBase
        {
            [HttpGet]
            public string Test1()
            {
               return "ok";
            }
        }
    }
    ```

9.  Result when never login, Authorize failed
    `Error: response status is 401`
10. How to send request with Authorize and jwt token?
    1.  Postmane
    2.  Headers
        1.  Authorization
            1.  Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
        **Format: Authorization : Bearer[space]jwtToken**

11. Result when request with Headers Authorization and jwt token
    `Status: 200 OK`, return `ok`
12. In Controller, also can straight away get the value of payload in jwt token
    
    `Demo2Controller.cs` 
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        [Authorize]
        public class DemoController : ControllerBase
        {
            [HttpGet]
            public string Test1()
            {
                var claim = this.User.FindFirst(ClaimTypes.Name); // ClaimTypes.Name = "yzk"
                return "ok" + claim.Value;
            }
        }
    }
    ```
13. Result when request with Headers Authorization and jwt token, and get the value in `ClaimTypes.Name`
    `Status: 200 OK`, return `okyzk`

14. When logout, just clear the JWT token on client side.
15. Also can allow access without login or skip the Authorize by using `[AllowAnonymous]`
    
    `Demo2Controller.cs` 
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        [Authorize]
        public class DemoController : ControllerBase
        {
            [HttpGet]
            public string Test1()
            {
                var claim = this.User.FindFirst(ClaimTypes.Name); // ClaimTypes.Name = "yzk"
                return "ok" + claim.Value;
            }
            [AllowAnonymous] // < -------- here
            [HttpGet]
            public string Test2()
            {
                return "666";
            }
        }
    }
    ```
16. Can also authorize with role

    `DemoController.cs` create a fake Login
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        public class DemoController : ControllerBase
        {
            private readonly IOptionsSnapshot<JWTSettings> jwtSettingsOpt;

            public DemoController(IOptionsSnapshot<JWTSettings> jwtSettingsOpt)
            {
                this.jwtSettingsOpt = jwtSettingsOpt;
            }

            public ActionResult<string> Login(string userName, string password)
            {
                if(userName=="yzk"&&password=="123456")
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));      // with build in type
                    claims.Add(new Claim(ClaimTypes.Name, userName));      // with build in type
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));      // <------ HERE ------
                    
                    string key = jwtSettingsOpt.Value.SecKey;
                    DateTime expires = DateTime.Now.AddSeconds(jwtSettingsOpt.Value.ExpireSeconds);
                    
                    byte[] secBytes = Encoding.UTF8.GetBytes(key);
                    var secKey = new SymmetricSecurityKey(secBytes);
                    var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new JwtSecurityToken(claims:claims, expires:expires, signingCredentials:credentials);
                    string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                    return jwt;
                }
                else{
                    return BadRequest();
                }
            }
        }
    }
    ```

    `Demo2Controller.cs` 
    ```
    using System.Security.Claims;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[action]")]
        [ApiController]
        [Authorize]
        public class DemoController : ControllerBase
        {
            [HttpGet]
            public string Test1()
            {
                var claim = this.User.FindFirst(ClaimTypes.Name); // ClaimTypes.Name = "yzk"
                return "ok" + claim.Value;
            }
            [HttpGet]
            [Authorize(Roles="admin")] // < -------- here
            public string Test3()
            {
                return "333";
            }
        }
    }
    ```

# OpenApi Swagger - add Authorization header    
`Program.cs`
```
builder.Services.AddSwaggerGen(c=>
{
    var scheme = new OpenApiSecurityScheme()
    {
        Description = "Authorization header \r\nExample:'Bearer12345abcdef'",
        Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "Authorization"},
        Scheme = "oauth2", Name="Authorization",
        In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Authorization", scheme);
    var requirement = new OpenApiSecurityRequirement();
    requirement[scheme] = new List<string>();
    c.AddSecurityRequirement(requirement);
})

var app = builder.Build() // <-------- add before this --------
```


# JWT Issue

[Link to video](https://www.bilibili.com/video/BV1pK41137He?p=151&spm_id_from=pageDriver&vd_source=0db8da3630570b65a3001fb44134ff14)

**Backgound**

User account already been deleted, but JWT unable to terminate / invalid from server immediately or make it expire immediately.

User login from multiple device, diff login using same JWT, might possibly steal by other 

**Solution**:

Add a new field : JWTVersion
Need to save the JWT's version into DB, then inject version as one of the key-value in JWT payload. Every new request of JWT, will make the JWTVersion upgrade 1 version

1. `MyUser.cs`
    ```
    using Microsoft.AspNetCore.Identity;

    namespace Identity框架1
    {
        public class MyUser : IdentityUser<long>
        {
            public string? WeiXinAccount { get; set; }
            public long JWTVersion { get; set; }
        }
    }
    ```
2. `DemoController.cs`
    ```
    using Identity框架1;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    namespace JWTWebApp1.Controllers
    {
        [Route("api/[controller]/[acitons]")]
        [ApiController]
        public class DemoController : ControllerBase
        {
            private readonly IOptionsSnapshot<JWTSettings> jwtSettingsOpt;
            private readonly UserManager<MyUser< userManager;

            public DemoController(IOptionsSnapshot<JWTSettings> jwtSettingsOpt, UserManager<MyUser> userManager)
            {
                this.jwtSettingsOpt = jwtSettingsOpt;
                this.userManager = userManager;
            }

            [HttpPost]
            [NotCheckJWTVersion]
            public async Task<ActionResult<string>> Login(string userName, string password)
            {
                var user = await userManager.FindByNameAsync(username);
                if(user==null)
                {
                    return BadRequest("用户名或者密码错误")；
                }
                if(await userManager.CheckPasswordAsync(user,password))
                {
                    await userManager.ResetAccessFailedCountAsync(user).CheckAsync();
                    user.JWTVersion++; // !!!
                    await userManager.UpdateAsync(user); // !!!

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                    claims.Add(new Claim("JWTVersion", user.JWTVersion.ToString())); // !!!
                    var roles = await userManager.GetRolesAsync(user);
                    foreach(var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    string key = jwtSettingsOpt.Value.SecKey;
                    DateTime expires = DateTime.Now.AddSeconds(jwtSettingsOpt.Value.ExpireSeconds);
                    
                    byte[] secBytes = Encoding.UTF8.GetBytes(key);
                    var secKey = new SymmetricSecurityKey(secBytes);
                    var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new JwtSecurityToken(claims:claims, expires:expires, signingCredentials:credentials);
                    string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                    return jwt;
                }
                else{
                    await userManager.AccessFailedAsync(user).CheckAsync();
                    return BadRequest("用户名或者密码错误");
                }
            }
        }
    }
    ```
3. `JWTVersionCheckFilter.cs`
    ```
    using Microsoft.AspNetCore.Mvc.Filters;
    namespace JWT的提前撤回
    {
        public class JWTVersionCheckFilter : IAsyncActionFilter
        {
            private readonly UserManager<MyUser> userManager;
            public JWTVersionCheckFilter(UserManager<MyUser> userManager)
            {
                this.userManager = userManager;
            }
            public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                ControllerActionDescriptor? ctrlActionDesc = context.ActionDescriptor as ControllerActionDescriptor;
                if(ctrlActionDesc==null)
                {
                    await next();
                    return;
                }
                if(ctrlActionDesc.MethodInfo.GetCustomAttributes(typeof(NotCheckJWTVersionAttribute),true).Any())
                {
                    await next();
                    return;
                }

                var claimJWTVer = context.HttpContext.User.FindFirst("JWTVersion");
                if(claimJWTVer==null)
                {
                    context.Result = new ObjectResult("payload中没有JWTVersion"){StatusCode=400};
                    return;
                }
                long jwtVerFromClient = Convert.ToInt64(claimJWTVer.Value);
                var claimUserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(claimUserId.Value);
                if(user==null)
                {
                    context.Result = new ObjectResult("user找不到") {StatusCode = 400};
                    return;
                }
                if(user.JWTVersion > jwtVerFromClient)
                {
                    context.Result = new ObjectResult("客户端的jwt过时") {StatusCode=400};
                    return;
                }
                await next();
            }
        }
    }
    ```
4. `Program.cs`
    ```
    builder.Services.Configure<MvcOptions>(opt => {
        opt.Filters.Add<JWTVersionCheckFilter>();
    });
    ```
5. `NotCheckJWTVersionAttribute.cs`
    ```
    namespace JWT的提前撤回
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class NotCheckJWTVersionAttribute : Attribute
        {

        }
    }
    ```