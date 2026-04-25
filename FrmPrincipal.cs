using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FeirinhaCodorna.Data;
using FeirinhaCodorna.Forms;

namespace FeirinhaCodorna
{
    public partial class Form1 : Form
    {
        private readonly BancoDados _db = new();
        private Panel painelConteudo = new();
        private FormCaixa? _formCaixa;

        public Form1()
        {
            InitializeComponent();
            ConfigurarJanela();
            CriarAreaConteudo();
            CriarMenu();
            AbrirCaixa();
        }

        private void ConfigurarJanela()
        {
            Text = "Feirinha Codorna";
            Size = new Size(1300, 720);
            MinimumSize = new Size(1100, 720);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9.5f);
            WindowState = FormWindowState.Maximized;
        }

        private void CriarMenu()
        {
            var painelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(235, 233, 228)
            };

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0)
            };

            // LOGO
            var pnlLogo = new Panel
            {
                Width = 200,
                Height = 160,
                BackColor = Color.FromArgb(235, 233, 228)
            };

            var logoPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logo_Feirinha_do_codorna.png");

            if (File.Exists(logoPath))
            {
                var pic = new PictureBox
                {
                    Image = Image.FromFile(logoPath),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };
                pnlLogo.Controls.Add(pic);
            }
            else
            {
                pnlLogo.Controls.Add(new Label
                {
                    Text = "Feirinha Codorna",
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 70, 70),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            var sep = new Panel
            {
                Width = 200,
                Height = 1,
                BackColor = Color.FromArgb(200, 200, 195),
                Margin = new Padding(0, 0, 0, 4)
            };

            layout.Controls.Add(pnlLogo);
            layout.Controls.Add(sep);

            layout.Controls.Add(RotuloSecao("VENDAS"));
            layout.Controls.Add(BotaoMenu("  Frente de Caixa", 0));

            layout.Controls.Add(RotuloSecao("CADASTROS"));
            layout.Controls.Add(BotaoMenu("  Clientes", 1));
            layout.Controls.Add(BotaoMenu("  Fornecedores", 2));
            layout.Controls.Add(BotaoMenu("  Estoque / Produtos", 3));
            layout.Controls.Add(BotaoMenu("  Funcionários", 7));

            layout.Controls.Add(RotuloSecao("FINANCEIRO"));
            layout.Controls.Add(BotaoMenu("  Despesas", 4));
            layout.Controls.Add(BotaoMenu("  Relatórios", 5));
            layout.Controls.Add(BotaoMenu("  Caixa / Turno", 6));

            layout.Controls.Add(RotuloSecao("CONFIGURAÇÕES"));
            layout.Controls.Add(BotaoMenu("  Usuários", 8)); // novo

            painelMenu.Controls.Add(layout);
            Controls.Add(painelMenu);
        }

        private Label RotuloSecao(string texto)
        {
            return new Label
            {
                Text = "  " + texto,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 150, 145),
                Width = 200,
                Height = 28,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(6, 0, 0, 2)
            };
        }

        private Button BotaoMenu(string texto, int indice)
        {
            var btn = new Button
            {
                Text = texto,
                Width = 200,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(80, 80, 75),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = indice,
                Margin = new Padding(0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(218, 216, 210);
            btn.Click += BotaoMenu_Click;

            return btn;
        }

        private void CriarAreaConteudo()
        {
            painelConteudo = new Panel
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(painelConteudo);
        }

        private void AbrirCaixa()
        {
            if (_formCaixa == null || _formCaixa.IsDisposed)
                _formCaixa = new FormCaixa(_db);

            AbrirTela(_formCaixa);
        }

        private void BotaoMenu_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            switch ((int)btn.Tag!)
            {
                case 0: AbrirCaixa(); break;
                case 1: AbrirTela(new FormClientes(_db)); break;
                case 2: AbrirTela(new FormFornecedores(_db)); break;
                case 3: AbrirTela(new FormEstoque(_db)); break;
                case 4: AbrirTela(new FormDespesas(_db)); break;
                case 5: AbrirTela(new FormRelatorio(_db)); break;
                case 6: AbrirTela(new FormGerenciamentoCaixa(_db)); break;
                case 7: AbrirTela(new FrmGerenciarFuncionarios()); break;
                case 8: AbrirTela(new FrmGerenciarUsuarios()); break; // novo
            }
        }

        public void AbrirTela(Form tela)
        {
            painelConteudo.Controls.Clear();

            tela.TopLevel = false;
            tela.FormBorderStyle = FormBorderStyle.None;
            tela.Dock = DockStyle.Fill;

            painelConteudo.Controls.Add(tela);
            tela.Show();
        }
    }
}