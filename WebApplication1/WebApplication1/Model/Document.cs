using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("documents")]
public class Document
{
    [Key]
    [Column("document_id")]
    public int DocumentId { get; set; }

    [Required]
    [Column("document_name")]
    public string DocumentName { get; set; }
    
    public ICollection<UserDocument> UserDocuments { get; set; }
    
    public ICollection<DocumentAccess> DocumentAccesses { get; set; } = new List<DocumentAccess>();
}