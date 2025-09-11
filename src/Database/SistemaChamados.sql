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



