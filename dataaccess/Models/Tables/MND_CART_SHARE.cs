using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("CART_SHARES")]
public class MND_CART_SHARE : IDbTable
{
    [_dap.Key]
    public int csh_id { get; set; }
    public required int csh_carid { get; set; }
    public required int csh_usrid { get; set; }
}
