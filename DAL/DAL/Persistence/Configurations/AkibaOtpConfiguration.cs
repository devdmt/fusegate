using DAL.Model.Akiba;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Persistence.Configurations;

internal sealed class AkibaOtpConfiguration : IEntityTypeConfiguration<AkibaOtp>
{
    public void Configure(EntityTypeBuilder<AkibaOtp> builder)
    {
        builder.ToTable("OTPs");
        builder.HasKey(x => x.Id);
    }
}
