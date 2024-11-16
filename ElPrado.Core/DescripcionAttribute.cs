using System.Xml.Linq;

namespace ElPrado.Core
{
    public class DescripcionAttribute : Attribute
    {
        private string descripcion;

        public string GetDescripcion() => descripcion;

        public DescripcionAttribute(string descripcion)
        {
            this.descripcion = descripcion;
        }
    }
}
