using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static decimal CalcularComissao(decimal valor)
    {
        if (valor < 100) return 0;
        if (valor < 500) return valor * 0.01m;
        return valor * 0.05m;
    }

    static void Main()
    {
        // Lê o JSON de vendas com verificação de null
        var json = File.ReadAllText("vendas.json");
        var dados = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(json);

        // Verifica se dados não é null e tem a chave "vendas"
        if (dados?.ContainsKey("vendas") != true)
        {
            Console.WriteLine("Erro: Estrutura do JSON inválida.");
            return;
        }

        var resultados = new Dictionary<string, Dictionary<string, decimal>>();
        
        foreach (var venda in dados["vendas"]!)
        {
            // Verifica se vendedor e valor não são null
            var vendedor = venda["vendedor"]?.ToString();
            var valorStr = venda["valor"]?.ToString();

            if (string.IsNullOrEmpty(vendedor) || string.IsNullOrEmpty(valorStr))
            {
                Console.WriteLine("Aviso: Venda com dados incompletos, ignorando...");
                continue;
            }

            if (!decimal.TryParse(valorStr, out decimal valor))
            {
                Console.WriteLine($"Aviso: Valor inválido para vendedor {vendedor}, ignorando...");
                continue;
            }

            var comissao = CalcularComissao(valor);

            if (!resultados.ContainsKey(vendedor))
            {
                resultados[vendedor] = new Dictionary<string, decimal> 
                { 
                    { "total_vendas", 0 }, 
                    { "total_comissao", 0 } 
                };
            }

            resultados[vendedor]["total_vendas"] += valor;
            resultados[vendedor]["total_comissao"] += comissao;
        }

        foreach (var vendedor in resultados.Keys)
        {
          resultados[vendedor]["total_comissao"] = Math.Round(resultados[vendedor]["total_comissao"], 2);
        }

        // Salva resultados
        var resultadoJson = JsonSerializer.Serialize(resultados, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("comissoes.json", resultadoJson);

        Console.WriteLine("Comissões calculadas! Verifique comissoes.json");
    }
}
