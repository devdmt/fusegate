using API.Infrastructure.Common.Contract;
namespace API.Infrastructure.Common;
public interface IEntity
{
    List<DomainEvent> DomainEvents { get; }
}

public interface IEntity<TId> : IEntity
{
    TId Id { get; }
}