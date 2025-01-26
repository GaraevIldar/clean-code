using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("user_name")]
    public string UserName { get; set; }

    [Required]
    [Column("user_password")]
    public string UserPassword { get; set; }

    [Required]
    [Column("user_salt")]
    public string UserSalt { get; set; } 
    
    public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
}
