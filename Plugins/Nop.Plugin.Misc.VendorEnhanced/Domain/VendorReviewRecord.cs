using Nop.Core;
using System;

namespace Nop.Plugin.Misc.VendorEnhanced.Domain
{
    /// <summary>
    /// Represents a Vendor Review record
    /// </summary>
    public partial class VendorReviewRecord : BaseEntity
    {
        public int CustomerID { get; set; }

        public int VendorID { get; set; }

        public int StoreID { get; set; }

        public bool IsApproved { get; set; }

        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public bool CustomerNotifiedOfReply { get; set; }

        public int Rating { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }

    public partial class VendorReviewSummary : BaseEntity
    {
        public int VendorID { get; set; }

        public int TotalReviews { get; set; }

        public int RatingSum { get; set; }
    }

    public partial class VendorReviewList : BaseEntity
    {
        public int CustomerID { get; set; }

        public string CustomerUsername { get; set; }

        public int VendorID { get; set; }

        public string VendorName { get; set; }

        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public int Rating { get; set; }

        public bool IsApproved { get; set; }

        public DateTime CreatedOnUtc { get; set; }
    }
}