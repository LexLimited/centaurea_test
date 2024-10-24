using Microsoft.Net.Http.Headers;

namespace CentaureaTest.Models
{

    public sealed class DataGrid
    {
        public DataGridSignature Signature { get; private set; }

        public DataGrid(DataGridSignature signature)
        {
            Signature = signature;
        }
    }

}