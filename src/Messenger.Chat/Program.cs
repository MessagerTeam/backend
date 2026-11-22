using Messenger.Shared.Auth;
using Messenger.Shared.Health;
using Messenger.Shared.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessengerJwt(builder.Configuration);
builder.Services.AddMessengerHealthChecks();
builder.Services.AddControllers();
builder.Services.AddMessengerSwagger(
    title: "Messenger Chat API",
    description: "Чаты, сообщения и история.",
    serviceXmlFile: "Messenger.Chat.xml");

var app = builder.Build();

app.UseMessengerSwagger("Messenger Chat API");

app.UseAuthentication();
app.UseAuthorization();

app.MapMessengerHealthChecks();
app.MapControllers();

app.Run();
