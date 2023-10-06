using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<User?> Fetch(string mail, string passwordHash);

        Task<IList<User>> Fetchs();
    }
}
