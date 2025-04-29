using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Domain.Entities;
using Tesis.Domain.Models;

namespace Tesis.DataAcces.Repository.IRepository
{
    public interface IUserRepository:IRepository<User>
    {
        void Update(User User);
    }
}
