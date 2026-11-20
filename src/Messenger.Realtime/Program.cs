using Messenger.Realtime.Hubs;
using Messenger.Shared.Auth;
using Messenger.Shared.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessengerJwt(builder.Configuration);
builder.Services.AddMessengerHealthChecks();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapMessengerHealthChecks();
app.MapHub<ChatHub>("/hubs/chat");
app.MapGet("/", () => Results.Ok(new { service = "realtime", status = "ok" }));

app.Run();
