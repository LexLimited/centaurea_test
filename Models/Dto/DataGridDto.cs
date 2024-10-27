namespace CentaureaTest.Models.Dto
{

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
    public sealed class DataGridDto
    {
        public string Name { get; set; }

        public DataGridSignatureDto Signature { get; set; }

        public List<DataGridRow> Rows { get; set; }

        public DataGrid ToDataGrid()
        {
            var ret = new DataGrid();
            ret.Name = Name;
            ret.Signature = Signature.ToDataGridSignature();
            ret.Rows = Rows;
            return ret;
        }
    }

}