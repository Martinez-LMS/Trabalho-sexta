using System;
using System.Collections.Generic;

public class Program
{
    var pedidos = new List<Pedido>();
var clientes = new List<Cliente>();
int proximoIdProduto = produtos.Max(p => p.Id) + 1;
int proximoIdPedido = 1;

app.MapGet("/produtos", () => produtos);
app.MapGet("/produtos/{id}", (int id) =>
{
    var p = produtos.FirstOrDefault(x => x.Id == id);
    return p != null ? Results.Ok(p) : Results.NotFound("Produto não encontrado");
});
app.MapPost("/produtos", (Produto novo) =>
{
    novo.Id = proximoIdProduto++;
    produtos.Add(novo);
    return Results.Created($"/produtos/{novo.Id}", novo);
});
app.MapPut("/produtos/{id}", (int id, Produto atualizado) =>
{
    var existente = produtos.FirstOrDefault(p => p.Id == id);
    if (existente == null) return Results.NotFound("Produto não encontrado");

    existente.Nome = atualizado.Nome;
    existente.Preco = atualizado.Preco;
    return Results.Ok(existente);
});
app.MapDelete("/produtos/{id}", (int id) =>
{
    var produto = produtos.FirstOrDefault(p => p.Id == id);
    if (produto == null) return Results.NotFound("Produto não encontrado");

    produtos.Remove(produto);
    return Results.Ok($"Produto {produto.Nome} removido.");
});

app.MapPost("/clientes", (Cliente cliente) =>
{
    clientes.Add(cliente);
    return Results.Created("/clientes", cliente);
});

app.MapGet("/clientes", () => clientes);

app.MapPost("/pedidos", (Pedido pedido) =>
{
    pedido.Id = proximoIdPedido++;
    pedidos.Add(pedido);
    return Results.Created($"/pedidos/{pedido.Id}", pedido);
});

app.MapGet("/pedidos", () => pedidos);

app.MapGet("/pedidos/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    return pedido != null ? Results.Ok(pedido) : Results.NotFound("Pedido não encontrado");
});

app.MapDelete("/pedidos/{id}", (int id) =>
{
    var pedido = pedidos.FirstOrDefault(p => p.Id == id);
    if (pedido == null) return Results.NotFound("Pedido não encontrado");
    pedidos.Remove(pedido);
    return Results.Ok($"Pedido #{pedido.Id} removido.");
});

app.Run();
    }
