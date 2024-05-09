using Npgsql;

namespace DAL;

public class DbContext:IDisposable
{
    private readonly string ConnectionString = "Host=localhost;Username=postgres;Database=seeker";

    public DbContext()
    {
        Connection = new NpgsqlConnection(ConnectionString);
        Connection.Open();
    }

    public NpgsqlConnection Connection { get; }

    public void Dispose()
    {
        Connection?.Close();
        Connection?.Dispose();
    }
}