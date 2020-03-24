using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.VendorEnhanced.Models
{
    public class VendorReviewModel : BaseNopEntityModel
    {
        public int CustomerID { get; set; }

        public int VendorID { get; set; }

        public string VendorName { get; set; }

        public string VendorSeName { get; set; }

        public int StoreID { get; set; }

        public bool IsApproved { get; set; }

        [NopResourceDisplayName("Reviews.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Reviews.Fields.ReviewText")]
        public string ReviewText { get; set; }

        public string ReplyText { get; set; }

        public bool CustomerNotifiedOfReply { get; set; }

        [NopResourceDisplayName("Reviews.Fields.Rating")]
        public int Rating { get; set; }

        public int HelpfulYesTotal { get; set; }

        public int HelpfulNoTotal { get; set; }

        [NopResourceDisplayName("VendorEnhanced.DateAndTime")]
        [UIHint("DateTimeNullable")]
        public DateTime? CreatedOnUtc { get; set; }

        public bool SuccessfullyVendorReviewed { get; set; }

        public string Result { get; set; }
    }
}
