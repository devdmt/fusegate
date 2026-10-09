using DAL.Model.FlexiFund;
using DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

public class FlexiFundConfiguration : IEntityTypeConfiguration<FlexiFund>
{
    public void Configure(EntityTypeBuilder<FlexiFund> builder)
    {
        builder.ToTable("FlexiFund");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PolicyId).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CalculationMode).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Frequency).HasMaxLength(32).IsRequired();

        builder.Property(x => x.DeathBenefitPct).HasPrecision(18, 8);
        builder.Property(x => x.TargetPremium).HasPrecision(18, 2);
        builder.Property(x => x.TargetSumAssured).HasPrecision(18, 2);
        builder.Property(x => x.SumAssured).HasPrecision(18, 2);
        builder.Property(x => x.MonthlyEquivalent).HasPrecision(18, 2);
        builder.Property(x => x.SavingsPremium).HasPrecision(18, 2);
        builder.Property(x => x.RidersTotal).HasPrecision(18, 2);
        builder.Property(x => x.Phcl).HasPrecision(18, 2);
        builder.Property(x => x.TotalPremiumPayable).HasPrecision(18, 2);
        builder.Property(x => x.TotalFunds).HasPrecision(18, 2);

        builder.Property(x => x.IssueDate).HasDateOnlyConversion();
        builder.Property(x => x.MaturityDate).HasDateOnlyConversion();

        builder.HasIndex(x => x.PolicyId);
    }
}
