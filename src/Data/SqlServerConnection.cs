using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Config;
using SistemaChamados.Controllers;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Data
{
    public class SqlServerConnection : IDatabaseConnection, IDisposable
    {
        private readonly string _connectionString;

        public SqlServerConnection(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // MÉTODOS DE CONEXÃO
        public bool TestarConexao()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao testar conexão: {ex.Message}");
                return false;
            }
        }

        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        WHERE IdChamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarChamadoFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar chamado: {ex.Message}");
            }

            return null;
        }

        public List<Chamados> ListarTodosChamados()
        {
            var chamados = new List<Chamados>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        ORDER BY DataChamado DESC";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chamados.Add(CriarChamadoFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados: {ex.Message}");
            }

            return chamados;
        }

        public List<Chamados> ListarChamadosPorFuncionario(int funcionarioId)
        {
            var chamados = new List<Chamados>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        WHERE Afetado = @FuncionarioId
                        ORDER BY DataChamado DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                chamados.Add(CriarChamadoFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por funcionário: {ex.Message}");
            }

            return chamados;
        }

        public List<Chamados> ListarChamadosPorTecnico(int tecnicoId)
        {
            var chamados = new List<Chamados>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        WHERE TecnicoResponsavel = @TecnicoId
                        ORDER BY Prioridade DESC, DataChamado ASC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TecnicoId", tecnicoId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                chamados.Add(CriarChamadoFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por técnico: {ex.Message}");
            }

            return chamados;
        }

        public List<Chamados> ListarChamadosPorStatus(StatusChamado status)
        {
            var chamados = new List<Chamados>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        WHERE Status = @Status
                        ORDER BY DataChamado DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Status", (int)status);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                chamados.Add(CriarChamadoFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por status: {ex.Message}");
            }

            return chamados;
        }

        public List<Chamados> ListarChamadosPorPrioridade(int prioridade)
        {
            var chamados = new List<Chamados>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT IdChamado, Categoria, Contestacoes, Prioridade, Descricao, Afetado,
                               DataChamado, Status, TecnicoResponsavel, DataResolucao
                        FROM Chamados 
                        WHERE Prioridade = @Prioridade
                        ORDER BY DataChamado DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Prioridade", prioridade);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                chamados.Add(CriarChamadoFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por prioridade: {ex.Message}");
            }

            return chamados;
        }

        public bool RemoverFuncionario(int id)
        {
            return ExcluirFuncionario(id);
        }

        public bool RemoverChamado(int idChamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Chamados WHERE IdChamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", idChamado);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover chamado: {ex.Message}");
                return false;
            }
        }

        // MÉTODOS DE RELATÓRIOS
        public DataTable ObterRelatorioGeral()
        {
            var dataTable = new DataTable();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT c.IdChamado, c.Categoria, c.Descricao, c.Prioridade, c.Status,
                               c.DataChamado, c.DataResolucao,
                               f1.Nome as Solicitante, f2.Nome as Tecnico
                        FROM Chamados c
                        INNER JOIN Funcionarios f1 ON c.Afetado = f1.Id
                        LEFT JOIN Funcionarios f2 ON c.TecnicoResponsavel = f2.Id
                        ORDER BY c.DataChamado DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório geral: {ex.Message}");
            }

            return dataTable;
        }

        public DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var dataTable = new DataTable();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT c.IdChamado, c.Categoria, c.Descricao, c.Prioridade, c.Status,
                               c.DataChamado, c.DataResolucao,
                               f1.Nome as Solicitante, f2.Nome as Tecnico
                        FROM Chamados c
                        INNER JOIN Funcionarios f1 ON c.Afetado = f1.Id
                        LEFT JOIN Funcionarios f2 ON c.TecnicoResponsavel = f2.Id
                        WHERE c.DataChamado BETWEEN @DataInicio AND @DataFim
                        ORDER BY c.DataChamado DESC";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@DataInicio", dataInicio);
                        command.Parameters.AddWithValue("@DataFim", dataFim.AddDays(1)); // Incluir dia completo

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar relatório por período: {ex.Message}");
            }

            return dataTable;
        }

        // MÉTODOS DE ESTATÍSTICAS
        public EstatisticasFuncionarios ObterEstatisticasFuncionarios()
        {
            var stats = new EstatisticasFuncionarios();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT 
                            COUNT(*) as Total,
                            SUM(CASE WHEN TipoFuncionario = 'Administrador' THEN 1 ELSE 0 END) as Administradores,
                            SUM(CASE WHEN TipoFuncionario = 'Técnico' THEN 1 ELSE 0 END) as Tecnicos,
                            SUM(CASE WHEN TipoFuncionario = 'Funcionário' THEN 1 ELSE 0 END) as Funcionarios,
                            SUM(CASE WHEN Ativo = 1 THEN 1 ELSE 0 END) as Ativos
                        FROM Funcionarios";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            stats.TotalFuncionarios = (int)reader["Total"];
                            stats.TotalAdministradores = (int)reader["Administradores"];
                            stats.TotalTecnicos = (int)reader["Tecnicos"];
                            stats.TotalFuncionariosComuns = (int)reader["Funcionarios"];
                            stats.FuncionariosAtivos = (int)reader["Ativos"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas de funcionários: {ex.Message}");
            }

            return stats;
        }

        // MÉTODOS AUXILIARES
        private Funcionarios CriarFuncionarioPorTipo(SqlDataReader reader)
        {
            try
            {
                string tipoFuncionario = reader["TipoFuncionario"].ToString();

                Funcionarios funcionario;
                if (tipoFuncionario == "Funcionário")
                {
                    funcionario = new Funcionario();
                }
                else if (tipoFuncionario == "Técnico")
                {
                    funcionario = new Tecnico();
                }
                else if (tipoFuncionario == "Administrador")
                {
                    funcionario = new ADM();
                }
                else
                {
                    throw new ArgumentException($"Tipo de funcionário inválido: {tipoFuncionario}");
                }

                // Preencher propriedades comuns
                funcionario.Id = (int)reader["Id"];
                funcionario.Nome = reader["Nome"].ToString();
                funcionario.Cpf = reader["Cpf"].ToString();
                funcionario.Email = reader["Email"].ToString();
                funcionario.Senha = reader["Senha"].ToString();
                funcionario.DataCadastro = (DateTime)reader["DataCadastro"];
                funcionario.Ativo = (bool)reader["Ativo"];

                // Preencher propriedades específicas
                if (funcionario is Tecnico tecnico && !reader.IsDBNull(reader.GetOrdinal("Especializacao")))
                {
                    tecnico.Especializacao = reader["Especializacao"].ToString();
                }
                else if (funcionario is Funcionario func)
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Departamento")))
                        func.Departamento = reader["Departamento"].ToString();
                    if (!reader.IsDBNull(reader.GetOrdinal("Cargo")))
                        func.Cargo = reader["Cargo"].ToString();
                }

                return funcionario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar funcionário: {ex.Message}");
                return null;
            }
        }

        private Chamados CriarChamadoFromReader(SqlDataReader reader)
        {
            return new Chamados
            {
                IdChamado = (int)reader["IdChamado"],
                Categoria = reader["Categoria"].ToString(),
                Contestacoes = reader.IsDBNull(reader.GetOrdinal("Contestacoes")) ? null : reader["Contestacoes"].ToString(),
                Prioridade = (int)reader["Prioridade"],
                Descricao = reader["Descricao"].ToString(),
                Afetado = (int)reader["Afetado"],
                DataChamado = (DateTime)reader["DataChamado"],
                Status = (StatusChamado)(int)reader["Status"],
                TecnicoResponsavel = reader.IsDBNull(reader.GetOrdinal("TecnicoResponsavel")) ?
                    (int?)null : (int)reader["TecnicoResponsavel"],
                DataResolucao = reader.IsDBNull(reader.GetOrdinal("DataResolucao")) ?
                    (DateTime?)null : (DateTime)reader["DataResolucao"]
            };
        }

        public void Dispose()
        {
            // Cleanup resources if needed
            GC.SuppressFinalize(this);

        }
        public bool AbrirConexao()
        {
            return TestarConexao();
        }

        public void FecharConexao()
        {
            // Implementação base - conexões são abertas/fechadas por método
        }
        public bool ValidarLogin(string email, string senha)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM Funcionarios WHERE Email = @Email AND Senha = @Senha AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Senha", senha); // Em produção, usar hash

                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao validar login: {ex.Message}");
                return false;
            }
        }

        public Funcionarios BuscarFuncionarioPorEmail(string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario, 
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM Funcionarios 
                        WHERE Email = @Email AND Ativo = 1";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarFuncionarioPorTipo(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por email: {ex.Message}");
            }

            return null;
        }

        public Funcionarios BuscarFuncionarioPorId(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario,
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM Funcionarios 
                        WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CriarFuncionarioPorTipo(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por ID: {ex.Message}");
            }

            return null;
        }

        public List<Funcionarios> ListarTodosFuncionarios()
        {
            var funcionarios = new List<Funcionarios>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario,
                               DataCadastro, Ativo, Especializacao, Departamento, Cargo
                        FROM Funcionarios 
                        ORDER BY Nome";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var funcionario = CriarFuncionarioPorTipo(reader);
                            if (funcionario != null)
                                funcionarios.Add(funcionario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar funcionários: {ex.Message}");
            }

            return funcionarios;
        }

        public List<Tecnico> ListarTecnicos()
        {
            var tecnicos = new List<Tecnico>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT Id, Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario,
                               DataCadastro, Ativo, Especializacao
                        FROM Funcionarios 
                        WHERE TipoFuncionario = 'Técnico' AND Ativo = 1
                        ORDER BY Nome";

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tecnico = (Tecnico)CriarFuncionarioPorTipo(reader);
                            if (tecnico != null)
                                tecnicos.Add(tecnico);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar técnicos: {ex.Message}");
            }

            return tecnicos;
        }

        public int InserirFuncionario(Funcionarios funcionario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO Funcionarios (Nome, Cpf, Email, Senha, NivelAcesso, TipoFuncionario, 
                                                 DataCadastro, Ativo, Especializacao, Departamento, Cargo)
                        VALUES (@Nome, @Cpf, @Email, @Senha, @NivelAcesso, @TipoFuncionario,
                                @DataCadastro, @Ativo, @Especializacao, @Departamento, @Cargo);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                        command.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                        command.Parameters.AddWithValue("@Email", funcionario.Email);
                        command.Parameters.AddWithValue("@Senha", funcionario.Senha);
                        command.Parameters.AddWithValue("@NivelAcesso", funcionario.NivelAcesso);
                        command.Parameters.AddWithValue("@TipoFuncionario", funcionario.TipoFuncionario);
                        command.Parameters.AddWithValue("@DataCadastro", funcionario.DataCadastro);
                        command.Parameters.AddWithValue("@Ativo", funcionario.Ativo);

                        // Campos específicos opcionais
                        string especializacao = (funcionario is Tecnico tecnico) ? tecnico.Especializacao : null;
                        string departamento = (funcionario is Funcionario func) ? func.Departamento : null;
                        string cargo = (funcionario is Funcionario func2) ? func2.Cargo : null;

                        command.Parameters.AddWithValue("@Especializacao", (object)especializacao ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Departamento", (object)departamento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Cargo", (object)cargo ?? DBNull.Value);

                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir funcionário: {ex.Message}");
                return 0;
            }
        }

        public bool AtualizarFuncionario(Funcionarios funcionario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        UPDATE Funcionarios 
                        SET Nome = @Nome, Cpf = @Cpf, Email = @Email,
                            NivelAcesso = @NivelAcesso, TipoFuncionario = @TipoFuncionario,
                            Ativo = @Ativo, Especializacao = @Especializacao,
                            Departamento = @Departamento, Cargo = @Cargo
                        WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", funcionario.Id);
                        command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                        command.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                        command.Parameters.AddWithValue("@Email", funcionario.Email);
                        command.Parameters.AddWithValue("@NivelAcesso", funcionario.NivelAcesso);
                        command.Parameters.AddWithValue("@TipoFuncionario", funcionario.TipoFuncionario);
                        command.Parameters.AddWithValue("@Ativo", funcionario.Ativo);

                        // Campos específicos
                        string especializacao = (funcionario is Tecnico tecnico) ? tecnico.Especializacao : null;
                        string departamento = (funcionario is Funcionario func) ? func.Departamento : null;
                        string cargo = (funcionario is Funcionario func2) ? func2.Cargo : null;

                        command.Parameters.AddWithValue("@Especializacao", (object)especializacao ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Departamento", (object)departamento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Cargo", (object)cargo ?? DBNull.Value);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar funcionário: {ex.Message}");
                return false;
            }
        }

        public bool AlterarSenha(int funcionarioId, string novaSenha)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Funcionarios SET Senha = @NovaSenha WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NovaSenha", novaSenha);
                        command.Parameters.AddWithValue("@Id", funcionarioId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar senha: {ex.Message}");
                return false;
            }
        }

        public bool AlterarNivelAcesso(int funcionarioId, int novoNivel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Determinar TipoFuncionario baseado no nível
                    string tipoFuncionario;
                    if (novoNivel == 1)
                        tipoFuncionario = "Funcionário";
                    else if (novoNivel == 2)
                        tipoFuncionario = "Técnico";
                    else if (novoNivel == 3)
                        tipoFuncionario = "Administrador";
                    else
                        throw new ArgumentException("Nível inválido");

                    string sql = @"UPDATE Funcionarios 
                                  SET NivelAcesso = @NovoNivel, TipoFuncionario = @TipoFuncionario 
                                  WHERE Id = @Id";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NovoNivel", novoNivel);
                        command.Parameters.AddWithValue("@TipoFuncionario", tipoFuncionario);
                        command.Parameters.AddWithValue("@Id", funcionarioId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao alterar nível de acesso: {ex.Message}");
                return false;
            }
        }

        public bool ExcluirFuncionario(int funcionarioId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Verificar se há chamados ativos
                    string sqlCheck = @"
                        SELECT COUNT(*) FROM Chamados 
                        WHERE (Afetado = @Id OR TecnicoResponsavel = @Id) 
                        AND Status NOT IN (4, 5)"; // Não resolvido/fechado

                    using (var checkCommand = new SqlCommand(sqlCheck, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Id", funcionarioId);
                        int chamadosAtivos = (int)checkCommand.ExecuteScalar();

                        if (chamadosAtivos > 0)
                        {
                            throw new InvalidOperationException($"Funcionário possui {chamadosAtivos} chamado(s) ativo(s)");
                        }
                    }

                    // Excluir funcionário
                    string sqlDelete = "DELETE FROM Funcionarios WHERE Id = @Id";
                    using (var deleteCommand = new SqlCommand(sqlDelete, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@Id", funcionarioId);
                        return deleteCommand.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir funcionário: {ex.Message}");
                return false;
            }
        }
        // MÉTODOS PARA CHAMADOS - CORRIGIDOS PARA MODELO "Chamados"
        public int InserirChamado(Chamados chamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        INSERT INTO Chamados (Categoria, Descricao, Prioridade, Afetado, DataChamado, 
                                            Status, TecnicoResponsavel, Contestacoes)
                        VALUES (@Categoria, @Descricao, @Prioridade, @Afetado, @DataChamado,
                                @Status, @TecnicoResponsavel, @Contestacoes);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Afetado", chamado.Afetado);
                        command.Parameters.AddWithValue("@DataChamado", chamado.DataChamado);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoResponsavel",
                            chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Contestacoes",
                            string.IsNullOrEmpty(chamado.Contestacoes) ? (object)DBNull.Value : chamado.Contestacoes);

                        var result = command.ExecuteScalar();
                        int novoId = Convert.ToInt32(result);
                        chamado.IdChamado = novoId;
                        return novoId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir chamado: {ex.Message}");
                return 0;
            }
        }

        public bool AtualizarChamado(Chamados chamado)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"
                        UPDATE Chamados 
                        SET Categoria = @Categoria, Descricao = @Descricao, Prioridade = @Prioridade,
                            Status = @Status, TecnicoResponsavel = @TecnicoResponsavel, 
                            DataResolucao = @DataResolucao, Contestacoes = @Contestacoes
                        WHERE IdChamado = @IdChamado";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                        command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                        command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                        command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                        command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                        command.Parameters.AddWithValue("@TecnicoResponsavel",
                            chamado.TecnicoResponsavel.HasValue ? (object)chamado.TecnicoResponsavel.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@DataResolucao",
                            chamado.DataResolucao.HasValue ? (object)chamado.DataResolucao.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Contestacoes",
                            string.IsNullOrEmpty(chamado.Contestacoes) ? (object)DBNull.Value : chamado.Contestacoes);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chamado: {ex.Message}");
                return false;
            }
        }
    }
}


