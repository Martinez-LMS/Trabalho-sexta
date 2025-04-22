public class Program
{
    public static void Main()
    {
        // Criando produtos
        Produto produto1 = new Produto("Camiseta", 100);
        Produto produto2 = new Produto("Calça", 150);

        // Criando cliente
        Cliente cliente = new Cliente("João", "joao@email.com");

        // Criando pedido e adicionando produtos
        Pedido pedido = new Pedido(1, cliente);
        pedido.AdicionarProduto(produto1);
        pedido.AdicionarProduto(produto2);

        // Cliente faz pedido
        cliente.FazerPedido(pedido);

        // Calculando total do pedido
        double total = pedido.CalcularTotal();
        Console.WriteLine($"Total do pedido: {total}");

        // Aplicando desconto de 10% no produto
        double precoComDesconto = produto1.CalcularDesconto(10);
        Console.WriteLine($"Preço do produto com desconto: {precoComDesconto}");
    }
}
