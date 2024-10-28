namespace CentaureaTest.Models.Dto
{

    /// <summary>
    /// This class exist because I couldn't figure out how to deeserialize
    /// polymorphic objects.
    /// Not used internally, only used to receive jsons.
    /// </summary>
    public sealed class CreateDataGridSignatureDto
    {
        public List<CreateDataGridFieldSignatureDto> Fields { get; set; }
    }

    public static class CreateDataGridSignatureDtoExtension
    {
        public static IEnumerable<FieldsTable> ToFieldsTables(this List<CreateDataGridFieldSignatureDto> fieldSignatureDtos)
        {
            return fieldSignatureDtos.Select(sig => sig.ToFieldsTable());
        }
    }

}