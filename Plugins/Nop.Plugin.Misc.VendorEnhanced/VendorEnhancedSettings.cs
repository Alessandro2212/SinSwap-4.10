using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.VendorEnhanced
{
    public class VendorEnhancedSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Vendor Review based on vendor id is required in vendor detail page
        /// </summary>
        public bool EnableVendorReview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether QRCode based on vendor id is required in vendor create
        /// </summary>
        public bool EnableVendorQRCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Pictures based on vendor id is required in vendor create
        /// </summary>
        public bool EnableVendorPictures { get; set; }
    }
}
