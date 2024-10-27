using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentaureaTest.Models
{

    [Table(name: "fields")]
    public class FieldsTable
    {
        [Key]
        public int Id { get; set; }
        public int GridId { get; set; }
        public string Name { get; set; }
        public DataGridValueType Type { get; set; }
    }

    public sealed class RegexFieldsTable : FieldsTable
    {
        public string? RegexPattern { get; set; }
    }

    public sealed class RefFieldsTable : FieldsTable
    {
        public int? ReferencedGridId { get; set; }
    }

    public sealed class MultiSelectFieldsTable : FieldsTable
    {
        public int? OptionTableId { get; set; }
    }

    public sealed class SingleSelectFieldsTable : FieldsTable
    {
        public int? OptionTableId { get; set; }
    }

    public static class FieldsTableExtension
    {
        public static FieldsTable ToFieldsTable(this DataGridFieldSignature fieldSignature, int gridId)
        {
            return fieldSignature switch
            {
                DataGridRegexFieldSignature sig => new RegexFieldsTable
                {
                    Name = sig.Name,
                    Type = sig.Type,
                    GridId = gridId,
                    RegexPattern = sig.RegexPattern,
                },
                DataGridRefFieldSignature sig => new RefFieldsTable
                {
                    Name = sig.Name,
                    Type = sig.Type,
                    GridId = gridId,
                    ReferencedGridId = sig.ReferencedGridId,
                },
                DataGridSingleSelectFieldSignature sig => new SingleSelectFieldsTable
                {
                    Name = sig.Name,
                    Type = sig.Type,
                    GridId = gridId,
                    OptionTableId = sig.OptionTableId,
                },
                DataGridMultiSelectFieldSignature sig => new MultiSelectFieldsTable
                {
                    Name = sig.Name,
                    Type = sig.Type,
                    GridId = gridId,
                    OptionTableId = sig.OptionTableId,
                },
                _ => new FieldsTable
                {
                    Name = fieldSignature.Name,
                    Type = fieldSignature.Type,
                    GridId = gridId,
                },
            };
        }

        public static IEnumerable<FieldsTable> ToFieldsTables(this IEnumerable<DataGridFieldSignature> fieldSignatures, int gridId)
        {
            return fieldSignatures.Select(sig => ToFieldsTable(sig, gridId));
        }
    }

}