using Messenger.Shared.Auth;
using Messenger.Shared.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessengerJwt(builder.Configuration);
builder.Services.AddMessengerHealthChecks();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapMessengerHealthChecks();
app.MapGet("/", () => Results.Ok(new { service = "user", status = "ok" }));

app.Run();
