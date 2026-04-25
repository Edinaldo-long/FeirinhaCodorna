using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormEstoque : Form
    {
        private readonly BancoDados _db;
        private ListView lstProdutos = new();
        private TextBox txtBusca = new();
        private Button btnNovo = new(), btnEditar = new(), btnExcluir = new(), btnEntrada = new();

        public FormEstoque(BancoDados db)
        {
            _db = db;
            MontarLayout();
            CarregarProdutos();
        }

        private void MontarLayout()
        {
            Text = "Estoque / Produtos";
            BackColor = Color.FromArgb(245, 245, 242);

            var pnlTopo = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };

            txtBusca.PlaceholderText = "BUSCAR POR NOME OU CÓDIGO...";
            txtBusca.Width = 260;
            txtBusca.Location = new Point(8, 8);
            txtBusca.CharacterCasing = CharacterCasing.Upper;
            txtBusca.TextChanged += (s, e) => CarregarProdutos(txtBusca.Text);

            void EstiloBtn(Button b, Color cor, string txt, int x, int w = 100)
            {
                b.Text = txt; b.Location = new Point(x, 6); b.Width = w; b.Height = 30;
                b.BackColor = cor; b.ForeColor = Color.White;
                b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0;
                b.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold); b.Cursor = Cursors.Hand;
            }
            EstiloBtn(btnNovo, Color.FromArgb(29, 158, 117), "+ NOVO", 276);
            EstiloBtn(btnEntrada, Color.FromArgb(24, 95, 165), "ENTRADA", 384, 90);
            EstiloBtn(btnEditar, Color.FromArgb(100, 100, 100), "EDITAR", 482, 90);
            EstiloBtn(btnExcluir, Color.FromArgb(163, 45, 45), "EXCLUIR", 580, 90);

            btnNovo.Click += (s, e) => AbrirFormulario(null);
            btnEditar.Click += (s, e) => EditarSelecionado();
            btnExcluir.Click += (s, e) => ExcluirSelecionado();
            btnEntrada.Click += (s, e) => EntradaEstoque();

            pnlTopo.Controls.AddRange(new Control[] { txtBusca, btnNovo, btnEntrada, btnEditar, btnExcluir });

            lstProdutos.Dock = DockStyle.Fill;
            lstProdutos.View = View.Details;
            lstProdutos.FullRowSelect = true;
            lstProdutos.GridLines = true;
            lstProdutos.Font = new Font("Segoe UI", 9.5f);
            lstProdutos.DoubleClick += (s, e) => EditarSelecionado();

            lstProdutos.Columns.Add("EAN", 110);
            lstProdutos.Columns.Add("NOME", 220);
            lstProdutos.Columns.Add("UNIDADE", 70);
            lstProdutos.Columns.Add("PREÇO", 90);
            lstProdutos.Columns.Add("ESTOQUE", 90);
            lstProdutos.Columns.Add("MÍN.", 70);
            lstProdutos.Columns.Add("SITUAÇÃO", 100);
            lstProdutos.Columns.Add("PESÁVEL", 70);

            Controls.AddRange(new Control[] { lstProdutos, pnlTopo });
        }

        private void CarregarProdutos(string filtro = "")
        {
            lstProdutos.Items.Clear();
            var lista = _db.ListarProdutos();
            if (!string.IsNullOrWhiteSpace(filtro))
                lista = lista.Where(p =>
                    p.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    p.CodigoEan.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    p.CodigoInterno.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var p in lista)
            {
                var item = new ListViewItem(p.CodigoEan);
                item.SubItems.Add(p.Nome);
                item.SubItems.Add(p.Unidade);
                item.SubItems.Add($"R$ {p.Preco:F2}");
                item.SubItems.Add($"{p.Estoque:F3}");
                item.SubItems.Add($"{p.EstoqueMinimo:F3}");
                item.SubItems.Add(p.EstoqueBaixo ? "⚠ BAIXO" : "OK");
                item.SubItems.Add(p.Pesavel ? "SIM" : "NÃO");
                item.Tag = p;
                if (p.EstoqueBaixo) item.ForeColor = Color.FromArgb(163, 45, 45);
                lstProdutos.Items.Add(item);
            }
        }

        private void EditarSelecionado()
        {
            if (lstProdutos.SelectedItems.Count == 0) return;
            AbrirFormulario((Produto)lstProdutos.SelectedItems[0].Tag!);
        }

        private void ExcluirSelecionado()
        {
            if (lstProdutos.SelectedItems.Count == 0) return;
            var p = (Produto)lstProdutos.SelectedItems[0].Tag!;
            if (MessageBox.Show($"Excluir \"{p.Nome}\"?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _db.ExcluirProduto(p.Id);
                CarregarProdutos(txtBusca.Text);
            }
        }

        private void EntradaEstoque()
        {
            if (lstProdutos.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione um produto para registrar entrada.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var p = (Produto)lstProdutos.SelectedItems[0].Tag!;
            using var dlg = new FormEntradaEstoque(p);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.Quantidade > 0)
            {
                _db.EntrarEstoque(p.Id, dlg.Quantidade);
                CarregarProdutos(txtBusca.Text);
                MessageBox.Show($"Entrada de {dlg.Quantidade:F3} {p.Unidade} registrada para \"{p.Nome}\".",
                    "Entrada registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AbrirFormulario(Produto? produto)
        {
            var fornecedores = _db.ListarFornecedores();
            using var dlg = new FormCadastroProduto(produto, fornecedores);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ProdutoEditado != null)
            {
                _db.SalvarProduto(dlg.ProdutoEditado);
                CarregarProdutos(txtBusca.Text);
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Diálogo entrada de estoque
    // ══════════════════════════════════════════════════════════════════
    public class FormEntradaEstoque : Form
    {
        public decimal Quantidade { get; private set; }
        private TextBox txtQtd = new();

        public FormEntradaEstoque(Produto p)
        {
            Text = $"ENTRADA — {p.Nome}";
            Size = new Size(320, 160);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 245, 242);

            var lbl = new Label
            {
                Text = $"QUANTIDADE A ENTRAR ({p.Unidade}):",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(12, 16),
                AutoSize = true
            };
            txtQtd.Location = new Point(12, 38); txtQtd.Width = 280;
            txtQtd.Font = new Font("Segoe UI", 11f);
            txtQtd.Text = "0";

            var btnOk = new Button
            {
                Text = "CONFIRMAR",
                Location = new Point(12, 72),
                Width = 130,
                Height = 32,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) =>
            {
                if (!decimal.TryParse(txtQtd.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal q) || q <= 0)
                {
                    MessageBox.Show("Informe uma quantidade válida.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
                Quantidade = q;
            };

            var btnCancelar = new Button
            {
                Text = "CANCELAR",
                Location = new Point(162, 72),
                Width = 130,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 9f)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            Controls.AddRange(new Control[] { lbl, txtQtd, btnOk, btnCancelar });
        }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Diálogo cadastro / edição de produto
    // ══════════════════════════════════════════════════════════════════
    public class FormCadastroProduto : Form
    {
        public Produto? ProdutoEditado { get; private set; }

        private TextBox txtEan, txtNome, txtPreco, txtEstoque, txtEstMin;
        private ComboBox cmbUnidade, cmbFornecedor;
        private CheckBox chkPesavel;
        private readonly List<Fornecedor> _fornecedores;
        private readonly Produto? _produtoOriginal;

        public FormCadastroProduto(Produto? produto, List<Fornecedor> fornecedores)
        {
            _fornecedores = fornecedores;
            _produtoOriginal = produto;
            Text = produto == null ? "NOVO PRODUTO" : "EDITAR PRODUTO";
            Size = new Size(480, 420);
            MinimumSize = new Size(480, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var pnl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 12, 16, 4),
                ColumnCount = 2,
                RowCount = 9
            };
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 9; i++)
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

            txtEan = Campo(pnl, 0, "EAN (CÓD. BARRAS):");
            txtNome = Campo(pnl, 1, "NOME *:");
            txtPreco = Campo(pnl, 2, "PREÇO R$:");
            txtEstoque = Campo(pnl, 3, "ESTOQUE ATUAL:");
            txtEstMin = Campo(pnl, 4, "ESTOQUE MÍNIMO:");

            // Unidade
            pnl.Controls.Add(Rotulo("UNIDADE:"), 0, 5);
            cmbUnidade = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbUnidade.Items.AddRange(new object[] { "kg", "un", "l", "ml", "g", "cx", "pct" });
            cmbUnidade.SelectedIndex = 0;
            pnl.Controls.Add(cmbUnidade, 1, 5);

            // Fornecedor
            pnl.Controls.Add(Rotulo("FORNECEDOR:"), 0, 6);
            cmbFornecedor = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFornecedor.Items.Add("(Nenhum)");
            foreach (var f in fornecedores) cmbFornecedor.Items.Add(f.Nome);
            cmbFornecedor.SelectedIndex = 0;
            pnl.Controls.Add(cmbFornecedor, 1, 6);

            // Pesável
            pnl.Controls.Add(Rotulo("PESÁVEL:"), 0, 7);
            chkPesavel = new CheckBox
            {
                Text = "Vendido por peso",
                Font = new Font("Segoe UI", 9.5f),
                Dock = DockStyle.Fill,
                Checked = true
            };
            pnl.Controls.Add(chkPesavel, 1, 7);

            // Botões
            var pnlBotoes = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(8)
            };
            var btnSalvar = new Button
            {
                Text = "SALVAR",
                Width = 110,
                Height = 34,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnSalvar.FlatAppearance.BorderSize = 0;
            var btnCancelar = new Button
            {
                Text = "CANCELAR",
                Width = 110,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 9f)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnSalvar.Click += (s, e) => Salvar();
            pnlBotoes.Controls.AddRange(new Control[] { btnSalvar, btnCancelar });

            Controls.AddRange(new Control[] { pnl, pnlBotoes });

            if (produto != null)
            {
                txtEan.Text = produto.CodigoEan;
                txtNome.Text = produto.Nome;
                txtPreco.Text = produto.Preco.ToString("F2");
                txtEstoque.Text = produto.Estoque.ToString("F3");
                txtEstMin.Text = produto.EstoqueMinimo.ToString("F3");
                chkPesavel.Checked = produto.Pesavel;
                int uIdx = cmbUnidade.Items.IndexOf(produto.Unidade);
                if (uIdx >= 0) cmbUnidade.SelectedIndex = uIdx;
                if (produto.FornecedorId > 0)
                {
                    int fIdx = fornecedores.FindIndex(f => f.Id == produto.FornecedorId);
                    if (fIdx >= 0) cmbFornecedor.SelectedIndex = fIdx + 1;
                }
                // Estoque não editável diretamente — use "Entrada"
                txtEstoque.BackColor = Color.FromArgb(235, 235, 232);
                txtEstoque.ReadOnly = true;
            }
            else
            {
                txtPreco.Text = "0,00";
                txtEstoque.Text = "0,000";
                txtEstMin.Text = "5,000";
            }
        }

        private Label Rotulo(string txt) => new Label
        {
            Text = txt,
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        private TextBox Campo(TableLayoutPanel pnl, int row, string rotulo)
        {
            pnl.Controls.Add(Rotulo(rotulo), 0, row);
            var txt = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                CharacterCasing = CharacterCasing.Upper
            };
            pnl.Controls.Add(txt, 1, row);
            return txt;
        }

        private void Salvar()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome do produto é obrigatório.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; return;
            }
            if (!decimal.TryParse(txtPreco.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal preco))
            {
                MessageBox.Show("Preço inválido.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; return;
            }
            decimal.TryParse(txtEstoque.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal estoque);
            decimal.TryParse(txtEstMin.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal estMin);

            int fornId = cmbFornecedor.SelectedIndex > 0
                ? _fornecedores[cmbFornecedor.SelectedIndex - 1].Id : 0;

            ProdutoEditado = new Produto
            {
                Id = _produtoOriginal?.Id ?? 0,
                CodigoEan = txtEan.Text.Trim(),
                Nome = txtNome.Text.Trim(),
                Preco = preco,
                Estoque = estoque,
                EstoqueMinimo = estMin,
                Unidade = cmbUnidade.SelectedItem?.ToString() ?? "kg",
                Pesavel = chkPesavel.Checked,
                FornecedorId = fornId
            };
        }
    }
}