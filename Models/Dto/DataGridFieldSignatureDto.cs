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
    /// <remarks>
    /// NB! This one is a particularly nasty class, which must be refactored
    /// </remarks>
    public sealed class DataGridFieldSignatureDto
    {
        public string Name { get; set; }
        public DataGridValueType Type { get; set; }

        /// <remarks>See FieldTable.Order</remarks>
        public int Order { get; set; }
        public string? RegexPattern { get; set; }
        public int? ReferencedGridId { get; set;  }
        public List<string>? Options { get; set; }

        private DataGridFieldSignatureDtoMissingPropertyException NewMissingPropertyExeption(string missingPropertyName)
        {
            return new DataGridFieldSignatureDtoMissingPropertyException(this, missingPropertyName);
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
                _ => new DataGridFieldSignature(Name, Type, Order),
            };
        }
    }

}