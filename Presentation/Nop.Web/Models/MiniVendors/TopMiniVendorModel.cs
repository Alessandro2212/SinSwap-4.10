using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.MiniVendors
{
    public partial class TopMiniVendorModel : BaseNopModel
    {
        public TopMiniVendorModel()
        {
        }

        public int TotalVendors { get; set; }

        public List<MiniVendorModel> MiniVendors { get; set; }
    }
}