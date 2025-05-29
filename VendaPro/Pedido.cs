 public Pedido(int numeroPedido, Cliente cliente)
    {
        NumeroPedido = numeroPedido;
        Cliente = cliente;
    }

    public void AdicionarProduto(Produto produto)
    {
        Produtos.Add(produto);
    }

    public double CalcularTotal()
    {
        double total = 0;
        foreach (var produto in Produtos)
            total += produto.Preco;
        return total;
    }
}