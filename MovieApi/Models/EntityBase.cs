namespace MovieApi.Models;

// Making the class abstract ensures that it cannot be instantiated on its own, as it is meant to provide shared functionality rather than represent a standalone concept.
public abstract class EntityBase
{
    // private setters to ensure that changes can only be made within the class, maintaining the integrity of the object.
    public Guid Id { get; private init; } = Guid.NewGuid();
    public DateTimeOffset Created { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.UtcNow;
    protected void UpdateLastModified() => LastModified = DateTimeOffset.UtcNow;
}