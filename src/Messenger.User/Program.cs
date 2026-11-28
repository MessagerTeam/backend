using Messenger.Shared.Auth;
using Messenger.Shared.Health;
using Messenger.User.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMessengerJwt(builder.Configuration)
    .AddMessengerHealthChecks()
    .AddUserPersistence(builder.Configuration)
    .AddUserAuth()
    .AddUserApi(builder.Configuration);

var app = builder.Build();

app.MigrateUserDatabase();
app.UseUserApi();
app.MapUserEndpoints();

app.Run();
