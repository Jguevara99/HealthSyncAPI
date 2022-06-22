using ContosoPizza.Middlewares.Extensions;
using ContosoPizza.Extensions.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);
// IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services
    .AddConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCustomExceptionHandlerMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
