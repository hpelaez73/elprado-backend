using System.Data;

namespace Afip.Data.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
