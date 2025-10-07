using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using My_books;
using My_books.Data.Services;
using My_books.Exceptions.MiddleWares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Serilog Configuration
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(configuration)

    .CreateLogger();





//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();


// جایگزینی Logger پیشفرض ASP.NET Core با Serilog
builder.Host.UseSerilog();

// ----------------- Services -----------------
builder.Services.AddControllers();

#region [-Database-Context-]
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<ProjectDbContext>(options => options.UseSqlServer(connectionString));
#endregion

#region [-Dependency-Injection-]
builder.Services.AddTransient<BookService>();
builder.Services.AddTransient<AuthorService>();
builder.Services.AddTransient<PublisherService>();
#endregion

//builder.Services.AddApiVersioning();
//builder.Services.AddApiVersioning(config=>
//{
//    config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
//    config.AssumeDefaultVersionWhenUnspecified = true;
//    config.ApiVersionReader = new HeaderApiVersionReader("custom-version-header"); 
//    config.ApiVersionReader = new MediaTypeApiVersionReader(); 
//});

// OpenAPI / Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// ثبت لاگ تمام درخواست‌ها
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();

#region [-Exception-Handling-]
app.ConfigureBuildInExceptionHandler();
// app.ConfigureCustomExceptionHandler();
#endregion

app.MapControllers();

app.Run();
