using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Models.MiniVendors;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the mini vendor model factory
    /// </summary>
    public partial class MiniVendorModelFactory : IMiniVendorModelFactory
    {

        private readonly IPictureService _pictureService;
        private readonly IVendorService _vendorService;
        //private readonly IRepository<VendorPictureRecord> _vendorPictureRecordRepository;

        public MiniVendorModelFactory(
           IPictureService pictureService,
           IVendorService vendorService)
        {          
            this._pictureService = pictureService;
            this._vendorService = vendorService;
         
        }
        public TopMiniVendorModel PrepareTopCategoryMiniVendorModel()
        {
            //query to retrieve the top vendors of a specific category

            throw new NotImplementedException();
        }

        /// <summary>
        /// query to retriever the top (best) vendors of the home page
        /// </summary>
        /// <returns></returns>
        public TopMiniVendorModel PrepareTopMiniVendorModel()
        {
            
            var vendors = _vendorService.GetAllVendors();
            
            if(!vendors.Any())
                return new TopMiniVendorModel { };

            TopMiniVendorModel model = new TopMiniVendorModel();
            List<MiniVendorModel> miniVendors = new List<MiniVendorModel>();

            foreach(Vendor vendor in vendors.ToList())
            {

            }

            vendors.ToList().ForEach(x => { miniVendors.Add(new MiniVendorModel 
                    { 
                        Id = x.Id,
                        Name = x.Name
                        //Age = x.
                        
                    } 
                ); 
            });

                
            

            return new TopMiniVendorModel { };
        }
    }
}