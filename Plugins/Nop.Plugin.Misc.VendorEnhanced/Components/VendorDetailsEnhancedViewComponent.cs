using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
//using Nop.Plugin.Misc.VendorEnhanced.Domain;
using Nop.Plugin.Misc.VendorEnhanced.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Media;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Nop.Plugin.Misc.VendorEnhanced.Components
{
    [ViewComponent(Name = "VendorDetailsEnhanced")]
    public class VendorDetailsEnhancedViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly VendorEnhancedSettings _vendorEnhancedSettings;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IVendorService _vendorService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPictureService _pictureService;
        private readonly IRepository<VendorPictureRecord> _vendorPictureRecordRepository;

        public VendorDetailsEnhancedViewComponent(IStoreContext storeContext, 
            ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            VendorEnhancedSettings vendorEnhancedSettings,
            IRepository<Vendor> vendorRepository,
            IVendorService vendorService,
            MediaSettings mediaSettings,
            IPictureService pictureService,
            IRepository<VendorPictureRecord> vendorPictureRecordRepository)
        {
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._vendorEnhancedSettings = vendorEnhancedSettings;
            this._vendorRepository = vendorRepository;
            this._vendorService = vendorService;
            this._mediaSettings = mediaSettings;
            this._pictureService = pictureService;
            this._vendorPictureRecordRepository = vendorPictureRecordRepository;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var model = new VendorDetailsPicturesModel();


            //Vendor QRCode
            ViewBag.EnableVendorQRCode = _vendorEnhancedSettings.EnableVendorQRCode;

            if (_vendorEnhancedSettings.EnableVendorQRCode)
            {
                int vendorID = 0;

                Dictionary<string, dynamic> vendorDetails = new Dictionary<string, dynamic>();

                if (additionalData != null)
                {
                    foreach (var prop in additionalData.GetType().GetProperties())
                    {
                        var propertyName = prop.Name;
                        var propertyValue = additionalData.GetType().GetProperty(propertyName).GetValue(additionalData, null);

                        if (propertyName == "Id")
                        {
                            vendorID = Convert.ToInt32(propertyValue);

                            vendorDetails.Add("vendor_id", vendorID);
                        }
                    }

                    if (vendorID != 0)
                    {
                        var vendor = _vendorRepository.Table.FirstOrDefault(c => c.Id == vendorID && !c.Deleted);

                        if (vendor != null)
                        {
                            vendorDetails.Add("vendor_name", vendor.Name);
                            vendorDetails.Add("vendor_email", vendor.Email);
                        }

                        using (MemoryStream ms = new MemoryStream())
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(vendorDetails), QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            using (Bitmap bitMap = qrCode.GetGraphic(20))
                            {
                                bitMap.Save(ms, ImageFormat.Png);
                                ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                            }
                        }
                    }
                    else
                    {
                        ViewBag.QRCodeImage = "";
                    }
                }
            }


            //Vendor Pictures
            ViewBag.EnableVendorPictures = _vendorEnhancedSettings.EnableVendorPictures;

            if (_vendorEnhancedSettings.EnableVendorPictures)
            {
                int vID = 0;

                if (additionalData != null)
                {
                    foreach (var prop in additionalData.GetType().GetProperties())
                    {
                        var propertyName = prop.Name;
                        var propertyValue = additionalData.GetType().GetProperty(propertyName).GetValue(additionalData, null);

                        if (propertyName == "Id")
                        {
                            vID = Convert.ToInt32(propertyValue);
                        }
                    }
                }

                var vendorP = _vendorService.GetVendorById(vID);

                model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
                model.DefaultPictureModel = PrepareVendorDetailsPictureModel(vendorP, out IList<PictureModel> allPictureModels);
                model.PictureModels = allPictureModels;
            }


            return View("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/_VendorDetailsEnhanced.cshtml", model);
        }

        #region Methods
        /// <summary>
        /// Key for vendor picture caching on the vendor details page
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized vendor name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string VENDOR_DETAILS_PICTURES_MODEL_KEY = "Nop.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string VENDOR_DETAILS_PICTURES_PATTERN_KEY = "Nop.pres.vendor.picture";
        public const string VENDOR_DETAILS_PICTURES_PATTERN_KEY_BY_ID = "Nop.pres.vendor.picture-{0}-";

        /// <summary>
        /// Prepare the vendor details picture model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="allPictureModels">All picture models</param>
        /// <returns>Picture model for the default vendor</returns>
        protected virtual PictureModel PrepareVendorDetailsPictureModel(Vendor vendor, out IList<PictureModel> allPictureModels)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            //default picture size
            var defaultPictureSize = _mediaSettings.ProductDetailsPictureSize;

            //Vendor Pictures
            var queryPictures = from pp in _vendorPictureRecordRepository.Table
                                where pp.VendorId == vendor.Id
                                orderby pp.DisplayOrder, pp.Id
                                select pp;

            var vendorPictures = queryPictures.ToList();

            if (vendorPictures.Count == 0)
            {
                var noPictureModel = new PictureModel();

                allPictureModels = new List<PictureModel>();

                return null;
            }

            //Default picture
            var defaultPicture = vendorPictures.FirstOrDefault();

            var defaultPictureModel = new PictureModel
            {
                ImageUrl = _pictureService.GetPictureUrl(defaultPicture.PictureId, defaultPictureSize),
                ThumbImageUrl = _pictureService.GetPictureUrl(defaultPicture.PictureId, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture.PictureId),
                Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), vendor.Name),
                AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), vendor.Name),
            };


            //All pictures
            var pictureModels = new List<PictureModel>();

            foreach (var vendorPicture in vendorPictures)
            {
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(vendorPicture.PictureId, defaultPictureSize),
                    ThumbImageUrl = _pictureService.GetPictureUrl(vendorPicture.PictureId, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(vendorPicture.PictureId),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), vendor.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), vendor.Name),
                };

                var pictureData = _pictureService.GetPictureById(vendorPicture.PictureId)
                    ?? throw new Exception("Picture cannot be loaded");

                pictureModels.Add(pictureModel);
            }

            allPictureModels = pictureModels;

            return defaultPictureModel;
        }
        #endregion
    }
}
