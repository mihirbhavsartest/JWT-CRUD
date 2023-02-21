using Npgsql;

namespace CRUD_WO_ORM.DataSource
{
    public class DataSource
    {
        public NpgsqlDataSource source;
        public DataSource()
        {
            var connectionString = "Server=localhost;Port=5432;Database=User;User Id=postgres;Password=12345678;";
            source = NpgsqlDataSource.Create(connectionString);
        }
        
        
    }
}
