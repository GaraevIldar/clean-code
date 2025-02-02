using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user_documents")]
public class UserDocument
{
    [Key]
    [Column("user_documents_id")]
    public int UserDocumentId { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required]
    [Column("document_id")]
    public int DocumentId { get; set; }

    [ForeignKey("DocumentId")]
    public Document Document { get; set; }
}