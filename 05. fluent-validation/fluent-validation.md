# FluentValidation
FluentValidation is a .NET library for building strongly-typed validation rules.

similar to `EF Core`'s `Fluent API` which we can validate the model by creating them in a single class.

## How to use

1. NuGet: FluentValidation.AspNetCore
2. `program.cs`
   ```
    builder.Services.AddFluentValidation(fv=>{
        Assembly assembly = Assembly.GetExecutingAssembly();
        fv.RegisterValidatorsFromAssembly(assembly);
    })
    ```

3. `DemoControlller.cs` ---> **AddNewUserRequest**
   ```
    [HttpPost]
    public async Task<ActionReuslt> AddNew (AddNewUserRequest req)
    [
        MyUser user = new MyUser{UserName=req.UserName, Email=req.Email};
        await userManager.CreateAsync(user, req.Password).CheckAsync();
        return Ok();
    ]
   ```

4. `AddNewUserRequest`
   ```
    public class AddNewUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
    }
   ```
5. NET Core build in validator `System.ComponentModel.DataAnnotations`
   ```
    using System.ComponentModel.DataAnnotations;

    public class AddNewUserRequest
    {
        [MinLength(3)]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string Password2 { get; set; }
    }
   ```

   Disadvantages of build in validator `System.ComponentModel.DataAnnotations`
   - model and db very tight couple
   - violation of the single responsibility principle 违反 单一职责原则 
   - a lot of validation need to customize

5. `FluentValidation` library
   ```
    using FluentValidation;
    namespace DataValudationdemo
    {
        public class AddNewUserRequestValidator:AbstractValidator<AddNewUserRequest>
        {
            public AddNewUserRequestValidator()
            {
                RuleFor(x=>x.Email).NotNull().EmailAddress().WithMessage("Email must be legal").Must(x=>x.EndsWith("@gmail.com") || x.EndsWith("@hotmail.com")).WithMessage("Email only support gmail or hotmail");
                
                RuleFor(x=>x.UserName).NotNull().Length(3,10);
                RuleFor(x=>x.Password).Equal(x=>x.Password2).WithMessage("Both password must be same");
            }
        }
    }
   ```

6. validate if user already exist `UserManager<MyUser> userManager`
   ```
    using FluentValidation;
    namespace DataValudationdemo
    {
        public class AddNewUserRequestValidator:AbstractValidator<AddNewUserRequest>
        {
            public AddNewUserRequestValidator(UserManager<MyUser> userManager)
            {
                RuleFor(x=>x.Email).NotNull().EmailAddress().WithMessage("Email must be legal").Must(x=>x.EndsWith("@gmail.com") || x.EndsWith("@hotmail.com")).WithMessage("Email only support gmail or hotmail");
                
                RuleFor(x=>x.UserName).NotNull().Length(3,10).MustAsync(async (x,_)=>await userManager.FindByNameAsync(x)==null).WithMessage(x=>$"UserName {x.UserName} already existed");
                RuleFor(x=>x.Password).Equal(x=>x.Password2).WithMessage(x=>$"Password{x.Password} and {x.Password2} not same");
            }
        }
    }
   ```