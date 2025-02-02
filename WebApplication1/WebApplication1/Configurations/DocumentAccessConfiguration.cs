using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DocumentAccessConfiguration : IEntityTypeConfiguration<DocumentAccess>
{
    public void Configure(EntityTypeBuilder<DocumentAccess> builder)
    {
        builder.ToTable("document_access");

        builder.HasKey(da => da.DocumentAccessId);

        builder.Property(da => da.Status)
            .IsRequired()
            .HasColumnName("status");

        builder.HasOne(da => da.Document)
            .WithMany(d => d.DocumentAccesses)
            .HasForeignKey(da => da.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(da => da.User)
            .WithMany(u => u.DocumentAccesses)
            .HasForeignKey(da => da.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasCheckConstraint("CHK_status", "status IN ('creator', 'reader', 'editor')");
    }
}