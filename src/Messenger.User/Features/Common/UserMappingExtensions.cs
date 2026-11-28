using Messenger.User.Api.Contracts;
using UserEntity = Messenger.User.Domain.User;

namespace Messenger.User.Features.Common;

/// <summary>Маппинг доменного пользователя в DTO.</summary>
public static class UserMappingExtensions
{
    public static AuthUser ToAuthUser(this UserEntity user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        DisplayName = user.DisplayName,
        AvatarUrl = null,
        Email = user.Email,
    };

    public static UserMe ToUserMe(this UserEntity user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        DisplayName = user.DisplayName,
        AvatarUrl = null,
        Email = user.Email,
        Bio = user.Bio,
        CreatedAt = user.CreatedAt,
    };
}
