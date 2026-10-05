using System;

class Program
{
    static void Main()
    {
        // Exemplo de entrada
        decimal valorOriginal = 1000.00m; // R$ 1.000,00
        DateTime dataVencimento = new DateTime(2026, 09, 20); // Exemplo: Vencimento em 20/09/2026
        DateTime dataHoje = DateTime.Today; // Data atual do sistema

        Console.WriteLine("=== CÁLCULO DE JUROS E MULTA POR ATRASO ===");
        Console.WriteLine($"Valor Original: R$ {valorOriginal:N2}");
        Console.WriteLine($"Data de Vencimento: {dataVencimento:dd/MM/yyyy}");
        Console.WriteLine($"Data de Hoje: {dataHoje:dd/MM/yyyy}");
        Console.WriteLine(new string('-', 45));

        // Verifica se está em atraso
        if (dataHoje <= dataVencimento)
        {
            Console.WriteLine("O título está em dia ou a vencer. Não há juros/multa aplicáveis.");
            Console.WriteLine($"Valor Total a Pagar: R$ {valorOriginal:N2}");
            return;
        }

        // Calcula a quantidade de dias em atraso
        int diasAtraso = (dataHoje - dataVencimento).Days;

        // Regra: Multa de 2,5% ao dia sobre o valor original
        // Nota: Juros diários simples de 2,5% ao dia (Valor * 0.025 * dias)
        decimal taxaJurosDiaria = 0.025m;
        decimal valorJuros = valorOriginal * taxaJurosDiaria * diasAtraso;
        decimal valorTotal = valorOriginal + valorJuros;

        // Exibição dos resultados
        Console.WriteLine($"Dias em Atraso: {diasAtraso} dia(s)");
        Console.WriteLine($"Taxa Aplicada: 2,5% ao dia");
        Console.WriteLine($"Valor dos Juros/Multa: R$ {valorJuros:N2}");
        Console.WriteLine($"Valor Total Atualizado: R$ {valorTotal:N2}");
        Console.WriteLine(new string('-', 45));
    }
}

