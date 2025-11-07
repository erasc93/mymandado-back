using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models.information_schema;

public class StoredProcedure
{
    public string ROUTINE_SCHEMA { get; set; }
    public string ROUTINE_NAME { get; set; }
    public string ROUTINE_TYPE { get; set; }
    public string SQL_PATH { get; set; }
}
