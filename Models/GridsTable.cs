using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Npgsql.Replication;

namespace CentaureaTest.Models
{

    [Table(name: "grids")]
    public sealed class GridsTable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}";
        }
    }

    public static class GridsTableExtension
    {
        public static GridsTable ToGridsTable(this DataGrid grid)
        {
            return new GridsTable{ Id = grid.Id, Name = grid.Name};
        }
    }

}