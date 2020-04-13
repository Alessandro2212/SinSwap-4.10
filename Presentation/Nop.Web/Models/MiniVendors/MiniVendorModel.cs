using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Validators.Vendors;

namespace Nop.Web.Models.MiniVendors
{
    [Validator(typeof(MiniVendorModel))]
    public class MiniVendorModel : BaseNopModel
    {
        public MiniVendorModel()
        {
        }

        public int Id { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Age")]
        public string Age { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.City")]
        public string City { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }

        public string SeName { get; set; }

    }
}