-- Script para criação das tabelas do Sistema de Chamados
-- SQL Server Database
-- Usar o banco de dados (substitua pelo nome do seu banco)
USE SistemaChamados
GO

-- Criar tabela Funcionarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Funcionarios' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Funcionarios](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Nome] [nvarchar](100) NOT NULL,
        [Cpf] [nvarchar](14) NOT NULL,
        [Email] [nvarchar](100) NOT NULL,
        [Senha] [nvarchar](255) NOT NULL,
        [NivelAcesso] [int] NOT NULL,
        [DataCriacao] [datetime] NOT NULL DEFAULT GETDATE(),
        [Ativo] [bit] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Funcionarios] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Funcionarios_Email] UNIQUE ([Email]),
        CONSTRAINT [UK_Funcionarios_Cpf] UNIQUE ([Cpf])
    )
END
GO

-- Criar tabela Chamados
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Chamados' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Chamados](
        [IdChamado] [int] IDENTITY(1,1) NOT NULL,
        [Categoria] [nvarchar](50) NOT NULL,
        [Contestacoes] [ntext] NULL,
        [Prioridade] [int] NOT NULL DEFAULT 2,
        [Descricao] [ntext] NOT NULL,
        [Afetado] [int] NOT NULL,
        [DataChamado] [datetime] NOT NULL DEFAULT GETDATE(),
        [Status] [int] NOT NULL DEFAULT 1,
        [TecnicoResponsavel] [int] NULL,
        [DataResolucao] [datetime] NULL,
        [DataUltimaAtualizacao] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Chamados] PRIMARY KEY CLUSTERED ([IdChamado] ASC),
        CONSTRAINT [FK_Chamados_Afetado] FOREIGN KEY ([Afetado]) REFERENCES [dbo].[Funcionarios]([Id]),
        CONSTRAINT [FK_Chamados_TecnicoResponsavel] FOREIGN KEY ([TecnicoResponsavel]) REFERENCES [dbo].[Funcionarios]([Id])
    )
END
GO

-- Criar índices para melhor performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chamados_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Chamados_Status] ON [dbo].[Chamados] ([Status])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chamados_Prioridade')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Chamados_Prioridade] ON [dbo].[Chamados] ([Prioridade])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chamados_DataChamado')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Chamados_DataChamado] ON [dbo].[Chamados] ([DataChamado])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Chamados_TecnicoResponsavel')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Chamados_TecnicoResponsavel] ON [dbo].[Chamados] ([TecnicoResponsavel])
END
GO

-- Criar tabela de auditoria para chamados (opcional)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChamadosAuditoria' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ChamadosAuditoria](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [IdChamado] [int] NOT NULL,
        [Acao] [nvarchar](50) NOT NULL,
        [UsuarioId] [int] NOT NULL,
        [DataAcao] [datetime] NOT NULL DEFAULT GETDATE(),
        [ValorAnterior] [ntext] NULL,
        [ValorNovo] [ntext] NULL,
        [Observacoes] [nvarchar](500) NULL,
        CONSTRAINT [PK_ChamadosAuditoria] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ChamadosAuditoria_Chamado] FOREIGN KEY ([IdChamado]) REFERENCES [dbo].[Chamados]([IdChamado]),
        CONSTRAINT [FK_ChamadosAuditoria_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Funcionarios]([Id])
    )
END
GO

-- Criar constraints de check para validação
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Funcionarios_NivelAcesso')
BEGIN
    ALTER TABLE [dbo].[Funcionarios]
    ADD CONSTRAINT [CK_Funcionarios_NivelAcesso] CHECK ([NivelAcesso] IN (1, 2, 3))
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Chamados_Prioridade')
BEGIN
    ALTER TABLE [dbo].[Chamados]
    ADD CONSTRAINT [CK_Chamados_Prioridade] CHECK ([Prioridade] BETWEEN 1 AND 4)
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Chamados_Status')
BEGIN
    ALTER TABLE [dbo].[Chamados]
    ADD CONSTRAINT [CK_Chamados_Status] CHECK ([Status] BETWEEN 1 AND 5)
END
GO

-- Criar trigger para atualizar DataUltimaAtualizacao automaticamente
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Chamados_UpdateTimestamp')
BEGIN
    EXEC('
    CREATE TRIGGER [dbo].[TR_Chamados_UpdateTimestamp]
    ON [dbo].[Chamados]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE [dbo].[Chamados]
        SET [DataUltimaAtualizacao] = GETDATE()
        FROM [dbo].[Chamados] c
        INNER JOIN inserted i ON c.[IdChamado] = i.[IdChamado]
    END
    ')
END
GO

-- Inserir dados iniciais (usuário administrador padrão)
IF NOT EXISTS (SELECT * FROM [dbo].[Funcionarios] WHERE [Email] = 'admin@sistema.com')
BEGIN
    INSERT INTO [dbo].[Funcionarios] ([Nome], [Cpf], [Email], [Senha], [NivelAcesso])
    VALUES ('A', '00000000000', 'admin@sistema.com', 'admin123', 1)
    
    PRINT 'Usuário administrador padrão criado:'
    PRINT 'Email: admin@sistema.com'
    PRINT 'Senha: admin123'
    PRINT 'IMPORTANTE: Altere a senha padrão após o primeiro login!'
END
GO

-- Inserir categorias padrão (se necessário criar tabela de categorias)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categorias' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Categorias](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Nome] [nvarchar](50) NOT NULL,
        [Descricao] [nvarchar](200) NULL,
        [Ativo] [bit] NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Categorias] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UK_Categorias_Nome] UNIQUE ([Nome])
    )
    
    -- Inserir categorias padrão
    INSERT INTO [dbo].[Categorias] ([Nome], [Descricao]) VALUES
    ('Hardware', 'Problemas relacionados a equipamentos físicos'),
    ('Software', 'Problemas relacionados a programas e aplicações'),
    ('Rede', 'Problemas de conectividade e rede'),
    ('Email', 'Problemas relacionados ao correio eletrônico'),
    ('Impressora', 'Problemas com impressoras e dispositivos de impressão'),
    ('Sistema', 'Problemas relacionados ao sistema operacional'),
    ('Acesso', 'Problemas de login e permissões'),
    ('Outros', 'Outros tipos de problemas não categorizados')
END
GO

PRINT 'Script de criação das tabelas executado com sucesso!'
PRINT 'Tabelas criadas:'
PRINT '- Funcionarios'
PRINT '- Chamados'
PRINT '- ChamadosAuditoria'
PRINT '- Categorias'
PRINT ''
PRINT 'Índices e constraints criados para otimização e integridade dos dados.'
PRINT 'Usuário administrador padrão disponível para primeiro acesso.'

SELECT name FROM sys.databases WHERE name = 'SistemaChamados';

-- Verificar se tabelas existem
USE SistemaChamados;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;

-- Verificar se usuário admin existe
SELECT * FROM Funcionarios WHERE Email = 'admin@sistema.com';

-- Verificar dados da tabela
SELECT Id, Nome, Email, NivelAcesso FROM Funcionarios;

select * from Funcionarios;

insert into Funcionarios ([Nome], [Cpf], [Email], [Senha], [NivelAcesso]) values ('C', '00000000002', 'funcionario2@sistema.com', 'funcionario123', 2);


-- Script para crear las tablas del Sistema de Chamados
-- Ejecutar en SQL Server Management Studio

USE SistemaChamados; -- Cambiar por el nombre de tu base de datos
GO

-- Tabla Funcionarios (actualizada para incluir los 3 niveles)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Funcionarios' AND xtype='U')
BEGIN
    CREATE TABLE Funcionarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nome NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Senha NVARCHAR(255) NOT NULL, -- En producción usar hash
        TipoFuncionario NVARCHAR(20) NOT NULL CHECK (TipoFuncionario IN ('Administrador', 'Técnico', 'Funcionário')),
        DataCadastro DATETIME NOT NULL DEFAULT GETDATE(),
        Ativo BIT NOT NULL DEFAULT 1,
        
        -- Campos específicos para Técnico
        Especializacao NVARCHAR(100) NULL,
        
        -- Campos específicos para Funcionário
        Departamento NVARCHAR(100) NULL,
        Cargo NVARCHAR(100) NULL
    );
END

-- Tabela Chamados
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Chamados' AND xtype='U')
BEGIN
    CREATE TABLE Chamados (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Titulo NVARCHAR(100) NOT NULL,
        Descricao NVARCHAR(1000) NOT NULL,
        Status INT NOT NULL DEFAULT 1, -- 1=Aberto, 2=EmAndamento, 3=Aguardando, 4=Resolvido, 5=Fechado, 6=Cancelado
        Prioridade INT NOT NULL DEFAULT 2, -- 1=Baixa, 2=Normal, 3=Alta, 4=Crítica
        FuncionarioId INT NOT NULL, -- Quem criou o chamado
        TecnicoId INT NULL, -- Técnico responsável
        DataAbertura DATETIME NOT NULL DEFAULT GETDATE(),
        DataAtribuicao DATETIME NULL,
        DataResolucao DATETIME NULL,
        DataFechamento DATETIME NULL,
        Categoria NVARCHAR(50) NOT NULL DEFAULT 'Outros',
        Observacoes NVARCHAR(2000) NULL,
        
        -- Foreign Keys
        CONSTRAINT FK_Chamados_Funcionario FOREIGN KEY (FuncionarioId) REFERENCES Funcionarios(Id),
        CONSTRAINT FK_Chamados_Tecnico FOREIGN KEY (TecnicoId) REFERENCES Funcionarios(Id)
    );
END

-- Criar índices para melhor performance

CREATE INDEX IX_Funcionarios_Tipo ON Funcionarios(TipoFuncionario);
CREATE INDEX IX_Chamados_Status ON Chamados(Status);
CREATE INDEX IX_Chamados_Funcionario ON Chamados(FuncionarioId);
CREATE INDEX IX_Chamados_Tecnico ON Chamados(TecnicoId);
CREATE INDEX IX_Chamados_DataAbertura ON Chamados(DataAbertura);

-- Inserir dados iniciais

-- Inserir usuário Administrador padrão
IF NOT EXISTS (SELECT * FROM Funcionarios WHERE Email = 'admin@sistema.com')
BEGIN
    INSERT INTO Funcionarios (Nome, Email, Senha, TipoFuncionario, Ativo)
    VALUES ('Administrador do Sistema', 'admin@sistema.com', 'admin123', 'Administrador', 1);
END

-- Inserir usuário Técnico de exemplo
IF NOT EXISTS (SELECT * FROM Funcionarios WHERE Email = 'tecnico@sistema.com')
BEGIN
    INSERT INTO Funcionarios (Nome, Email, Senha, TipoFuncionario, Ativo, Especializacao)
    VALUES ('João Silva - Técnico', 'tecnico@sistema.com', 'tecnico123', 'Técnico', 1, 'Hardware e Redes');
END

-- Inserir usuário Funcionário de exemplo
IF NOT EXISTS (SELECT * FROM Funcionarios WHERE Email = 'funcionario@sistema.com')
BEGIN
    INSERT INTO Funcionarios (Nome, Email, Senha, TipoFuncionario, Ativo, Departamento, Cargo)
    VALUES ('Maria Santos - Funcionária', 'funcionario@sistema.com', 'funcionario123', 'Funcionário', 1, 'Recursos Humanos', 'Analista');
END

-- Inserir alguns chamados de exemplo
IF NOT EXISTS (SELECT * FROM Chamados WHERE Id = 1)
BEGIN
    DECLARE @FuncionarioId INT;
    SELECT @FuncionarioId = Id FROM Funcionarios WHERE Email = 'funcionario@sistema.com';
    
    IF @FuncionarioId IS NOT NULL
    BEGIN
        INSERT INTO Chamados (Titulo, Descricao, Status, Prioridade, FuncionarioId, Categoria)
        VALUES 
        ('Computador não liga', 'O computador da mesa 15 não está ligando. Já tentei verificar os cabos e a fonte.', 1, 3, @FuncionarioId, 'Hardware'),
        ('Problema com impressora', 'A impressora HP do departamento está com erro de papel atolado, mas não conseguimos resolver.', 1, 2, @FuncionarioId, 'Impressora'),
        ('Acesso ao sistema', 'Preciso de acesso ao sistema financeiro para fazer os relatórios mensais.', 1, 2, @FuncionarioId, 'Acesso');
    END
END

-- Verificar se tudo foi criado corretamente
SELECT 'Verificação das tabelas criadas:' as Resultado;
SELECT 'Funcionarios' as Tabela, COUNT(*) as Total FROM Funcionarios;
SELECT 'Chamados' as Tabela, COUNT(*) as Total FROM Chamados;

-- Mostrar usuários criados
SELECT 'Usuários padrão criados:' as Info;

USE SistemaChamados;
GO



-- Script SQL CORRIGIDO para compatibilidade total com o código C#
-- Execute este script para corrigir as incompatibilidades

USE SistemaChamados;
GO

-- Dropar tabelas existentes se houver incompatibilidade
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Chamados')
    DROP TABLE Chamados;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ChamadosAuditoria')
    DROP TABLE ChamadosAuditoria;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Funcionarios')
    DROP TABLE Funcionarios;
GO

-- CRIAR TABELA FUNCIONARIOS COMPATÍVEL COM C#
CREATE TABLE Funcionarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Cpf NVARCHAR(14) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Senha NVARCHAR(255) NOT NULL,
    NivelAcesso INT NOT NULL, -- CAMPO ESSENCIAL para o código C#
    TipoFuncionario NVARCHAR(20) NOT NULL, -- CAMPO ESSENCIAL para SqlServerConnection
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE(),
    Ativo BIT NOT NULL DEFAULT 1,
    
    -- Campos específicos opcionais
    Especializacao NVARCHAR(100) NULL,
    Departamento NVARCHAR(100) NULL,
    Cargo NVARCHAR(100) NULL,
    
    -- Constraints
    CONSTRAINT CK_NivelAcesso CHECK (NivelAcesso IN (1, 2, 3)),
    CONSTRAINT CK_TipoFuncionario CHECK (TipoFuncionario IN ('Funcionário', 'Técnico', 'Administrador'))
);
GO

-- CRIAR TABELA CHAMADOS COMPATÍVEL COM AMBOS OS MODELOS
CREATE TABLE Chamados (
    -- Campos para modelo "Chamados" (plural) - usado nos Controllers
    IdChamado INT IDENTITY(1,1) PRIMARY KEY,
    Categoria NVARCHAR(50) NOT NULL,
    Contestacoes NTEXT NULL,
    Prioridade INT NOT NULL DEFAULT 2,
    Descricao NTEXT NOT NULL,
    Afetado INT NOT NULL,
    DataChamado DATETIME NOT NULL DEFAULT GETDATE(),
    Status INT NOT NULL DEFAULT 1,
    TecnicoResponsavel INT NULL,
    DataResolucao DATETIME NULL,
    DataUltimaAtualizacao DATETIME NOT NULL DEFAULT GETDATE(),
    
    -- Campos adicionais para compatibilidade com modelo "Chamado" (singular)
    Id AS IdChamado, -- Campo computado para compatibilidade
    Titulo AS SUBSTRING(Descricao, 1, 100), -- Título derivado da descrição
    FuncionarioId AS Afetado, -- Alias para compatibilidade
    TecnicoId AS TecnicoResponsavel, -- Alias para compatibilidade
    DataAbertura AS DataChamado, -- Alias para compatibilidade
    DataAtribuicao DATETIME NULL,
    DataFechamento DATETIME NULL,
    Observacoes NVARCHAR(2000) NULL,
    
    -- Foreign Keys
    CONSTRAINT FK_Chamados_Afetado FOREIGN KEY (Afetado) REFERENCES Funcionarios(Id),
    CONSTRAINT FK_Chamados_TecnicoResponsavel FOREIGN KEY (TecnicoResponsavel) REFERENCES Funcionarios(Id),
    
    -- Constraints de validação
    CONSTRAINT CK_Chamados_Status CHECK (Status BETWEEN 1 AND 5),
    CONSTRAINT CK_Chamados_Prioridade CHECK (Prioridade BETWEEN 1 AND 4)
);
GO

-- Índices para performance
CREATE INDEX IX_Funcionarios_Email ON Funcionarios(Email);
CREATE INDEX IX_Funcionarios_NivelAcesso ON Funcionarios(NivelAcesso);
CREATE INDEX IX_Funcionarios_TipoFuncionario ON Funcionarios(TipoFuncionario);
CREATE INDEX IX_Chamados_Status ON Chamados(Status);
CREATE INDEX IX_Chamados_Prioridade ON Chamados(Prioridade);
CREATE INDEX IX_Chamados_Afetado ON Chamados(Afetado);
CREATE INDEX IX_Chamados_TecnicoResponsavel ON Chamados(TecnicoResponsavel);
CREATE INDEX IX_Chamados_DataChamado ON Chamados(DataChamado);
GO

-- Trigger para atualizar timestamp
CREATE TRIGGER TR_Chamados_UpdateTimestamp
ON Chamados
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Chamados 
    SET DataUltimaAtualizacao = GETDATE()
    FROM Chamados c
    INNER JOIN inserted i ON c.IdChamado = i.IdChamado;
END
GO



-- INSERIR DADOS INICIAIS CORRETOS
-- Usuário Administrador
INSERT INTO Funcionarios (Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario, Ativo)
VALUES ('Admin Sistema', '00000000000', 'admin@sistema.com', 'admin123', 3, 'Administrador', 1);

-- Usuário Técnico
INSERT INTO Funcionarios (Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario, Ativo, Especializacao)
VALUES ('João Silva', '11111111111', 'tecnico@sistema.com', 'tecnico123', 2, 'Técnico', 1, 'Hardware e Redes');

-- Usuário Funcionário
INSERT INTO Funcionarios (Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario, Ativo, Departamento, Cargo)
VALUES ('Maria Santos', '22222222222', 'funcionario@sistema.com', 'funcionario123', 1, 'Funcionário', 1, 'RH', 'Analista');
GO

-- Chamados de exemplo
DECLARE @FuncionarioId INT = (SELECT Id FROM Funcionarios WHERE Email = 'funcionario@sistema.com');
DECLARE @TecnicoId INT = (SELECT Id FROM Funcionarios WHERE Email = 'tecnico@sistema.com');

INSERT INTO Chamados (Categoria, Descricao, Prioridade, Afetado, Status, TecnicoResponsavel, DataAtribuicao, Observacoes)
VALUES 
('Hardware', 'Computador não liga - Mesa 15. Já verificamos cabos e fonte de alimentação.', 3, @FuncionarioId, 2, @TecnicoId, GETDATE(), 'Chamado atribuído automaticamente'),
('Impressora', 'Impressora HP com erro de papel atolado que não conseguimos resolver.', 2, @FuncionarioId, 1, NULL, NULL, NULL),
('Sistema', 'Necessário acesso ao sistema financeiro para relatórios mensais.', 2, @FuncionarioId, 1, NULL, NULL, NULL);
GO

-- Verificações finais
PRINT 'Verificando estrutura das tabelas...';
SELECT 'Funcionários cadastrados:' as Info;
SELECT Id, Nome, Email, NivelAcesso, TipoFuncionario FROM Funcionarios;

PRINT '';
SELECT 'Chamados cadastrados:' as Info;
SELECT IdChamado, Categoria, LEFT(Descricao, 50) + '...' as Descricao, Status, Prioridade FROM Chamados;

PRINT '';
PRINT 'Estrutura do banco corrigida e compatível com o código C#!';
PRINT 'Usuários de teste:';
PRINT '- admin@sistema.com / admin123 (Administrador)';
PRINT '- tecnico@sistema.com / tecnico123 (Técnico)';
PRINT '- funcionario@sistema.com / funcionario123 (Funcionário)';