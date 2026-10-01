using DAL.Model;
using DAL.Model.FlexiContributions;
using DAL.Model.FlexiFuture;
using DAL.Model.FlexiFund;
using DAL.Model.Pensioner;
using DAL.Model.Shared;
using DAL.Persistence;
using DAL.Persistence.Configurations;
using DAL.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL
{
   
     public class AkibappDbContext  : DbContext
    {
       
        public string CurrentUserId { get; set; }

        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<APIUSER> user { get; set; }
        public DbSet<Partners> Partners { get; set; }       
        public DbSet<MsureRequests> msureRequests { get; set; }
        public DbSet<PensionCustomer> customers { get; set; }
        public DbSet<MpesaSettings> mpesaSettings { get; set; }
        public DbSet<MpesaToken> mpesaToken { get; set; }
        public DbSet<PensionerBeneficiaries> beneficiaries { get; set; }
        public DbSet<PensionerBalanceRequest>  pensionerBalanceRequests { get; set; }
        public DbSet<PensionerFund> PensionerFund { get; set; } 
        public DbSet<ContributionSettings> contributionSettings { get; set; }
        public DbSet<PensionContributions> pensionContributions { get; set; }
        public DbSet<CustomerRoles> customerRoles { get; set; }
        public DbSet<PensionerGurdian>  gurdian { get; set; }

        public DbSet<FlexiFutureProductSettings> FlexiFutureProductSettings { get; set; }
        public DbSet<FlexiFutureFrequencyDiscount> FlexiFutureFrequencyDiscounts { get; set; }
        public DbSet<FlexiFutureRateTable> FlexiFutureRateTables { get; set; }
        public DbSet<FlexiFutureRate> FlexiFutureRates { get; set; }
        public DbSet<FlexiFutureQuote> FlexiFutureQuotes { get; set; }
        public DbSet<FlexiFutureQuoteSpouse> FlexiFutureQuoteSpouses { get; set; }
        public DbSet<FlexiFutureQuoteChild> FlexiFutureQuoteChildren { get; set; }
        public DbSet<FlexiFutureQuoteRider> FlexiFutureQuoteRiders { get; set; }
        public DbSet<FlexiFuturePolicy> FlexiFuturePolicies { get; set; }
        public DbSet<FlexiFutureFamilyMember> FlexiFutureFamilyMembers { get; set; }
        public DbSet<FlexiFutureBeneficiary> FlexiFutureBeneficiaries { get; set; }
        public DbSet<FlexiFutureGuardian> FlexiFutureGuardians { get; set; }
        public DbSet<FlexiFutureHealthInfo> FlexiFutureHealthInfo { get; set; }
        public DbSet<FlexiFutureOccupationHazard> FlexiFutureOccupationHazards { get; set; }
        public DbSet<HealthQuestionsLibrary> HealthQuestionsLibrary { get; set; }
        public DbSet<QuotesShare> QuotesShares { get; set; }
        public DbSet<FlexiContribution> FlexiContributions { get; set; }
        public DbSet<PaymentInstruction> PaymentInstructions { get; set; }
        public DbSet<FlexiFund> FlexiFunds { get; set; }

        public AkibappDbContext(DbContextOptions<AkibappDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string priceDecimalType = "decimal(18,2)";
            const string idDecimalType = "decimal(18,0)";
            builder.Entity<APIUSER>().ToTable("APIUSERS");
            builder.Entity<Customers>().ToTable("Customers").HasKey(a => a.Id);
            builder.Entity<PensionerGurdian>().ToTable("Gurdian").HasKey(a => a.Id);
            builder.Entity<PensionerFund>().ToTable("PensionerFund").HasKey(a => a.Id);
            builder.Entity<PensionContributions>().ToTable("Contributions").HasKey(a => a.Id);
            builder.Entity<ContributionSettings>().ToTable("ContributionSettings").HasKey(a => a.Id);

            builder.ApplyConfiguration(new FlexiFutureProductSettingsConfiguration());
            builder.ApplyConfiguration(new FlexiFutureFrequencyDiscountConfiguration());
            builder.ApplyConfiguration(new FlexiFutureRateTableConfiguration());
            builder.ApplyConfiguration(new FlexiFutureRateConfiguration());
            builder.ApplyConfiguration(new FlexiFutureQuoteConfiguration());
            builder.ApplyConfiguration(new FlexiFutureQuoteSpouseConfiguration());
            builder.ApplyConfiguration(new FlexiFutureQuoteChildConfiguration());
            builder.ApplyConfiguration(new FlexiFutureQuoteRiderConfiguration());
            builder.ApplyConfiguration(new FlexiFuturePolicyConfiguration());
            builder.ApplyConfiguration(new FlexiFutureFamilyMemberConfiguration());
            builder.ApplyConfiguration(new FlexiFutureBeneficiaryConfiguration());
            builder.ApplyConfiguration(new FlexiFutureGuardianConfiguration());
            builder.ApplyConfiguration(new FlexiFutureHealthInfoConfiguration());
            builder.ApplyConfiguration(new FlexiFutureOccupationHazardConfiguration());
            builder.ApplyConfiguration(new HealthQuestionsLibraryConfiguration());
            builder.ApplyConfiguration(new QuotesShareConfiguration());
            builder.ApplyConfiguration(new FlexiContributionConfiguration());
            builder.ApplyConfiguration(new PaymentInstructionConfiguration());
            builder.ApplyConfiguration(new FlexiFundConfiguration());

            DateOnlyModelConfiguration.ApplyDateOnlyConversions(builder);
        }


        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    UpdateAuditEntities();
        //    return base.SaveChangesAsync(cancellationToken);
        //}


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            try
            {
                var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = now;
                entity.UpdatedBy = CurrentUserId;

            } 
            
            }catch(Exception ex)
            {

            }
        }
    }
}
