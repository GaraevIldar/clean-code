using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("document_access")]
public class DocumentAccess
{
    [Key]
    [Column("document_access_id")]
    public int DocumentAccessId { get; set; }

    [Required]
    [Column("document_id")]
    public int DocumentId { get; set; }

    [ForeignKey("DocumentId")]
    public Document Document { get; set; }  

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } 

    [Required]
    [Column("status")]
    public string Status { get; set; } 
}