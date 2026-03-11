// ====================================================

using DAL.Model;
using DAL.Model.FuneralExpense;
using DAL.Model.HealthDeclaration;
using DAL.Model.LastExpense;

using DAL.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace DAL
{
  
    /// <summary>
    /// 
    /// </summary>
    // IdentityDbContext<TenziAdminUsers, TenziApplicationRole, string>
    public class ApplicationDbContext  : DbContext
    {
       
        public string CurrentUserId { get; set; }

        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<APIUSER> user { get; set; }
        public DbSet<Partners> Partners { get; set; }       
        public DbSet<MsureRequests> msureRequests { get; set; }
        public DbSet<Customers> customers { get; set; }
        public DbSet<PartnersProducts>  partnersProducts { get; set; }
        public DbSet<FuneralExpenseQuotation> FuneralExpenseQuotations { get; set; }
        public DbSet<FuneralExpenseOnboarding> FuneralExpenseOnboardings { get; set; }
        public DbSet<MemberHealth> MemberHealths { get; set; }
        public DbSet<PaymentContribution> PaymentContributions { get; set; }
        public DbSet<MemberPayment> MemberPayments { get; set; }
        public DbSet<CustomerProduct> customerProducts { get; set; }
        public DbSet<MpesaToken> mpesaToken { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<MpesaSettings> mpesaSettings { get; set; }
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<Beneficiaries> Beneficiaries { get; set; } 
        public DbSet<OTPOnboarding> OTPOnboardings { get; set; }
        public DbSet<PolicyActivation> PolicyActivations { get; set; }
        public DbSet<CustomerHealthDeclaration> CustomerHealthDeclarations { get; set; }
        public DbSet<CustomerHealthDeclarationAnswer> CustomerHealthDeclarationAnswers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
          public Task<PartnersProducts?> GetPartnerProductsAsync(string partnerCode)
        {
            // Using EF Core LINQ to mirror the provided SQL logic:
            // SELECT a.[Id],a.[Name],a.[Description] ,a.[PartnerId] ,a.[ProductId] ,a.[Image] ,a.[Active]  
            // FROM [dbo].[partnersProducts] a , Products b, Partners c 
            // WHERE a.ProductId= b.Id and a.PartnerId=c.Id and b.[Productenum]=@productEnum and c.PartnerCode=@partnerId

            return partnersProducts
                .Join(
                    Set<Products>(), // assuming a DbSet<Products> is available, otherwise adjust as needed
                    a => a.ProductId,
                    b => b.Id,
                    (a, b) => new { a, b }
                )
                .Join(
                    Set<Partners>(), // assuming a DbSet<Partners> is available, otherwise adjust as needed
                    ab => ab.a.PartnerId,
                    c => c.Id,
                    (ab, c) => new { ab.a, ab.b, c }
                )
                .Where(x => x.c.PartnerCode == partnerCode)
                .Select(x => x.a)
                .FirstOrDefaultAsync()!;

             
        }
        /// <summary>
        /// Global helper to get active partner products for a given product type and partner.
        /// This encapsulates:
        /// SELECT a.[Id], a.[Name], a.[Description], a.[PartnerId], a.[ProductId],
        ///        a.[Image], a.[Active]
        /// FROM [dbo].[partnersProducts] a, [dbo].[Products] b
        /// WHERE a.ProductId = b.Id
        ///   AND b.[Productenum] = @productEnum
        ///   AND a.[PartnerId] = @partnerId
        /// </summary>
        /// <param name="productEnum">Product type to filter on.</param>
        /// <param name="partnerId">Partner identifier.</param>
        /// <returns>List of matching <see cref="PartnersProducts"/> records.</returns>
        public Task<PartnersProducts?> GetPartnerProductsAsync(Productenum productEnum, string partnerCode)
        {
            // Using EF Core LINQ to mirror the provided SQL logic:
            // SELECT a.[Id],a.[Name],a.[Description] ,a.[PartnerId] ,a.[ProductId] ,a.[Image] ,a.[Active]  
            // FROM [dbo].[partnersProducts] a , Products b, Partners c 
            // WHERE a.ProductId= b.Id and a.PartnerId=c.Id and b.[Productenum]=@productEnum and c.PartnerCode=@partnerId

            return partnersProducts
                .Join(
                    Set<Products>(), // assuming a DbSet<Products> is available, otherwise adjust as needed
                    a => a.ProductId,
                    b => b.Id,
                    (a, b) => new { a, b }
                )
                .Join(
                    Set<Partners>(), // assuming a DbSet<Partners> is available, otherwise adjust as needed
                    ab => ab.a.PartnerId,
                    c => c.Id,
                    (ab, c) => new { ab.a, ab.b, c }
                )
                .Where(x => x.b.productenum == productEnum && x.c.PartnerCode == partnerCode)
                .Select(x => x.a)
                .FirstOrDefaultAsync()!;

             
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string priceDecimalType = "decimal(18,2)";
            const string idDecimalType = "decimal(18,0)";
            // builder.Entity<TransactionsUploadTemp>().Property(e => e.id).ValueGeneratedOnAdd();
            //  builder.Entity<TransactionsUploadTemp>().Property(e => e.Key).ValueGeneratedNever();
            builder.Entity<APIUSER>().ToTable("APIUSERS");
            builder.Entity<Customers>().ToTable("Customers").HasKey(a => a.Id);
            builder.Entity<Guardian>().ToTable("Guardian").HasKey(a => a.Id);
            builder.Entity<Beneficiaries>().ToTable("Beneficiaries").HasKey(a => a.Id);
            builder.Entity<OTP>().ToTable("OTPs").HasKey(a => a.Id);
            // Health declarations
            builder.Entity<CustomerHealthDeclaration>(e =>
            {
                e.ToTable("CustomerHealthDeclarations");
                e.HasKey(x => x.Id);
                e.Property(x => x.CustomerId).HasMaxLength(64).IsRequired();
                e.Property(x => x.ContextKey).HasMaxLength(100);
                e.Property(x => x.HeightCm).HasPrecision(9, 2);
                e.Property(x => x.WeightKg).HasPrecision(9, 2);
                e.Property(x => x.Bmi).HasPrecision(9, 2);
                e.HasIndex(x => new { x.CustomerId, x.ContextKey })
                    .IsUnique()
                    .HasFilter("[ContextKey] IS NOT NULL");
                e.HasMany(x => x.Answers)
                    .WithOne(x => x.Declaration)
                    .HasForeignKey(x => x.DeclarationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CustomerHealthDeclarationAnswer>(e =>
            {
                e.ToTable("CustomerHealthDeclarationAnswers");
                e.HasKey(x => x.Id);
                e.Property(x => x.Question).HasConversion<int>();
                e.Property(x => x.Narration).HasMaxLength(int.MaxValue);
                e.HasIndex(x => new { x.DeclarationId, x.Question }).IsUnique();
                e.HasIndex(x => x.DeclarationId);
            });
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
