namespace VendaPro.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public List<Produto> Produtos { get; set; } = new List<Produto>();
        public decimal Total { get; set; }

        public Pedido()
        {
            DataPedido = DateTime.Now;
        }

        public void CalcularTotal()
        {
            Total = Produtos.Sum(p => p.Preco * p.Quantidade);
        }
    }
}
