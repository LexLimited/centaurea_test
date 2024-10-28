namespace CentaureaTest.Models.Dto
{

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
    public sealed class CreateDataGridDto
    {
        public string Name { get; set; }
        public CreateDataGridSignatureDto Signature { get; set; }
        public List<DataGridRowDto> Rows { get; set; }

        public GridsTable ToGridsTable()
        {
            return new GridsTable{ Name = Name };
        }
    }

}