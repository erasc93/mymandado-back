using Dapper.Contrib.Extensions;

namespace information_schema.tables;

[Table("AppUsers")]
public class AppUser
{
    [Key]
    public int id { get; set; }
    public string username { get; set; }
    public string firstname { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}
