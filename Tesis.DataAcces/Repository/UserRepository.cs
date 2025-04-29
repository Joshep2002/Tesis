using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Entities;

namespace Tesis.DataAcces.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(User User)
        {
            _context.Update(User);
        }
    }
}
