# Dependency Injection

Software design pattern to separate the dependencies of a clas from its implementation.

Dependency injection helps in creating loosely coupled components by providing dependencies to an object from an external source, rather than creating them within the object itself.


## Lifetime of Dependency Injection

1. Transient
   
   >It created an instance each time they are requested and are never shared. It is used mainly for lightweight stateless services. 
2. Scoped
   >It creates an isntance once per scope, which is created on every request to the application.
3. Singleton
   >This creates only single instances which are shared among all components that require it.

## Advantages of using dependency injection in .NET Core

- Loose Coupling
  > Dependency injection allows us to decouple our classes from their dependencies. This makes our code more maintainable and easier to test.
- Testability
  > Dependency injection makes our code more testable because we can mock out dependencies in our unit test.
- Extensibility
  > Dependency injection makes our code more extensible because we can easily swap out dependencies.
- Reusability
  > Dependency injection makes our code more reusable because we can easily share dependencies between different classes.

## What are the different types of dependency injection?
- Constructor Injection
  > Constructor injection is the most common type of dependency injection. In constructor injection, we inject the dependency into the class constructor.
- Property Injection
  > Property Injection is less common than constructor injection. In property injection, we inject the dependency into a property of the class.


[Link to Reference DI in .Net Core](https://www.c-sharpcorner.com/article/dependency-injection-in-net-core/)



## Dependency Injection **Extension**
1. role
   1. Client 
   2. Service

`LogServices (Service)`

```
namespace Microsoft.Extensions.DependecyInjection
{
    public static class ConsoleLogExtensions
    {
        public static void AddConsoleLog(this IServiceCollection services)
        {
            services.AddScoped<ILogProvider, ConsoleLogProvider>();
        }
    }
}
```

`ConsoleAppMainlSender (Client)`

```
namespace ConsoleAppMailSender
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            // original method
            services.AddScoped<ILogProvider, ConsoleLogProvider>();
            // extension method, can straight away get the hint
            services.AddConsoleLog();
        }
    }
}
```