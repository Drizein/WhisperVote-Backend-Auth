using Domain.Enums;

namespace Domain.Entities;

public class User : _BaseEntity
{
    private string _username;
    private string _password;
    private Role _role = Role.User;
    private int _strikes;

    public User(string username, string password)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
        _password = password ?? throw new ArgumentNullException(nameof(password));
        _strikes = 0;
    }

    public string Username
    {
        get => _username;
        set => _username = value;
    }

    public string Password
    {
        get => _password;
        set => _password = value;
    }

    public Guid Guid => _guid;

    public Role Role
    {
        get => _role;
        set => _role = value;
    }

    public int Strikes
    {
        get => _strikes;
        set => _strikes = value;
    }
}