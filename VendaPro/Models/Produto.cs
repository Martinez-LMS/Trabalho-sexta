namespace VendaPro.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public double Preco { get; set; }

        public Produto() { }

        public Produto(string nome, int quantidade, double preco)
        {
            Nome = nome;
            Quantidade = quantidade;
            Preco = preco;
        }

        public double CalcularDesconto(double percentual)
        {
            if (percentual < 0 || percentual > 100)
                throw new ArgumentException("Percentual inválido.");
            return Preco - (Preco * percentual / 100);
        }
    }
}
