using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models.Auth
{

    [Table(name: "GridPermissions")]
    public sealed class GridPermission
    {
        [Key]
        public int PermissionId { get; set; } 
        public int GridId { get; set; }
        public string UserName { get; set; }
    }

}