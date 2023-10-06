using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface IRequestRepository : IRepository
    {
        Task Add(Request operation);

        Task<IList<Request>> FetchsAndClean();
    }
}
