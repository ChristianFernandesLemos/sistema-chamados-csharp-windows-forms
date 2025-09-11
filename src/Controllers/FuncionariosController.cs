using System;
using System.Collections.Generic;
using SistemaChamados.Data;
using SistemaChamados.Interfaces;
using SistemaChamados.Models;

namespace SistemaChamados.Controllers
{
    // Controlador para gerenciar operações de funcionários
    public class FuncionariosController
    {
        private readonly IDatabaseConnection _database;

        // Construtor
        public FuncionariosController(IDatabaseConnection database)
        {
            _database = database;
        }

        // Realizar login
        public Funcionarios RealizarLogin(string email, string senha)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    throw new ArgumentException("Email e senha são obrigatórios");
                }

                if (_database.ValidarLogin(email, senha))
                {
                    return _database.BuscarFuncionarioPorEmail(email);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao realizar login: {ex.Message}");
            }
        }

        // Adicionar funcionário (apenas ADM)
        public bool AdicionarFuncionario(Funcionarios funcionarioLogado, string nome, string cpf, string email, string senha, int nivelAcesso, string tipoFuncionario)
        {
            try
            {
                // Verificar se o usuário logado é administrador
                if (!(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Apenas administradores podem adicionar funcionários");
                }

                // Verificar se já existe funcionário com este email
                var funcionarioExistente = _database.BuscarFuncionarioPorEmail(email);
                if (funcionarioExistente != null)
                {
                    throw new Exception("Já existe um funcionário com este email");
                }

                Funcionarios novoFuncionario;

                // Criar instância baseada no tipo
                switch (tipoFuncionario.ToLower())
                {
                    case "tecnico":
                        novoFuncionario = new Tecnico(0, nome, cpf, email, senha, nivelAcesso);
                        break;
                    case "adm":
                        novoFuncionario = new ADM(0, nome, cpf, email, senha, nivelAcesso);
                        break;
                    default:
                        throw new ArgumentException("Tipo de funcionário inválido. Use 'tecnico' ou 'adm'");
                }

                return _database.InserirFuncionario(novoFuncionario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar funcionário: {ex.Message}");
            }
        }

        // Alterar senha (ADM pode alterar de qualquer um, outros apenas a própria)
        public bool AlterarSenha(Funcionarios funcionarioLogado, int idFuncionario, string novaSenha)
        {
            try
            {
                if (string.IsNullOrEmpty(novaSenha))
                {
                    throw new ArgumentException("Nova senha não pode ser vazia");
                }

                // Verificar permissões
                if (funcionarioLogado.Id != idFuncionario && !(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Você só pode alterar sua própria senha");
                }

                var funcionario = _database.BuscarFuncionarioPorId(idFuncionario);
                if (funcionario == null)
                {
                    throw new Exception("Funcionário não encontrado");
                }

                funcionario.Senha = novaSenha;
                return _database.AtualizarFuncionario(funcionario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar senha: {ex.Message}");
            }
        }

        // Alterar própria senha
        public bool AlterarPropriaSenha(Funcionarios funcionarioLogado, string senhaAtual, string novaSenha)
        {
            try
            {
                if (string.IsNullOrEmpty(senhaAtual) || string.IsNullOrEmpty(novaSenha))
                {
                    throw new ArgumentException("Senha atual e nova senha são obrigatórias");
                }

                // Verificar senha atual
                if (funcionarioLogado.Senha != senhaAtual)
                {
                    throw new Exception("Senha atual incorreta");
                }

                funcionarioLogado.Senha = novaSenha;
                return _database.AtualizarFuncionario(funcionarioLogado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao alterar própria senha: {ex.Message}");
            }
        }

        // Atualizar dados do funcionário
        public bool AtualizarFuncionario(Funcionarios funcionarioLogado, Funcionarios funcionarioParaAtualizar)
        {
            try
            {
                // Verificar permissões
                if (funcionarioLogado.Id != funcionarioParaAtualizar.Id && !(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Você só pode atualizar seus próprios dados");
                }

                return _database.AtualizarFuncionario(funcionarioParaAtualizar);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar funcionário: {ex.Message}");
            }
        }

        // Remover funcionário (apenas ADM)
        public bool RemoverFuncionario(Funcionarios funcionarioLogado, int idFuncionario)
        {
            try
            {
                // Verificar se o usuário logado é administrador
                if (!(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Apenas administradores podem remover funcionários");
                }

                // Não permitir que o administrador remova a si mesmo
                if (funcionarioLogado.Id == idFuncionario)
                {
                    throw new Exception("Você não pode remover sua própria conta");
                }

                return _database.RemoverFuncionario(idFuncionario);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover funcionário: {ex.Message}");
            }
        }

        // Listar todos os funcionários (apenas ADM)
        public List<Funcionarios> ListarTodosFuncionarios(Funcionarios funcionarioLogado)
        {
            try
            {
                // Verificar se o usuário logado é administrador
                if (!(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Apenas administradores podem listar todos os funcionários");
                }

                return _database.ListarTodosFuncionarios();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar funcionários: {ex.Message}");
            }
        }

        // Listar apenas técnicos (para atribuição de chamados)
        public List<Funcionarios> ListarTecnicos()
        {
            try
            {
                var todosFuncionarios = _database.ListarTodosFuncionarios();
                return todosFuncionarios.FindAll(f => f is Tecnico);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar técnicos: {ex.Message}");
            }
        }

        // Buscar funcionário por ID
        public Funcionarios BuscarFuncionarioPorId(int id)
        {
            try
            {
                return _database.BuscarFuncionarioPorId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar funcionário: {ex.Message}");
            }
        }

        // Buscar funcionário por email
        public Funcionarios BuscarFuncionarioPorEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email é obrigatório");
                }

                return _database.BuscarFuncionarioPorEmail(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar funcionário por email: {ex.Message}");
            }
        }

        // Validar permissões de acesso
        public bool ValidarPermissao(Funcionarios funcionario, string operacao)
        {
            try
            {
                switch (operacao.ToLower())
                {
                    case "visualizar_todos_chamados":
                        return funcionario is ADM;
                    
                    case "adicionar_funcionario":
                        return funcionario is ADM;
                    
                    case "remover_funcionario":
                        return funcionario is ADM;
                    
                    case "alterar_senha_outros":
                        return funcionario is ADM;
                    
                    case "marcar_chamado_resolvido":
                        return funcionario is Tecnico || funcionario is ADM;
                    
                    case "alterar_prioridade_chamado":
                        return funcionario is Tecnico || funcionario is ADM;
                    
                    case "visualizar_proprios_chamados":
                        return true; // Todos podem visualizar seus próprios chamados
                    
                    case "criar_chamado":
                        return true; // Todos podem criar chamados
                    
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar permissão: {ex.Message}");
            }
        }

        // Obter estatísticas de funcionários (apenas ADM)
        public Dictionary<string, int> ObterEstatisticasFuncionarios(Funcionarios funcionarioLogado)
        {
            try
            {
                if (!(funcionarioLogado is ADM))
                {
                    throw new UnauthorizedAccessException("Apenas administradores podem ver estatísticas");
                }

                var estatisticas = new Dictionary<string, int>();
                var todosFuncionarios = _database.ListarTodosFuncionarios();

                estatisticas["Total"] = todosFuncionarios.Count;
                estatisticas["Administradores"] = todosFuncionarios.FindAll(f => f is ADM).Count;
                estatisticas["Tecnicos"] = todosFuncionarios.FindAll(f => f is Tecnico).Count;

                return estatisticas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter estatísticas de funcionários: {ex.Message}");
            }
        }
    }
}
