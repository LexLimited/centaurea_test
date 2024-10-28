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

        public List<DataGridRowDto> Rows { get; set; }

        public DataGrid ToDataGrid()
        {
            var ret = new DataGrid
            {
                Name = Name,
                Signature = Signature.ToDataGridSignature(),
                Rows = Rows.ToDataGridRow().ToList()
            };
            return ret;
        }
    }

}