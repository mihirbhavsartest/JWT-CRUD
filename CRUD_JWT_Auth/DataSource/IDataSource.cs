using Npgsql;

namespace CRUD_JWT_Auth.DataSource
{
    public interface IDataSource
    {
        public NpgsqlDataSource source { get; set; }
    }
}
