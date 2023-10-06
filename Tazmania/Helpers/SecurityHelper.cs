using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tazmania.Helpers
{
    public class SecurityHelper
    {
        /// <summary>
        /// Codifica un testo in SHA256
        /// </summary>
        /// <param name="clear">Valore in chiaro</param>
        public static string GenerateSHA256(string clear)
        {
            var bytes = new UTF8Encoding().GetBytes(clear);
            byte[] hashBytes;
            using (var algorithm = SHA256.Create())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }
    }
}
