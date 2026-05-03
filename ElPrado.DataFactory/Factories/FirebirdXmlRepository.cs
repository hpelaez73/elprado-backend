using Dapper;
using ElPrado.DataFactory.Interfaces;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Data;
using System.Xml.Linq;

namespace ElPrado.DataFactory.Factories
{
    public class FirebirdXmlRepository : IXmlRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FirebirdXmlRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using var conn = _connectionFactory.Create();
            const string sql = "SELECT XML_DATA FROM DP_KEYS";

            var xmlStrings = conn.Query<string>(sql);

            return xmlStrings.Select(XElement.Parse).ToList().AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var conn = _connectionFactory.Create();

            const string sql = @"
            INSERT INTO DP_KEYS (FRIENDLY_NAME, XML_DATA)
            VALUES (@FriendlyName, @XmlData)";

            conn.Execute(sql, new
            {
                FriendlyName = friendlyName,
                XmlData = element.ToString(SaveOptions.DisableFormatting)
            });
        }
    }
}
