using Restaurants.API.Extensions;
using Serilog;

try
{
var builder = WebApplication.CreateBuilder(args);

builder.AddBuilderExtensions();

var app = builder.Build();

await app.AddMiddleWare();

app.Run();

}
catch(Exception ex){
    Log.Fatal(ex, "Program startup failed!");
}
finally{
    Log.CloseAndFlush();
}

public partial class Program{}

/*
    The logic of public partial class Program{} :

    Partial classes in C# Allows us to write the same class definition accross multiple files or in different sections of the same file.
    It's useful for keeping the class definition code tidy. And it integrates well with version control systems.

    Though our use case here is a little bit different.
    If different parts of a class defined with partial classes (We got a main class Program and a partial class as it's extension here) has different access levels,
    the compiler will merge the class definition into one taking the most "public" access level as the finite one.

    in this case the main Program is an internal class but we extended it with an empty partial Program that is public, 
    making the whole Program class as public so it can be used in Integration testing (see Restaurants.API.Test.Controllers) via xUnit dependancy injection.  
*/