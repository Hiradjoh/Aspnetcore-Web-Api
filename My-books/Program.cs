using Microsoft.EntityFrameworkCore;
using My_books;
using My_books.Data.Services;
using My_books.Exceptions.MiddleWares;

var builder = WebApplication.CreateBuilder(args);
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


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

app.UseHttpsRedirection();
app.UseAuthorization();

#region [-Exception-Handling-]
// Exception Handling
app.ConfigureBuildInExceptionHandler();
// app.ConfigureCustomExceptionHandler();
#endregion

app.MapControllers();
app.Run();

