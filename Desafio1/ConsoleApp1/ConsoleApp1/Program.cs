using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

class Program
{
    static void Main()
    {
        // Variável do tipo string jsonInput que recebe  conjunto de dados nome do vendedor e // comissão. Onde Vendas representa uma tabela
        string jsonInput = @"{
          ""vendas"": [
            { ""vendedor"": ""João Silva"", ""valor"": 1200.50 },
            { ""vendedor"": ""João Silva"", ""valor"": 950.75 },
            { ""vendedor"": ""João Silva"", ""valor"": 1800.00 },
            { ""vendedor"": ""João Silva"", ""valor"": 1400.30 },
            { ""vendedor"": ""João Silva"", ""valor"": 1100.90 },
            { ""vendedor"": ""João Silva"", ""valor"": 1550.00 },
            { ""vendedor"": ""João Silva"", ""valor"": 1700.80 },
            { ""vendedor"": ""João Silva"", ""valor"": 250.30 },
            { ""vendedor"": ""João Silva"", ""valor"": 480.75 },
            { ""vendedor"": ""João Silva"", ""valor"": 320.40 },
            
            { ""vendedor"": ""Maria Souza"", ""valor"": 2100.40 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 1350.60 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 950.20 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 1600.75 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 1750.00 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 1450.90 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 400.50 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 180.20 },
            { ""vendedor"": ""Maria Souza"", ""valor"": 90.75 },
            
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 800.50 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1200.00 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1950.30 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1750.80 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 1300.60 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 300.40 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 500.00 },
            { ""vendedor"": ""Carlos Oliveira"", ""valor"": 125.75 },
            
            { ""vendedor"": ""Ana Lima"", ""valor"": 1000.00 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 1100.50 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 1250.75 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 1400.20 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 1550.90 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 1650.00 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 75.30 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 420.90 },
            { ""vendedor"": ""Ana Lima"", ""valor"": 315.40 }
          ]
        }";

        // Desserialização do JSON para os objetos C# onde é atribuido as informações                //numa variàvel dados
        var dados = JsonSerializer.Deserialize<RelatorioVendas>(jsonInput, new JsonSerializerOptions
        {
            /*Propriedade do JsonSerializerOptions no .NET que define se a busca por nomes de propriedades no JSON ignora diferenças entre letras maiúsculas e minúsculas durante a desserialização*/
            PropertyNameCaseInsensitive = true
        });
        // Caso não encontre dados na variável dados retorna via console mensagem 
        if (dados?.Vendas == null)
        {
            Console.WriteLine("Nenhum dado encontrado.");
            return;
        }

        // Calculo da comissão por venda e agrupando por vendedor
        var resultado = dados.Vendas
            .Select(v => new
            {
                v.Vendedor,
                v.Valor,
                Comissao = CalcularComissao(v.Valor)
            })
            .GroupBy(v => v.Vendedor)
            .Select(g => new
            {
                Vendedor = g.Key,
                TotalVendas = g.Sum(v => v.Valor),
                TotalComissao = g.Sum(v => v.Comissao)
            });

        // Exibe os resultados formatados
        Console.WriteLine("=== RELATÓRIO DE COMISSÕES ===");
        foreach (var item in resultado)
        {
            Console.WriteLine($"Vendedor: {item.Vendedor}");
            Console.WriteLine($"  - Total de Vendas:   R$ {item.TotalVendas:N2}");
            Console.WriteLine($"  - Total de Comissão: R$ {item.TotalComissao:N2}");
            Console.WriteLine(new string('-', 35));
        }
    }

    // Método que aplica as regras de comissão por venda
    static decimal CalcularComissao(decimal valor)
    {
        if (valor < 100.00m)
        {
            return 0.00m; // Abaixo de R$ 100,00 não gera comissão
        }
        else if (valor < 500.00m)
        {
            return valor * 0.01m; // Abaixo de R$ 500,00 gera 1%
        }
        else
        {
            return valor * 0.05m; // A partir de R$ 500,00 gera 5%
        }
    }
}

// Classes de mapeamento do JSON. Tem o metodo Vendas para obter e setar //relatório
public class RelatorioVendas
{
    public List<Venda> Vendas { get; set; }
}
// Classe Vendas com metodos Vendedor e valor da venda para obter e inserir 
public class Venda
{
    public string Vendedor { get; set; }
    public decimal Valor { get; set; }
}
