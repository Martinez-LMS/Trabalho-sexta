public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    public decimal Total { get; set; }
}