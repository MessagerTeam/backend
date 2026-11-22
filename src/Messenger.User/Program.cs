using Messenger.Shared.Auth;
using Messenger.Shared.Health;
using Messenger.Shared.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessengerJwt(builder.Configuration);
builder.Services.AddMessengerHealthChecks();
builder.Services.AddControllers();
builder.Services.AddMessengerSwagger(
    title: "Messenger User API",
    description: "Регистрация, вход, профиль и поиск пользователей.",
    serviceXmlFile: "Messenger.User.xml");

var app = builder.Build();

app.UseMessengerSwagger("Messenger User API");

app.UseAuthentication();
app.UseAuthorization();

app.MapMessengerHealthChecks();
app.MapControllers();

app.Run();
