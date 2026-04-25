using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormPagamentoMisto : Form
    {
        // ── resultado público ────────────────────────────────────────
        public List<ParcialPagamento> Pagamentos { get; } = new();
        public Cliente? ClienteSelecionado => _cliente;

        // ── estado interno ───────────────────────────────────────────
        private readonly decimal _total;
        private readonly BancoDados _db;
        private Cliente? _cliente;
        private readonly List<ParcialPagamento> _parcelas = new();

        // ── controles ────────────────────────────────────────────────
        private Label lblRestante = null!;
        private Label lblAviso = null!;
        private NumericUpDown numValor = null!;
        private ComboBox cmbForma = null!;
        private Button btnAdicionar = null!;
        private DataGridView grid = null!;
        private Button btnConfirmar = null!;
        private Button btnCancelar = null!;

        // painel de cliente
        private Panel pnlCliente = null!;
        private Label lblClienteInfo = null!;
        private Button btnBuscarCli = null!;
        private Button btnLimparCli = null!;

        // ── cores ────────────────────────────────────────────────────
        private static readonly Color Verde = Color.FromArgb(29, 158, 117);
        private static readonly Color Laranja = Color.FromArgb(230, 126, 34);
        private static readonly Color Fundo = Color.FromArgb(245, 245, 242);
        private static readonly Color Panel2 = Color.FromArgb(235, 233, 228);

        public FormPagamentoMisto(decimal total, Cliente? clienteInicial, BancoDados db)
        {
            _total = total;
            _cliente = clienteInicial;
            _db = db;
            Build();
        }

        // ─────────────────────────────────────────────────────────────
        // INTERFACE
        // ─────────────────────────────────────────────────────────────
        private void Build()
        {
            Text = "Pagamento Misto";
            Size = new Size(520, 600);
            MinimumSize = new Size(520, 600);
            MaximumSize = new Size(520, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Fundo;
            Font = new Font("Segoe UI", 9.5f);

            // ── cabeçalho ─────────────────────────────────────────
            var topo = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Verde };
            topo.Controls.Add(new Label
            {
                Text = $"Pagamento  —  Total: R$ {_total:F2}",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            Controls.Add(topo);

            int y = 82;

            // ── painel cliente ────────────────────────────────────
            pnlCliente = new Panel
            {
                Location = new Point(16, y),
                Size = new Size(486, 46),
                BackColor = Color.FromArgb(255, 250, 230),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblClienteInfo = new Label
            {
                Location = new Point(8, 4),
                Size = new Size(280, 36),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 70, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnBuscarCli = new Button
            {
                Text = "🔍 Selecionar cliente",
                Location = new Point(294, 6),
                Size = new Size(178, 32),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Laranja,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscarCli.FlatAppearance.BorderSize = 0;
            btnBuscarCli.Click += BtnBuscarCli_Click;

            btnLimparCli = new Button
            {
                Text = "✕ Remover",
                Location = new Point(294, 6),
                Size = new Size(90, 32),
                Font = new Font("Segoe UI", 8.5f),
                FlatStyle = FlatStyle.Flat,
                BackColor = Panel2,
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnLimparCli.FlatAppearance.BorderSize = 0;
            btnLimparCli.Click += (s, e) => DefinirCliente(null);

            pnlCliente.Controls.AddRange(new Control[] { lblClienteInfo, btnBuscarCli, btnLimparCli });
            Controls.Add(pnlCliente);
            y += 54;

            // ── restante ──────────────────────────────────────────
            lblRestante = new Label
            {
                Location = new Point(16, y),
                Width = 486,
                Height = 28,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };
            Controls.Add(lblRestante);
            y += 34;

            // ── linha de entrada ──────────────────────────────────
            var pnlEntrada = new Panel
            {
                Location = new Point(16, y),
                Size = new Size(486, 50),
                BackColor = Panel2
            };

            cmbForma = new ComboBox
            {
                Location = new Point(8, 10),
                Width = 130,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f)
            };
            cmbForma.Items.AddRange(new object[]
                { "Dinheiro", "Débito", "Crédito", "Pix", "Caderneta" });
            cmbForma.SelectedIndex = 0;
            cmbForma.SelectedIndexChanged += (s, e) => OnFormaChanged();

            numValor = new NumericUpDown
            {
                Location = new Point(150, 10),
                Width = 150,
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 99999,
                Increment = 0.01m,
                Font = new Font("Segoe UI", 12f),
                TextAlign = HorizontalAlignment.Right
            };

            btnAdicionar = new Button
            {
                Text = "+ Adicionar",
                Location = new Point(312, 8),
                Size = new Size(130, 34),
                BackColor = Verde,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdicionar.FlatAppearance.BorderSize = 0;
            btnAdicionar.Click += BtnAdicionar_Click;

            pnlEntrada.Controls.AddRange(new Control[] { cmbForma, numValor, btnAdicionar });
            Controls.Add(pnlEntrada);
            y += 58;

            // ── aviso ─────────────────────────────────────────────
            lblAviso = new Label
            {
                Location = new Point(16, y),
                Width = 486,
                Height = 22,
                ForeColor = Color.Crimson,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic)
            };
            Controls.Add(lblAviso);
            y += 26;

            // ── grid ──────────────────────────────────────────────
            grid = new DataGridView
            {
                Location = new Point(16, y),
                Size = new Size(486, 150),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f),
                ColumnHeadersHeight = 32,
                RowTemplate = { Height = 32 }
            };
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Panel2;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            grid.EnableHeadersVisualStyles = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Forma",
                Width = 130,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Valor",
                Width = 290,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            grid.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "",
                Width = 40,
                Text = "✕",
                UseColumnTextForButtonValue = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                FlatStyle = FlatStyle.Flat
            });
            grid.CellContentClick += Grid_CellContentClick;
            Controls.Add(grid);
            y += 158;

            // ── botões de ação ────────────────────────────────────
            btnConfirmar = new Button
            {
                Text = "✔  Confirmar Pagamento",
                Location = new Point(16, y),
                Size = new Size(230, 42),
                BackColor = Verde,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;
            Controls.Add(btnConfirmar);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(272, y),
                Size = new Size(130, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand,
                BackColor = Panel2,
                ForeColor = Color.FromArgb(80, 80, 75)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancelar);

            var btnAtalho = new Button
            {
                Text = "↓ Preencher\nrestante",
                Location = new Point(412, y),
                Size = new Size(90, 42),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 7.5f),
                Cursor = Cursors.Hand,
                BackColor = Panel2,
                ForeColor = Color.FromArgb(80, 80, 75),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnAtalho.FlatAppearance.BorderSize = 0;
            btnAtalho.Click += (s, e) => numValor.Value = Math.Max(0, Restante());
            Controls.Add(btnAtalho);

            DefinirCliente(_cliente);
            AtualizarUI();
        }

        // ─────────────────────────────────────────────────────────────
        // CLIENTE
        // ─────────────────────────────────────────────────────────────
        private void BtnBuscarCli_Click(object? sender, EventArgs e)
        {
            using var dlg = new FormBuscaCliente(_db);
            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.ClienteSelecionado != null)
                DefinirCliente(dlg.ClienteSelecionado);
        }

        private void DefinirCliente(Cliente? c)
        {
            _cliente = c;

            if (c == null)
            {
                lblClienteInfo.Text = "Nenhum cliente selecionado";
                lblClienteInfo.Size = new Size(280, 36);
                btnBuscarCli.Visible = true;
                btnLimparCli.Visible = false;
                // remove eventuais parcelas de caderneta
                _parcelas.RemoveAll(p => p.Forma == FormaPagamento.Fiado);
            }
            else
            {
                decimal disponivel = c.LimiteFiado - c.SaldoFiado;
                lblClienteInfo.Text = $"{c.Nome}  |  Caderneta: R$ {disponivel:F2} disponível";
                lblClienteInfo.Size = new Size(280, 36);
                btnBuscarCli.Visible = false;
                btnLimparCli.Visible = true;
            }

            lblAviso.Text = "";
            pnlCliente.BackColor = Color.FromArgb(255, 250, 230);
            AtualizarUI();
        }

        // ─────────────────────────────────────────────────────────────
        // LÓGICA DE PARCELAS
        // ─────────────────────────────────────────────────────────────
        private decimal Restante() => _total - _parcelas.Sum(p => p.Valor);

        private void OnFormaChanged()
        {
            // destaca painel quando seleciona caderneta sem cliente
            bool alerta = cmbForma.Text == "Caderneta" && _cliente == null;
            pnlCliente.BackColor = alerta
                ? Color.FromArgb(255, 220, 150)
                : Color.FromArgb(255, 250, 230);
        }

        private void BtnAdicionar_Click(object? sender, EventArgs e)
        {
            decimal valor = numValor.Value;
            if (valor <= 0) { MostrarAviso("Informe um valor maior que zero."); return; }

            decimal restante = Restante();
            if (valor > restante + 0.005m)
            {
                MostrarAviso($"Valor excede o restante (R$ {restante:F2}). Ajuste.");
                return;
            }

            var forma = cmbForma.Text switch
            {
                "Dinheiro" => FormaPagamento.Dinheiro,
                "Débito" => FormaPagamento.CartaoDebito,
                "Crédito" => FormaPagamento.CartaoCredito,
                "Pix" => FormaPagamento.Pix,
                "Caderneta" => FormaPagamento.Fiado,
                _ => FormaPagamento.Dinheiro
            };

            if (forma == FormaPagamento.Fiado)
            {
                if (_cliente == null)
                {
                    MostrarAviso("Selecione um cliente para usar a caderneta →");
                    PiscarBotao(btnBuscarCli);
                    return;
                }
                decimal disponivel = _cliente.LimiteFiado - _cliente.SaldoFiado
                    - _parcelas.Where(p => p.Forma == FormaPagamento.Fiado).Sum(p => p.Valor);
                if (valor > disponivel + 0.005m)
                {
                    MostrarAviso($"Limite insuficiente. Disponível: R$ {disponivel:F2}");
                    return;
                }
            }

            var existente = _parcelas.FirstOrDefault(p => p.Forma == forma);
            if (existente != null) existente.Valor += valor;
            else _parcelas.Add(new ParcialPagamento { Forma = forma, Valor = valor });

            lblAviso.Text = "";
            numValor.Value = 0;
            AtualizarUI();
        }

        private async void PiscarBotao(Button btn)
        {
            Color original = btn.BackColor;
            for (int i = 0; i < 3; i++)
            {
                btn.BackColor = Color.White;
                await Task.Delay(120);
                btn.BackColor = original;
                await Task.Delay(120);
            }
        }

        private void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                _parcelas.RemoveAt(e.RowIndex);
                AtualizarUI();
            }
        }

        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            if (Math.Abs(Restante()) > 0.01m)
            {
                MostrarAviso("O total das parcelas não fecha com o valor da venda.");
                return;
            }
            Pagamentos.AddRange(_parcelas);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AtualizarUI()
        {
            decimal restante = Restante();

            lblRestante.Text = restante > 0.005m
                ? $"Falta distribuir: R$ {restante:F2}"
                : "✔  Tudo distribuído";
            lblRestante.ForeColor = restante > 0.005m
                ? Color.FromArgb(180, 80, 0)
                : Color.FromArgb(20, 120, 60);

            grid.Rows.Clear();
            foreach (var p in _parcelas)
            {
                string formaTexto = p.Forma switch
                {
                    FormaPagamento.Dinheiro => "💵 Dinheiro",
                    FormaPagamento.CartaoDebito => "💳 Débito",
                    FormaPagamento.CartaoCredito => "💳 Crédito",
                    FormaPagamento.Pix => "📲 Pix",
                    FormaPagamento.Fiado => "📒 Caderneta",
                    _ => p.Forma.ToString()
                };
                int idx = grid.Rows.Add(formaTexto, $"R$ {p.Valor:F2}");
                grid.Rows[idx].DefaultCellStyle.BackColor =
                    p.Forma == FormaPagamento.Fiado
                        ? Color.FromArgb(255, 250, 220)
                        : Color.White;
            }

            bool ok = _parcelas.Count > 0 && Math.Abs(restante) <= 0.01m;
            btnConfirmar.Enabled = ok;
            btnConfirmar.BackColor = ok ? Verde : Color.FromArgb(180, 180, 175);
        }

        private void MostrarAviso(string msg) => lblAviso.Text = msg;
    }

    // ── model auxiliar ────────────────────────────────────────────────
    public class ParcialPagamento
    {
        public FormaPagamento Forma { get; set; }
        public decimal Valor { get; set; }
    }
}