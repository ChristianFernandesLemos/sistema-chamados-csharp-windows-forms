using System;
using System.Collections.Generic;
using System.Data;
using SistemaChamados.Models;

namespace SistemaChamados.Interfaces
{
    // Interface para operações de banco de dados
    public interface IDatabaseConnection : IDisposable
    {
        // Métodos de conexão
        bool AbrirConexao();
        void FecharConexao();
        bool TestarConexao();

        // Métodos para Funcionários
        bool InserirFuncionario(Funcionarios funcionario);
        bool AtualizarFuncionario(Funcionarios funcionario);
        bool RemoverFuncionario(int id);
        Funcionarios BuscarFuncionarioPorId(int id);
        Funcionarios BuscarFuncionarioPorEmail(string email);
        List<Funcionarios> ListarTodosFuncionarios();
        bool ValidarLogin(string email, string senha);

        // Métodos para Chamados
        bool InserirChamado(Chamados chamado);
        bool AtualizarChamado(Chamados chamado);
        bool RemoverChamado(int idChamado);
        Chamados BuscarChamadoPorId(int idChamado);
        List<Chamados> ListarTodosChamados();
        List<Chamados> ListarChamadosPorTecnico(int idTecnico);
        List<Chamados> ListarChamadosPorStatus(StatusChamado status);
        List<Chamados> ListarChamadosPorPrioridade(int prioridade);

        // Métodos de relatórios
        DataTable ObterRelatorioGeral();
        DataTable ObterRelatorioPorPeriodo(DateTime dataInicio, DateTime dataFim);
    }
}
