using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class TopVendorViewComponent : NopViewComponent
    {
        private readonly IMiniVendorModelFactory _miniVendorModelFactory;
        private readonly VendorSettings _vendorSettings;

        public TopVendorViewComponent(IMiniVendorModelFactory miniVendorModelFactory,
            VendorSettings vendorSettings)
        {
            this._miniVendorModelFactory = miniVendorModelFactory;
            this._vendorSettings = vendorSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _miniVendorModelFactory.PrepareTopMiniVendorModel(5);
            if (model == null || model.MiniVendors == null || !model.MiniVendors.Any())
                return Content("");

            return View(model);
        }
    }
}
