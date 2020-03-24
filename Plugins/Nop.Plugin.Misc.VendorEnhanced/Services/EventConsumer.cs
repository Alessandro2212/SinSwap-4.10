using Microsoft.AspNetCore.Html;
using Nop.Services.Vendors;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Events;
using Nop.Services.Plugins;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.Misc.VendorEnhanced.Services
{
    /// <summary>
    /// Represents event consumer of the vendor enhanced plugin
    /// </summary>
    public class EventConsumer :
        IConsumer<PageRenderingEvent>,
        IConsumer<AdminTabStripCreated>
    {
        #region Fields
        private readonly IVendorService _vendorService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly VendorEnhancedSettings _vendorEnhancedSettings;
        #endregion

        #region Ctor
        public EventConsumer(IVendorService vendorService,
            IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            VendorEnhancedSettings vendorEnhancedSettings)
        {
            this._vendorService = vendorService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._vendorEnhancedSettings = vendorEnhancedSettings;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public void HandleEvent(PageRenderingEvent eventMessage)
        {
            if (eventMessage?.Helper?.ViewContext?.ActionDescriptor == null)
                return;

            //check whether the plugin is installed and is active
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Nop.Plugin.Misc.VendorEnhanced");

            if (pluginDescriptor == null)
                return;

            if (!pluginDescriptor?.Installed ?? false)
                return;
        }

        /// <summary>
        /// Handle admin tabstrip created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async void HandleEvent(AdminTabStripCreated eventMessage)
        {
            if (eventMessage?.Helper == null)
                return;

            //we need vendor details page
            var tabsElementId = "vendor-edit";
            if (!eventMessage.TabStripName.Equals(tabsElementId))
                return;

            //check whether the plugin is installed and is active
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Nop.Plugin.Misc.VendorEnhanced");

            if (pluginDescriptor == null)
                return;

            if (!pluginDescriptor?.Installed ?? false)
                return;

            //get the view model
            if (!(eventMessage.Helper.ViewData.Model is VendorModel vendorModel))
                return;

            //check whether a vendor exists
            var vendor = _vendorService.GetVendorById(vendorModel.Id);

            var model = new Models.VendorModel();
            if (vendor == null)
            {
                model.Id = 0;
            }
            else
            {
                model.Id = vendorModel.Id;
            }


            if (_vendorEnhancedSettings.EnableVendorQRCode)
            {
                //compose script to create a new tab for qrcode
                var vendorQRCodeTabElementId = "tab-qrcode";
                var vendorQRCodeTab = new HtmlString($@"
                <script>
                    $(document).ready(function() {{
                        $(`
                            <li>
                                <a data-tab-name='{vendorQRCodeTabElementId}' data-toggle='tab' href='#{vendorQRCodeTabElementId}'>
                                    {_localizationService.GetResource("VendorEnhanced.QRCode")}
                                </a>
                            </li>
                        `).appendTo('#{tabsElementId} .nav-tabs:first');
                        $(`
                            <div class='tab-pane' id='{vendorQRCodeTabElementId}'>
                                {
                                        (await eventMessage.Helper.PartialAsync("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/_VendorQRCode.cshtml", model)).RenderHtmlContent()
                                            .Replace("</script>", "<\\/script>") //we need escape a closing script tag to prevent terminating the script block early
                                    }
                            </div>
                        `).appendTo('#{tabsElementId} .tab-content:first');
                    }});
                </script>");

                //add this tab as a block to render on the vendor details page
                eventMessage.BlocksToRender.Add(vendorQRCodeTab);
            }


            if (_vendorEnhancedSettings.EnableVendorPictures)
            {
                //compose script to create a new tab for pictures
                var vendorPicturesTabElementId = "tab-pictures";
                var vendorPicturesTab = new HtmlString($@"
                <script>
                    $(document).ready(function() {{
                        $(`
                            <li>
                                <a data-tab-name='{vendorPicturesTabElementId}' data-toggle='tab' href='#{vendorPicturesTabElementId}'>
                                    {_localizationService.GetResource("VendorEnhanced.Pictures")}
                                </a>
                            </li>
                        `).appendTo('#{tabsElementId} .nav-tabs:first');
                        $(`
                            <div class='tab-pane' id='{vendorPicturesTabElementId}'>
                                {
                                        (await eventMessage.Helper.PartialAsync("~/Plugins/Nop.Plugin.Misc.VendorEnhanced/Views/_VendorPictures.cshtml", model)).RenderHtmlContent()
                                            .Replace("</script>", "<\\/script>") //we need escape a closing script tag to prevent terminating the script block early
                                    }
                            </div>
                        `).appendTo('#{tabsElementId} .tab-content:first');
                    }});
                </script>");

                //add this tab as a block to render on the vendor details page
                eventMessage.BlocksToRender.Add(vendorPicturesTab);
            }
        }
        #endregion
    }
}