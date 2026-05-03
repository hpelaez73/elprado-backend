using System.Data;

namespace ElPrado.DataFactory.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
