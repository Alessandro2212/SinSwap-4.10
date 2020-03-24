using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.VendorEnhanced.Data;
using Nop.Plugin.Misc.VendorEnhanced.Domain;
using Nop.Plugin.Misc.VendorEnhanced.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Misc.VendorEnhanced.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<VendorReviewService>().As<IVendorReviewService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<VendorReviewObjectContext>("nop_object_context_vendor_review_zip");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<VendorReviewRecord>>().As<IRepository<VendorReviewRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_vendor_review_zip"))
                .InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<VendorPictureObjectContext>("nop_object_context_vendor_picture_zip");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<VendorPictureRecord>>().As<IRepository<VendorPictureRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_vendor_picture_zip"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}