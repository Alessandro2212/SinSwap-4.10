using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorReview")]
        public bool EnableVendorReview { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorQRCode")]
        public bool EnableVendorQRCode { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.VendorEnhanced.EnableVendorPictures")]
        public bool EnableVendorPictures { get; set; }
    }
}