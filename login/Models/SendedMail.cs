//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace login.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SendedMail
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public string Fk_User { get; set; }
        public int Fk_ObejctMail { get; set; }
        public int IdMail { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual ObjectMail ObjectMail { get; set; }
    }
}
