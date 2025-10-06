using SistemaChamados.Controllers;
using SistemaChamados.Data;
using SistemaChamados.Models;
using SistemaChamados.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaChamados.Forms
{
    public partial class LoginForm : Form
    {
        private FuncionariosController _funcionariosController;
        private TextBox txtEmail;
        private TextBox txtSenha;
        private Button btnLogin;
        private Label lblEmail;
        private Label lblSenha;
        private Label lblTitulo;
        private CheckBox chkMostrarSenha;

        public LoginForm()
        {
            InitializeComponent();
            InicializarControladores();
        }

        private void InicializarControladores()
        {
            try
            {
                var connectionString = DatabaseConfig.ConnectionString;
                var database = new SqlServerConnection(connectionString);
                _funcionariosController = new FuncionariosController(database);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com o banco de dados: {ex.Message}",
                    "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.txtEmail = new TextBox();
            this.txtSenha = new TextBox();
            this.btnLogin = new Button();
            this.lblEmail = new Label();
            this.lblSenha = new Label();
            this.lblTitulo = new Label();
            this.chkMostrarSenha = new CheckBox();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(80, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(240, 26);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Sistema de Chamados";

            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(50, 80);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(35, 13);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Email:";

            // txtEmail
            this.txtEmail.Location = new System.Drawing.Point(50, 100);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(300, 20);
            this.txtEmail.TabIndex = 2;
            this.txtEmail.Text = "admin@sistema.com";

            // lblSenha
            this.lblSenha.AutoSize = true;
            this.lblSenha.Location = new System.Drawing.Point(50, 140);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(41, 13);
            this.lblSenha.TabIndex = 3;
            this.lblSenha.Text = "Senha:";

            // txtSenha
            this.txtSenha.Location = new System.Drawing.Point(50, 160);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.PasswordChar = '*';
            this.txtSenha.Size = new System.Drawing.Size(300, 20);
            this.txtSenha.TabIndex = 4;
            this.txtSenha.Text = "admin123";
            this.txtSenha.KeyPress += new KeyPressEventHandler(this.txtSenha_KeyPress);

            // chkMostrarSenha
            this.chkMostrarSenha.AutoSize = true;
            this.chkMostrarSenha.Location = new System.Drawing.Point(50, 190);
            this.chkMostrarSenha.Name = "chkMostrarSenha";
            this.chkMostrarSenha.Size = new System.Drawing.Size(102, 17);
            this.chkMostrarSenha.TabIndex = 5;
            this.chkMostrarSenha.Text = "Mostrar senha";
            this.chkMostrarSenha.UseVisualStyleBackColor = true;
            this.chkMostrarSenha.CheckedChanged += new EventHandler(this.chkMostrarSenha_CheckedChanged);

            // btnLogin
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(50, 230);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(300, 35);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Entrar";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.chkMostrarSenha);
            this.Controls.Add(this.txtSenha);
            this.Controls.Add(this.lblSenha);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Login - Sistema de Chamados";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Por favor, informe o email.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSenha.Text))
                {
                    MessageBox.Show("Por favor, informe a senha.", "Campo Obrigatório",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSenha.Focus();
                    return;
                }

                btnLogin.Enabled = false;
                btnLogin.Text = "Entrando...";

                var funcionario = _funcionariosController.RealizarLogin(txtEmail.Text.Trim(), txtSenha.Text);

                if (funcionario != null)
                {
                    this.Hide();
                    var menuPrincipal = new MenuPrincipalForm(funcionario);
                    menuPrincipal.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Email ou senha incorretos.", "Erro de Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSenha.Clear();
                    txtSenha.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao realizar login: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Entrar";
            }
        }

        private void txtSenha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.PasswordChar = chkMostrarSenha.Checked ? '\0' : '*';
        }
    }
}
