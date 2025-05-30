using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using VendaPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

var app = builder.Build();

// Funções auxiliares de validação
bool NomeEhValido(string nome) => !string.IsNullOrWhiteSpace(nome) && !nome.Any(char.IsDigit);
bool EmailEhValido(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
bool PrecoEhValido(decimal preco) => preco >= 0;
bool QuantidadeEhValida(int quantidade) => quantidade >= 0;

// **Produtos***********************************

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


    novoProduto.Id = proximoIdProduto++;
    produtos.Add(novoProduto);
    return Results.Created($"/produtos/{novoProduto.Id}", novoProduto);
})
    .WithName("AdicionarProduto");

app.MapPut("/produtos/atualizar/{id}", (int id, Produto produtoAtualizado) =>
{
    var produtoExistente = produtos.FirstOrDefault(p => p.Id == id);
    if (produtoExistente == null) return Results.NotFound("Produto não encontrado");

    if (!NomeEhValido(produtoAtualizado.Nome)) return Results.BadRequest("Nome do produto inválido.");
    if (!PrecoEhValido(produtoAtualizado.Preco)) return Results.BadRequest("Preço do produto não pode ser negativo.");

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

// **Clientes*****************************

app.MapPost("/clientes/adicionar", (Cliente novoCliente) =>
{
    if (!NomeEhValido(novoCliente.Nome)) return Results.BadRequest("Nome do cliente inválido.");
    if (!EmailEhValido(novoCliente.Email)) return Results.BadRequest("Email inválido.");

    clientes.Add(novoCliente);
    return Results.Created("/clientes", novoCliente);
})
    .WithName("AdicionarCliente");

app.MapGet("/clientes/listar", () => clientes)
    .WithName("ListarClientes");

app.MapGet("/clientes/obter/{clienteId}", (int id) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Id == id);
    return cliente != null ? Results.Ok(cliente) : Results.NotFound("Cliente não encontrado");
})
    .WithName("ObterClientePorId");

app.MapDelete("/clientes/deletar/{clienteId}", (int id) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Id == id);
    if (cliente == null) return Results.NotFound("Cliente não encontrado");

    clientes.Remove(cliente);
    return Results.Ok($"Cliente {cliente.Nome} removido.");
})
    .WithName("DeletarCliente");

// **Pedidos******************************

app.MapPost("/pedidos/adicionar", (Pedido novoPedido) =>
{
    if (novoPedido.Produtos.Any(item => !QuantidadeEhValida(item.Quantidade)))
        return Results.BadRequest("Quantidade dos produtos no pedido não pode ser negativa.");

    novoPedido.Id = proximoIdPedido++;
    novoPedido.CalcularTotal();
    pedidos.Add(novoPedido);
    return Results.Created($"/pedidos/{novoPedido.Id}", novoPedido);
})
    .WithName("AdicionarPedido");

app.MapGet("/pedidos/listar", () => pedidos)
    .WithName("ListarPedidos");

app.MapGet("/pedidos/obter/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    return pedido != null ? Results.Ok(pedido) : Results.NotFound("Pedido não encontrado");
})
    .WithName("ObterPedidoPorId");

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
