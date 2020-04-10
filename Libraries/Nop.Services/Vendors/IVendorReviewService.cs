using Nop.Core;
using Nop.Core.Domain.Vendors;
//using Nop.Plugin.Misc.VendorEnhanced.Domain;
using System.Collections.Generic;

namespace Nop.Services.Vendors
{
    public interface IVendorReviewService
    {
        /// <summary>
        /// Gets all vendor reviews
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor Review</returns>
        IPagedList<VendorReviewList> GetAllVendorReview(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets all vendor reviews by vendor id
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor Review</returns>
        IList<VendorReviewList> GetVendorReviewByVendorId(int vendorId = 0);

        /// <summary>
        /// Gets a vendor review
        /// </summary>
        /// <param name="vendorReviewId">Vendor Review identifier</param>
        /// <returns>Vendor Review</returns>
        VendorReviewList GetVendorReviewById(int vendorReviewId);

        /// <summary>
        /// Gets a total vendor review
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Total Vendor Review</returns>
        int GetTotalVendorReview(int vendorId);

        /// <summary>
        /// Gets a vendor rating sum
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor Rating Sum</returns>
        int GetVendorRatingSum(int vendorId);

        /// <summary>
        /// Inserts a vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        void InsertVendorReview(VendorReviewRecord vendorReview);

        /// <summary>
        /// Updates a vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        void UpdateVendorReview(VendorReviewRecord vendorReview);

        /// <summary>
        /// Deletes a vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        void DeleteVendorReview(VendorReviewRecord vendorReview);
    }
}
