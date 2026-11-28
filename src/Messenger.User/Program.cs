using Messenger.Shared.Auth;
using Messenger.Shared.Correlation;
using Messenger.Shared.Health;
using Messenger.Shared.Swagger;
using Messenger.User.Extensions;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMessengerJwt(builder.Configuration);
builder.Services.AddMessengerHealthChecks();
builder.Services.AddControllers();
builder.Services.AddUserAuth();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(
            ApiErrorFactory.FromModelState(context.HttpContext, context.ModelState));
});

var connectionString = builder.Configuration.GetConnectionString("UsersDb")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:UsersDb is not configured.");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMessengerSwagger(
    builder.Configuration,
    title: "Messenger User API",
    description: "Регистрация, вход, профиль и поиск пользователей.",
    serviceXmlFile: "Messenger.User.xml");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

app.UseCorrelationId();
app.UseExceptionHandler();

app.UseMessengerSwagger("Messenger User API");

app.UseAuthentication();
app.UseAuthorization();

app.MapMessengerHealthChecks();
app.MapControllers();

app.Run();
