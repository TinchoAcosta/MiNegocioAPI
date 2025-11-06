namespace MiNegocioAPI.model
{
    public class Usuario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string clave { get; set; }
        public string telefono { get; set; }
        public string dni { get; set; }
        public ICollection<ServicioPropio> servicioPropios { get; set; } = new List<ServicioPropio>();

        public Usuario() { }
        public Usuario(int id, string nombre, string apellido, string email, string clave, string telefono, string dni)
        {
            this.id = id;
            this.nombre = nombre;
            this.apellido = apellido;
            this.email = email;
            this.clave = clave;
            this.telefono = telefono;
            this.dni = dni;
        }
    }
}