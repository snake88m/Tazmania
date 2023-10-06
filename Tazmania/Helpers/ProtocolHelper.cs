using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Helpers
{
    public class ProtocolHelper
    {
        /// <summary>
        /// Calcolo il checksum con complemento a 1
        /// Tutti i byte del messaggio vengono inclusi nel calcolo
        /// Viene restituito il checksum calcolato su 2 bytes (short)
        /// </summary>
        public static byte[] CalculateCHSAt1(byte[] message)
        {
            short summary = (short)~message.Sum(r => r);

            byte[] checksum = BitConverter.GetBytes(summary);

            Array.Reverse(checksum);

            return checksum;
        }

        /// <summary>
        /// Converte il valore intero nei corrispettivi 4 bytes
        /// </summary>
        public static byte[] ConvertToBytes(int value)
        {
            byte[] message = BitConverter.GetBytes(value);
            Array.Reverse(message);
            return message;
        }

        /// <summary>
        /// Converte il valore intero nei corrispettivi 4 bytes e restituisce quelli specificati nel take
        /// </summary>
        public static byte[] ConvertToBytes(int value, int take)
        {
            if (!Enumerable.Range(1, 4).Contains(take))
            {
                // un int è composto da massimo 4 byte
                throw new ArgumentOutOfRangeException(nameof(take));
            }

            byte[] message = BitConverter.GetBytes(value);
            Array.Reverse(message);
            return message.Skip(1).Take(take).ToArray();
        }

        /// <summary>
        /// Imposta un singolo bit a true
        /// </summary>
        public static byte SetBit(byte value, int position)
        {
            //left-shift 1, then bitwise OR
            return (byte)(value | (1 << position));
        }

        /// <summary>
        /// Imposta un singolo bit a false
        /// </summary>
        public static byte UnsetBit(byte value, int position)
        {
            //left-shift 1, then take complement, then bitwise AND
            return (byte)(value & ~(1 << position));
        }

        /// <summary>
        /// Legge lo stato di un singolo bit
        /// </summary>
        public static bool GetBit(byte value, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((value & (1 << pos)) != 0);
        }
    }
}
