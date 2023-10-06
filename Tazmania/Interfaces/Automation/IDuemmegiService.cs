using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Automation
{
    public interface IDuemmegiService
    {
        Task ReadIO(IEnumerable<IO> ios);

        Task WriteOutput(IEnumerable<IO> ios);
    }
}
