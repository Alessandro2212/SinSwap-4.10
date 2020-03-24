using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.VendorEnhanced.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nop.Plugin.Misc.VendorEnhanced
{
    public class VendorEnhancedMethod : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly VendorReviewObjectContext _vendorReviewObjectContext;
        private readonly VendorPictureObjectContext _vendorPictureObjectContext;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;
        private readonly INopFileProvider _fileProvider;
        #endregion

        #region Ctor
        public VendorEnhancedMethod(IWorkContext workContext, ILocalizationService localizationService, IWebHelper webHelper, VendorReviewObjectContext vendorReviewObjectContext,
            VendorPictureObjectContext vendorPictureObjectContext, WidgetSettings widgetSettings, ISettingService settingService, INopFileProvider fileProvider)
        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._vendorReviewObjectContext = vendorReviewObjectContext;
            this._vendorPictureObjectContext = vendorPictureObjectContext;
            this._widgetSettings = widgetSettings;
            this._settingService = settingService;
            this._fileProvider = fileProvider;
        }
        #endregion

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            //return new List<string> { PublicWidgetZones.VendorDetailsTop, AdminWidgetZones.VendorDetailsInfoBottom };
            return new List<string> { PublicWidgetZones.VendorDetailsTop, "VendorDetailsEnhanced" };
        }

        #region Methods
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/VendorEnhanced/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == "vendordetails_top")
            {
                return "VendorReviewOverview";
            }
            else if (widgetZone == "VendorDetailsEnhanced")
            {
                return "VendorDetailsEnhanced";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new VendorEnhancedSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Instructions", "<p>Configure Vendor Enhanced</p>");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorReview", "Enable Vendor Review");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorReview.Hint", "Vendor Review will be shown on Vendor detail page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorQRCode", "Enable Vendor QRCode");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorQRCode.Hint", "QRCode section will be added in Vendors on Administration, also QRCode will be shown on Vendor detail page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorPictures", "Enable Vendor Pictures");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorPictures.Hint", "Pictures section will be added in Vendors on Administration, also Picture gallery will be shown on Vendor detail page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideAltAttribute", "Alt");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideAltAttribute.Hint", "Override \"alt\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. vendor name).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideTitleAttribute", "Title");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideTitleAttribute.Hint", "Override \"title\" attribute for \"img\" HTML element. If empty, then a default rule will be used (e.g. vendor name).");
            _localizationService.AddOrUpdatePluginLocaleResource("PageTitle.VendorReview", "Vendor Review");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.VendorReviewFor", "Vendor reviews for");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.Vendor", "Vendor");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.Customer", "Customer");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.VendorReviewFirst", "Be the first to review this vendor");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.SuccessfullyVendorReviewed", "Successfully Vendor Reviewed");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.QRCode", "QRCode");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.Pictures", "Pictures");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.QRCodeSaveBeforeEdit", "You need to save the vendor before QRCode will be auto generated for this vendor page.");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.AddPicturesButton", "Add vendor picture");
            _localizationService.AddOrUpdatePluginLocaleResource("VendorEnhanced.PicturesSaveBeforeEdit", "You need to save the vendor before you can upload pictures for this vendor page.");

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.Title", "Vendor Review");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.GetAllVendorReview.Title", "Get all Vendor Review");

            //database objects
            _vendorReviewObjectContext.Install();
            _vendorPictureObjectContext.Install();

            InsertWidgetZone();

            base.Install();

            // mark widget as active
            _widgetSettings.ActiveWidgetSystemNames.Add(this.PluginDescriptor.SystemName);
            _settingService.SaveSetting(_widgetSettings);
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<VendorEnhancedSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Instructions");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorReview");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorReview.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorQRCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorQRCode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorPictures");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorPictures.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideAltAttribute");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideAltAttribute.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideTitleAttribute");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideTitleAttribute.Hint");
            _localizationService.DeletePluginLocaleResource("PageTitle.VendorReview");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.VendorReviewFor");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.Vendor");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.Customer");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.VendorReviewFirst");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.SuccessfullyVendorReviewed");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.QRCode");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.Pictures");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.QRCodeSaveBeforeEdit");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.AddPicturesButton");
            _localizationService.DeletePluginLocaleResource("VendorEnhanced.PicturesSaveBeforeEdit");

            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.Title");
            _localizationService.DeletePluginLocaleResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.GetAllVendorReview.Title");

            //database objects
            _vendorReviewObjectContext.Uninstall();
            _vendorPictureObjectContext.Uninstall();

            RemoveWidgetZone();

            base.Uninstall();

            // mark widget as disabled
            _widgetSettings.ActiveWidgetSystemNames.Remove(this.PluginDescriptor.SystemName);
            _settingService.SaveSetting(_widgetSettings);
        }

        private void InsertWidgetZone()
        {
            string strPath = _fileProvider.MapPath("~/Views/Catalog/Vendor.cshtml");
            string strFileContents = File.ReadAllText(strPath);
            int strPoint1 = strFileContents.IndexOf(@"@await Component.InvokeAsync(""Widget"", new { widgetZone = PublicWidgetZones.VendorDetailsTop, additionalData = Model })");
            int strPoint2 = strFileContents.IndexOf(@"@*description*@", strPoint1);
            string strResults = "";
            strResults = strFileContents.Insert(strPoint2, @"@await Component.InvokeAsync(""Widget"", new { widgetZone = ""VendorDetailsEnhanced"", additionalData = Model })" + Environment.NewLine);
            File.WriteAllText(strPath, strResults);
        }

        private void RemoveWidgetZone()
        {
            string strPath = _fileProvider.MapPath("~/Views/Catalog/Vendor.cshtml");
            string[] strSearch = new string[2];
            StringBuilder sb = new StringBuilder("");
            string[] lines = File.ReadAllLines(strPath);
            strSearch[0] = @"@await Component.InvokeAsync(""Widget"", new { widgetZone = ""VendorDetailsEnhanced"", additionalData = Model })";
            foreach (string line in lines)
            {
                if (line.Trim() != strSearch[0] && line.Trim() != strSearch[1])
                {
                    sb.Append(line + Environment.NewLine);
                }
            }
            File.WriteAllText(strPath, sb.ToString());
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            string pluginMenuName = _localizationService.GetResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Vendor Review");
            string getllbooktableMenuName = _localizationService.GetResource("Plugins.Nop.Plugin.Misc.VendorEnhanced.Menu.GetAllVendorReview.Title", languageId: _workContext.WorkingLanguage.Id, defaultValue: "Get all Vendor Review");

            const string adminUrlPart = "Admin/";

            var pluginMainMenu = new SiteMapNode
            {
                Title = pluginMenuName,
                Visible = true,
                SystemName = "VendorEnhanced-Main-Menu",
                IconClass = "fa-table"
            };

            pluginMainMenu.ChildNodes.Add(new SiteMapNode
            {
                Title = getllbooktableMenuName,
                Url = _webHelper.GetStoreLocation() + adminUrlPart + "ManageVendorEnhancedAdmin/List",
                Visible = true,
                SystemName = "VendorEnhanced-Get-All-Vendor-Review-Menu",
                IconClass = "fa-table"
            });

            rootNode.ChildNodes.Add(pluginMainMenu);
        }
        #endregion
    }
}
