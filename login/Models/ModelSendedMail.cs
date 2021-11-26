using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace login.Models
{
    public class ModelSendedMail
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int fkUser { get; set; }
        public int fkObjectMail { get; set; }
        public int idMail { get; set; }
    }
}