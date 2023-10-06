using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Repositories
{
    public interface IIORepository: IRepository
    {
        Task<IList<IO>> Fetchs(params IOMajor[] majors);

        Task SetActive(int id, bool isActive);

        Task SetValue(int id, float value);
    }
}
