namespace CentaureaTest.Models.Dto
{

    public sealed class DataGridFieldSignatureDto
    {
        public string Name { get; set; }
        public DataGridValueType Type { get; set; }
        public string? RegexPattern { get; set; }
        public int? ReferencedGridId { get; set;  }
        public int? OptionTableId { get; set; }

        public DataGridFieldSignature ToDataGridFieldSignature()
        {
            return Type switch
            {
                DataGridValueType.Regex => new DataGridRegexFieldSignature(Name, RegexPattern ?? throw new Exception("Regex DataGridFieldSignatureDto must contain a regex pattern")),
                DataGridValueType.Ref => new DataGridRefFieldSignature(Name, ReferencedGridId ?? throw new Exception("Ref DataGridFieldSignatureDto must contain a referenced grid id")),
                DataGridValueType.SingleSelect => new DataGridSingleSelectFieldSignature(Name, OptionTableId ?? throw new Exception("SingleSelect DataGridFieldSignatureDto must contain an option table id")),
                DataGridValueType.MultiSelect => new DataGridMultiSelectFieldSignature(Name, OptionTableId ?? throw new Exception("MultiSelect DataGridFieldSignatureDto must contain an option table id")),
                _ => new DataGridFieldSignature(Name, Type),
            };
        }
    }

}