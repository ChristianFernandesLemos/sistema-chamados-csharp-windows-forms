using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Data
{
    // Implementação da conexão com SQL Server
    public class SqlServerConnection : IDatabaseConnection
    {
        private readonly string _connectionString;
        private readonly SqlConnection _connection;

        // Construtor
        public SqlServerConnection(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
        }

        // Construtor com parâmetros do servidor
        public SqlServerConnection(string server, string database, string username, string password)
        {
            _connectionString = $"Server={server};Database={database};User Id={username};Password={password};";
            _connection = new SqlConnection(_connectionString);
        }

        // Construtor para autenticação Windows
        public SqlServerConnection(string server, string database)
        {
            _connectionString = $"Server={server};Database={database};Integrated Security=true;";
            _connection = new SqlConnection(_connectionString);
        }

        #region Métodos de Conexão

        public bool AbrirConexao()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir conexão: {ex.Message}");
                return false;
            }
        }

        public void FecharConexao()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fechar conexão: {ex.Message}");
            }
        }

        public bool TestarConexao()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao testar conexão: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Métodos para Funcionários

        public bool InserirFuncionario(Funcionarios funcionario)
        {
            try
            {
                string query = @"INSERT INTO Funcionarios (Nome, Cpf, Email, Senha, NivelAcesso) 
                                VALUES (@Nome, @Cpf, @Email, @Senha, @NivelAcesso)";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                    command.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                    command.Parameters.AddWithValue("@Email", funcionario.Email);
                    command.Parameters.AddWithValue("@Senha", funcionario.Senha);
                    command.Parameters.AddWithValue("@NivelAcesso", funcionario.NivelAcesso);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir funcionário: {ex.Message}");
                return false;
            }
        }

        public bool AtualizarFuncionario(Funcionarios funcionario)
        {
            try
            {
                string query = @"UPDATE Funcionarios SET Nome = @Nome, Cpf = @Cpf, Email = @Email, 
                                Senha = @Senha, NivelAcesso = @NivelAcesso WHERE Id = @Id";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", funcionario.Id);
                    command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                    command.Parameters.AddWithValue("@Cpf", funcionario.Cpf);
                    command.Parameters.AddWithValue("@Email", funcionario.Email);
                    command.Parameters.AddWithValue("@Senha", funcionario.Senha);
                    command.Parameters.AddWithValue("@NivelAcesso", funcionario.NivelAcesso);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar funcionário: {ex.Message}");
                return false;
            }
        }

        public bool RemoverFuncionario(int id)
        {
            try
            {
                string query = "DELETE FROM Funcionarios WHERE Id = @Id";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover funcionário: {ex.Message}");
                return false;
            }
        }

        public Funcionarios BuscarFuncionarioPorId(int id)
        {
            try
            {
                string query = "SELECT * FROM Funcionarios WHERE Id = @Id";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Determinar tipo de funcionário baseado no nível de acesso
                            int nivelAcesso = Convert.ToInt32(reader["NivelAcesso"]);
                            
                            if (nivelAcesso == 1) // Administrador
                            {
                                return new ADM(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                );
                            }
                            else // Técnico
                            {
                                return new Tecnico(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                );
                            }
                        }
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário: {ex.Message}");
            }
            return null;
        }

        public Funcionarios BuscarFuncionarioPorEmail(string email)
        {
            try
            {
                string query = "SELECT * FROM Funcionarios WHERE Email = @Email";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int nivelAcesso = Convert.ToInt32(reader["NivelAcesso"]);
                            
                            if (nivelAcesso == 1)
                            {
                                return new ADM(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                );
                            }
                            else
                            {
                                return new Tecnico(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                );
                            }
                        }
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar funcionário por email: {ex.Message}");
            }
            return null;
        }

        public List<Funcionarios> ListarTodosFuncionarios()
        {
            var funcionarios = new List<Funcionarios>();
            try
            {
                string query = "SELECT * FROM Funcionarios";

                using (var command = new SqlCommand(query, _connection))
                {
                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int nivelAcesso = Convert.ToInt32(reader["NivelAcesso"]);
                            
                            if (nivelAcesso == 1)
                            {
                                funcionarios.Add(new ADM(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                ));
                            }
                            else
                            {
                                funcionarios.Add(new Tecnico(
                                    Convert.ToInt32(reader["Id"]),
                                    Convert.ToString(reader["Nome"]),
                                    reader["Cpf"].ToString(),
                                    reader["Email"].ToString(),
                                    reader["Senha"].ToString(),
                                    nivelAcesso
                                ));
                            }
                        }
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar funcionários: {ex.Message}");
            }
            return funcionarios;
        }

        public bool ValidarLogin(string email, string senha)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Funcionarios WHERE Email = @Email AND Senha = @Senha";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Senha", senha);

                    AbrirConexao();
                    int count = (int)command.ExecuteScalar();
                    FecharConexao();

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao validar login: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Métodos para Chamados

        public bool InserirChamado(Chamados chamado)
        {
            try
            {
                string query = @"INSERT INTO Chamados (Categoria, Contestacoes, Prioridade, Descricao, 
                                Afetado, DataChamado, Status, TecnicoResponsavel) 
                                VALUES (@Categoria, @Contestacoes, @Prioridade, @Descricao, 
                                @Afetado, @DataChamado, @Status, @TecnicoResponsavel)";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                    command.Parameters.AddWithValue("@Contestacoes", chamado.Contestacoes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                    command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                    command.Parameters.AddWithValue("@Afetado", chamado.Afetado);
                    command.Parameters.AddWithValue("@DataChamado", chamado.DataChamado);
                    command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                    command.Parameters.AddWithValue("@TecnicoResponsavel", chamado.TecnicoResponsavel);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir chamado: {ex.Message}");
                return false;
            }
        }

        public bool AtualizarChamado(Chamados chamado)
        {
            try
            {
                string query = @"UPDATE Chamados SET Categoria = @Categoria, Contestacoes = @Contestacoes, 
                                Prioridade = @Prioridade, Descricao = @Descricao, Status = @Status, 
                                TecnicoResponsavel = @TecnicoResponsavel, DataResolucao = @DataResolucao 
                                WHERE IdChamado = @IdChamado";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@IdChamado", chamado.IdChamado);
                    command.Parameters.AddWithValue("@Categoria", chamado.Categoria);
                    command.Parameters.AddWithValue("@Contestacoes", chamado.Contestacoes ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Prioridade", chamado.Prioridade);
                    command.Parameters.AddWithValue("@Descricao", chamado.Descricao);
                    command.Parameters.AddWithValue("@Status", (int)chamado.Status);
                    command.Parameters.AddWithValue("@TecnicoResponsavel", chamado.TecnicoResponsavel);
                    command.Parameters.AddWithValue("@DataResolucao", chamado.DataResolucao ?? (object)DBNull.Value);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar chamado: {ex.Message}");
                return false;
            }
        }

        public bool RemoverChamado(int idChamado)
        {
            try
            {
                string query = "DELETE FROM Chamados WHERE IdChamado = @IdChamado";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@IdChamado", idChamado);

                    AbrirConexao();
                    int result = command.ExecuteNonQuery();
                    FecharConexao();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover chamado: {ex.Message}");
                return false;
            }
        }

        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                string query = "SELECT * FROM Chamados WHERE IdChamado = @IdChamado";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@IdChamado", idChamado);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var chamado = new Chamados
                            {
                                IdChamado = Convert.ToInt32(reader["IdChamado"]),
                                Categoria = reader["Categoria"].ToString(),
                                Contestacoes = reader["Contestacoes"]?.ToString(),
                                Prioridade = Convert.ToInt32(reader["Prioridade"]),
                                Descricao = reader["Descricao"].ToString(),
                                Afetado = Convert.ToInt32(reader["Afetado"]),
                                DataChamado = Convert.ToDateTime(reader["DataChamado"]),
                                Status = (StatusChamado)Convert.ToInt32(reader["Status"]),
                                TecnicoResponsavel = Convert.ToInt32(reader["TecnicoResponsavel"]),
                                DataResolucao = reader["DataResolucao"] != DBNull.Value ? 
                                    Convert.ToDateTime(reader["DataResolucao"]) : (DateTime?)null
                            };
                            return chamado;
                        }
                    }
                    FecharConexao();
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
                string query = "SELECT * FROM Chamados ORDER BY DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chamado = new Chamados
                            {
                                IdChamado = Convert.ToInt32(reader["IdChamado"]),
                                Categoria = reader["Categoria"].ToString(),
                                Contestacoes = reader["Contestacoes"]?.ToString(),
                                Prioridade = Convert.ToInt32(reader["Prioridade"]),
                                Descricao = reader["Descricao"].ToString(),
                                Afetado = Convert.ToInt32(reader["Afetado"]),
                                DataChamado = Convert.ToDateTime(reader["DataChamado"]),
                                Status = (StatusChamado)Convert.ToInt32(reader["Status"]),
                                TecnicoResponsavel = Convert.ToInt32(reader["TecnicoResponsavel"]),
                                DataResolucao = reader["DataResolucao"] != DBNull.Value ? 
                                    Convert.ToDateTime(reader["DataResolucao"]) : (DateTime?)null
                            };
                            chamados.Add(chamado);
                        }
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados: {ex.Message}");
            }
            return chamados;
        }

        public List<Chamados> ListarChamadosPorTecnico(int idTecnico)
        {
            var chamados = new List<Chamados>();
            try
            {
                string query = "SELECT * FROM Chamados WHERE TecnicoResponsavel = @IdTecnico ORDER BY DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@IdTecnico", idTecnico);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chamado = new Chamados
                            {
                                IdChamado = Convert.ToInt32(reader["IdChamado"]),
                                Categoria = reader["Categoria"].ToString(),
                                Contestacoes = reader["Contestacoes"]?.ToString(),
                                Prioridade = Convert.ToInt32(reader["Prioridade"]),
                                Descricao = reader["Descricao"].ToString(),
                                Afetado = Convert.ToInt32(reader["Afetado"]),
                                DataChamado = Convert.ToDateTime(reader["DataChamado"]),
                                Status = (StatusChamado)Convert.ToInt32(reader["Status"]),
                                TecnicoResponsavel = Convert.ToInt32(reader["TecnicoResponsavel"]),
                                DataResolucao = reader["DataResolucao"] != DBNull.Value ? 
                                    Convert.ToDateTime(reader["DataResolucao"]) : (DateTime?)null
                            };
                            chamados.Add(chamado);
                        }
                    }
                    FecharConexao();
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
                string query = "SELECT * FROM Chamados WHERE Status = @Status ORDER BY DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Status", (int)status);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chamado = new Chamados
                            {
                                IdChamado = Convert.ToInt32(reader["IdChamado"]),
                                Categoria = reader["Categoria"].ToString(),
                                Contestacoes = reader["Contestacoes"]?.ToString(),
                                Prioridade = Convert.ToInt32(reader["Prioridade"]),
                                Descricao = reader["Descricao"].ToString(),
                                Afetado = Convert.ToInt32(reader["Afetado"]),
                                DataChamado = Convert.ToDateTime(reader["DataChamado"]),
                                Status = (StatusChamado)Convert.ToInt32(reader["Status"]),
                                TecnicoResponsavel = Convert.ToInt32(reader["TecnicoResponsavel"]),
                                DataResolucao = reader["DataResolucao"] != DBNull.Value ? 
                                    Convert.ToDateTime(reader["DataResolucao"]) : (DateTime?)null
                            };
                            chamados.Add(chamado);
                        }
                    }
                    FecharConexao();
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
                string query = "SELECT * FROM Chamados WHERE Prioridade = @Prioridade ORDER BY DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Prioridade", prioridade);

                    AbrirConexao();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chamado = new Chamados
                            {
                                IdChamado = Convert.ToInt32(reader["IdChamado"]),
                                Categoria = reader["Categoria"].ToString(),
                                Contestacoes = reader["Contestacoes"]?.ToString(),
                                Prioridade = Convert.ToInt32(reader["Prioridade"]),
                                Descricao = reader["Descricao"].ToString(),
                                Afetado = Convert.ToInt32(reader["Afetado"]),
                                DataChamado = Convert.ToDateTime(reader["DataChamado"]),
                                Status = (StatusChamado)Convert.ToInt32(reader["Status"]),
                                TecnicoResponsavel = Convert.ToInt32(reader["TecnicoResponsavel"]),
                                DataResolucao = reader["DataResolucao"] != DBNull.Value ? 
                                    Convert.ToDateTime(reader["DataResolucao"]) : (DateTime?)null
                            };
                            chamados.Add(chamado);
                        }
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar chamados por prioridade: {ex.Message}");
            }
            return chamados;
        }

        #endregion

        #region Métodos de Relatórios

        public DataTable ObterRelatorioGeral()
        {
            var dataTable = new DataTable();
            try
            {
                string query = @"SELECT c.IdChamado, c.Categoria, c.Descricao, c.Prioridade, 
                                c.Status, c.DataChamado, c.DataResolucao,
                                f1.Nome as Afetado, f2.Nome as TecnicoResponsavel
                                FROM Chamados c
                                LEFT JOIN Funcionarios f1 ON c.Afetado = f1.Id
                                LEFT JOIN Funcionarios f2 ON c.TecnicoResponsavel = f2.Id
                                ORDER BY c.DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    AbrirConexao();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter relatório geral: {ex.Message}");
            }
            return dataTable;
        }

        public DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var dataTable = new DataTable();
            try
            {
                string query = @"SELECT c.IdChamado, c.Categoria, c.Descricao, c.Prioridade, 
                                c.Status, c.DataChamado, c.DataResolucao,
                                f1.Nome as Afetado, f2.Nome as TecnicoResponsavel
                                FROM Chamados c
                                LEFT JOIN Funcionarios f1 ON c.Afetado = f1.Id
                                LEFT JOIN Funcionarios f2 ON c.TecnicoResponsavel = f2.Id
                                WHERE c.DataChamado BETWEEN @DataInicio AND @DataFim
                                ORDER BY c.DataChamado DESC";

                using (var command = new SqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@DataInicio", dataInicio);
                    command.Parameters.AddWithValue("@DataFim", dataFim);

                    AbrirConexao();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter relatório por período: {ex.Message}");
            }
            return dataTable;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            FecharConexao();
            _connection?.Dispose();
        }

        #endregion
    }
}
