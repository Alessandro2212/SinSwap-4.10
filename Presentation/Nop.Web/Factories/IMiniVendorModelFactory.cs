using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.MiniVendors;

namespace Nop.Web.Factories
{
    public partial interface IMiniVendorModelFactory
    {
        TopMiniVendorModel PrepareTopMiniVendorModel();

        TopMiniVendorModel PrepareTopCategoryMiniVendorModel();

    }
}
