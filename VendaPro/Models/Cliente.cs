namespace VendaPro.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        public Cliente(string nome, string email, int id)
        {
            Nome = nome;
            Email = email;
            Id = Id;

        }
    }
}
