using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using VendaPro.Models;  
using System;
using System.Collections.Generic;
using System.Linq;

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
    novoProduto.Id = proximoIdProduto++;
    produtos.Add(novoProduto);
    return Results.Created($"/produtos/{novoProduto.Id}", novoProduto);
})
    .WithName("AdicionarProduto");

app.MapPut("/produtos/atualizar/{id}", (int id, Produto produtoAtualizado) =>  
{
    var produtoExistente = produtos.FirstOrDefault(p => p.Id == id);
    if (produtoExistente == null) return Results.NotFound("Produto não encontrado");

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

// **Pedidos******************************

app.MapPost("/pedidos/adicionar", (Pedido novoPedido) =>  
{
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
