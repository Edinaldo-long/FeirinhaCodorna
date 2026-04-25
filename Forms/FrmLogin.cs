using System;
using System.Drawing;
using System.Windows.Forms;
using FeirinhaCodorna.Data;

namespace FeirinhaCodorna
{
    public partial class FrmLogin : Form
    {
        private TextBox txtLogin;
        private TextBox txtSenha;
        private Button btnEntrar;
        private BancoDados banco = new BancoDados();

        public FrmLogin()
        {
            this.Text = "Login - Feirinha Codorna";
            this.Size = new Size(300, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            Label lblLogin = new Label { Text = "Usuário:", Location = new Point(50, 20), AutoSize = true };
            Label lblSenha = new Label { Text = "Senha:", Location = new Point(50, 80), AutoSize = true };

            txtLogin = new TextBox { Location = new Point(50, 45), Size = new Size(180, 25), TabIndex = 0 };
            txtSenha = new TextBox { Location = new Point(50, 105), Size = new Size(180, 25), PasswordChar = '•', TabIndex = 1 };

            btnEntrar = new Button { Text = "Entrar", Location = new Point(100, 150), Size = new Size(80, 30), TabIndex = 2 };
            btnEntrar.Click += BtnEntrar_Click;
            btnEntrar.BackColor = Color.LightGreen;

            // SEM AcceptButton — controlamos o Enter manualmente em cada campo

            // Enter no usuário → foca na senha
            txtLogin.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtSenha.Focus();
                }
            };

            // Enter na senha → faz o login
            txtSenha.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnEntrar_Click(null, EventArgs.Empty);
                }
            };

            this.Controls.Add(lblLogin);
            this.Controls.Add(txtLogin);
            this.Controls.Add(lblSenha);
            this.Controls.Add(txtSenha);
            this.Controls.Add(btnEntrar);
        }

        private void BtnEntrar_Click(object? sender, EventArgs e)
        {
            string? perfil = banco.ValidarLogin(txtLogin.Text, txtSenha.Text);

            if (perfil != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorretos!", "Erro de Acesso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSenha.Clear();
                txtSenha.Focus();
            }
        }
    }
}