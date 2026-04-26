using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormTroco : Form
    {
        public decimal ValorRecebido { get; private set; }
        public decimal Troco { get; private set; }

        private readonly decimal _total;
        private readonly List<ItemVenda> _itens;
        private bool _listaAberta = false;

        private TextBox txtValorPago = null!;
        private Label lblTrocoValor = null!;
        private Label lblTrocoObs = null!;
        private Label lblTrocoTitulo = null!;
        private Panel painelTroco = null!;
        private Button btnConfirmar = null!;
        private Button btnCancelar = null!;
        private Button btnToggle = null!;
        private DataGridView grdItens2 = null!;

        private static readonly Color Verde = Color.FromArgb(29, 158, 117);
        private static readonly Color VerdeEscuro = Color.FromArgb(15, 110, 86);
        private static readonly Color VerdeClaro = Color.FromArgb(225, 245, 238);
        private static readonly Color Vermelho = Color.FromArgb(163, 45, 45);
        private static readonly Color VermelhoClaro = Color.FromArgb(252, 235, 235);
        private static readonly Color Cinza = Color.FromArgb(180, 180, 178);

        public FormTroco(decimal total, List<ItemVenda> itens)
        {
            _total = total;
            _itens = itens;
            Build();
        }

        private void Build()
        {
            Text = "Pagamento em Dinheiro";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 245, 242);

            const int W = 440;
            const int X = 20;
            const int IW = W - 40;
            int y = 16;

            var fTitulo = new Font("Segoe UI", 13F, FontStyle.Bold);
            var fPequena = new Font("Segoe UI", 10F);
            var fBotao = new Font("Segoe UI", 12F, FontStyle.Bold);
            var fTotal = new Font("Segoe UI", 30F, FontStyle.Bold);

            // ── Título ───────────────────────────────────────────────
            Controls.Add(new Label
            {
                Text = "Pagamento em Dinheiro",
                Font = fTitulo,
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(X, y),
                AutoSize = true
            });
            y += 36;

            // ── Toggle itens ─────────────────────────────────────────
            btnToggle = new Button
            {
                Text = $"▼   Ver itens da compra  ({_itens.Count} produto{(_itens.Count != 1 ? "s" : "")})",
                Font = fPequena,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(232, 232, 228),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(X, y),
                Size = new Size(IW, 32),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderColor = Color.FromArgb(205, 205, 200);
            btnToggle.FlatAppearance.BorderSize = 1;
            btnToggle.Click += ToggleItens;
            Controls.Add(btnToggle);
            y += 36;

            // ── Grid itens (oculto) ───────────────────────────────────
            grdItens2 = new DataGridView
            {
                Location = new Point(X, y),
                Size = new Size(IW, 140),
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowTemplate = { Height = 30 },
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.White
            };
            grdItens2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grdItens2.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grdItens2.Columns.Add("Produto", "Produto");
            grdItens2.Columns.Add("Qtd", "Qtd");
            grdItens2.Columns.Add("Subtotal", "Subtotal");
            grdItens2.Columns["Produto"].Width = IW - 160;
            grdItens2.Columns["Qtd"].Width = 80;
            grdItens2.Columns["Subtotal"].Width = 80;
            grdItens2.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grdItens2.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 245, 242);
            };
            foreach (var item in _itens)
            {
                string qtd = item.Quantidade % 1 == 0
                    ? $"{item.Quantidade:F0} un"
                    : $"{item.Quantidade:F3} kg";
                grdItens2.Rows.Add(item.ProdutoNome, qtd, $"R$ {item.Subtotal:F2}");
            }
            Controls.Add(grdItens2);

            // ── Painel total verde ────────────────────────────────────
            var pTotal = new Panel { Location = new Point(X, y), Size = new Size(IW, 90), BackColor = Verde };
            var lTLabel = new Label { Text = "TOTAL A PAGAR", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(159, 225, 203), AutoSize = true };
            var lTValor = new Label { Text = $"R$ {_total:F2}", Font = fTotal, ForeColor = Color.White, AutoSize = true };
            pTotal.Controls.Add(lTLabel);
            pTotal.Controls.Add(lTValor);
            pTotal.Layout += (s, e) =>
            {
                lTLabel.Left = (pTotal.Width - lTLabel.PreferredWidth) / 2; lTLabel.Top = 8;
                lTValor.Left = (pTotal.Width - lTValor.PreferredWidth) / 2; lTValor.Top = lTLabel.Bottom + 2;
            };
            Controls.Add(pTotal);
            y += 98;

            // ── Pergunta ─────────────────────────────────────────────
            Controls.Add(new Label
            {
                Text = "Quanto o cliente entregou?",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(X, y),
                AutoSize = true
            });
            y += 30;

            // ── Campo valor ───────────────────────────────────────────
            txtValorPago = new TextBox
            {
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                Location = new Point(X, y),
                Size = new Size(IW, 56),
                TextAlign = HorizontalAlignment.Center,
                PlaceholderText = "R$ 0,00"
            };
            txtValorPago.TextChanged += (s, e) => AtualizarTroco();
            txtValorPago.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.' && e.KeyChar != '\b')
                    e.Handled = true;
            };
            txtValorPago.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && btnConfirmar.Enabled)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    ConfirmarPagamento();
                }
            };
            Controls.Add(txtValorPago);
            y += 64;

            // ── Painel troco ──────────────────────────────────────────
            painelTroco = new Panel
            {
                Location = new Point(X, y),
                Size = new Size(IW, 94),
                BackColor = Color.FromArgb(232, 232, 228),
                BorderStyle = BorderStyle.FixedSingle
            };
            lblTrocoTitulo = new Label { Text = "TROCO A DEVOLVER", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(110, 110, 110), AutoSize = true };
            lblTrocoValor = new Label { Text = "—", Font = new Font("Segoe UI", 26F, FontStyle.Bold), ForeColor = Color.FromArgb(110, 110, 110), AutoSize = true };
            lblTrocoObs = new Label { Text = "", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(110, 110, 110), AutoSize = true };
            painelTroco.Controls.AddRange(new Control[] { lblTrocoTitulo, lblTrocoValor, lblTrocoObs });
            painelTroco.Layout += (s, e) =>
            {
                int cx = painelTroco.Width / 2;
                lblTrocoTitulo.Left = cx - lblTrocoTitulo.PreferredWidth / 2; lblTrocoTitulo.Top = 6;
                lblTrocoValor.Left = cx - lblTrocoValor.PreferredWidth / 2; lblTrocoValor.Top = lblTrocoTitulo.Bottom + 2;
                lblTrocoObs.Left = cx - lblTrocoObs.PreferredWidth / 2; lblTrocoObs.Top = lblTrocoValor.Bottom + 2;
                int h = lblTrocoObs.Bottom + 10;
                if (painelTroco.Height != h) painelTroco.Height = h;
            };
            Controls.Add(painelTroco);
            y += 102;

            // ── Botões ────────────────────────────────────────────────
            int bw = (IW - 10) / 2;

            // Cancelar — volta ao carrinho para tentar outra forma
            btnCancelar = new Button
            {
                Text = "✖  Outra forma",
                Font = fBotao,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(215, 215, 210),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(X, y),
                Size = new Size(bw, 52),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCancelar);

            // Confirmar — só habilitado quando o troco está OK
            btnConfirmar = new Button
            {
                Text = "✔  Confirmar Dinheiro",
                Font = fBotao,
                FlatStyle = FlatStyle.Flat,
                BackColor = Cinza,
                ForeColor = Color.White,
                Enabled = false,
                Location = new Point(X + bw + 10, y),
                Size = new Size(bw, 52),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += (s, e) => ConfirmarPagamento();
            Controls.Add(btnConfirmar);

            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;

            y += 60;

            // ── Aviso ─────────────────────────────────────────────────
            Controls.Add(new Label
            {
                Text = "⚠  Clique em \"Outra forma\" se o cliente quiser pagar de outro modo.",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 100, 0),
                Location = new Point(X, y),
                Size = new Size(IW, 18),
                AutoSize = false
            });
            y += 22;

            ClientSize = new Size(W, y + 12);
            Shown += (s, e) => { txtValorPago.Focus(); txtValorPago.SelectAll(); };
        }

        // ─────────────────────────────────────────────────────────────
        // Confirmar
        // ─────────────────────────────────────────────────────────────
        private void ConfirmarPagamento()
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        // ─────────────────────────────────────────────────────────────
        // Lógica de troco
        // ─────────────────────────────────────────────────────────────
        private void AtualizarTroco()
        {
            string texto = txtValorPago.Text.Replace(',', '.');
            bool valido = decimal.TryParse(texto, NumberStyles.Any,
                               CultureInfo.InvariantCulture, out decimal pago) && pago > 0;

            if (!valido) { SetNeutro(); return; }

            decimal troco = pago - _total;

            if (troco < 0) SetErro($"Faltam R$ {Math.Abs(troco):F2}");
            else if (troco == 0) SetOk("Sem troco", "Valor exato", pago, 0);
            else SetOk($"R$ {troco:F2}", "Devolver ao cliente", pago, troco);
        }

        private void SetNeutro()
        {
            lblTrocoTitulo.ForeColor = Color.FromArgb(110, 110, 110);
            lblTrocoValor.Text = "—"; lblTrocoValor.Font = new Font("Segoe UI", 26F, FontStyle.Bold); lblTrocoValor.ForeColor = Color.FromArgb(110, 110, 110);
            lblTrocoObs.Text = ""; lblTrocoObs.ForeColor = Color.FromArgb(110, 110, 110);
            painelTroco.BackColor = Color.FromArgb(232, 232, 228);
            btnConfirmar.Enabled = false;
            btnConfirmar.BackColor = Cinza;
            painelTroco.PerformLayout();
        }

        private void SetErro(string falta)
        {
            lblTrocoTitulo.ForeColor = Vermelho;
            lblTrocoValor.Text = "Valor insuficiente"; lblTrocoValor.Font = new Font("Segoe UI", 16F, FontStyle.Bold); lblTrocoValor.ForeColor = Vermelho;
            lblTrocoObs.Text = falta; lblTrocoObs.ForeColor = Vermelho;
            painelTroco.BackColor = VermelhoClaro;
            btnConfirmar.Enabled = false;
            btnConfirmar.BackColor = Cinza;
            painelTroco.PerformLayout();
        }

        private void SetOk(string valorTexto, string obs, decimal pago, decimal troco)
        {
            ValorRecebido = pago;
            Troco = troco;
            lblTrocoTitulo.ForeColor = VerdeEscuro;
            lblTrocoValor.Text = valorTexto; lblTrocoValor.Font = new Font("Segoe UI", troco == 0 ? 22F : 26F, FontStyle.Bold); lblTrocoValor.ForeColor = VerdeEscuro;
            lblTrocoObs.Text = obs; lblTrocoObs.ForeColor = VerdeEscuro;
            painelTroco.BackColor = VerdeClaro;
            btnConfirmar.Enabled = true;
            btnConfirmar.BackColor = Verde;
            painelTroco.PerformLayout();
        }

        // ─────────────────────────────────────────────────────────────
        // Toggle lista de itens
        // ─────────────────────────────────────────────────────────────
        private void ToggleItens(object? sender, EventArgs e)
        {
            _listaAberta = !_listaAberta;
            int delta = grdItens2.Height * (_listaAberta ? 1 : -1);
            grdItens2.Visible = _listaAberta;

            foreach (Control c in Controls)
                if (c.Top >= grdItens2.Bottom - 4 && c != grdItens2)
                    c.Top += delta;

            ClientSize = new Size(ClientSize.Width, ClientSize.Height + delta);

            btnToggle.Text = _listaAberta
                ? $"▲   Ocultar itens da compra  ({_itens.Count} produto{(_itens.Count != 1 ? "s" : "")})"
                : $"▼   Ver itens da compra  ({_itens.Count} produto{(_itens.Count != 1 ? "s" : "")})";
        }
    }
}