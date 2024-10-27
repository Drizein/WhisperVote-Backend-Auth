namespace Domain.Entities;

public abstract class _BaseEntity
{
    protected Guid _guid { get; set; }
    protected readonly DateTime _createdAt = DateTime.Now;

    public Guid Id
    {
        get => _guid;
        set => _guid = value == Guid.Empty ? Guid.NewGuid() : value;
    }
}