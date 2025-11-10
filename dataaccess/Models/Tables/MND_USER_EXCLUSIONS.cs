using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("PRODUCTS_EXCLUSIONS")]
public class MND_USER_EXCLUSIONS:IDbTable
{
    public required int xld_userid { get; set; }
    public required int xld_prdid { get; set; }
}


