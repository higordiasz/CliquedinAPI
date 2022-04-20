using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliquedinAPI.Models.Conta
{
    public class ContaCliquedin
    {
        public string Username { get; set; }
        public string ContaID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Imap_ID { get; set; }
        public string Imap_Port { get; set; }
        public string Email_Password { get; set; }
        public bool Imap_SSL { get; set; }
        public bool Verificar_Email { get; set; }
    }
}
