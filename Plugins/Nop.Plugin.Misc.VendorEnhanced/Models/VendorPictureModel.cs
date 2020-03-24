using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    /// <summary>
    /// Represents a vendor picture model
    /// </summary>
    public partial class VendorPictureModel : BaseNopEntityModel
    {
        #region Properties
        public int VendorId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
        public string PictureUrl { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideAltAttribute")]
        public string OverrideAltAttribute { get; set; }

        [NopResourceDisplayName("Plugins.Nop.Plugin.Misc.VendorEnhanced.OverrideTitleAttribute")]
        public string OverrideTitleAttribute { get; set; }
        #endregion
    }
}