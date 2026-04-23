using System.Data;

namespace ElPrado.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
