using System;
using System.Collections.Generic;
using System.Data;
using SistemaChamados.Data;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Controllers
{
    // Controlador para gerenciar operações de chamados
    public class ChamadosController
    {
        private readonly IDatabaseConnection _database;

        // Construtor
        public ChamadosController(IDatabaseConnection database)
        {
            _database = database;
        }

        // Criar novo chamado
        public bool CriarChamado(string categoria, string descricao, int afetado, int prioridade = 2)
        {
            try
            {
                var chamado = new Chamados
                {
                    Categoria = categoria,
                    Descricao = descricao,
                    Afetado = afetado,
                    Prioridade = prioridade
                };

                chamado.CriarChamado();
                return _database.InserirChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar chamado: {ex.Message}");
            }
        }

        // Atribuir técnico a um chamado
        public bool AtribuirTecnico(int idChamado, int idTecnico)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                {
                    throw new Exception("Chamado não encontrado");
                }

                chamado.AtribuirTecnico(idTecnico);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atribuir técnico: {ex.Message}");
            }
        }

        // Alterar prioridade do chamado
        public bool AlterarPrioridade(int idChamado, int novaPrioridade)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                {
                    throw new Exception("Chamado não encontrado");
                }

                chamado.AlterarPrioridade(novaPrioridade);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar prioridade: {ex.Message}");
            }
        }

        // Alterar status do chamado
        public bool AlterarStatus(int idChamado, StatusChamado novoStatus)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                {
                    throw new Exception("Chamado não encontrado");
                }

                chamado.AlterarStatus(novoStatus);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar status: {ex.Message}");
            }
        }

        // Marcar chamado como resolvido
        public bool MarcarComoResolvido(int idChamado, int idTecnico)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                {
                    throw new Exception("Chamado não encontrado");
                }

                // Verificar se o técnico é responsável pelo chamado
                if (chamado.TecnicoResponsavel != idTecnico)
                {
                    throw new Exception("Técnico não é responsável por este chamado");
                }

                chamado.AlterarStatus(StatusChamado.Resolvido);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao marcar chamado como resolvido: {ex.Message}");
            }
        }

        // Adicionar contestação ao chamado
        public bool AdicionarContestacao(int idChamado, string contestacao)
        {
            try
            {
                var chamado = _database.BuscarChamadoPorId(idChamado);
                if (chamado == null)
                {
                    throw new Exception("Chamado não encontrado");
                }

                chamado.AdicionarContestacao(contestacao);
                return _database.AtualizarChamado(chamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar contestação: {ex.Message}");
            }
        }

        // Listar chamados por técnico
        public List<Chamados> ListarChamadosPorTecnico(int idTecnico)
        {
            try
            {
                return _database.ListarChamadosPorTecnico(idTecnico);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar chamados por técnico: {ex.Message}");
            }
        }

        // Listar todos os chamados
        public List<Chamados> ListarTodosChamados()
        {
            try
            {
                return _database.ListarTodosChamados();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar chamados: {ex.Message}");
            }
        }

        // Listar chamados por status
        public List<Chamados> ListarChamadosPorStatus(StatusChamado status)
        {
            try
            {
                return _database.ListarChamadosPorStatus(status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar chamados por status: {ex.Message}");
            }
        }

        // Listar chamados por prioridade
        public List<Chamados> ListarChamadosPorPrioridade(int prioridade)
        {
            try
            {
                return _database.ListarChamadosPorPrioridade(prioridade);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar chamados por prioridade: {ex.Message}");
            }
        }

        // Buscar chamado por ID
        public Chamados BuscarChamadoPorId(int idChamado)
        {
            try
            {
                return _database.BuscarChamadoPorId(idChamado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar chamado: {ex.Message}");
            }
        }

        // Obter estatísticas de chamados
        public Dictionary<string, int> ObterEstatisticas()
        {
            try
            {
                var estatisticas = new Dictionary<string, int>();
                var todosChamados = _database.ListarTodosChamados();

                estatisticas["Total"] = todosChamados.Count;
                estatisticas["Abertos"] = todosChamados.FindAll(c => c.Status == StatusChamado.Aberto).Count;
                estatisticas["EmAndamento"] = todosChamados.FindAll(c => c.Status == StatusChamado.EmAndamento).Count;
                estatisticas["Resolvidos"] = todosChamados.FindAll(c => c.Status == StatusChamado.Resolvido).Count;
                estatisticas["Fechados"] = todosChamados.FindAll(c => c.Status == StatusChamado.Fechado).Count;
                estatisticas["PrioridadeAlta"] = todosChamados.FindAll(c => c.Prioridade >= 3).Count;
                estatisticas["PrioridadeCritica"] = todosChamados.FindAll(c => c.Prioridade == 4).Count;

                return estatisticas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter estatísticas: {ex.Message}");
            }
        }

        // Obter relatório geral
        public DataTable ObterRelatorioGeral()
        {
            try
            {
                return _database.ObterRelatorioGeral();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter relatório: {ex.Message}");
            }
        }

        // Obter relatório por período
        public DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                return _database.ObterRelatorioPorPeriodo(dataInicio, dataFim);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter relatório por período: {ex.Message}");
            }
        }
    }
}
