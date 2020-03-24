using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.VendorEnhanced
{
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Nop.Plugin.Misc.VendorEnhanced.VendorReview", "VendorReview/{vendorId}",
                 new { controller = "VendorEnhanced", action = "VendorReview" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1;
    }
}
