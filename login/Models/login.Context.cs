//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a �t� g�n�r� � partir d'un mod�le.
//
//     Des modifications manuelles apport�es � ce fichier peuvent conduire � un comportement inattendu de votre application.
//     Les modifications manuelles apport�es � ce fichier sont remplac�es si le code est r�g�n�r�.
// </auto-generated>
//------------------------------------------------------------------------------

namespace login.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class loginEntities : DbContext
    {
        public loginEntities()
            : base("name=loginEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Abonne> Abonne { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Civilite> Civilite { get; set; }
        public virtual DbSet<DynMenu> DynMenu { get; set; }
        public virtual DbSet<Etat> Etat { get; set; }
        public virtual DbSet<Extension> Extension { get; set; }
        public virtual DbSet<ObjectMail> ObjectMail { get; set; }
        public virtual DbSet<ORGANISATION> ORGANISATION { get; set; }
        public virtual DbSet<SendedMail> SendedMail { get; set; }
        public virtual DbSet<TypeFichier> TypeFichier { get; set; }
        public virtual DbSet<UNITE> UNITE { get; set; }
        public virtual DbSet<user_history> user_history { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<ApplicationsUsers> ApplicationsUsers { get; set; }
    }
}
