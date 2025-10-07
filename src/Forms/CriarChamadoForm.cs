using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using global::SistemaChamados.Controllers;
using global::SistemaChamados.Models;

    namespace SistemaChamados.Forms
    {
        public partial class CriarChamadoForm : Form
        {
            private ChamadosController _chamadosController;
            private Funcionarios _funcionarioLogado;

            private ComboBox cmbCategoria;
            private ComboBox cmbPrioridade;
            private TextBox txtDescricao;
            private Button btnSalvar;
            private Button btnCancelar;
            private Label lblCategoria;
            private Label lblPrioridade;
            private Label lblDescricao;
            private Label lblTitulo;
            private RichTextBox rtbDescricao;

            public CriarChamadoForm(Funcionarios funcionario, ChamadosController chamadosController)
            {
                _funcionarioLogado = funcionario;
                _chamadosController = chamadosController;
                InitializeComponent();
                ConfigurarFormulario();
            }

            private void InitializeComponent()
            {
                this.cmbCategoria = new ComboBox();
                this.cmbPrioridade = new ComboBox();
                this.txtDescricao = new TextBox();
                this.rtbDescricao = new RichTextBox();
                this.btnSalvar = new Button();
                this.btnCancelar = new Button();
                this.lblCategoria = new Label();
                this.lblPrioridade = new Label();
                this.lblDescricao = new Label();
                this.lblTitulo = new Label();
                this.SuspendLayout();

                // 
                // lblTitulo
                // 
                this.lblTitulo.AutoSize = true;
                this.lblTitulo.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
                this.lblTitulo.Location = new Point(30, 20);
                this.lblTitulo.Name = "lblTitulo";
                this.lblTitulo.Size = new Size(200, 26);
                this.lblTitulo.TabIndex = 0;
                this.lblTitulo.Text = "Novo Chamado";

                // 
                // lblCategoria
                // 
                this.lblCategoria.AutoSize = true;
                this.lblCategoria.Location = new Point(30, 70);
                this.lblCategoria.Name = "lblCategoria";
                this.lblCategoria.Size = new Size(58, 13);
                this.lblCategoria.TabIndex = 1;
                this.lblCategoria.Text = "Categoria:";

                // 
                // cmbCategoria
                // 
                this.cmbCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
                this.cmbCategoria.FormattingEnabled = true;
                this.cmbCategoria.Location = new Point(30, 90);
                this.cmbCategoria.Name = "cmbCategoria";
                this.cmbCategoria.Size = new Size(250, 21);
                this.cmbCategoria.TabIndex = 2;

                // 
                // lblPrioridade
                // 
                this.lblPrioridade.AutoSize = true;
                this.lblPrioridade.Location = new Point(320, 70);
                this.lblPrioridade.Name = "lblPrioridade";
                this.lblPrioridade.Size = new Size(60, 13);
                this.lblPrioridade.TabIndex = 3;
                this.lblPrioridade.Text = "Prioridade:";

                // 
                // cmbPrioridade
                // 
                this.cmbPrioridade.DropDownStyle = ComboBoxStyle.DropDownList;
                this.cmbPrioridade.FormattingEnabled = true;
                this.cmbPrioridade.Location = new Point(320, 90);
                this.cmbPrioridade.Name = "cmbPrioridade";
                this.cmbPrioridade.Size = new Size(200, 21);
                this.cmbPrioridade.TabIndex = 4;

                // 
                // lblDescricao
                // 
                this.lblDescricao.AutoSize = true;
                this.lblDescricao.Location = new Point(30, 130);
                this.lblDescricao.Name = "lblDescricao";
                this.lblDescricao.Size = new Size(158, 13);
                this.lblDescricao.TabIndex = 5;
                this.lblDescricao.Text = "Descrição do Problema:";

                // 
                // rtbDescricao
                // 
                this.rtbDescricao.Location = new Point(30, 150);
                this.rtbDescricao.Name = "rtbDescricao";
                this.rtbDescricao.Size = new Size(490, 200);
                this.rtbDescricao.TabIndex = 6;
                this.rtbDescricao.Text = "";

                // 
                // btnSalvar
                // 
                this.btnSalvar.BackColor = Color.FromArgb(40, 167, 69);
                this.btnSalvar.FlatStyle = FlatStyle.Flat;
                this.btnSalvar.ForeColor = Color.White;
                this.btnSalvar.Location = new Point(320, 370);
                this.btnSalvar.Name = "btnSalvar";
                this.btnSalvar.Size = new Size(100, 35);
                this.btnSalvar.TabIndex = 7;
                this.btnSalvar.Text = "Criar Chamado";
                this.btnSalvar.UseVisualStyleBackColor = false;
                this.btnSalvar.Click += new EventHandler(this.btnSalvar_Click);

                // 
                // btnCancelar
                // 
                this.btnCancelar.BackColor = Color.FromArgb(108, 117, 125);
                this.btnCancelar.FlatStyle = FlatStyle.Flat;
                this.btnCancelar.ForeColor = Color.White;
                this.btnCancelar.Location = new Point(430, 370);
                this.btnCancelar.Name = "btnCancelar";
                this.btnCancelar.Size = new Size(90, 35);
                this.btnCancelar.TabIndex = 8;
                this.btnCancelar.Text = "Cancelar";
                this.btnCancelar.UseVisualStyleBackColor = false;
                this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

                // 
                // CriarChamadoForm
                // 
                this.AutoScaleDimensions = new SizeF(6F, 13F);
                this.AutoScaleMode = AutoScaleMode.Font;
                this.BackColor = Color.White;
                this.ClientSize = new Size(550, 430);
                this.Controls.Add(this.btnCancelar);
                this.Controls.Add(this.btnSalvar);
                this.Controls.Add(this.rtbDescricao);
                this.Controls.Add(this.lblDescricao);
                this.Controls.Add(this.cmbPrioridade);
                this.Controls.Add(this.lblPrioridade);
                this.Controls.Add(this.cmbCategoria);
                this.Controls.Add(this.lblCategoria);
                this.Controls.Add(this.lblTitulo);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "CriarChamadoForm";
                this.StartPosition = FormStartPosition.CenterParent;
                this.Text = "Novo Chamado - Sistema de Chamados";
                this.ResumeLayout(false);
                this.PerformLayout();
            }

            private void ConfigurarFormulario()
            {
                try
                {
                    // Configurar combo de categorias
                    cmbCategoria.Items.Clear();
                    cmbCategoria.Items.AddRange(new string[]
                    {
                    "Hardware",
                    "Software",
                    "Rede",
                    "Impressora",
                    "Email",
                    "Sistema",
                    "Telefonia",
                    "Acesso",
                    "Backup",
                    "Outro"
                    });
                    cmbCategoria.SelectedIndex = 0;

                    // Configurar combo de prioridades
                    cmbPrioridade.Items.Clear();
                    cmbPrioridade.Items.Add(new ComboBoxItem { Text = "Baixa", Value = 1 });
                    cmbPrioridade.Items.Add(new ComboBoxItem { Text = "Média", Value = 2 });
                    cmbPrioridade.Items.Add(new ComboBoxItem { Text = "Alta", Value = 3 });
                    cmbPrioridade.Items.Add(new ComboBoxItem { Text = "Crítica", Value = 4 });
                    cmbPrioridade.SelectedIndex = 1; // Média por padrão

                    // Configurar rich text box
                    rtbDescricao.Font = new Font("Segoe UI", 9F);
                    rtbDescricao.ScrollBars = RichTextBoxScrollBars.Vertical;

                    // Focar no primeiro campo
                    cmbCategoria.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao configurar formulário: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void btnSalvar_Click(object sender, EventArgs e)
            {
                try
                {
                    // Validar campos
                    if (!ValidarCampos())
                        return;

                    // Desabilitar botão durante salvamento
                    btnSalvar.Enabled = false;
                    btnSalvar.Text = "Salvando...";

                    // Obter dados do formulário
                    string categoria = cmbCategoria.Text;
                    var prioridadeItem = (ComboBoxItem)cmbPrioridade.SelectedItem;
                    int prioridade = prioridadeItem.Value;
                    string descricao = rtbDescricao.Text.Trim();

                    // Criar chamado
                    var chamado = new Chamados
                    {
                        Categoria = categoria,
                        Prioridade = prioridade,
                        Descricao = descricao,
                        Afetado = _funcionarioLogado.Id,
                        DataChamado = DateTime.Now,
                        Status = StatusChamado.Aberto
                    };

                    int idChamado = _chamadosController.CriarChamado(chamado);

                    if (idChamado > 0)
                    {
                        MessageBox.Show($"Chamado criado com sucesso!\nNúmero do chamado: {idChamado}",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Erro ao criar chamado. Tente novamente.",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar chamado: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnSalvar.Enabled = true;
                    btnSalvar.Text = "Criar Chamado";
                }
            }

            private bool ValidarCampos()
            {
                if (cmbCategoria.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecione uma categoria.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCategoria.Focus();
                    return false;
                }

                if (cmbPrioridade.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecione uma prioridade.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPrioridade.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(rtbDescricao.Text))
                {
                    MessageBox.Show("Informe a descrição do problema.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescricao.Focus();
                    return false;
                }

                if (rtbDescricao.Text.Trim().Length < 10)
                {
                    MessageBox.Show("A descrição deve ter pelo menos 10 caracteres.", "Descrição Insuficiente",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescricao.Focus();
                    return false;
                }

                return true;
            }

            private void btnCancelar_Click(object sender, EventArgs e)
            {
                if (FormFoiModificado())
                {
                    DialogResult result = MessageBox.Show("Existem dados não salvos. Deseja realmente cancelar?",
                        "Confirmar Cancelamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                        return;
                }

                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

            private bool FormFoiModificado()
            {
                return cmbCategoria.SelectedIndex != 0 ||
                       cmbPrioridade.SelectedIndex != 1 ||
                       !string.IsNullOrWhiteSpace(rtbDescricao.Text);
            }
        }
    }