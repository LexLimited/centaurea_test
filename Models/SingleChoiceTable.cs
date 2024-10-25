using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "single_choice")]
    public sealed class SingleChoiceTable
    {
        [Key]
        public int Id { get; set; }
        public int TableId { get; set; }
        public int FieldId { get; set; }
        public string Option { get; set; }
    }

}