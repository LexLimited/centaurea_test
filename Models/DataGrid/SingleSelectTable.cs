using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "single_select")]
    public sealed class SingleSelectTable
    {
        [Key]
        public int Id { get; set; }
        public int FieldId { get; set; }
        public string Option { get; set; }

        public SingleSelectTable(int fieldId, string option)
        {
            FieldId = fieldId;
            Option = option;
        }
    }

}