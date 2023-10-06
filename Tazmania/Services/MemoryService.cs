using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;
using Tazmania.Interfaces.Services;

namespace Tazmania.Services
{
    public class MemoryService : IMemoryService
    {
        public IEnumerable<IO> IOs { get; private set; } = null!;

        /// <summary>
        /// Ritorna un input valido. Se non esiste solleva un'eccezione
        /// </summary>
        public IO GetInput(int id)
        {
            return IOs.Single(r => r.Id == id && (r.Type == IOType.Input || r.Type == IOType.InputValue));
        }

        /// <summary>
        /// Ritorna un output valido. Se non esiste solleva un'eccezione
        /// </summary>
        public IO GetOutput(int id)
        {
            return IOs.Single(r => r.Id == id && r.Type == IOType.Output);
        }

        public void SetOutput(int id, bool status)
        {
            //IO output = IOs.Single(r => r.Id == id && r.Type == IOType.Output);

            //if (!output.ParentId.HasValue || !status)
            //{
            //    IOs.Single(r => r.Id == id && r.Type == IOType.Output).Output.NewStatus = status;
            //}
            //else
            //{
            //    IO parentOutput = IOs.Single(r => r.Id == output.ParentId);

            //    if (!parentOutput.IsActive || !parentOutput.Output.NewStatus)
            //    {
            //        IOs.Single(r => r.Id == id && r.Type == IOType.Output).Output.NewStatus = status;
            //    }
            //}
            
            IOs.Single(r => r.Id == id && r.Type == IOType.Output).Output.NewStatus = status;
        }

        public void InitIOs(IEnumerable<IO> ios)
        {
            if (IOs != null)
            {
                throw new ArgumentException("Alredy initialized");
            }

            IOs = ios;
        }
    }
}
