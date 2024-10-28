namespace CentaureaTest.Models.Dto
{

    public sealed class DataGridFieldSignatureDtoMissingPropertyException : Exception
    {
        public DataGridFieldSignatureDtoMissingPropertyException(DataGridFieldSignatureDto dto, string missingPropertyName)
            : base($"{dto.Type} DataGridFieldSignatureDto must contain '{missingPropertyName}' property")
        {}
    }

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
    public sealed class DataGridFieldSignatureDto
    {
        public string Name { get; set; }
        public DataGridValueType Type { get; set; }

        /// <remarks>See FieldTable.Order</remarks>
        public int Order { get; set; }
        public string? RegexPattern { get; set; }
        public int? ReferencedGridId { get; set;  }
        public int? OptionTableId { get; set; }

        private DataGridFieldSignatureDtoMissingPropertyException NewMissingPropertyExeption(string missingPropertyName)
        {
            return new DataGridFieldSignatureDtoMissingPropertyException(this, missingPropertyName);
        }

        public void Validate()
        {
            switch (Type)
            {
                case DataGridValueType.Regex: throw NewMissingPropertyExeption("RegexPattern");
                case DataGridValueType.Ref: throw NewMissingPropertyExeption("RefencedGridId");
                case DataGridValueType.SingleSelect: throw NewMissingPropertyExeption("OptionTableId");
                case DataGridValueType.MultiSelect: throw NewMissingPropertyExeption("OptionTableId");
                default: break;
            };
        }

        public DataGridFieldSignature ToDataGridFieldSignature()
        {
            return Type switch
            {
                DataGridValueType.Regex => new DataGridRegexFieldSignature(
                    Name, RegexPattern ?? throw NewMissingPropertyExeption("RegexPattern"), Order
                ),
                DataGridValueType.Ref => new DataGridRefFieldSignature(
                    Name, ReferencedGridId ?? throw NewMissingPropertyExeption("ReferencedGridId"), Order
                ),
                DataGridValueType.SingleSelect => new DataGridSingleSelectFieldSignature(
                    Name, OptionTableId ?? throw NewMissingPropertyExeption("OptionTableId"), Order
                ),
                DataGridValueType.MultiSelect => new DataGridMultiSelectFieldSignature(
                    Name, OptionTableId ?? throw NewMissingPropertyExeption("OptionTableId"), Order
                ),
                _ => new DataGridFieldSignature(Name, Type, Order),
            };
        }
    }

}