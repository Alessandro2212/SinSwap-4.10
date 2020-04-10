using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Vendors;
//using Nop.Plugin.Misc.VendorEnhanced.Domain;
using Nop.Plugin.Misc.VendorEnhanced.Services;
using Nop.Services.Configuration;
using Nop.Services.Vendors;
using Nop.Web.Framework.Components;
using System;

namespace Nop.Plugin.Misc.VendorEnhanced.Components
{
    [ViewComponent(Name = "VendorReviewOverview")]
    public class VendorReviewOverviewViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly VendorEnhancedSettings _vendorEnhancedSettings;
        private readonly IVendorReviewService _vendorReviewService;

        public VendorReviewOverviewViewComponent(IStoreContext storeContext,
            ISettingService settingService,
            IWebHelper webHelper,
            VendorEnhancedSettings vendorEnhancedSettings,
            IVendorReviewService vendorReviewService)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._vendorEnhancedSettings = vendorEnhancedSettings;
            this._vendorReviewService = vendorReviewService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var model = new VendorReviewSummary();

            ViewBag.EnableVendorReview = _vendorEnhancedSettings.EnableVendorReview;

            if (additionalData != null)
            {
                foreach (var prop in additionalData.GetType().GetProperties())
                {
                    var propertyName = prop.Name;
                    var propertyValue = additionalData.GetType().GetProperty(propertyName).GetValue(additionalData, null);

                    if (propertyName == "Id")
                    {
                        model.VendorID = Convert.ToInt32(propertyValue);

                        var total_reviews = _vendorReviewService.GetTotalVendorReview(model.VendorID);

                        model.TotalReviews = total_reviews;

                        var rating_sum = _vendorReviewService.GetVendorRatingSum(model.VendorID);

                        model.RatingSum = rating_sum;
                    }
                }
            }

            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/_VendorReviewOverview.cshtml", model);
        }
    }
}
