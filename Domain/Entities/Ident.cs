namespace Domain.Entities;

public class Ident : _BaseEntity
{
    private string _identId;

    public string IdentId
    {
        get => _identId;
        set => _identId = value;
    }

    public Ident(string identId)
    {
        _identId = identId;
    }
}