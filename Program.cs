using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(opt =>
        {
            opt.Events = new JwtBearerEvents()
            {
                OnMessageReceived = msg =>
                {
                    var token = msg?.Request.Headers.Authorization.ToString();
                    string path = msg?.Request.Path ?? "";
                    if (!string.IsNullOrEmpty(token))

                    {
                        Console.WriteLine("Access token");
                        Console.WriteLine($"URL: {path}");
                        Console.WriteLine($"Token: {token}\r\n");
                    }
                    else
                    {
                        Console.WriteLine("Access token");
                        Console.WriteLine("URL: " + path);
                        Console.WriteLine("Token: No access token provided\r\n");
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = msg =>
                {
                    Console.WriteLine(msg.Exception.Message);
                    return Task.CompletedTask;
                }

            };


        });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Any", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Any");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.UseAuthorization();

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
