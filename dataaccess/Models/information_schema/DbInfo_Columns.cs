namespace models.information_schema;

public class DbInfo_Columns
{
    public required string Field { get; set; }
    public required string Type { get; set; }
    public required string Null { get; set; }
    public required string Key { get; set; }
    public required string Extra { get; set; }
}