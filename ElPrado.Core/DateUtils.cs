namespace ElPrado.Core
{
    public static class DateUtils
    {
        public static DateOnly FinDeMes(DateTime fecha)
        {
            // Devuelve el último día del mes de la fecha proporcionada
            return new DateOnly(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
        }
        public static DateOnly FinDeMes()
        {
            // Devuelve el último día del mes en curso
            return FinDeMes(DateTime.Today);
        }

    }
}
