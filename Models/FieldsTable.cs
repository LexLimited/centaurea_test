using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "fields")]
    public sealed class FieldsTable
    {
        [Key]
        public int Id { get; set; }
        public int GridId { get; set; }
        public DataGridValueType Type { get; set; } 
    }

}