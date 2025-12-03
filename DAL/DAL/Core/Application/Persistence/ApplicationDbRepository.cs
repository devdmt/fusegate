using Mapster;
using Ardalis.Specification.EntityFrameworkCore;
using DAL.Common.Contract;
using Ardalis.Specification;
namespace DAL.Core.Application.Persistence
{

    // Inherited from Ardalis.Specification's RepositoryBase<T>
    public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
        where T : class, IAggregateRoot
    {
        public ApplicationDbRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        // We override the default behavior when mapping to a dto.
        // We're using Mapster's ProjectToType here to immediately map the result from the database.
        protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
            ApplySpecification(specification, false)
                .ProjectToType<TResult>();
    }  
    public class AppDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
        where T : class, IAggregateRoot
    {
        public AppDbRepository(AkibappDbContext dbContext)
            : base(dbContext)
        {
        }

        // We override the default behavior when mapping to a dto.
        // We're using Mapster's ProjectToType here to immediately map the result from the database.
        protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
            ApplySpecification(specification, false)
                .ProjectToType<TResult>();
    }

 
}
