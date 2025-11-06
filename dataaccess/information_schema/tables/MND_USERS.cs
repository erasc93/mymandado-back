using _dap=Dapper.Contrib.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace dataaccess.information_schema.tables;
public interface IDbTable { }
[_dap.Table("USERS")]
public class MND_USERS :IDbTable
{
    [_dap.Key]
    public int usr_id { get; set; }
    public required string usr_name { get; set; }
}

[_dap.Table("PRODUCTS")]
public class MND_PRODUCT:IDbTable

{
    [_dap.Key]
    public int prd_id{ get; set; }
    public required string prd_name{ get; set; }
    public string? prd_unit { get; set; }
}


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

