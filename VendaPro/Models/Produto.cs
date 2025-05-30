public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }

    public Produto() { }

    public Produto(string nome, int quantidade, decimal preco)
    {
        Nome = nome;
        Quantidade = quantidade;
        Preco = preco;
    }
}