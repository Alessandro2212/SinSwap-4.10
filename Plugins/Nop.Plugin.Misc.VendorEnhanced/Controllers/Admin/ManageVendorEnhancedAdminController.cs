using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.VendorEnhanced.Services;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using Nop.Plugin.Misc.VendorEnhanced.Domain;

namespace Nop.Plugin.Misc.VendorEnhanced.Controllers.Admin
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [Route("Admin/ManageVendorEnhancedAdmin/")]
    public class ManageVendorEnhancedAdminController : BasePluginController
    {
        #region Fields
        private readonly IVendorReviewService _vendorReviewService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public ManageVendorEnhancedAdminController(IVendorReviewService vendorReviewService, 
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            this._vendorReviewService = vendorReviewService;
            this._localizationService = localizationService;
            this._workContext = workContext;
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("List")]
        public ActionResult List()
        {
            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/Admin/List.cshtml");
        }

        [HttpPost]
        [Route("List")]
        public ActionResult List(DataSourceRequest command)
        {
            IEnumerable<VendorReviewList> gridModel;

            if (_workContext.CurrentCustomer.IsVendor())
            {
                gridModel = _vendorReviewService.GetVendorReviewByVendorId(_workContext.CurrentVendor.Id);
            } else
            {
                gridModel = _vendorReviewService.GetAllVendorReview();
            }

            var grids = new DataSourceResult()
            {
                Data = gridModel,
                Total = gridModel.Count()
            };

            return Json(grids);
        }
        #endregion
    }
}
