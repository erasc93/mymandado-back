using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("CART_ITEMS")]
public class MND_CART_ITEM:IDbTable

{

    [_dap.Key]
    public int crt_itid { get; set; }
    public required int crt_qtty { get; set; }
    public required int crt_userid { get; set; }
    public required int crt_prdid { get; set; }
    public bool crt_isdone { get; set; }
}

