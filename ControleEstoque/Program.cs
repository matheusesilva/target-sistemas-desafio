using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main()
    {
        // Carrega os dados do estoque
        var json = File.ReadAllText("estoque.json");
        var opcoesJson = new JsonSerializerOptions
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          PropertyNameCaseInsensitive = true
        };
        var dados = JsonSerializer.Deserialize<Dictionary<string, List<Produto>>>(json, opcoesJson)!;
        var produtos = dados["estoque"]!;
        
        // Carrega ou cria o arquivo de movimentações
        var movimentacoes = CarregarMovimentacoes();
        
        Console.WriteLine("---- SISTEMA DE CONTROLE DE ESTOQUE ----\n");
        
        // Pergunta o tipo de operação
        Console.WriteLine("Selecione o tipo de operação:");
        Console.WriteLine("1 - Entrada (Adicionar estoque)");
        Console.WriteLine("2 - Saída (Remover estoque)");
        Console.Write("Opção: ");
        
        var opcao = Console.ReadLine();
        var isEntrada = opcao == "1";
        
        if (opcao != "1" && opcao != "2")
        {
            Console.WriteLine("Opção inválida!");
            return;
        }
        
        // Lista os produtos disponíveis
        Console.WriteLine("\nProdutos disponíveis:");
        for (int i = 0; i < produtos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {produtos[i].DescricaoProduto} (Estoque: {produtos[i].Estoque})");
        }
        
        // Seleciona o produto
        Console.Write("\nSelecione o produto (número): ");
        if (!int.TryParse(Console.ReadLine(), out int produtoIndex) || produtoIndex < 1 || produtoIndex > produtos.Count)
        {
            Console.WriteLine("Produto inválido!");
            return;
        }
        
        var produtoSelecionado = produtos[produtoIndex - 1];
        var estoqueAnterior = produtoSelecionado.Estoque;
        
        Console.WriteLine($"\nProduto selecionado: {produtoSelecionado.DescricaoProduto}");
        Console.WriteLine($"Estoque atual: {estoqueAnterior}");
        
        // Insere a quantidade
        Console.Write($"\nQuantidade para {(isEntrada ? "ENTRADA" : "SAÍDA")}: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }
        
        // Validação para saída
        if (!isEntrada && quantidade > produtoSelecionado.Estoque)
        {
            Console.WriteLine($"Erro: Quantidade solicitada ({quantidade}) maior que estoque disponível ({produtoSelecionado.Estoque})!");
            return;
        }
        
        // Calcula o novo estoque para exibição
        var novoEstoque = isEntrada ? estoqueAnterior + quantidade : estoqueAnterior - quantidade;
        
        // ETAPA DE CONFIRMAÇÃO
        Console.WriteLine("\n---- CONFIRMAÇÃO DA MOVIMENTAÇÃO ----");
        Console.WriteLine($"Produto: {produtoSelecionado.DescricaoProduto}");
        Console.WriteLine($"Operação: {(isEntrada ? "ENTRADA" : "SAÍDA")}");
        Console.WriteLine($"Quantidade: {quantidade}");
        Console.WriteLine($"Estoque atual: {estoqueAnterior}");
        Console.WriteLine($"Novo estoque: {novoEstoque}");
        Console.Write("\nConfirmar esta movimentação? (S/N): ");
        
        var confirmacao = Console.ReadLine()?.ToUpper();
        if (confirmacao != "S" && confirmacao != "SIM")
        {
            Console.WriteLine("Movimentação cancelada pelo usuário.");
            return;
        }
        
        // Gera ID numérico sequencial
        var idMovimentacao = movimentacoes.Count > 0 ? movimentacoes[movimentacoes.Count - 1].Id + 1 : 1;
        
        // Atualiza o estoque
        if (isEntrada)
        {
            produtoSelecionado.Estoque += quantidade;
        }
        else
        {
            produtoSelecionado.Estoque -= quantidade;
        }
        
        // Cria a movimentação
        var movimentacao = new Movimentacao
        {
            Id = idMovimentacao,
            Data = DateTime.Now,
            Tipo = isEntrada ? "ENTRADA" : "SAIDA",
            CodigoProduto = produtoSelecionado.CodigoProduto,
            DescricaoProduto = produtoSelecionado.DescricaoProduto,
            Quantidade = quantidade,
            EstoqueAnterior = estoqueAnterior,
            EstoqueAtual = produtoSelecionado.Estoque
        };
        
        // Adiciona à lista de movimentações
        movimentacoes.Add(movimentacao);
        
        // Salva as alterações
        SalvarEstoque(dados);
        SalvarMovimentacoes(movimentacoes);
        
        // Exibe o resultado final
        Console.WriteLine("\n---- MOVIMENTAÇÃO REGISTRADA ----");
        Console.WriteLine($"ID: {idMovimentacao}");
        Console.WriteLine($"Produto: {produtoSelecionado.DescricaoProduto}");
        Console.WriteLine($"Operação: {(isEntrada ? "ENTRADA" : "SAÍDA")}");
        Console.WriteLine($"Quantidade: {quantidade}");
        Console.WriteLine($"Estoque anterior: {estoqueAnterior}");
        Console.WriteLine($"Novo estoque: {produtoSelecionado.Estoque}");
        Console.WriteLine($"Data/Hora: {movimentacao.Data:dd/MM/yyyy HH:mm:ss}");
    }
    
    static List<Movimentacao> CarregarMovimentacoes()
    {
        if (!File.Exists("movimentacoes.json"))
            return new List<Movimentacao>();
            
        var json = File.ReadAllText("movimentacoes.json");
        return JsonSerializer.Deserialize<List<Movimentacao>>(json) ?? new List<Movimentacao>();
    }
    
    static void SalvarMovimentacoes(List<Movimentacao> movimentacoes)
    {
        var json = JsonSerializer.Serialize(movimentacoes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("movimentacoes.json", json);
    }
    
    static void SalvarEstoque(Dictionary<string, List<Produto>> dadosEstoque)
    {
        var json = JsonSerializer.Serialize(dadosEstoque, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("estoque.json", json);
    }
}

// Classes
public class Produto
{
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public int Estoque { get; set; }
}

public class Movimentacao
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int EstoqueAnterior { get; set; }
    public int EstoqueAtual { get; set; }
}
