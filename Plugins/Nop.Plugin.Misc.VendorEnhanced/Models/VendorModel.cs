using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    /// <summary>
    /// Represents a vendor model
    /// </summary>
    public partial class VendorModel : BaseNopEntityModel
    {
        #region Ctor
        public VendorModel()
        {
            VendorPictureModels = new List<VendorPictureModel>();
            AddPictureModel = new VendorPictureModel();
            VendorPictureSearchModel = new VendorPictureSearchModel();
        }
        #endregion

        #region Properties
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        //Pictures
        public VendorPictureModel AddPictureModel { get; set; }

        public IList<VendorPictureModel> VendorPictureModels { get; set; }

        public VendorPictureSearchModel VendorPictureSearchModel { get; set; }
        #endregion
    }
}