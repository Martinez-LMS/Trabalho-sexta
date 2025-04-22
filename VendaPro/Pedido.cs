using System;
using System.Collections.Generic;

public class Pedido
{
    public int NumeroPedido { get; set; }
    public Cliente Cliente { get; set; }
    public List<Produto> Produtos { get; set; }

    public Pedido(int numeroPedido, Cliente cliente)
    {
        NumeroPedido = numeroPedido;
        Cliente = cliente;
        Produtos = new List<Produto>();
    }

    public void AdicionarProduto(Produto produto)
    {
        Produtos.Add(produto);
    }

    public double CalcularTotal()
    {
        double total = 0;
        foreach (var produto in Produtos)
        {
            total += produto.Preco;
        }
        return total;
    }
}
