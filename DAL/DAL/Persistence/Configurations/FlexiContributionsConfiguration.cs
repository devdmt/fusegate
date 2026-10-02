using DAL.Model.FlexiContributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

// Both tables are created by IPP/Migrations/Scripts/AddFlexiContributions.sql and are
// deliberately not represented in the EF migration snapshot.
//
// The OUTPUT clause is left disabled (NoOutputClauseConvention suppresses it for every
// table that is not FlexiFuture*/FlexiFund*). That is correct here: both entities have
// client-generated Guid keys and no identity, computed or rowversion columns, so EF needs
// nothing back from the INSERT and SELECT @@ROWCOUNT covers update concurrency.
//
// Consequence: do NOT declare the SQL DEFAULT constraints to EF via HasDefaultValue().
// That sets ValueGenerated.OnAdd, forcing a read-back that the suppressed OUTPUT clause
// cannot serve. Defaults live in SQL only; the service assigns the flags explicitly.

public class FlexiContributionConfiguration : IEntityTypeConfiguration<FlexiContribution>
{
    public void Configure(EntityTypeBuilder<FlexiContribution> builder)
    {
        builder.ToTable("FlexiContributions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Naration).HasMaxLength(1000);
        builder.Property(x => x.Reference).HasMaxLength(1000);
        builder.Property(x => x.BatchReference).HasMaxLength(100);
        builder.Property(x => x.ThirdpartyRef).HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.Property(x => x.CreatedFromIP).HasMaxLength(100);
        builder.Property(x => x.CreatedFromBrowser).HasMaxLength(1000);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.FlexiFuturePolicyId);
    }
}

public class PaymentInstructionConfiguration : IEntityTypeConfiguration<PaymentInstruction>
{
    public void Configure(EntityTypeBuilder<PaymentInstruction> builder)
    {
        builder.ToTable("PaymentInstructions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Naration).HasMaxLength(1000);
        builder.Property(x => x.ApprovedBy).HasMaxLength(50);
        builder.Property(x => x.ApprovedIP).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.Property(x => x.CreatedFromIP).HasMaxLength(100);
        builder.Property(x => x.CreatedFromBrowser).HasMaxLength(1000);
        builder.Property(x => x.UploadDocumentsBase64);

        // Supports the "one pending instruction per customer+policy+type" lookup that both
        // the create and the upload endpoint rely on.
        builder.HasIndex(x => new { x.CustomerId, x.FlexiFuturePolicyId, x.ProcessingStatus });
    }
}
