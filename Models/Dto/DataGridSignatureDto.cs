namespace CentaureaTest.Models.Dto
{

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
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