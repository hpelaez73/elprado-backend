using QuestPDF.Infrastructure;

namespace ElPrado.Reports.Configuration
{
    public class DocSettings
    {
        public static void Configurar()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}
