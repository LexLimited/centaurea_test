namespace CentaureaTest.Models.Dto
{

    public sealed class DataGridSignatureDto
    {
        public List<DataGridFieldSignatureDto> Fields { get; set; }

        public DataGridSignature ToDataGridSignature()
        {
            var ret = new DataGridSignature();
            ret.Fields = Fields.Select(field => field.ToDataGridFieldSignature()).ToList();
            return ret;
        }
    }

}