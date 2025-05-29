public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public double Preco { get; set; }

    public Produto(string nome, double preco)
    {
        Nome = nome;
        Preco = preco;
    }

    public double CalcularDesconto(double percentual)
    {
        if (percentual < 0 || percentual > 100)
            throw new ArgumentException("Percentual inválido.");
        return Preco - (Preco * percentual / 100);
    }
}