using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("PRODUCTS")]
public class MND_PRODUCT:IDbTable

{
    [_dap.Key]
    public int prd_id{ get; set; }
    public required string prd_name{ get; set; }
    public string? prd_unit { get; set; }
}

