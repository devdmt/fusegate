using DAL.Model.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

public class QuotesShareConfiguration : IEntityTypeConfiguration<QuotesShare>
{
    public void Configure(EntityTypeBuilder<QuotesShare> builder)
    {
        try
        {
            builder.ToTable("QuotesShare");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Context).HasMaxLength(64).IsRequired();
            builder.Property(x => x.ToEmail).HasMaxLength(256).IsRequired();
            builder.HasIndex(x => new { x.Context, x.QuoteId });
        }
        catch (Exception)
        {
            throw;
        }
    }
}
