using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public RequestType Type { get; set; }

        [Range(0, int.MaxValue)]
        public int EntityId { get; set; }

        public bool IsActive { get; set; }

        public float Value { get; set; }
    }
}
