
using HackerNews.Models;
using HackerNews.Services;
using HackerNews.Services.Interfaces;
using Polly;

namespace HackerNews;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(c =>
        {
            c.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
        });

        var apiOptions = builder.Configuration.GetRequiredSection("Api").Get<ApiOptions>();
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetRequiredSection(key: "Api"));

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<ICachingService, CachingService>();
        builder.Services.AddSingleton<IRateLimiterService, RateLimiterService>();

        builder.Services.AddTransient<IHackerNewsService, HackerNewsService>();
        builder.Services.AddTransient<ICommunicationService, CommunicationService>();
        builder.Services.AddHttpClient<ICommunicationService, CommunicationService>(client =>
        {
            client.BaseAddress = new Uri(apiOptions!.BaseAddress);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddTransientHttpErrorPolicy(
            x => x.WaitAndRetryAsync(apiOptions!.RetryAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));


        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseAuthorization();
        app.UseHttpsRedirection();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapControllers();

        app.Run();
    }
}