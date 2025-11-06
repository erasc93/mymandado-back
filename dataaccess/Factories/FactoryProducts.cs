using core_mandado.models;
using dataaccess.information_schema.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataaccess.Factories;

public class FactoryProducts()
{

    public MND_PRODUCT FromView(Product product)
    {
        MND_PRODUCT output;
        output = new MND_PRODUCT()
        {
            prd_id = product.id,
            prd_name = product.name,
            prd_unit = product.unit
        };
        return output;
    }
    public Product ToView(MND_PRODUCT p)
    {
        return new Product()
        {
            id = p.prd_id,
            name = p.prd_name,
            unit = p.prd_unit
        };
    }
}
