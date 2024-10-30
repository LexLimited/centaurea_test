using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models.Auth
{

    [Table("GridPermissions")]
    public sealed class GridPermission
    {
        [Key]
        public int GridId { get; set; }

        public string UserId { get; set; }
    }

}