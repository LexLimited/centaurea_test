using CentaureaTest.Data;

namespace CentaureaTest.Models
{

    public sealed class DataGrid
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public DataGridSignature Signature { get; private set; }

        public List<DataGridRow> Rows { get; private set; }

        public DataGrid()
        {
            Id = 0;
            Name = string.Empty;
            Signature = new DataGridSignature();
            Rows = new List<DataGridRow>();
        }

        public DataGrid(int id, string name, DataGridSignature signature, IEnumerable<DataGridRow> rows)
        {
            Id = id;
            Name = name;
            Signature = signature;
            Rows = rows.ToList();
        }
    }

    public static class DataGridExtension
    {
        public static DataGrid GetDataGrid(this ApplicationDbContext dbContext, int id)
        {
            var fieldsQuery =
                from grid in dbContext.Grids
                join field in dbContext.Fields
                on grid.Id equals field.GridId
                where grid.Id == id
                select new DataGridFieldSignature
                {
                    Name = "unnamed",
                    Type = field.Type,
                };

            var signature = new DataGridSignature(fieldsQuery.AsEnumerable());
            var rows = new List<DataGridRow>();

            return new DataGrid(id, "unnamed", signature, rows);
        }
    }

}