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
    
    public partial class ORGANISATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ORGANISATION()
        {
            this.ORGANISATION1 = new HashSet<ORGANISATION>();
        }
    
        public string FK_USER { get; set; }
        public int FK_ORG { get; set; }
        public string FK_SUP { get; set; }
        public int ETAT { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ORGANISATION> ORGANISATION1 { get; set; }
        public virtual ORGANISATION ORGANISATION2 { get; set; }
        public virtual UNITE UNITE { get; set; }
    }
}
