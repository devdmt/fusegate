// ====================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Identity;
using DAL.Model;
using DAL.Models.Interfaces;
namespace DAL
{
    
    /// <summary>   
    /// 
    /// </summary>
    // IdentityDbContext<TenziAdminUsers, TenziApplicationRole, string>
    public class MainDbContext  : DbContext

    {
       
        public string CurrentUserId { get; set; }

        public IDbConnection Connection => Database.GetDbConnection();
       
        
        public DbSet<MsureRequests> msureRequests { get; set; }
        
       
       
      
        //public DbSet<LabourCost> LabourCosts { get; set; }
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string priceDecimalType = "decimal(18,2)";
            const string idDecimalType = "decimal(18,0)";
            
           


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
