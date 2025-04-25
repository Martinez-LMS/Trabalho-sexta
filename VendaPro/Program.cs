using System;
using System.Collections.Generic;

public class Program
{
    public static List<Produto> CatalogoProdutos = new List<Produto>
    {
        new Produto("Arroz 5kg", 19.90), new Produto("Feijão 1kg", 7.49), new Produto("Macarrão 500g", 3.29),
        new Produto("Molho de Tomate", 2.89), new Produto("Leite 1L", 4.59), new Produto("Açúcar 1kg", 3.19),
        new Produto("Café 500g", 12.49), new Produto("Pão de Forma", 6.79), new Produto("Margarina 500g", 5.89),
        new Produto("Ovos - dúzia", 10.99), new Produto("Farinha 1kg", 4.99), new Produto("Óleo 900ml", 7.89),
        new Produto("Sal 1kg", 2.39), new Produto("Detergente", 2.79), new Produto("Sabão em Pó", 9.99),
        new Produto("Desinfetante", 6.49), new Produto("Papel Higiênico", 8.99), new Produto("Shampoo", 11.29),
        new Produto("Condicionador", 11.49), new Produto("Escova de Dente", 3.99), new Produto("Creme Dental", 4.19),
        new Produto("Sabonete", 2.29), new Produto("Desodorante", 9.49), new Produto("Água 1,5L", 2.89),
        new Produto("Refrigerante 2L", 8.49), new Produto("Suco 1L", 6.99), new Produto("Biscoito", 2.99),
        new Produto("Chocolate", 5.49), new Produto("Cereal", 7.29), new Produto("Iogurte", 2.59)
    };

    public static void Main()
    {
        var pedidos = new List<Pedido>();
        var totaisComDesconto = new List<double>();

        while (true)
        {
            Console.Write("\nNome do cliente: ");
            string nome = Console.ReadLine();
            Console.Write("Email do cliente: ");
            string email = Console.ReadLine();
            var cliente = new Cliente(nome, email);

            Console.Write("Número do pedido: ");
            int numero = int.Parse(Console.ReadLine());
            var pedido = new Pedido(numero, cliente);
            double totalDesconto = 0;

            // Mostrar catálogo
            Console.WriteLine("\nCatálogo de Produtos:");
            for (int i = 0; i < CatalogoProdutos.Count; i++)
                Console.WriteLine($"[{i + 1}] {CatalogoProdutos[i].Nome} - R$ {CatalogoProdutos[i].Preco:F2}");

            // Selecionar produtos
            while (true)
            {
                Console.Write("\nID do produto (Enter para finalizar): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                if (int.TryParse(input, out int id) && id >= 1 && id <= CatalogoProdutos.Count)
                {
                    var produto = CatalogoProdutos[id - 1];
                    double desconto = 0;

                    while (true)
                    {
                        Console.Write($"Desconto para {produto.Nome} (%): ");
                        string entradaDesconto = Console.ReadLine();

                        if (double.TryParse(entradaDesconto, out desconto))
                        {
                            try
                            {
                                double precoFinal = desconto > 0 ? produto.CalcularDesconto(desconto) : produto.Preco;
                                totalDesconto += precoFinal;
                                pedido.AdicionarProduto(produto);
                                Console.WriteLine($"Adicionado: {produto.Nome} - R$ {precoFinal:F2}");
                                break;
                            }
                            catch (ArgumentException e)
                            {
                                Console.WriteLine("❌ " + e.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ Entrada inválida. Digite um número válido.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("❌ ID inválido. Tente novamente.");
                }
            }

            cliente.FazerPedido(pedido);
            double totalBruto = pedido.CalcularTotal();

            Console.WriteLine($"\nTotal sem desconto: R$ {totalBruto:F2}");
            Console.WriteLine($"Total com desconto: R$ {totalDesconto:F2}");

            pedidos.Add(pedido);
            totaisComDesconto.Add(totalDesconto);

            Console.Write("\nAdicionar outro pedido? (s/n): ");
            if (Console.ReadLine().ToLower() != "s") break;
        }

        // Resumo
        Console.WriteLine("\n===== RESUMO DOS PEDIDOS =====");
        for (int i = 0; i < pedidos.Count; i++)
        {
            var p = pedidos[i];
            Console.WriteLine($"\nPedido #{p.NumeroPedido} - {p.Cliente.Nome} - {p.Cliente.Email}");
            foreach (var prod in p.Produtos)
                Console.WriteLine($"- {prod.Nome} - R$ {prod.Preco:F2}");

            Console.WriteLine($"Total sem desconto: R$ {p.CalcularTotal():F2}");
            Console.WriteLine($"Total com desconto: R$ {totaisComDesconto[i]:F2}");
        }

        Console.WriteLine("\nFim do programa.");
    }
}