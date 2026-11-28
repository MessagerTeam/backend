using Messenger.User.Api.Contracts;
using Messenger.User.Features.Common;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.User.Features.Users.GetMe;

/// <summary>Профиль текущего пользователя.</summary>
public sealed class GetMeHandler(UserDbContext db)
{
    public async Task<UserMe> HandleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null || user.DeletedAt is not null)
        {
            throw new DomainException(
                StatusCodes.Status401Unauthorized,
                "Пользователь не найден.");
        }

        return user.ToUserMe();
    }
}
