using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FeirinhaCodorna.Data;

namespace FeirinhaCodorna.Forms
{
    public class FrmGerenciarUsuarios : Form
    {
        private DataGridView dgvUsuarios;
        private TextBox txtLogin, txtSenha;
        private ComboBox cmbPerfil;
        private BancoDados banco = new BancoDados();

        public FrmGerenciarUsuarios()
        {
            this.Text = "Gerenciamento de Usuários";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            txtLogin = new TextBox { PlaceholderText = "Login", Location = new Point(20, 20), Size = new Size(150, 25) };
            txtSenha = new TextBox { PlaceholderText = "Senha", Location = new Point(180, 20), Size = new Size(150, 25), PasswordChar = '•' };

            cmbPerfil = new ComboBox { Location = new Point(340, 20), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPerfil.Items.AddRange(new string[] { "Administrador", "Operador" });
            cmbPerfil.SelectedIndex = 1;

            Button btnSalvar = new Button { Text = "Salvar / Atualizar", Location = new Point(20, 60), Size = new Size(120, 30), BackColor = Color.LightGreen };
            btnSalvar.Click += BtnSalvar_Click;

            Button btnExcluir = new Button { Text = "Excluir Selecionado", Location = new Point(150, 60), Size = new Size(130, 30), BackColor = Color.Salmon };
            btnExcluir.Click += BtnExcluir_Click;

            dgvUsuarios = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(440, 280),
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.AddRange(new Control[] { txtLogin, txtSenha, cmbPerfil, btnSalvar, btnExcluir, dgvUsuarios });
            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            var lista = banco.ListarUsuarios();
            var dt = new DataTable();
            dt.Columns.Add("Login");
            dt.Columns.Add("Perfil");

            foreach (var (login, perfil) in lista)
                dt.Rows.Add(login, perfil);

            dgvUsuarios.DataSource = dt;
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Preencha Login e Senha!");
                return;
            }

            // Passa a senha pura — SalvarUsuario() já faz o hash internamente
            banco.SalvarUsuario(txtLogin.Text, txtSenha.Text, cmbPerfil.Text);
            MessageBox.Show("Usuário salvo com sucesso!");

            txtLogin.Clear();
            txtSenha.Clear();
            CarregarUsuarios();
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                string login = dgvUsuarios.SelectedRows[0].Cells["Login"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(login)) return;

                if (MessageBox.Show($"Excluir o usuário '{login}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    banco.ExcluirUsuario(login);
                    CarregarUsuarios();
                }
            }
        }
    }
}