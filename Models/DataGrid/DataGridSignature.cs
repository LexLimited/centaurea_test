using System.Text.Json.Serialization;

namespace CentaureaTest.Models
{

    public class DataGridFieldSignature
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DataGridValueType Type { get; set; }

        /// <remarks>See FieldTable.Order</remarks>
        public int Order { get; set; }
        public DataGridFieldSignature(string name, DataGridValueType type, int order)
        {
            Name = name;
            Type = type;
            Order = order;
        }

        public static DataGridFieldSignature From(FieldsTable table)
        {
            return new(table.Name, table.Type, table.Order);
        }

        public override string ToString()
        {
            return $"Base field: Name: {Name}, Type: {Type}, Order: {Order}";
        }
    }

    public sealed class DataGridRegexFieldSignature : DataGridFieldSignature
    {
        public string RegexPattern { get; set; }

        public DataGridRegexFieldSignature(string name, string regexPattern, int order) : base(name, DataGridValueType.Regex, order)
        {
            RegexPattern = regexPattern;
        }

        public override string ToString()
        {
            return $"Regex field: Name: {Name}, Type: {Type}, RegexPattern: {RegexPattern}";
        }
    }

    public sealed class DataGridRefFieldSignature : DataGridFieldSignature
    {
        public int ReferencedGridId { get; set; }

        public DataGridRefFieldSignature(string name, int referencedGridId, int order) : base(name, DataGridValueType.Ref, order)
        {
            ReferencedGridId = referencedGridId;
        }
    }

    public sealed class DataGridSignature
    {
        public List<DataGridFieldSignature> Fields { get; set; }

        public DataGridSignature()
        {
            Fields = new List<DataGridFieldSignature>();
        }

        public DataGridSignature(IEnumerable<DataGridFieldSignature> fields)
        {
            Fields = fields.ToList();
        }

        public DataGridSignature(IEnumerable<FieldsTable> fields)
        {
            Fields = fields.Select(field => new DataGridFieldSignature(field.Name, field.Type, field.Order)).ToList();
        }

        /// <summary>
        /// Validates that values satisfy the constraints specify in the signature
        /// </summary>
        /// <remarks>Single and multi select values are not validated</remarks>
        public (bool ok, string error) ValidateValues(IEnumerable<DataGridValue> valuesEnumerable)
        {
            // NB! Probably worth optimizing, maybe not
            var values = valuesEnumerable.ToList();

            if (values.Count != Fields.Count)
            {
                return (false, "Number of values does not match the signature");
            }

            for (int i = 0; i < values.Count; ++i)
            {
                var (ok, message) = values[i].Validate(Fields[i]);
                if (!ok)
                {
                    return (false, message);
                }
            }

            return (true, string.Empty);
        }
    }

    public static class DataGridSignatureExtension
    {
        public static DataGridFieldSignature ToDataGridFieldSignature(this FieldsTable fieldsTable)
        {
            return fieldsTable.Type switch
                {
                    DataGridValueType.Regex => new DataGridRegexFieldSignature(
                        fieldsTable.Name,
                        ((RegexFieldsTable)fieldsTable)?.RegexPattern ?? throw new Exception("Bad regex field"),
                        fieldsTable.Order
                    ),
                    DataGridValueType.Ref => new DataGridRefFieldSignature(
                        fieldsTable.Name,
                        ((RefFieldsTable)fieldsTable)?.ReferencedGridId ?? throw new Exception("Bad ref field"),
                        fieldsTable.Order
                    ),
                    _ => new DataGridFieldSignature(fieldsTable.Name, fieldsTable.Type, fieldsTable.Order)
                };
        }

        public static DataGridSignature ToDataGridSignature(this IEnumerable<FieldsTable> fieldsTableEnumerable)
        {
            return new DataGridSignature(fieldsTableEnumerable.Select(ToDataGridFieldSignature));
        }
    }

}