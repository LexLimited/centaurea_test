using System.Text;
using CentaureaTest.Data;

namespace CentaureaTest.Models
{

    public sealed class DataGrid
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DataGridSignature Signature { get; set; }

        public List<DataGridRow> Rows { get; set; }

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

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Id: {Id}, Name: {Name}");
            
            foreach (var field in Signature.Fields)
            {
                builder.AppendLine($"Field: {field.ToString()}");
            }

            foreach (var row in Rows)
            {
                builder.AppendLine($"Row: {row.ToString()}");
            }

            return builder.ToString();
        }
    }

    public static class DataGridExtension
    {
        public static DataGrid? GetDataGrid(this ApplicationDbContext dbContext, int id)
        {
            var gridSignature = (
                from grid in dbContext.Grids
                join field in dbContext.Fields on grid.Id equals field.GridId
                where grid.Id == id
                select new { Grid = grid, Field = field }
            ).ToList();

            if (!gridSignature.Any())
            {
                return null;
            }

            var firstGrid = gridSignature.First().Grid;
            var fields = new List<DataGridFieldSignature>();

            foreach (var item in gridSignature)
            {
                // Map each field based on type
                var field = item.Field;
                DataGridFieldSignature fieldSignature = field.Type switch
                {
                    DataGridValueType.Regex => new DataGridRegexFieldSignature(
                        field.Name,
                        ((RegexFieldsTable)field)?.RegexPattern ?? throw new Exception("Bad regex field"),
                        field.Order
                    ),
                    DataGridValueType.Ref => new DataGridRefFieldSignature(
                        field.Name,
                        ((RefFieldsTable)field)?.ReferencedGridId ?? throw new Exception("Bad ref field"),
                        field.Order
                    ),
                    DataGridValueType.SingleSelect => new DataGridSingleSelectFieldSignature(
                        field.Name,
                        ((SingleSelectFieldsTable)field)?.OptionTableId ?? throw new Exception("Bad single select field"),
                        field.Order
                    ),
                    DataGridValueType.MultiSelect => new DataGridMultiSelectFieldSignature(
                        field.Name,
                        ((MultiSelectFieldsTable)field)?.OptionTableId ?? throw new Exception("Bad multi select field"),
                        field.Order
                    ),
                    _ => new DataGridFieldSignature(field.Name, field.Type, field.Order) // Default for basic fields
                };

                fields.Add(fieldSignature);
            }

            var signature = new DataGridSignature(fields);
            var rows = new List<DataGridRow>();

            return new DataGrid(id, firstGrid.Name, signature, rows);
        }
    }

}