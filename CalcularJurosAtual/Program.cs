using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("---- CALCULADORA DE JUROS E MULTA ----");
        
        // Solicita o valor original
        Console.Write("Digite o valor original em R$ com duas casas decimais (Ex: 100.00): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal valorOriginal) || valorOriginal <= 0)
        {
            Console.WriteLine("Valor inválido!");
            return;
        }
        
        // Solicita a data de vencimento
        Console.Write("Digite a data de vencimento (mm/dd/aaaa): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dataVencimento))
        {
            Console.WriteLine("Data inválida!");
            return;
        }
        
        // Data de hoje
        DateTime dataHoje = DateTime.Today;
        
        // Calcula os juros
        var resultado = CalcularJuros(valorOriginal, dataVencimento, dataHoje);
        
        Console.WriteLine("\n---- RESULTADO ----");
        Console.WriteLine($"Valor original: R$ {valorOriginal:F2}");
        Console.WriteLine($"Data de vencimento: {dataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"Data de hoje: {dataHoje:dd/MM/yyyy}");
        Console.WriteLine($"Dias em atraso: {resultado.DiasAtraso}");
        
        if (resultado.DiasAtraso > 0)
        {
            Console.WriteLine($"Multa por dia: 2,5%");
            Console.WriteLine($"Valor da multa: R$ {resultado.ValorMulta:F2}");
            Console.WriteLine($"Valor total com juros: R$ {resultado.ValorTotal:F2}");
        }
        else
        {
            Console.WriteLine("Sem atraso - não há juros a pagar");
            Console.WriteLine($"Valor total: R$ {valorOriginal:F2}");
        }
    }


    
    static ResultadoCalculo CalcularJuros(decimal valorOriginal, DateTime dataVencimento, DateTime dataHoje)
    {
        // Calcula dias em atraso
        int diasAtraso = (dataHoje - dataVencimento).Days;
        diasAtraso = Math.Max(0, diasAtraso); // Não permite dias negativos
        
        decimal valorMulta = 0;
        decimal valorTotal = valorOriginal;
        
        if (diasAtraso > 0)
        {
            // Calcula juros compostos: Valor * (1 + taxa)^dias
            double taxaDiaria = 0.025; // 2,5% ao dia
            double fatorJuros = Math.Pow(1 + taxaDiaria, diasAtraso);
            valorTotal = valorOriginal * (decimal)fatorJuros;
            valorMulta = valorTotal - valorOriginal;
        }
        
        return new ResultadoCalculo
        {
            DiasAtraso = diasAtraso,
            ValorMulta = valorMulta,
            ValorTotal = valorTotal
        };
    }
}

// Classe para armazenar o resultado
public class ResultadoCalculo
{
    public int DiasAtraso { get; set; }
    public decimal ValorMulta { get; set; }
    public decimal ValorTotal { get; set; }
}
