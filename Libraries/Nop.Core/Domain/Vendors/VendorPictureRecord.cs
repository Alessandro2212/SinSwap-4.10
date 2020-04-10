using Nop.Core;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor picture mapping
    /// </summary>
    public partial class VendorPictureRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the vendor identifier
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
