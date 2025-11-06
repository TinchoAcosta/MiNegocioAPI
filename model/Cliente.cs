namespace MiNegocioAPI.model
{
    public class Cliente
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string dni { get; set; }
        public string? domicilio { get; set; }
        public ICollection<Turno> turnos { get; set; } = new List<Turno>();

        public Cliente() { }
        public Cliente(int id, string nombre, string apellido, string telefono, string email, string dni, string domicilio)
        {
            this.id = id;
            this.nombre = nombre;
            this.apellido = apellido;
            this.telefono = telefono;
            this.email = email;
            this.dni = dni;
            this.domicilio = domicilio;
        }
    }
}