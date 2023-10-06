using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazmania.Entities;

namespace Tazmania.Interfaces.Services
{
    public interface IMailService
    {
        void SendAsync(MailObject mailObject);
    }
}
