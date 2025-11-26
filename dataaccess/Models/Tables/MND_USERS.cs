using _dap = Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;
using core_mandado.Users;

namespace models.tables;
public interface IDbTable { }
[_dap.Table("USERS")]
public class MND_USERS : IDbTable
{
    [_dap.Key]
    public int usr_id { get; set; }
    public required User.Role usr_role { get; set; }
    public required string usr_name { get; set; }
    public string usr_hashword { get; set; }

}