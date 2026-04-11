using ElPrado.Core.Configuration;
using ElPrado.Reports.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ElPrado.Reports.Services
{
    public class ReportImageService : IReportImageService
    {
        private readonly string _basePath;
        private readonly IMemoryCache _cache;

        public ReportImageService(IOptions<ImagenSettings> options, IMemoryCache cache)
        {
            _basePath = Path.Combine(options.Value.BasePath, "reportes");
            _cache = cache;
        }

        public byte[] GetLogoEmpresa() => GetCached("logo_empresa.png");
        public byte[] GetLogoArca() => GetCached("logo_arca.png");
        public byte[] GetSelloPagado() => GetCached("sello_pagado.png");

        private byte[] GetCached(string fileName)
        {
            return _cache.GetOrCreate<byte[]>(fileName, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);

                var path = Path.Combine(_basePath, fileName);

                if (!File.Exists(path))
                    throw new FileNotFoundException($"No se encontró {fileName}");

                return File.ReadAllBytes(path);
            })!;
        }
    }
}
