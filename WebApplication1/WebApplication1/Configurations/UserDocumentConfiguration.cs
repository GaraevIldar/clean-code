using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
{
    public void Configure(EntityTypeBuilder<UserDocument> builder)
    {
        builder.ToTable("user_documents");

        builder.HasKey(ud => ud.UserDocumentId);

        builder.HasOne(ud => ud.User)
            .WithMany(u => u.UserDocuments)
            .HasForeignKey(ud => ud.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ud => ud.Document)
            .WithMany(d => d.UserDocuments)
            .HasForeignKey(ud => ud.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}