using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using My_books;
using My_books.Data;
using My_books.Data.Models;
using My_books.Data.Services;
using My_books.Data.Services.AuthorizationHandlers;
using My_books.Data.ViewModels.Authentication;
using My_books.Exceptions.MiddleWares;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Host.UseSerilog((context, loggerConfig) =>
{

    loggerConfig.ReadFrom.Configuration(context.Configuration);
    loggerConfig.WriteTo.Seq("http://localhost:5341");
    //loggerConfig.WriteTo.Console();// dar app setting tarif shode 


}); 
//Serilog Configuration


// مشخص کردن Serilog.Log به جای فقط Log
//Serilog.Log.Logger = new Serilog.LoggerConfiguration()
//    .ReadFrom.Configuration(Configuration)
//    .CreateLogger();

// جایگزینی Logger پیشفرض ASP.NET Core با Serilog
//builder.Host.UseSerilog();






//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();




// ----------------- Services -----------------
builder.Services.AddControllers();

#region [-Database-Context-]
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<ProjectDbContext>(options => options.UseSqlServer(connectionString));
#endregion



//builder.Services.AddApiVersioning();
builder.Services.AddApiVersioning(options =>
{

    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("custom-version-header");
    options.ApiVersionReader = new MediaTypeApiVersionReader();

});





// OpenAPI / Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//token validation Parmiters

var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWT:Secret"])),//emza token va kilid emza token

    ValidateIssuer = true,
    ValidIssuer = Configuration["JWT:Issuer"],//sader konande token

    ValidateAudience = true,
    ValidAudience = Configuration["JWT:Audience"],//token baraye koja sader shode

    ValidateLifetime = true,
    ClockSkew=TimeSpan.Zero,//ekhtelaf zamani bein server va client
};


#region [-Dependency-Injection-]
builder.Services.AddSingleton<IAuthorizationHandler, PublisherAuthorizationHandler>();

builder.Services.AddTransient <PublisherService > ();
builder.Services.AddTransient<BookService>();
builder.Services.AddTransient<AuthorService>(); 
builder.Services.AddTransient<LogsService>();
builder.Services.AddSingleton(tokenValidationParameters);

#endregion



// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()// ezafe kardan identity be project
    .AddEntityFrameworkStores<ProjectDbContext>()// moshakhad mikone ke az kodom  DbContext baraye zakhire etelaat identity(claim,role , users) estefade kone
    .AddDefaultTokenProviders();//tamam provider haye token ro baraye identity ezafe mikone(mesle token baraye reset kardan password ya email confirmation)

//AddAuthentication
builder.Services.AddAuthentication(options=> // az che scheme estefade kone 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// ag mikhad user ro authenticate kone az che schema estefade kone
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//ag dasteresi qeyr moja bood az che schema bayad challenge bede 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;// hamon scheme delfult baraye har kari 

})
//Add JWT Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});
//builder.Services.AddAuthorization(options=>
//{
//    options.AddPolicy("PublisherWrite" , policy =>
//        policy.RequireRole("Admin", "Publisher"));

//    options.AddPolicy("PublisherDelete", policy =>
//       policy.RequireRole("Admin"));
//}); 

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PublisherWrite", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Publisher") ||
            context.User.IsInRole("Admin") 
           
        ));
    
});
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
//Authentication  & Authorization
app.UseAuthentication();
app.UseAuthorization();
AppDbInitializer.SeedRoles(app).Wait();

#region [-Exception-Handling-]
app.ConfigureBuildInExceptionHandler();
// app.ConfigureCustomExceptionHandler();
#endregion

app.MapControllers();

app.Run();
