namespace WebApplication1.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(d => d.DocumentId);

        builder.Property(d => d.DocumentName)
            .IsRequired()
            .HasColumnName("document_name");

        builder.HasMany(d => d.UserDocuments)
            .WithOne(ud => ud.Document)
            .HasForeignKey(ud => ud.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.DocumentAccesses)
            .WithOne(da => da.Document)
            .HasForeignKey(da => da.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
