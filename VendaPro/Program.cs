using System.Text.Json;
using System.IO;
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

// Carregar dados
var produtos = CarregarDados<Produto>("produtos.json");
var clientes = CarregarDados<Cliente>("clientes.json");
var pedidos = CarregarDados<Pedido>("pedidos.json");

// Identificadores automáticos
int proximoIdProduto = produtos.Any() ? produtos.Max(p => p.Id) + 1 : 1;
int proximoIdPedido = pedidos.Any() ? pedidos.Max(p => p.Id) + 1 : 1;

// Funções auxiliares
void SalvarDados<T>(string caminho, List<T> lista)
{
    var json = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(caminho, json);
}

List<T> CarregarDados<T>(string caminho)
{
    if (File.Exists(caminho))
    {
        var json = File.ReadAllText(caminho);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }
    return new List<T>();
}

bool NomeEhValido(string nome) => !string.IsNullOrWhiteSpace(nome) && !nome.Any(char.IsDigit);
bool EmailEhValido(string email) => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
bool PrecoEhValido(decimal preco) => preco >= 0;
bool QuantidadeEhValida(int quantidade) => quantidade >= 0;

var app = builder.Build();

// --------------------- PRODUTOS ---------------------
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
    if (!PrecoEhValido(novoProduto.Preco)) return Results.BadRequest("Preço inválido.");
    if (!QuantidadeEhValida(novoProduto.Quantidade)) return Results.BadRequest("Quantidade inválida.");
    if (string.IsNullOrWhiteSpace(novoProduto.Nome)) return Results.BadRequest("Nome inválido.");

    novoProduto.Id = proximoIdProduto++;
    produtos.Add(novoProduto);
    SalvarDados("produtos.json", produtos);
    return Results.Created($"/produtos/{novoProduto.Id}", novoProduto);
})
.WithName("AdicionarProduto");

app.MapPut("/produtos/atualizar/{id}", (int id, Produto produtoAtualizado) =>
{
    var produtoExistente = produtos.FirstOrDefault(p => p.Id == id);
    if (produtoExistente == null) return Results.NotFound("Produto não encontrado");
    if (!PrecoEhValido(produtoAtualizado.Preco)) return Results.BadRequest("Preço inválido.");
    if (!NomeEhValido(produtoAtualizado.Nome)) return Results.BadRequest("Nome inválido.");

    produtoExistente.Nome = produtoAtualizado.Nome;
    produtoExistente.Preco = produtoAtualizado.Preco;
    SalvarDados("produtos.json", produtos);
    return Results.Ok(produtoExistente);
})
.WithName("AtualizarProduto");

app.MapDelete("/produtos/deletar/{id}", (int id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound("Produto não encontrado");

    produtos.Remove(produto);
    SalvarDados("produtos.json", produtos);
    return Results.Ok($"Produto {produto.Nome} removido.");
})
.WithName("DeletarProduto");

// --------------------- CLIENTES ---------------------
app.MapPost("/clientes/adicionar", (Cliente novoCliente) =>
{
    if (!NomeEhValido(novoCliente.Nome)) return Results.BadRequest("Nome inválido.");
    if (!EmailEhValido(novoCliente.Email)) return Results.BadRequest("Email inválido.");

    novoCliente.Id = clientes.Any() ? clientes.Max(c => c.Id) + 1 : 1;
    clientes.Add(novoCliente);
    SalvarDados("clientes.json", clientes);
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
    SalvarDados("clientes.json", clientes);
    return Results.Ok($"Cliente {cliente.Nome} removido.");
})
.WithName("DeletarCliente");

// --------------------- PEDIDOS ---------------------
app.MapPost("/pedidos/criar", (int clienteId) =>
{
    var cliente = clientes.FirstOrDefault(c => c.Id == clienteId);
    if (cliente == null) return Results.BadRequest("Cliente não encontrado.");

    var novoPedido = new Pedido
    {
        Id = proximoIdPedido++,
        ClienteId = clienteId,
        DataPedido = DateTime.Now,
        Itens = new List<ItemPedido>(),
        Total = 0
    };

    pedidos.Add(novoPedido);
    SalvarDados("pedidos.json", pedidos);
    return Results.Created($"/pedidos/{novoPedido.Id}", novoPedido);
})
.WithName("CriarPedido");

app.MapPost("/pedidos/{pedidoId}/adicionar-item", (int pedidoId, ItemPedido novoItem) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);
    if (pedido == null) return Results.NotFound("Pedido não encontrado.");

    var produto = produtos.FirstOrDefault(p => p.Id == novoItem.ProdutoId);
    if (produto == null) return Results.NotFound("Produto não encontrado.");
    if (novoItem.Quantidade <= 0) return Results.BadRequest("Quantidade deve ser maior que zero.");
    if (produto.Quantidade < novoItem.Quantidade) return Results.BadRequest("Estoque insuficiente.");

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

    SalvarDados("produtos.json", produtos);
    SalvarDados("pedidos.json", pedidos);
    return Results.Ok(pedido);
})
.WithName("AdicionarItemAoPedido");

app.MapGet("/pedidos/listar", () => pedidos)
    .WithName("ListarPedidos");

app.MapGet("/pedidos/obter/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    return pedido != null ? Results.Ok(pedido) : Results.NotFound("Pedido não encontrado");
})
.WithName("ObterPedido");

app.MapDelete("/pedidos/deletar/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");

    pedidos.Remove(pedido);
    SalvarDados("pedidos.json", pedidos);
    return Results.Ok($"Pedido #{pedido.Id} removido.");
})
.WithName("DeletarPedido");

// --------------------- MIDDLEWARE ---------------------
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
