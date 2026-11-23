using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Messenger.Shared.Swagger;

/// <summary>Убирает требование Bearer для методов и контроллеров с [AllowAnonymous].</summary>
public sealed class AnonymousOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var isAnonymous =
            context.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() is not null
            || context.MethodInfo.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;

        if (isAnonymous)
        {
            operation.Security = [];
        }
    }
}
