using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class IdentRepository : _BaseRepository<Ident>, IIdentRepository
{
    public IdentRepository(CDbContext context) : base(context)
    {
    }
}