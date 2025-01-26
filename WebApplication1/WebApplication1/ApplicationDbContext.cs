using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<UserDocument> UserDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserName).IsRequired().HasColumnName("user_name");
            entity.Property(u => u.UserPassword).IsRequired().HasColumnName("user_password");
            entity.Property(u => u.UserSalt).IsRequired().HasColumnName("user_salt");
            
            entity.HasMany(u => u.UserDocuments)
                  .WithOne(ud => ud.User)
                  .HasForeignKey(ud => ud.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(d => d.DocumentId);
            entity.Property(d => d.DocumentName).IsRequired().HasColumnName("document_name");
            
            entity.HasMany(d => d.UserDocuments)
                  .WithOne(ud => ud.Document)
                  .HasForeignKey(ud => ud.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<UserDocument>(entity =>
        {
            entity.ToTable("user_documents");
            entity.HasKey(ud => ud.UserDocumentId);
            entity.HasOne(ud => ud.User)
                  .WithMany(u => u.UserDocuments)
                  .HasForeignKey(ud => ud.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ud => ud.Document)
                  .WithMany(d => d.UserDocuments)
                  .HasForeignKey(ud => ud.DocumentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
