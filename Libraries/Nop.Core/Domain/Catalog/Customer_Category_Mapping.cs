using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public class Customer_Category_Mapping : BaseEntity
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}
