using _dap = Dapper.Contrib.Extensions;

namespace models.tables;

[_dap.Table("CART")]
public class MND_CART:IDbTable
{
    [_dap.Key]
    public int car_id { get; set; }
    /// secondary key
    public required int car_crtnb { get; set; } 
    /// secondary key
    public required int car_usrid { get; set; }
    public required string car_name { get; set; }
    public required string car_desc { get; set; }
}
