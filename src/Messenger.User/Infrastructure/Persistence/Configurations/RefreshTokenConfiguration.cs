using Messenger.User.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.User.Infrastructure.Persistence.Configurations;

/// <summary>Маппинг таблицы <c>refresh_tokens</c>.</summary>
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(x => x.Id)
            .HasName("pk_refresh_tokens");

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.FamilyId)
            .HasColumnName("family_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasColumnType("bytea")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .HasColumnName("revoked_at")
            .HasColumnType("timestamptz");

        builder.Property(x => x.ReplacedById)
            .HasColumnName("replaced_by_id");

        builder.Property(x => x.UserAgent)
            .HasColumnName("user_agent")
            .HasColumnType("text");

        builder.Property(x => x.Ip)
            .HasColumnName("ip")
            .HasColumnType("inet");

        builder.HasOne(x => x.ReplacedBy)
            .WithMany(x => x.ReplacedByTokens)
            .HasForeignKey(x => x.ReplacedById)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_refresh_tokens_refresh_tokens_replaced_by_id");

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("ux_refresh_tokens_token_hash");

        builder.HasIndex(x => new { x.UserId, x.FamilyId })
            .HasDatabaseName("ix_refresh_tokens_user_family");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("ix_refresh_tokens_expires_at");
    }
}
