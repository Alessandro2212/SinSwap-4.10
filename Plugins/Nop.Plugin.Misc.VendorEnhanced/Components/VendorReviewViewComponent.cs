using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.VendorEnhanced.Components
{
    [ViewComponent(Name = "VendorReview")]
    public class VendorReviewViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public VendorReviewViewComponent(IStoreContext storeContext, 
            ISettingService settingService, 
            IWebHelper webHelper)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/VendorReview.cshtml");
        }
    }
}
