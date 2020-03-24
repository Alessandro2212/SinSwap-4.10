using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.VendorEnhanced.Domain;
using System;
using Nop.Services.Vendors;
using System.Collections.Generic;
using Nop.Services.Customers;
using System.Linq;

namespace Nop.Plugin.Misc.VendorEnhanced.Services
{
    public class VendorReviewService : IVendorReviewService
    {
        #region Field
        private readonly IRepository<VendorReviewRecord> _vendorReviewRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IVendorService _vendorService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<VendorReviewList> _vendorListRepository;
        #endregion

        #region Ctr
        public VendorReviewService(IRepository<VendorReviewRecord> vendorReviewRepository, IRepository<Vendor> vendorRepository, IVendorService vendorService, 
            ICustomerService customerService, IRepository<VendorReviewList> vendorListRepository)
        {
            this._vendorReviewRepository = vendorReviewRepository;
            this._vendorRepository = vendorRepository;
            this._vendorService = vendorService;
            this._customerService = customerService;
            this._vendorListRepository = vendorListRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets all vendor reviews
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Vendor Review</returns>
        public virtual IPagedList<VendorReviewList> GetAllVendorReview(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var vendorreviews = _vendorReviewRepository.Table;

            IList<VendorReviewList> query = new List<VendorReviewList>();

            foreach (var vendorreview in vendorreviews) {
                var model = new VendorReviewList();

                var vendor = _vendorService.GetVendorById(vendorreview.VendorID);

                var customer = _customerService.GetCustomerById(vendorreview.CustomerID);

                model.CustomerID = vendorreview.CustomerID;
                model.CustomerUsername = customer.Email;
                model.VendorID = vendorreview.VendorID;
                model.VendorName = vendor.Name;
                model.Title = vendorreview.Title;
                model.ReviewText = vendorreview.ReviewText;
                model.ReplyText = vendorreview.ReplyText;
                model.Rating = vendorreview.Rating;
                model.IsApproved = vendorreview.IsApproved;
                model.CreatedOnUtc = vendorreview.CreatedOnUtc;

                query.Add(model);
            }

            return new PagedList<VendorReviewList>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all vendor reviews by vendor id
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="vendorId">Vendor Identifier</param>
        /// <returns>Vendor Review</returns>
        public virtual IList<VendorReviewList> GetVendorReviewByVendorId(int vendorId = 0)
        {
            var vendorreviews = _vendorReviewRepository.Table.Where(q => q.VendorID == vendorId);

            IList<VendorReviewList> query = new List<VendorReviewList>();

            foreach (var vendorreview in vendorreviews)
            {
                var model = new VendorReviewList();

                var vendor = _vendorService.GetVendorById(vendorId);

                var customer = _customerService.GetCustomerById(vendorreview.CustomerID);

                model.CustomerID = vendorreview.CustomerID;
                model.CustomerUsername = customer.Email;
                model.VendorID = vendorreview.VendorID;
                model.VendorName = vendor.Name;
                model.Title = vendorreview.Title;
                model.ReviewText = vendorreview.ReviewText;
                model.ReplyText = vendorreview.ReplyText;
                model.Rating = vendorreview.Rating;
                model.IsApproved = vendorreview.IsApproved;
                model.CreatedOnUtc = vendorreview.CreatedOnUtc;

                query.Add(model);
            }

            return query;
        }

        /// <summary>
        /// Gets a vendor review
        /// </summary>
        /// <param name="vendorReviewId">Vendor Review identifier</param>
        /// <returns>Vendor Review</returns>
        public virtual VendorReviewList GetVendorReviewById(int vendorReviewId)
        {
            if (vendorReviewId == 0)
                return null;

            return _vendorListRepository.GetById(vendorReviewId);
        }

        /// <summary>
        /// Gets a total vendor review
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Total Vendor Review</returns>
        public virtual int GetTotalVendorReview(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            int rCount = 0;

            var vendorreviews = _vendorReviewRepository.Table;

            rCount = (from vendorreview in vendorreviews
                              where vendorreview.VendorID == vendorId
                              select vendorreview)
                              .Count();

            return rCount;
        }

        /// <summary>
        /// Gets a vendor rating sum
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Vendor Rating Sum</returns>
        public virtual int GetVendorRatingSum(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            int rSum = 0;

            var vendorreviews = _vendorReviewRepository.Table;

            var rSumRatings = (from vendorreview in vendorreviews
                    where vendorreview.VendorID == vendorId
                    group vendorreview by vendorreview.VendorID into vendorGroup
                    select new
                    {
                        TotalSum = vendorGroup.Sum(x => x.Rating),
                    });

            foreach (var rSumRating in rSumRatings)
            {
                rSum = rSumRating.TotalSum;
            }

            return rSum;
        }

        /// <summary>
        /// Inserts a vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        public virtual void InsertVendorReview(VendorReviewRecord vendorReview)
        {
            if (vendorReview == null)
                throw new ArgumentNullException(nameof(vendorReview));

            _vendorReviewRepository.Insert(vendorReview);
        }

        /// <summary>
        /// Updates the vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        public virtual void UpdateVendorReview(VendorReviewRecord vendorReview)
        {
            if (vendorReview == null)
                throw new ArgumentNullException(nameof(vendorReview));

            _vendorReviewRepository.Update(vendorReview);
        }

        /// <summary>
        /// Deletes a vendor review
        /// </summary>
        /// <param name="vendorReview">Vendor Review</param>
        public virtual void DeleteVendorReview(VendorReviewRecord vendorReview)
        {
            if (vendorReview == null)
                throw new ArgumentNullException(nameof(vendorReview));

            _vendorReviewRepository.Delete(vendorReview);
        }
        #endregion
    }
}