using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Entities
{
    public class IO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Identifica l'indirizzo di memoria su DFCP in cui è presente questo valore
        /// </summary>
        [Range(1, int.MaxValue)]
        public int DFCPAddress { get; set; }

        /// <summary>
        /// Se valorizzato vuol dire che il cambio stato deve essere dato tramite comando punto virtuale su DFCP
        /// </summary>
        [Range(1, int.MaxValue)]
        public int DFCPVirtual { get; set; }

        /// <summary>
        /// Morsetto del modulo di campo a cui è collegato l'IO
        /// </summary>
        [Range(0, 7)]
        public int ModulePin { get; set; }

        public bool IsActive { get; set; }

        public float Value { get; set; }

        public float Correction { get; set; }

        /// <summary>
        /// Si applica ai tipi Input. Indica se il componente lavora con logica inversa
        /// </summary>
        public bool IsInverted { get; set; }

        public IOMajor Major { get; set; }

        public int IOGroupId { get; set; }

        public IOGroup IOGroup { get; set; } = null!;

        public int? ParentId { get; set; }

        /// <summary>
        /// Se valorizzato lo stato dell'output verrà impostato a ogni avvio dell'applicazione
        /// </summary>
        public bool? DefaultIsActive { get; set; }

        [NotMapped]
        public float ValueCorrected
        {
            get { return Value + Correction; }
        }

        [NotMapped]
        public IOType Type 
        {
            get
            {
                //return (Major == IOMajor.ContactSensor || Major == IOMajor.EmergencyButton || Major == IOMajor.RainSensor) ? IOType.Input : Major == IOMajor.TemperatureSensor ? IOType.InputValue : IOType.Output;
                return (int)Major >= 40 ? IOType.InputValue : (int)Major >= 20 ? IOType.Input : IOType.Output;
            }
        }

        [NotMapped]
        public IOInput Input { get; set; } = new IOInput();

        [NotMapped]
        public IOOutput Output { get; set; } = new IOOutput();

        [NotMapped]
        public bool HasVirtual => DFCPVirtual > 0;

    }
}
