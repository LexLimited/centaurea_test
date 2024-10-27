namespace CentaureaTest.Models.Dto
{

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