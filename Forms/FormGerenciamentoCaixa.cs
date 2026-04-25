using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormGerenciamentoCaixa : Form
    {
        private readonly BancoDados _db;
        private TurnoCaixa? _turnoAtual;

        private Panel _pnlStatus = new();
        private Label _lblStatusTurno = new();
        private Label _lblInfoTurno = new();
        private Button _btnAbrirCaixa = new();
        private Button _btnSangria = new();
        private Button _btnReforco = new();
        private Button _btnFecharCaixa = new();
        private DataGridView _grdMovimentacoes = new();
        private Label _lblTotalSangrias = new();
        private Label _lblSaldoEsperado = new();

        public FormGerenciamentoCaixa(BancoDados db)
        {
            _db = db;
            // SEM InitializeComponent — tudo construído via código
            ConstruirInterface();
            CarregarEstado();
        }

        private void ConstruirInterface()
        {
            Text = "Gerenciamento de Caixa";
            BackColor = Color.FromArgb(245, 245, 242);
            AutoScroll = true;

            // ── Título ────────────────────────────────────────────────
            var lblTitulo = new Label
            {
                Text = "💰  Gerenciamento de Caixa",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            // ── Painel de status ──────────────────────────────────────
            _pnlStatus = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(860, 90),
                BackColor = Color.White
            };
            _pnlStatus.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 215), 1),
                    0, 0, _pnlStatus.Width - 1, _pnlStatus.Height - 1);

            _lblStatusTurno = new Label
            {
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 12)
            };
            _lblInfoTurno = new Label
            {
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(16, 44)
            };
            _pnlStatus.Controls.AddRange(new Control[] { _lblStatusTurno, _lblInfoTurno });

            // ── Botões de ação ────────────────────────────────────────
            _btnAbrirCaixa = CriarBotao("🔓  Abrir Caixa", Color.FromArgb(29, 158, 117));
            _btnAbrirCaixa.Location = new Point(20, 168);
            _btnAbrirCaixa.Click += (s, e) => AbrirCaixa();

            _btnSangria = CriarBotao("💸  Sangria", Color.FromArgb(200, 80, 60));
            _btnSangria.Location = new Point(230, 168);
            _btnSangria.Click += (s, e) => RealizarMovimentacao("Sangria");

            _btnReforco = CriarBotao("💵  Reforço", Color.FromArgb(60, 130, 200));
            _btnReforco.Location = new Point(440, 168);
            _btnReforco.Click += (s, e) => RealizarMovimentacao("Reforço");

            _btnFecharCaixa = CriarBotao("🔒  Fechar Caixa", Color.FromArgb(100, 80, 60));
            _btnFecharCaixa.Location = new Point(650, 168);
            _btnFecharCaixa.Click += (s, e) => FecharCaixa();

            // ── Painel de resumo ──────────────────────────────────────
            var pnlResumo = new Panel
            {
                Location = new Point(20, 230),
                Size = new Size(860, 60),
                BackColor = Color.FromArgb(238, 252, 246)
            };
            pnlResumo.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(180, 230, 210), 1),
                    0, 0, pnlResumo.Width - 1, pnlResumo.Height - 1);

            _lblTotalSangrias = CriarLabelResumo("Sangrias: R$ 0,00  |  Reforços: R$ 0,00", new Point(16, 10));
            _lblSaldoEsperado = CriarLabelResumo("Saldo mínimo no caixa: R$ 0,00", new Point(460, 10));
            _lblSaldoEsperado.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            pnlResumo.Controls.AddRange(new Control[] { _lblTotalSangrias, _lblSaldoEsperado });

            // ── Grid de movimentações ─────────────────────────────────
            var lblMov = new Label
            {
                Text = "Movimentações do turno",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Location = new Point(20, 308)
            };

            _grdMovimentacoes = new DataGridView
            {
                Location = new Point(20, 334),
                Size = new Size(860, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grdMovimentacoes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 233, 228);
            _grdMovimentacoes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            _grdMovimentacoes.EnableHeadersVisualStyles = false;
            _grdMovimentacoes.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "Hora", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Tipo", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Valor", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Motivo", FillWeight = 50 }
            );
            _grdMovimentacoes.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    _grdMovimentacoes.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(248, 248, 245);
            };

            Controls.AddRange(new Control[]
            {
                lblTitulo, _pnlStatus,
                _btnAbrirCaixa, _btnSangria, _btnReforco, _btnFecharCaixa,
                pnlResumo, lblMov, _grdMovimentacoes
            });
        }

        // ─────────────────────────────────────────────────────────────
        // LÓGICA
        // ─────────────────────────────────────────────────────────────

        private void CarregarEstado()
        {
            _turnoAtual = _db.ObterTurnoAberto();
            AtualizarUI();
        }

        private void AtualizarUI()
        {
            bool aberto = _turnoAtual != null;

            if (aberto)
            {
                _lblStatusTurno.Text = "✅  Caixa Aberto";
                _lblStatusTurno.ForeColor = Color.FromArgb(29, 158, 117);
                _lblInfoTurno.Text =
                    $"Aberto em: {_turnoAtual!.DataAbertura:dd/MM/yyyy HH:mm}   |   " +
                    $"Troco inicial: R$ {_turnoAtual.TrocoInicial:F2}   |   " +
                    $"Turno #{_turnoAtual.Id}";
            }
            else
            {
                _lblStatusTurno.Text = "🔒  Caixa Fechado";
                _lblStatusTurno.ForeColor = Color.FromArgb(180, 60, 40);
                _lblInfoTurno.Text = "Nenhum turno aberto. Clique em \"Abrir Caixa\" para iniciar o expediente.";
            }

            _btnAbrirCaixa.Enabled = !aberto;
            _btnSangria.Enabled = aberto;
            _btnReforco.Enabled = aberto;
            _btnFecharCaixa.Enabled = aberto;

            if (aberto)
            {
                var movs = _db.ListarMovimentacoes(_turnoAtual!.Id);
                decimal sangrias = movs.Where(m => m.Tipo == "Sangria").Sum(m => m.Valor);
                decimal reforcos = movs.Where(m => m.Tipo == "Reforço").Sum(m => m.Valor);
                decimal saldo = _turnoAtual.TrocoInicial + reforcos - sangrias;

                _lblTotalSangrias.Text = $"Sangrias: R$ {sangrias:F2}  |  Reforços: R$ {reforcos:F2}";
                _lblSaldoEsperado.Text = $"Saldo mínimo no caixa: R$ {saldo:F2}";

                _grdMovimentacoes.Rows.Clear();
                foreach (var m in movs)
                {
                    var cor = m.Tipo == "Sangria" ? Color.FromArgb(255, 235, 232)
                            : m.Tipo == "Reforço" ? Color.FromArgb(232, 242, 255)
                            : Color.White;

                    int idx = _grdMovimentacoes.Rows.Add(
                        m.DataHora.ToString("HH:mm"),
                        m.Tipo,
                        $"R$ {m.Valor:F2}",
                        m.Motivo);
                    _grdMovimentacoes.Rows[idx].DefaultCellStyle.BackColor = cor;
                }
            }
            else
            {
                _grdMovimentacoes.Rows.Clear();
                _lblTotalSangrias.Text = "Sangrias: —  |  Reforços: —";
                _lblSaldoEsperado.Text = "Saldo mínimo no caixa: —";
            }
        }

        private void AbrirCaixa()
        {
            using var dlg = CriarDialogValor("Abrir Caixa",
                "Informe o troco inicial (dinheiro em caixa ao abrir):", "Abrir");

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var txt = dlg.Controls.OfType<TextBox>().First();
            if (!TryParseValor(txt.Text, out decimal troco) || troco < 0)
            {
                MessageBox.Show("Valor inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _db.AbrirTurno(troco);
            CarregarEstado();
            MessageBox.Show($"Caixa aberto!\nTroco inicial: R$ {troco:F2}",
                "Caixa Aberto", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RealizarMovimentacao(string tipo)
        {
            if (_turnoAtual == null) return;

            using var dlg = new Form
            {
                Text = tipo == "Sangria" ? "💸 Sangria de Caixa" : "💵 Reforço de Caixa",
                Size = new Size(400, 260),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };

            var lblVal = new Label
            {
                Text = tipo == "Sangria" ? "Valor a retirar do caixa:" : "Valor a adicionar ao caixa:",
                AutoSize = true,
                Location = new Point(16, 16),
                Font = new Font("Segoe UI", 10f)
            };
            var txtVal = new TextBox
            {
                Location = new Point(16, 42),
                Width = 350,
                Font = new Font("Segoe UI", 13f)
            };
            var lblMot = new Label
            {
                Text = tipo == "Sangria"
                    ? "Motivo (obrigatório):"
                    : "Motivo (opcional):",
                AutoSize = true,
                Location = new Point(16, 82),
                Font = new Font("Segoe UI", 10f)
            };
            var txtMot = new TextBox
            {
                Location = new Point(16, 108),
                Width = 350,
                Font = new Font("Segoe UI", 10f)
            };

            var btnOk = new Button
            {
                Text = tipo,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = tipo == "Sangria"
                    ? Color.FromArgb(200, 80, 60)
                    : Color.FromArgb(60, 130, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 36),
                Location = new Point(236, 170)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            dlg.Controls.AddRange(new Control[] { lblVal, txtVal, lblMot, txtMot, btnOk });
            dlg.AcceptButton = btnOk;
            txtVal.Select();

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            if (!TryParseValor(txtVal.Text, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Valor inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tipo == "Sangria" && string.IsNullOrWhiteSpace(txtMot.Text))
            {
                MessageBox.Show("Informe o motivo da sangria.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _db.RegistrarMovimentacao(_turnoAtual.Id, tipo, valor, txtMot.Text.Trim());
            AtualizarUI();

            string msg = tipo == "Sangria"
                ? $"Sangria de R$ {valor:F2} registrada.\nGuarde o dinheiro em local seguro!"
                : $"Reforço de R$ {valor:F2} registrado.";
            MessageBox.Show(msg, tipo + " Registrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FecharCaixa()
        {
            if (_turnoAtual == null) return;

            var movs = _db.ListarMovimentacoes(_turnoAtual.Id);
            decimal sangrias = movs.Where(m => m.Tipo == "Sangria").Sum(m => m.Valor);
            decimal reforcos = movs.Where(m => m.Tipo == "Reforço").Sum(m => m.Valor);
            decimal saldoMin = _turnoAtual.TrocoInicial + reforcos - sangrias;

            using var dlg = new Form
            {
                Text = "🔒 Fechar Caixa",
                Size = new Size(460, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };

            int y = 16;
            void AddInfo(string txt, bool negrito = false)
            {
                dlg.Controls.Add(new Label
                {
                    Text = txt,
                    AutoSize = true,
                    Location = new Point(16, y),
                    Font = new Font("Segoe UI", negrito ? 10f : 9.5f,
                        negrito ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = Color.FromArgb(60, 60, 60)
                });
                y += 24;
            }

            AddInfo($"Turno aberto em:   {_turnoAtual.DataAbertura:dd/MM/yyyy HH:mm}");
            AddInfo($"Troco inicial:      R$ {_turnoAtual.TrocoInicial:F2}");
            AddInfo($"Total sangrias:     R$ {sangrias:F2}");
            AddInfo($"Total reforços:     R$ {reforcos:F2}");
            y += 4;
            AddInfo($"Saldo mínimo esperado (sem vendas): R$ {saldoMin:F2}", negrito: true);
            y += 8;

            dlg.Controls.Add(new Label
            {
                Text = "Valor contado fisicamente agora:",
                AutoSize = true,
                Location = new Point(16, y),
                Font = new Font("Segoe UI", 10f)
            });
            y += 26;

            var txtContado = new TextBox
            {
                Location = new Point(16, y),
                Width = 400,
                Font = new Font("Segoe UI", 14f)
            };
            dlg.Controls.Add(txtContado);
            y += 50;

            var btnOk = new Button
            {
                Text = "Fechar Caixa",
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(100, 80, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 38),
                Location = new Point(290, y)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            dlg.Controls.Add(btnOk);
            dlg.AcceptButton = btnOk;
            txtContado.Select();

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            if (!TryParseValor(txtContado.Text, out decimal valorContado) || valorContado < 0)
            {
                MessageBox.Show("Valor inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _db.FecharTurno(_turnoAtual.Id, valorContado);

            var fechado = _db.ListarTurnosRecentes().First(t => t.Id == _turnoAtual.Id);
            string icone = fechado.Diferenca >= 0 ? "✅" : "⚠️";

            MessageBox.Show(
                $"Caixa fechado!\n\n" +
                $"Total de vendas:   R$ {fechado.TotalVendas:F2}\n" +
                $"Total sangrias:    R$ {fechado.TotalSangrias:F2}\n" +
                $"Saldo esperado:    R$ {fechado.SaldoEsperado:F2}\n" +
                $"Valor contado:     R$ {fechado.ValorContado:F2}\n\n" +
                $"{icone} Diferença: R$ {fechado.Diferenca:F2}",
                "Fechamento de Caixa",
                MessageBoxButtons.OK,
                fechado.Diferenca >= 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            _turnoAtual = null;
            AtualizarUI();
        }

        // ─────────────────────────────────────────────────────────────
        // AUXILIARES
        // ─────────────────────────────────────────────────────────────

        private Form CriarDialogValor(string titulo, string descricao, string textoBotao)
        {
            var dlg = new Form
            {
                Text = titulo,
                Size = new Size(380, 190),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };
            var lbl = new Label { Text = descricao, AutoSize = true, Location = new Point(16, 16), Font = new Font("Segoe UI", 10f) };
            var txt = new TextBox { Location = new Point(16, 46), Width = 330, Font = new Font("Segoe UI", 14f) };
            var btn = new Button
            {
                Text = textoBotao,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 36),
                Location = new Point(226, 100)
            };
            btn.FlatAppearance.BorderSize = 0;
            dlg.Controls.AddRange(new Control[] { lbl, txt, btn });
            dlg.AcceptButton = btn;
            return dlg;
        }

        private static bool TryParseValor(string texto, out decimal valor) =>
            decimal.TryParse(texto.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out valor);

        private Button CriarBotao(string texto, Color cor)
        {
            var btn = new Button
            {
                Text = texto,
                Size = new Size(190, 48),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = cor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label CriarLabelResumo(string texto, Point location) => new Label
        {
            Text = texto,
            AutoSize = true,
            Location = location,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(60, 60, 60)
        };
    }
}