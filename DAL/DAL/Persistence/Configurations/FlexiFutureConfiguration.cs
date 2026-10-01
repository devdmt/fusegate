using DAL.Model.FlexiFuture;
using DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

public class FlexiFutureProductSettingsConfiguration : IEntityTypeConfiguration<FlexiFutureProductSettings>
{
    public void Configure(EntityTypeBuilder<FlexiFutureProductSettings> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureProductSettings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RateSetVersion).HasMaxLength(50);
            builder.Property(x => x.EffectiveFrom).HasDateOnlyConversion();
            builder.Property(x => x.EffectiveTo).HasDateOnlyConversion();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureFrequencyDiscountConfiguration : IEntityTypeConfiguration<FlexiFutureFrequencyDiscount>
{
    public void Configure(EntityTypeBuilder<FlexiFutureFrequencyDiscount> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureFrequencyDiscount");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Frequency).HasMaxLength(32);
            builder.HasIndex(x => x.Frequency).IsUnique();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureRateTableConfiguration : IEntityTypeConfiguration<FlexiFutureRateTable>
{
    public void Configure(EntityTypeBuilder<FlexiFutureRateTable> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureRateTable");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(64);
            builder.Property(x => x.PremiumMode).HasMaxLength(16);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.LookupKind).HasMaxLength(32);
            builder.HasIndex(x => new { x.Code, x.PremiumMode }).IsUnique();
            builder.HasMany(x => x.Rates).WithOne(x => x.RateTable!).HasForeignKey(x => x.RateTableId);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureRateConfiguration : IEntityTypeConfiguration<FlexiFutureRate>
{
    public void Configure(EntityTypeBuilder<FlexiFutureRate> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureRate");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.RateTableId, x.TermYears, x.BandMin });
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureQuoteConfiguration : IEntityTypeConfiguration<FlexiFutureQuote>
{
    public void Configure(EntityTypeBuilder<FlexiFutureQuote> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureQuote");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.QuoteNumber).HasMaxLength(40);
            builder.Property(x => x.RefferalCode).HasColumnName("ReferralId").HasMaxLength(100);
            builder.Property(x => x.PartnerCode).HasMaxLength(50);
            builder.Property(x => x.IssueDate).HasDateOnlyConversion();
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
            builder.Property(x => x.MaturityDate).HasDateOnlyConversion();
            builder.Property(x => x.RetryCount).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.IpAddress).HasMaxLength(100);
            builder.Property(x => x.Browser).HasMaxLength(500);
            builder.HasIndex(x => x.QuoteNumber).IsUnique();
            builder.HasIndex(x => x.IdNumber);
            builder.HasMany(x => x.Spouses).WithOne(x => x.Quote!).HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Children).WithOne(x => x.Quote!).HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Riders).WithOne(x => x.Quote!).HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.Cascade);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureQuoteSpouseConfiguration : IEntityTypeConfiguration<FlexiFutureQuoteSpouse>
{
    public void Configure(EntityTypeBuilder<FlexiFutureQuoteSpouse> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureQuoteSpouse");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureQuoteChildConfiguration : IEntityTypeConfiguration<FlexiFutureQuoteChild>
{
    public void Configure(EntityTypeBuilder<FlexiFutureQuoteChild> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureQuoteChild");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DateOfBirth).HasDateOnlyConversion();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class FlexiFutureQuoteRiderConfiguration : IEntityTypeConfiguration<FlexiFutureQuoteRider>
{
    public void Configure(EntityTypeBuilder<FlexiFutureQuoteRider> builder)
    {
        try
        {
            builder.ToTable("FlexiFutureQuoteRider");
            builder.HasKey(x => x.Id);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
