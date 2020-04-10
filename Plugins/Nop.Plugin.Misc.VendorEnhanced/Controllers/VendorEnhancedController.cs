using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
//using Nop.Plugin.Misc.VendorEnhanced.Domain;
using Nop.Plugin.Misc.VendorEnhanced.Models;
using Nop.Plugin.Misc.VendorEnhanced.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Nop.Plugin.Misc.VendorEnhanced.Controllers
{
    public class VendorEnhancedController : BasePluginController
    {
        #region Fields
        private readonly IVendorReviewService _vendorReviewService;
        private readonly IVendorService _vendorService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly CatalogSettings _catalogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IStoreContext _storeContext;
        private readonly VendorEnhancedSettings _vendorEnhancedSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IRepository<VendorPictureRecord> _vendorPictureRecordRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Vendor> _vendorRepository;
        #endregion

        #region Ctor
        public VendorEnhancedController(IVendorReviewService vendorReviewService, 
            IVendorService vendorService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IUrlRecordService urlRecordService,
            CatalogSettings catalogSettings,
            CaptchaSettings captchaSettings,
            IStoreContext storeContext,
            VendorEnhancedSettings vendorEnhancedSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IRepository<VendorPictureRecord> vendorPictureRecordRepository,
            IEventPublisher eventPublisher,
            IRepository<Vendor> vendorRepository)
        {
            this._vendorReviewService = vendorReviewService;
            this._vendorService = vendorService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._urlRecordService = urlRecordService;
            this._catalogSettings = catalogSettings;
            this._captchaSettings = captchaSettings;
            this._storeContext = storeContext;
            this._vendorEnhancedSettings = vendorEnhancedSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._vendorPictureRecordRepository = vendorPictureRecordRepository;
            this._eventPublisher = eventPublisher;
            this._vendorRepository = vendorRepository;
        }
        #endregion

        #region Methods
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                EnableVendorReview = _vendorEnhancedSettings.EnableVendorReview,
                EnableVendorQRCode = _vendorEnhancedSettings.EnableVendorQRCode,
                EnableVendorPictures = _vendorEnhancedSettings.EnableVendorPictures
            };

            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _vendorEnhancedSettings.EnableVendorReview = model.EnableVendorReview;
            _vendorEnhancedSettings.EnableVendorQRCode = model.EnableVendorQRCode;
            _vendorEnhancedSettings.EnableVendorPictures = model.EnableVendorPictures;
            _settingService.SaveSetting(_vendorEnhancedSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public ActionResult VendorReview(int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null || vendor.Deleted)
                return RedirectToRoute("HomePage");

            var model = new VendorReviewModel();

            model.VendorID = vendor.Id;
            model.VendorName = _localizationService.GetLocalized(vendor, x => x.Name);
            model.VendorSeName = _urlRecordService.GetSeName(vendor);

            //only registered users can leave review
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            //default value
            model.Rating = _catalogSettings.DefaultProductRatingValue;

            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/VendorReview.cshtml", model);
        }

        [HttpPost, ActionName("VendorReview")]
        [PublicAntiForgery]
        public virtual IActionResult VendorReviewAdd(VendorReviewModel model)
        {
            var vendor = _vendorService.GetVendorById(model.VendorID);

            if (vendor != null)
            {
                model.VendorName = vendor.Name;
                model.VendorSeName = _urlRecordService.GetSeName(vendor);
            }

            var customerID = _workContext.CurrentCustomer.Id;

            if (ModelState.IsValid)
            {
                var vendorReview = new VendorReviewRecord
                {
                    CustomerID = customerID,
                    VendorID = model.VendorID,
                    StoreID = model.StoreID,
                    IsApproved = model.IsApproved,
                    Title = model.Title,
                    ReviewText = model.ReviewText,
                    ReplyText = model.ReplyText,
                    CustomerNotifiedOfReply = model.CustomerNotifiedOfReply,
                    Rating = model.Rating,
                    HelpfulYesTotal = model.HelpfulYesTotal,
                    HelpfulNoTotal = model.HelpfulNoTotal,
                    CreatedOnUtc = DateTime.Now
                };

                _vendorReviewService.InsertVendorReview(vendorReview);

                model.SuccessfullyVendorReviewed = true;
                model.Result = _localizationService.GetResource("VendorEnhanced.SuccessfullyVendorReviewed");
            }

            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/VendorReview.cshtml", model);
        }
        #endregion

        #region Vendor QRCode
        [HttpPost]
        public virtual IActionResult VendorQRCode(int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(vendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            Dictionary<string, dynamic> vendorDetails = new Dictionary<string, dynamic>();

            Dictionary<string, dynamic> returnData = new Dictionary<string, dynamic>();

            if (vendorId != 0)
            {
                vendorDetails.Add("vendor_id", vendorId);

                var vendorData = _vendorRepository.Table.FirstOrDefault(c => c.Id == vendorId && !c.Deleted);

                if (vendorData != null)
                {
                    vendorDetails.Add("vendor_name", vendorData.Name);
                    vendorDetails.Add("vendor_email", vendorData.Email);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(vendorDetails), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        returnData.Add("Result", true);
                        returnData.Add("QRCodeImage", "data:image/png;base64," + Convert.ToBase64String(ms.ToArray()));
                    }
                }
            }
            else
            {
                returnData.Add("Result", false);
            }

            return Json(returnData);
        }
        #endregion

        #region Vendor pictures
        [HttpPost]
        public virtual IActionResult VendorPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(vendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            if (this.GetVendorPicturesByVendorId(vendorId).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(vendor.Name));

            this.InsertVendorPicture(new VendorPictureRecord
            {
                PictureId = pictureId,
                VendorId = vendorId,
                DisplayOrder = displayOrder
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult VendorPictureList(VendorPictureSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedKendoGridJson();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(searchModel.VendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            //prepare model
            var model = this.PrepareVendorPictureListModel(searchModel, vendor);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult VendorPictureUpdate(VendorPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor picture with the specified id
            var vendorPicture = this.GetVendorPictureById(model.Id)
                ?? throw new ArgumentException("No vendor picture found with the specified id");

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(vendorPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            vendorPicture.DisplayOrder = model.DisplayOrder;
            this.UpdateVendorPicture(vendorPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult VendorPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor picture with the specified id
            var vendorPicture = this.GetVendorPictureById(id)
                ?? throw new ArgumentException("No vendor picture found with the specified id");

            var pictureId = vendorPicture.PictureId;
            this.DeleteVendorPicture(vendorPicture);

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }
        #endregion

        #region Add / Override Core Methods
        /// <summary>
        /// Prepare paged vendor picture list model
        /// </summary>
        /// <param name="searchModel">Vendor picture search model</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Vendor picture list model</returns>
        public virtual VendorPictureListModel PrepareVendorPictureListModel(VendorPictureSearchModel searchModel, Vendor vendor)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            //get vendor pictures
            var vendorPictures = this.GetVendorPicturesByVendorId(vendor.Id);

            //prepare grid model
            var model = new VendorPictureListModel
            {
                Data = vendorPictures.PaginationByRequestModel(searchModel).Select(vendorPicture =>
                {
                    //fill in model values from the entity
                    var vendorPictureModel = new VendorPictureModel
                    {
                        Id = vendorPicture.Id,
                        VendorId = vendorPicture.VendorId,
                        PictureId = vendorPicture.PictureId,
                        DisplayOrder = vendorPicture.DisplayOrder
                    };

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(vendorPicture.PictureId)
                        ?? throw new Exception("Picture cannot be loaded");

                    vendorPictureModel.PictureUrl = _pictureService.GetPictureUrl(picture);
                    vendorPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    vendorPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return vendorPictureModel;
                }),
                Total = vendorPictures.Count
            };

            return model;
        }

        /// <summary>
        /// Gets a vendor pictures by vendor identifier
        /// </summary>
        /// <param name="vendorId">The vendor identifier</param>
        /// <returns>Vendor pictures</returns>
        public virtual IList<VendorPictureRecord> GetVendorPicturesByVendorId(int vendorId)
        {
            var query = from pp in _vendorPictureRecordRepository.Table
                        where pp.VendorId == vendorId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;
            var vendorPictures = query.ToList();
            return vendorPictures;
        }

        /// <summary>
        /// Gets a product picture
        /// </summary>
        /// <param name="productPictureId">Product picture identifier</param>
        /// <returns>Product picture</returns>
        public virtual VendorPictureRecord GetVendorPictureById(int vendorPictureId)
        {
            if (vendorPictureId == 0)
                return null;

            return _vendorPictureRecordRepository.GetById(vendorPictureId);
        }

        /// <summary>
        /// Inserts a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        public virtual void InsertVendorPicture(VendorPictureRecord vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRecordRepository.Insert(vendorPicture);

            //event notification
            _eventPublisher.EntityInserted(vendorPicture);
        }

        /// <summary>
        /// Updates a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        public virtual void UpdateVendorPicture(VendorPictureRecord vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRecordRepository.Update(vendorPicture);

            //event notification
            _eventPublisher.EntityUpdated(vendorPicture);
        }

        /// <summary>
        /// Deletes a vendor picture
        /// </summary>
        /// <param name="vendorPicture">Vendor picture</param>
        public virtual void DeleteVendorPicture(VendorPictureRecord vendorPicture)
        {
            if (vendorPicture == null)
                throw new ArgumentNullException(nameof(vendorPicture));

            _vendorPictureRecordRepository.Delete(vendorPicture);

            //event notification
            _eventPublisher.EntityDeleted(vendorPicture);
        }
        #endregion
    }
}
