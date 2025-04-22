public class Produto
{
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
        {
            throw new ArgumentException("Percentual de desconto deve ser entre 0 e 100.");
        }
        return Preco - (Preco * percentual / 100);
    }
}
