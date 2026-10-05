using Microsoft.Data.SqlClient;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    //string de conexão o nome do banco(SEU_BANCO) deve ser estoque

    //private static string connectionString = "Data Source=SEU SErver;Initial Catalog=SEU_BANCO;UID=usuario;PWD=senha;TrustServerCertificate=True;";
    
    static void Main()
    {
        // seta continuar como verdadeiro boloeano
        bool continuar = true;
        // enquanto verdadeiro
        while (continuar)
        {
            Console.Clear();
            // titulo e anuncio do programa
            Console.WriteLine("=== CONTROLE DE ESTOQUE - SQL SERVER ===");
            // executa função pata exibir o estoque inicial
            ExibirEstoqueAtual();

            Console.WriteLine("\n--- Realizar Nova Movimentação ---");
            //Ler o codigo do produto ineserido pelo usuario e armazena em codigoProduto
            int codigoProduto = LerInteiro("Digite o Código do Produto (ou 0 para sair): ");
            //se codigoProduto for 0 paraliza processo
            if (codigoProduto == 0) break;
            //Pede para uausario escolher entrada ou saida
            Console.WriteLine("Escolha o tipo: [1] Entrada | [2] Saída");
            int tipoOpcao = LerInteiro("Opção: ");
            // se não for escolhido nenhuma das opção ocorre tratamento via mensagem
            if (tipoOpcao != 1 && tipoOpcao != 2)
            {
                Console.WriteLine("\n[Erro] Opção inválida!");
                Console.ReadKey();
                continue;
            }
            //caso seja movimentação pede a para usuário descrever 
            string tipoStr = (tipoOpcao == 1) ? "Entrada" : "Saída";

            Console.Write("Digite a descrição/motivo da movimentação: ");

            // se o usuario não colocar coloca sem descrição
            string descricaoMov = Console.ReadLine() ?? "Sem descrição";
            // Pede para usuario inserir a quantidade de produtos para estoque
            int quantidade = LerInteiro("Digite a quantidade: ");
            // Tratamento caso tentativa de inserção de quantidade  com valor negativo
            if (quantidade <= 0)
            {
                Console.WriteLine("\n[Erro] A quantidade deve ser maior que zero!");
                Console.ReadKey();
                continue;
            }

            try
            {
                // Atribuição de estoque em estoqueFinal via função ExecutarMovimentacaoNoBanco //para realizar movimentação
                int estoqueFinal = ExecutarMovimentacaoNoBanco(codigoProduto, tipoStr, descricaoMov, quantidade);

                Console.WriteLine("\n----------------------------------------");
                Console.WriteLine("Movimentação realizada com sucesso!");
                Console.WriteLine($"Qtde Final em Estoque: **{estoqueFinal}** unidades");
                Console.WriteLine("----------------------------------------");
            }
            //Tratamento de exceção caso processo falhe
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Erro ao movimentar]: {ex.Message}");
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
    //Função que salvo movimento de estoque no banco de dados
    static int ExecutarMovimentacaoNoBanco(int codigoProduto, string tipo, string descricao, int quantidade)
    {
        using (SqlConnection conexao = new SqlConnection(connectionString))
        {
            conexao.Open();
            //executa procedure de movimentação de estoque
            using (SqlCommand comando = new SqlCommand("sp_MovimentarEstoque", conexao))
            {
                //inserção de dados do estoque
                comando.CommandType = System.Data.CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@CodigoProduto", codigoProduto);
                comando.Parameters.AddWithValue("@Tipo", tipo);
                comando.Parameters.AddWithValue("@Descricao", descricao);
                comando.Parameters.AddWithValue("@Quantidade", quantidade);

                // Parâmetro de saída para recuperar o estoque final
                SqlParameter parametroEstoqueFinal = new SqlParameter("@EstoqueFinal", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                comando.Parameters.Add(parametroEstoqueFinal);

                comando.ExecuteNonQuery();

                return (int)parametroEstoqueFinal.Value;
            }
        }
    }
    // função responsavem para exibição de dados do estoque
    static void ExibirEstoqueAtual()
    {
        using (SqlConnection conexao = new SqlConnection(connectionString))
        {
            conexao.Open();
            // consulta de dados na tabela Produtos
            string query = "SELECT CodigoProduto, DescricaoProduto, EstoqueAtual FROM Produtos";
            using (SqlCommand comando = new SqlCommand(query, conexao))
            {
                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    Console.WriteLine("------------------------------------------------------");
                    // apresentação do dados do estoque
                    Console.WriteLine($"{"Cód",-5} | {"Produto",-30} | {"Estoque",-8}");
                    Console.WriteLine("------------------------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["CodigoProduto"],-5} | {reader["DescricaoProduto"],-30} | {reader["EstoqueAtual"],-8}");
                    }
                    Console.WriteLine("------------------------------------------------------");
                }
            }
        }
    }
    // função LerInteiro verifica se valor quantidade é um inteiro e trata caso não seja
    static int LerInteiro(string mensagem)
    {
        int valor;
        while (true)
        {
            Console.Write(mensagem);
            if (int.TryParse(Console.ReadLine(), out valor)) return valor;
            Console.WriteLine("Entrada inválida. Digite um número inteiro.");
        }
    }
}
