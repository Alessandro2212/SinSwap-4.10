using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    public partial class VendorDetailsPicturesModel : BaseNopEntityModel
    {
        public VendorDetailsPicturesModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
        }

        public bool DefaultPictureZoomEnabled { get; set; }

        public PictureModel DefaultPictureModel { get; set; }

        public IList<PictureModel> PictureModels { get; set; }
    }
}