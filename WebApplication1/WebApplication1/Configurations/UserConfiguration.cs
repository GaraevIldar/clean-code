using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasColumnName("user_name");

        builder.Property(u => u.UserPassword)
            .IsRequired()
            .HasColumnName("user_password");

        builder.Property(u => u.UserSalt)
            .IsRequired()
            .HasColumnName("user_salt");

        builder.HasMany(u => u.UserDocuments)
            .WithOne(ud => ud.User)
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.DocumentAccesses)
            .WithOne(da => da.User)
            .HasForeignKey(da => da.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}