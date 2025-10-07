using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaChamados.Controllers;
using SistemaChamados.Models;

namespace SistemaChamados.Forms
{
    public partial class DetalhesChamadoForm : Form
    {
        private Chamados _chamado;
        private Funcionarios _funcionarioLogado;
        private ChamadosController _controller;
        private TextBox txtDescricao;
        private TextBox txtContestacoes;
        private Label lblId, lblCategoria, lblStatus, lblPrioridade, lblData, lblSolicitante;
        private Button btnFechar;
        private Button btnAlterar;

        public DetalhesChamadoForm(Chamados chamado, Funcionarios funcionarioLogado, ChamadosController controller)
        {
            _chamado = chamado;
            _funcionarioLogado = funcionarioLogado;
            _controller = controller;
            InitializeComponent();
            PreencherDados();
        }

        private void InitializeComponent()
        {
            var lblTitulo = new Label
            {
                Text = "Detalhes do Chamado",
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                Location = new Point(12, 15),
                Size = new Size(300, 25)
            };

            // Informações básicas
            this.lblId = new Label { Location = new Point(12, 50), Size = new Size(200, 20) };
            this.lblCategoria = new Label { Location = new Point(12, 75), Size = new Size(200, 20) };
            this.lblStatus = new Label { Location = new Point(12, 100), Size = new Size(200, 20) };
            this.lblPrioridade = new Label { Location = new Point(250, 50), Size = new Size(200, 20) };
            this.lblData = new Label { Location = new Point(250, 75), Size = new Size(200, 20) };
            this.lblSolicitante = new Label { Location = new Point(250, 100), Size = new Size(200, 20) };

            var lblDescricao = new Label
            {
                Text = "Descrição:",
                Location = new Point(12, 130),
                Size = new Size(100, 15)
            };

            this.txtDescricao = new TextBox
            {
                Location = new Point(12, 150),
                Size = new Size(460, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White
            };

            var lblContestacoes = new Label
            {
                Text = "Contestações:",
                Location = new Point(12, 260),
                Size = new Size(100, 15)
            };

            this.txtContestacoes = new TextBox
            {
                Location = new Point(12, 280),
                Size = new Size(460, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White
            };

            this.btnAlterar = new Button
            {
                Text = "Alterar Status",
                Location = new Point(317, 390),
                Size = new Size(75, 30),
                Visible = _funcionarioLogado.NivelAcesso >= 2
            };
            this.btnAlterar.Click += BtnAlterar_Click;

            this.btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(397, 390),
                Size = new Size(75, 30)
            };
            this.btnFechar.Click += BtnFechar_Click;

            this.Text = "Detalhes do Chamado";
            this.Size = new Size(500, 470);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.AddRange(new Control[] {
                lblTitulo, this.lblId, this.lblCategoria, this.lblStatus,
                this.lblPrioridade, this.lblData, this.lblSolicitante,
                lblDescricao, this.txtDescricao,
                lblContestacoes, this.txtContestacoes,
                this.btnAlterar, this.btnFechar
            });
        }

        private void PreencherDados()
        {
            lblId.Text = $"ID: #{_chamado.IdChamado}";
            lblCategoria.Text = $"Categoria: {_chamado.Categoria}";
            lblStatus.Text = $"Status: {ObterTextoStatus((int)_chamado.Status)}";
            lblPrioridade.Text = $"Prioridade: {ObterTextoPrioridade(_chamado.Prioridade)}";
            lblData.Text = $"Data: {_chamado.DataChamado:dd/MM/yyyy HH:mm}";
            lblSolicitante.Text = $"Solicitante: ID {_chamado.Afetado}";
            txtDescricao.Text = _chamado.Descricao;
            txtContestacoes.Text = string.IsNullOrEmpty(_chamado.Contestacoes) ? 
                "Nenhuma contestação registrada." : _chamado.Contestacoes;
        }

        private string ObterTextoStatus(int status)
        {
            switch (status)
            {
                case 1: return "Aberto";
                case 2: return "Em Andamento";
                case 3: return "Resolvido";
                case 4: return "Fechado";
                case 5: return "Cancelado";
                default: return "Desconhecido";
            }
        }

        private string ObterTextoPrioridade(int prioridade)
        {
            switch (prioridade)
            {
                case 1: return "Baixa";
                case 2: return "Média";
                case 3: return "Alta";
                case 4: return "Crítica";
                default: return "Desconhecida";
            }
        }

        private void BtnAlterar_Click(object sender, EventArgs e)
        {
            var formStatus = new AlterarStatusForm();
            if (formStatus.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _controller.AlterarStatus(_chamado.IdChamado, formStatus.StatusSelecionado);
                    MessageBox.Show("Status alterado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao alterar status: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
