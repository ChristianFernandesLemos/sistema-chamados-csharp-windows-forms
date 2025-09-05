using System;
using System.Collections.Generic;

namespace SistemaChamados.Models
{
    // Classe base Funcionarios
    public abstract class Funcionarios
    {
        // Atributos
        public char Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int Id { get; set; }
        public int NivelAcesso { get; set; }

        // Construtor
        public Funcionarios(int id, char nome, string cpf, string email, string senha, int nivelAcesso)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            Senha = senha;
            NivelAcesso = nivelAcesso;
        }

        // Método para visualizar chamados
        public virtual void VisualizarChamados()
        {
            // Implementação base para visualizar chamados
            // Esta será sobrescrita nas classes filhas conforme necessário
        }
    }
}
