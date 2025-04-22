public class Cliente
{
    public string Nome { get; set; }
    public string Email { get; set; }

    public Cliente(string nome, string email)
    {
        Nome = nome;
        Email = email;
    }

    public void FazerPedido(Pedido pedido)
    {
        Console.WriteLine($"{Nome} fez um pedido com {pedido.Produtos.Count} produtos.");
    }
}
