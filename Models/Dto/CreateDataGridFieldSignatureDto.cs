using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace CentaureaTest.Models.Dto
{

    public sealed class CreateDataGridFieldSignatureDto
    {
        public string Name { get; set; }
        public DataGridValueType Type { get; set; }
        public int Order { get; set; }
        public string? RegexPattern { get; set; }
        public int? ReferencedGridId { get; set;  }
        public List<string>? Options { get; set; }

        Exception NewMissingPropertyException(string propertyName)
        {
            return new Exception($"CreateDataGridFieldSignature of type {Type} must specify '{propertyName}' property");
        }

        public FieldsTable ToFieldsTable()
        {
            switch (Type)
            {
                case DataGridValueType.Regex:
                    return new RegexFieldsTable
                    {
                        Name = Name,
                        Type = Type,
                        Order = Order,
                        RegexPattern = RegexPattern
                            ?? throw NewMissingPropertyException("RegexPattern"),
                    };
                case DataGridValueType.Ref:
                    return new RefFieldsTable
                    {
                        Name = Name,
                        Type = Type,
                        Order = Order,
                        ReferencedGridId = ReferencedGridId
                            ?? throw NewMissingPropertyException("ReferencedGridId"),
                    };
                default:
                    return new FieldsTable
                    {
                        Name = Name,
                        Type = Type,
                        Order = Order,
                    };
            }
        }
    }

}