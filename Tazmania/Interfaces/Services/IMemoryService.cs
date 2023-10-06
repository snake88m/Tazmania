using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Services
{
    public interface IMemoryService
    {
        IEnumerable<IO> IOs { get; }

        //IEnumerable<Heating> Heatings { get; }

        void InitIOs(IEnumerable<IO> ios);

        //void InitHeatings(IEnumerable<Heating> heatings);

        IO GetInput(int id);

        IO GetOutput(int id);

        void SetOutput(int outputId, bool newStatus);

    }
}
