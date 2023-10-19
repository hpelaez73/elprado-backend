namespace ElPrado.Core
{
    public class Resultados
    {
        public bool HayError { get; internal set; }
        public bool EstaOK
        {
            get { return !HayError; }
            internal set { HayError = !value; }
        }
        public List<string> Errores { get; internal set; }

        public Resultados()
        {
            HayError = false;
            Errores = new List<string>();
        }

        public Resultados(string mensajeError)
        {
            Errores = new List<string>();
            Agregar(mensajeError);
        }

        public Resultados(List<string> errores)
        {
            HayError = true;
            Errores = errores;
        }

        public void Agregar(string mensajeError)
        {
            HayError = true;
            Errores.Add(mensajeError);
        }

        public void Agregar(Resultados resultado)
        {
            if (resultado is null)
            {
                throw new ArgumentNullException(nameof(resultado));
            }

            if (resultado.HayError)
            {
                HayError = true;
                Errores.AddRange(resultado.Errores);
            }
        }
    }

    public class Resultados<T> : Resultados
    {
        public T? Valor { get; set; }

        public Resultados() : base()
        {
        }

        public Resultados(string mensajeError) : base(mensajeError)
        {
        }
    }
}
