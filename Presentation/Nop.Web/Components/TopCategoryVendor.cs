using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class TopCategoryVendorViewComponent : NopViewComponent
    {
        private readonly IMiniVendorModelFactory _miniVendorModelFactory;
        private readonly VendorSettings _vendorSettings;

        public TopCategoryVendorViewComponent(IMiniVendorModelFactory miniVendorModelFactory,
            VendorSettings vendorSettings)
        {
            this._miniVendorModelFactory = miniVendorModelFactory;
            this._vendorSettings = vendorSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _miniVendorModelFactory.PrepareTopCategoryMiniVendorModel(6);
            //var model = _topVendorModelFactory.PrepareVendorNavigationModel();
            //if (!model.Vendors.Any())
            //    return Content("");

            return View(model);
        }
    }
}
