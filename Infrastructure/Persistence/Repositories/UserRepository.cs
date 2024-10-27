using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : _BaseRepository<User>, IUserRepository
{
    public UserRepository(CDbContext context) : base(context)
    {
    }
}