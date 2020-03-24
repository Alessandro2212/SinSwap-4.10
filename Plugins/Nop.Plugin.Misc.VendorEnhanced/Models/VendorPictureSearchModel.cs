using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    /// <summary>
    /// Represents a vendor picture search model
    /// </summary>
    public partial class VendorPictureSearchModel : BaseSearchModel
    {
        #region Properties
        public int VendorId { get; set; }        
        #endregion
    }
}