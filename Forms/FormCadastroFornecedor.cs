using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormCadastroFornecedor : Form
    {
        private readonly BancoDados _db;
        private readonly Fornecedor _fornecedor;

        private TextBox txtNome = new();
        private TextBox txtCnpj = new();
        private TextBox txtTelefone = new();
        private TextBox txtEndereco = new();
        private TextBox txtNumero = new();
        private TextBox txtCidade = new();
        private TextBox txtEstado = new();
        private TextBox txtCep = new();
        private TextBox txtProdutos = new();
        private CheckBox chkAtivo = new();

        public FormCadastroFornecedor(BancoDados db, Fornecedor? fornecedor)
        {
            _db = db;
            _fornecedor = fornecedor ?? new Fornecedor { Ativo = true };
            ConstruirTela();
            PreencherCampos();
        }

        private void ConstruirTela()
        {
            Text = "Cadastro de Fornecedor";
            Size = new Size(480, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9.5f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(16),
                AutoSize = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            void AddCampo(string label, Control ctrl)
            {
                layout.Controls.Add(new Label
                {
                    Text = label,
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.FromArgb(80, 80, 75)
                });
                ctrl.Dock = DockStyle.Fill;
                layout.Controls.Add(ctrl);
            }

            txtNome.MaxLength = 100;
            txtCnpj.MaxLength = 20;
            txtTelefone.MaxLength = 20;
            txtEndereco.MaxLength = 200;
            txtNumero.MaxLength = 20;
            txtCidade.MaxLength = 100;
            txtEstado.MaxLength = 2;
            txtCep.MaxLength = 10;
            txtProdutos.MaxLength = 200;
            chkAtivo.Text = "Ativo";
            chkAtivo.Checked = true;

            AddCampo("Nome *", txtNome);
            AddCampo("CNPJ/CPF", txtCnpj);
            AddCampo("Telefone", txtTelefone);
            AddCampo("Endereço", txtEndereco);
            AddCampo("Número", txtNumero);
            AddCampo("Cidade", txtCidade);
            AddCampo("Estado", txtEstado);
            AddCampo("CEP", txtCep);
            AddCampo("Produtos", txtProdutos);
            AddCampo("", chkAtivo);

            // ── Botões ─────────────────────────────────────────────
            var pnlBotoes = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 46,
                Padding = new Padding(8),
                BackColor = Color.FromArgb(235, 233, 228)
            };

            var btnSalvar = new Button
            {
                Text = "Salvar",
                Width = 100,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(80, 140, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnSalvar.Click += Salvar;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Width = 100,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(180, 80, 80),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            pnlBotoes.Controls.Add(btnSalvar);
            pnlBotoes.Controls.Add(btnCancelar);

            Controls.Add(layout);
            Controls.Add(pnlBotoes);
        }

        private void PreencherCampos()
        {
            txtNome.Text = _fornecedor.Nome;
            txtCnpj.Text = _fornecedor.CnpjCpf;
            txtTelefone.Text = _fornecedor.Telefone;
            txtEndereco.Text = _fornecedor.Endereco;
            txtNumero.Text = _fornecedor.Numero;
            txtCidade.Text = _fornecedor.Cidade;
            txtEstado.Text = _fornecedor.Estado;
            txtCep.Text = _fornecedor.Cep;
            txtProdutos.Text = _fornecedor.Produtos;
            chkAtivo.Checked = _fornecedor.Ativo;
        }

        private void Salvar(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome é obrigatório.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            _fornecedor.Nome = txtNome.Text.Trim();
            _fornecedor.CnpjCpf = txtCnpj.Text.Trim();
            _fornecedor.Telefone = txtTelefone.Text.Trim();
            _fornecedor.Endereco = txtEndereco.Text.Trim();
            _fornecedor.Numero = txtNumero.Text.Trim();
            _fornecedor.Cidade = txtCidade.Text.Trim();
            _fornecedor.Estado = txtEstado.Text.Trim();
            _fornecedor.Cep = txtCep.Text.Trim();
            _fornecedor.Produtos = txtProdutos.Text.Trim();
            _fornecedor.Ativo = chkAtivo.Checked;

            _db.SalvarFornecedor(_fornecedor);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}