using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Core
{
    public class MailUser
    {
        [Key]
        public long Id { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
    }
}
