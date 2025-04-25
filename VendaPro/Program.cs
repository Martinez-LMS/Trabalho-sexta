using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        List<Pedido> todosOsPedidos = new List<Pedido>();
        List<double> totaisComDesconto = new List<double>(); // armazena o total com desconto para cada pedido

        bool continuar = true;

        while (continuar)
        {
            // Entrada de dados do cliente
            Console.WriteLine("\n--- Cadastro de Cliente ---");
            Console.Write("Nome do cliente: ");
            string nomeCliente = Console.ReadLine();

            Console.Write("Email do cliente: ");
            string emailCliente = Console.ReadLine();

            Cliente cliente = new Cliente(nomeCliente, emailCliente);

            // Entrada de dados do pedido
            Console.Write("Número do pedido: ");
            int numeroPedido = int.Parse(Console.ReadLine());

            Pedido pedido = new Pedido(numeroPedido, cliente);
            double totalComDesconto = 0;

            // Entrada de produtos (quantidade indefinida)
            Console.WriteLine("\n--- Cadastro de Produtos ---");
            while (true)
            {
                Console.Write("\nNome do produto (pressione Enter para finalizar): ");
                string nomeProduto = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nomeProduto))
                {
                    break; // Finaliza cadastro de produtos
                }

                Console.Write("Preço: ");
                double precoProduto = double.Parse(Console.ReadLine());

                Console.Write("Desconto (%): ");
                double percentualDesconto = double.Parse(Console.ReadLine());

                Produto produto = new Produto(nomeProduto, precoProduto);
                pedido.AdicionarProduto(produto);

                if (percentualDesconto > 0)
                {
                    double precoComDesconto = produto.CalcularDesconto(percentualDesconto);
                    totalComDesconto += precoComDesconto;
                    Console.WriteLine($"Preço com {percentualDesconto}% de desconto: R$ {precoComDesconto:F2}");
                }
                else
                {
                    totalComDesconto += precoProduto;
                    Console.WriteLine("Produto sem desconto.");
                }
            }

            // Cliente faz pedido
            cliente.FazerPedido(pedido);

            // Totais
            double totalSemDesconto = pedido.CalcularTotal();
            Console.WriteLine($"\nTotal do pedido (sem desconto): R$ {totalSemDesconto:F2}");
            Console.WriteLine($"Total do pedido (com desconto): R$ {totalComDesconto:F2}");

            todosOsPedidos.Add(pedido);
            totaisComDesconto.Add(totalComDesconto);

            // Verifica se deseja continuar
            Console.WriteLine("\nDeseja adicionar outro pedido para outro cliente? (s/n)");
            string resposta = Console.ReadLine();
            continuar = resposta.ToLower() == "s";
        }

        // Exibição dos pedidos feitos
        Console.WriteLine("\n===== RESUMO DOS PEDIDOS =====");
        for (int i = 0; i < todosOsPedidos.Count; i++)
        {
            var pedido = todosOsPedidos[i];
            var totalComDesconto = totaisComDesconto[i];

            Console.WriteLine($"\nPedido Nº {pedido.NumeroPedido}");
            Console.WriteLine($"Cliente: {pedido.Cliente.Nome} - {pedido.Cliente.Email}");
            Console.WriteLine("Produtos:");

            foreach (var produto in pedido.Produtos)
            {
                Console.WriteLine($"- {produto.Nome} - R$ {produto.Preco}");
            }

            double totalSemDesconto = pedido.CalcularTotal();
            Console.WriteLine($"Total sem desconto: R$ {totalSemDesconto:F2}");
            Console.WriteLine($"Total com desconto: R$ {totalComDesconto:F2}");
        }

        Console.WriteLine("\nFim do programa.");
    }
}
