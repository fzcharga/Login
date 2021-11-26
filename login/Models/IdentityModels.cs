using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Linq;

namespace login.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public int Fk_Abonne { get; set; }
        //public int Fk_Civilite { get; set; }

        public int IdCRM { get; set; }
        public int Fk_Civilite { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(new Claim("Fk_Abonne", this.Fk_Abonne.ToString()));
            userIdentity.AddClaim(new Claim("IdCRM", this.IdCRM.ToString()));
            userIdentity.AddClaim(new Claim("Fk_Civilite", this.Fk_Civilite.ToString()));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public static partial class IdentityExtensions
    {
        public static int GetGroupe(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Fk_Abonne");
            // Test for null to avoid issues during local testing
            return (claim != null) ? int.Parse(claim.Value) : 0;
        }

        public static int GetIdCrm(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IdCRM");
            // Test for null to avoid issues during local testing
            return (claim != null) ? int.Parse(claim.Value) : 0;
        }

        public static string GetCivilite(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Fk_Civilite");
            // Test for null to avoid issues during local testing
            var idCivilite = (claim != null) ? int.Parse(claim.Value) : 0;
            loginEntities db = new loginEntities();
            var civ = db.Civilite.FirstOrDefault(f => f.Id == idCivilite);
            if (civ != null)
                return civ.Libelle;

            return "";
        }

        public static string GetFullName(this System.Security.Principal.IPrincipal usr)
        {
            var fullNameClaim = ((ClaimsIdentity)usr.Identity).FindFirst("FullName");
            if (fullNameClaim != null)
                return fullNameClaim.Value;

            return usr.Identity.Name;
        }
    }
}