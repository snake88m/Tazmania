using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts.Responses
{
    public class IOFetchGroupResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Order { get; set; }

        public IEnumerable<IOContract> IOs { get; set; } = null!;
    }
}
