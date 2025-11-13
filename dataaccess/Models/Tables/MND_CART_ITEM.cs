using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("CART_ITEMS")]
public class MND_CART_ITEM : IDbTable
{

    [_dap.Key]
    public int crt_id { get; set; }
    public required int crt_usrid { get; set; }
    public required int crt_crtnb { get; set; }
    public required int crt_prdid { get; set; }
    public required int crt_qtty { get; set; }
    public required bool crt_isdone { get; set; }
}
