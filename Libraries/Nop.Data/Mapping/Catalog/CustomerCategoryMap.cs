using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a customer-category mapping configuration
    /// </summary>
    public partial class CustomerCategoryMap : NopEntityTypeConfiguration<Customer_Category_Mapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Customer_Category_Mapping> builder)
        {
            builder.ToTable(nameof(Customer_Category_Mapping));

            builder.Property(customer_category => customer_category.CustomerId).HasColumnName("CustomerId");
            builder.Property(customer_category => customer_category.CategoryId).HasColumnName("CategoryId");

            builder.HasOne(customer_category => customer_category.Customer)
                .WithMany()
                .HasForeignKey(customer_category => customer_category.CustomerId);

            builder.HasOne(customer_category => customer_category.Category)
                 .WithMany()
                 .HasForeignKey(customer_category => customer_category.CategoryId);

            base.Configure(builder);
        }

        #endregion
    }
}