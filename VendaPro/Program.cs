// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using VendaPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();     
builder.Services.AddSwaggerGen();

var produtos = new List<Produto>();
var pedidos = new List<Pedido>();
var clientes = new List<Cliente>();
int proximoIdProduto = 1;
int proximoIdPedido = 1;

bool NomeEhValido(string nome) => !string.IsNullOrWhiteSpace(nome) && !nome.Any(char.IsDigit);
bool EmailEhValido(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
bool PrecoEhValido(decimal preco) => preco >= 0;
bool QuantidadeEhValida(int quantidade) => quantidade >= 0;

var app = builder.Build();

// Produtos
app.MapGet("/produtos/listar", () => produtos)
    .WithName("ListarProdutos");

app.MapGet("/produtos/obter/{id}", (int id) =>
{
    var produto = produtos.FirstOrDefault(x => x.Id == id);
    return produto != null ? Results.Ok(produto) : Results.NotFound("Produto não encontrado");
})
    .WithName("ObterProdutoPorId");

app.MapPost("/produtos/adicionar", (Produto novoProduto) =>
{
    if (!PrecoEhValido(novoProduto.Preco)) return Results.BadRequest("Preço do produto não pode ser negativo.");
    if (!QuantidadeEhValida(novoProduto.Quantidade)) return Results.BadRequest("Quantidade do produto não pode ser negativo.");
    

    if (string.IsNullOrWhiteSpace(novoProduto.Nome))
        return Results.BadRequest("Nome do produto inválido");

    if (novoProduto.Preco < 0 || novoProduto.Quantidade < 0)
        return Results.BadRequest("Preço e quantidade devem ser positivos");

    novoProduto.Id = proximoIdProduto++;
    produtos.Add(novoProduto);
    return Results.Created($"/produtos/{novoProduto.Id}", novoProduto);
})
    .WithName("AdicionarProduto");

app.MapPut("/produtos/atualizar/{id}", (int id, Produto produtoAtualizado) =>
{
    var produtoExistente = produtos.FirstOrDefault(p => p.Id == id);
    if (!PrecoEhValido(produtoAtualizado.Preco)) return Results.BadRequest("Preço do produto não pode ser negativo.");
    if (produtoExistente == null) return Results.NotFound("Produto não encontrado");

    if (string.IsNullOrWhiteSpace(produtoAtualizado.Nome) || produtoAtualizado.Nome.Any(char.IsDigit))
        return Results.BadRequest("Nome inválido");

    if (produtoAtualizado.Preco < 0)
        return Results.BadRequest("Preço inválido");

    produtoExistente.Nome = produtoAtualizado.Nome;
    produtoExistente.Preco = produtoAtualizado.Preco;
    return Results.Ok(produtoExistente);
})
    .WithName("AtualizarProduto");

app.MapDelete("/produtos/deletar/{id}", (int id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound("Produto não encontrado");

    produtos.Remove(produto);
    return Results.Ok($"Produto {produto.Nome} removido.");
})
    .WithName("DeletarProduto");

// Clientes
app.MapPost("/clientes/adicionar", (Cliente novoCliente) =>
{
    if (!NomeEhValido(novoCliente.Nome)) return Results.BadRequest("Nome do cliente inválido.");
    if (!EmailEhValido(novoCliente.Email)) return Results.BadRequest("Email inválido.");
    if (string.IsNullOrWhiteSpace(novoCliente.Nome) || novoCliente.Nome.Any(char.IsDigit))
        return Results.BadRequest("Nome inválido");

    if (!novoCliente.Email.Contains("@"))
        return Results.BadRequest("Email inválido");

    novoCliente.Id = clientes.Count > 0 ? clientes.Max(c => c.Id) + 1 : 1;
    clientes.Add(novoCliente);
    return Results.Created("/clientes", novoCliente);
})
    .WithName("AdicionarCliente");

app.MapGet("/clientes/listar", () => clientes)
    .WithName("ListarClientes");

app.MapGet("/clientes/obter/{email}", (string email) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Email == email);
    return cliente != null ? Results.Ok(cliente) : Results.NotFound("Cliente não encontrado");
})
    .WithName("ObterClientePorEmail");

app.MapDelete("/clientes/deletar/{email}", (string email) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Email == email);
    if (cliente == null) return Results.NotFound("Cliente não encontrado");

    clientes.Remove(cliente);
    return Results.Ok($"Cliente {cliente.Nome} removido.");
})
    .WithName("DeletarCliente");

// Pedidos
app.MapPost("/pedidos/criar", (int clienteId) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Id == clienteId);
    if (cliente == null)
        return Results.BadRequest("Cliente não encontrado.");

    var novoPedido = new Pedido
    {
        Id = proximoIdPedido++,
        ClienteId = clienteId,
        DataPedido = DateTime.Now,
        Itens = new List<ItemPedido>(),
        Total = 0
    };

    pedidos.Add(novoPedido);
    return Results.Created($"/pedidos/{novoPedido.Id}", novoPedido);
})
.WithName("CriarPedido");

app.MapPost("/pedidos/{pedidoId}/adicionar-item", (int pedidoId, ItemPedido novoItem) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);
    if (pedido == null)
        return Results.NotFound("Pedido não encontrado.");

    var produto = produtos.FirstOrDefault(p => p.Id == novoItem.ProdutoId);
    if (produto == null)
        return Results.NotFound("Produto não encontrado.");

    if (novoItem.Quantidade <= 0)
        return Results.BadRequest("A quantidade deve ser maior que zero.");

    if (produto.Quantidade < novoItem.Quantidade)
        return Results.BadRequest($"Estoque insuficiente para o produto {produto.Nome}.");

    produto.Quantidade -= novoItem.Quantidade;

    var itemExistente = pedido.Itens.FirstOrDefault(i => i.ProdutoId == novoItem.ProdutoId);
    if (itemExistente != null)
    {
        itemExistente.Quantidade += novoItem.Quantidade;
    }
    else
    {
        pedido.Itens.Add(new ItemPedido { ProdutoId = novoItem.ProdutoId, Quantidade = novoItem.Quantidade });
    }

    pedido.Total = pedido.Itens.Sum(i =>
    {
        var prod = produtos.FirstOrDefault(p => p.Id == i.ProdutoId);
        return (prod?.Preco ?? 0) * i.Quantidade;
    });

    return Results.Ok(pedido);
})
.WithName("AdicionarItemAoPedido");

app.MapGet("/pedidos/listar", () => pedidos)
    .WithName("ListarPedidos");

app.MapGet("/pedidos/obter/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");

    return Results.Ok(pedido);
});

app.MapDelete("/pedidos/deletar/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");

    pedidos.Remove(pedido);
    return Results.Ok($"Pedido #{pedido.Id} removido.");
})
.WithName("DeletarPedido");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();