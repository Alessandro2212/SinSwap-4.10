using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.VendorEnhanced.Domain;

namespace Nop.Plugin.Misc.VendorEnhanced.DataMappings
{
    /// <summary>
    /// Represents a Vendor Review record mapping configuration
    /// </summary>
    public partial class VendorReviewMap : NopEntityTypeConfiguration<VendorReviewRecord>
    {
        #region Methods
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<VendorReviewRecord> builder)
        {
            builder.ToTable(nameof(VendorReviewRecord));
            builder.HasKey(record => record.Id);
        }
        #endregion
    }
}