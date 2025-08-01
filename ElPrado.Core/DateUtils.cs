namespace ElPrado.Core
{
    public static class DateUtils
    {
        public static DateTime FinDeMes(DateTime fecha)
        {
            // Devuelve el último día del mes de la fecha proporcionada
            return new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
        }
        public static DateTime FinDeMes()
        {
            // Devuelve el último día del mes en curso
            return FinDeMes(DateTime.Today);
        }

    }
}
