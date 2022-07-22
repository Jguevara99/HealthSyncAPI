using ContosoPizza.Middlewares.Extensions;
using ContosoPizza.Extensions.ServiceCollection;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);
// IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services
    .AddConfig(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbSeeder = scope.ServiceProvider.GetService<IDatabaseSeeder>();
    if(dbSeeder is not null)
        dbSeeder.Initialize().GetAwaiter().GetResult();
}

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
