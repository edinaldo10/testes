# Teste de programação

Ponto de partida do **Lab — Exercício de progamação de dados**.

---


## Desafio 1. 

---

Considerando que o json abaixo tem registros de vendas de um time comercial, faça um programa que leia os dados e calcule a comissão de cada vendedor, seguindo a seguinte regra para cada venda:
	Vendas abaixo de R$100,00 não gera comissão
	Vendas abaixo de R$500,00 gera 1% de comissão
	A partir de R$500,00 gera 5% de comissão
{
  "vendas": [
    { "vendedor": "João Silva", "valor": 1200.50 },
    { "vendedor": "João Silva", "valor": 950.75 },
    { "vendedor": "João Silva", "valor": 1800.00 },
    { "vendedor": "João Silva", "valor": 1400.30 },
    { "vendedor": "João Silva", "valor": 1100.90 },
    { "vendedor": "João Silva", "valor": 1550.00 },
    { "vendedor": "João Silva", "valor": 1700.80 },
    { "vendedor": "João Silva", "valor": 250.30 },
    { "vendedor": "João Silva", "valor": 480.75 },
    { "vendedor": "João Silva", "valor": 320.40 },
    
    { "vendedor": "Maria Souza", "valor": 2100.40 },
    { "vendedor": "Maria Souza", "valor": 1350.60 },
    { "vendedor": "Maria Souza", "valor": 950.20 },
    { "vendedor": "Maria Souza", "valor": 1600.75 },
    { "vendedor": "Maria Souza", "valor": 1750.00 },
    { "vendedor": "Maria Souza", "valor": 1450.90 },
    { "vendedor": "Maria Souza", "valor": 400.50 },
    { "vendedor": "Maria Souza", "valor": 180.20 },
    { "vendedor": "Maria Souza", "valor": 90.75 },
    
    { "vendedor": "Carlos Oliveira", "valor": 800.50 },
    { "vendedor": "Carlos Oliveira", "valor": 1200.00 },
    { "vendedor": "Carlos Oliveira", "valor": 1950.30 },
    { "vendedor": "Carlos Oliveira", "valor": 1750.80 },
    { "vendedor": "Carlos Oliveira", "valor": 1300.60 },
    { "vendedor": "Carlos Oliveira", "valor": 300.40 },
    { "vendedor": "Carlos Oliveira", "valor": 500.00 },
    { "vendedor": "Carlos Oliveira", "valor": 125.75 },
    
    { "vendedor": "Ana Lima", "valor": 1000.00 },
    { "vendedor": "Ana Lima", "valor": 1100.50 },
    { "vendedor": "Ana Lima", "valor": 1250.75 },
    { "vendedor": "Ana Lima", "valor": 1400.20 },
    { "vendedor": "Ana Lima", "valor": 1550.90 },
    { "vendedor": "Ana Lima", "valor": 1650.00 },
    { "vendedor": "Ana Lima", "valor": 75.30 },
    { "vendedor": "Ana Lima", "valor": 420.90 },
    { "vendedor": "Ana Lima", "valor": 315.40 }
  ]
}

---

## Desafio 2. 

---

Faça um programa onde eu possa lançar movimentações de estoque dos produtos que estão no json abaixo, dando entrada ou saída da mercadoria no meu depósito, onde cada movimentação deve ter:
	Um número identificador único.
	Uma descrição para identificar o tipo da movimentação realizada
E que ao final da movimentação me retorne a qtde final do estoque do produto movimentado. Atenção no fonte tem que ativar a string de conexão. Não use Docker.

{
 "estoque":
 [
   {
  "codigoProduto": 101,
  "descricaoProduto": "Caneta Azul",
  "estoque": 150
   },
   {
  "codigoProduto": 102,
  "descricaoProduto": "Caderno Universitário",
  "estoque": 75
   },
   {
  "codigoProduto": 103,
  "descricaoProduto": "Borracha Branca",
  "estoque": 200
   },
   {
  "codigoProduto": 104,
  "descricaoProduto": "Lápis Preto HB",
  "estoque": 320
   },
   {
  "codigoProduto": 105,
  "descricaoProduto": "Marcador de Texto Amarelo",
  "estoque": 90
   }
 ]
}


### Tabelas e procedure para o movimento do estoque

-- Criação da Tabela de Produtos
CREATE TABLE Produtos (
    CodigoProduto INT PRIMARY KEY,
    DescricaoProduto VARCHAR(150) NOT NULL,
    EstoqueAtual INT NOT NULL CHECK (EstoqueAtual >= 0)
);

-- Carga inicial com os dados do JSON fornecido
INSERT INTO Produtos (CodigoProduto, DescricaoProduto, EstoqueAtual) VALUES
(101, 'Caneta Azul', 150),
(102, 'Caderno Universitário', 75),
(103, 'Borracha Branca', 200),
(104, 'Lápis Preto HB', 320),
(105, 'Marcador de Texto Amarelo', 90);

-- Criação da Tabela de Histórico de Movimentações
CREATE TABLE Movimentacoes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CodigoProduto INT NOT NULL FOREIGN KEY REFERENCES Produtos(CodigoProduto),
    Tipo VARCHAR(10) NOT NULL CHECK (Tipo IN ('Entrada', 'Saída')),
    Descricao VARCHAR(255) NOT NULL,
    QuantidadeMovimentada INT NOT NULL CHECK (QuantidadeMovimentada > 0),
    EstoqueAnterior INT NOT NULL,
    EstoqueFinal INT NOT NULL,
    DataMovimentacao DATETIME DEFAULT GETDATE()
);

-- Criação de store procedure de Movimentações de Estoque

CREATE PROCEDURE sp_MovimentarEstoque
    @CodigoProduto INT,
    @Tipo VARCHAR(10),        -- 'Entrada' ou 'Saída'
    @Descricao VARCHAR(255),
    @Quantidade INT,
    @EstoqueFinal INT OUTPUT  -- Retorna a quantidade final para a aplicação
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @EstoqueAtual INT;

    -- Bloqueia a linha do produto para leitura segura (evita concorrência)
    SELECT @EstoqueAtual = EstoqueAtual 
    FROM Produtos WITH (UPDLOCK, HOLDLOCK) 
    WHERE CodigoProduto = @CodigoProduto;

    IF @EstoqueAtual IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Produto não encontrado.', 16, 1);
        RETURN;
    END

    -- Valida se há estoque suficiente para saída
    IF @Tipo = 'Saída' AND @EstoqueAtual < @Quantidade
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Estoque insuficiente para realizar esta saída.', 16, 1);
        RETURN;
    END

    -- Calcula o estoque final
    IF @Tipo = 'Entrada'
        SET @EstoqueFinal = @EstoqueAtual + @Quantidade;
    ELSE
        SET @EstoqueFinal = @EstoqueAtual - @Quantidade;

    -- Atualiza o saldo do produto
    UPDATE Produtos 
    SET EstoqueAtual = @EstoqueFinal 
    WHERE CodigoProduto = @CodigoProduto;

    -- Registra no histórico de movimentações
    INSERT INTO Movimentacoes (CodigoProduto, Tipo, Descricao, QuantidadeMovimentada, EstoqueAnterior, EstoqueFinal)
    VALUES (@CodigoProduto, @Tipo, @Descricao, @Quantidade, @EstoqueAtual, @EstoqueFinal);

    COMMIT TRANSACTION;
END



## Desafio 3. 

---

Faça um programa que a partir de um valor e de uma data de vencimento, calcule o valor dos juros na data de hoje considerando que a multa seja de 2,5% ao dia.

---
