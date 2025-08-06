using System.Net;
using LiveStreamingServerNet;
using LiveStreamingServerNet.Flv.Installer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOriginPolicy",
            policy =>
            {
                policy.AllowAnyOrigin() // Allows requests from any origin
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });


builder.Services.AddLiveStreamingServer(
    new IPEndPoint(IPAddress.Any, 1935),
    options => options.AddFlv()
);

var app = builder.Build();

app.UseWebSockets();
app.UseWebSocketFlv();

app.UseCors("AllowAnyOriginPolicy");
app.UseStaticFiles();

app.UseHttpFlv();

await app.RunAsync();