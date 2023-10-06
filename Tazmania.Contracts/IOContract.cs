using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Contracts
{
    public class IOContract
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public int Type { get; set; }

        public bool IsActive { get; set; }

        public float Value { get; set; }

        public float ValueCorrected { get; set; }

        public string? Major { get; set; }

        public string? GroupName { get; set; }

        public int ShutterUpId { get; set; }

        public bool ShutterUpIsActive { get; set; }
    }
}
