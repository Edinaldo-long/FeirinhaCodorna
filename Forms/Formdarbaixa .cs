using System;
using System.Drawing;
using System.Windows.Forms;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    /// <summary>
    /// Formulário modal para dar baixa em uma despesa pendente.
    /// Uso:
    ///   var frm = new FormDarBaixa(despesa);
    ///   if (frm.ShowDialog() == DialogResult.OK)
    ///   {
    ///       _db.DarBaixaDespesa(despesa.Id, frm.DataPagamento, frm.FormaPagamento);
    ///       CarregarDespesas(); // atualiza a lista
    ///   }
    /// </summary>
    public partial class FormDarBaixa : Form
    {
        // --- resultados públicos após confirmação ---
        public DateTime DataPagamento { get; private set; }
        public string FormaPagamento { get; private set; } = "";

        // --- controles ---
        private readonly Label lblTitulo;
        private readonly Label lblDescricao;
        private readonly Panel pnlInfo;
        private readonly Label lblValorLabel;
        private readonly Label lblValor;
        private readonly Label lblVencLabel;
        private readonly Label lblVenc;
        private readonly Label lblDataLabel;
        private readonly DateTimePicker dtpPagamento;
        private readonly Label lblFormaLabel;
        private readonly Panel pnlFormas;
        private readonly Button btnBoleto;
        private readonly Button btnCartao;
        private readonly Button btnDinheiro;
        private readonly Button btnPix;
        private readonly Label lblErroForma;
        private readonly Button btnConfirmar;
        private readonly Button btnCancelar;

        private Button? _formaSelecionada = null;

        private static readonly Color CorVerde = Color.FromArgb(26, 122, 74);
        private static readonly Color CorVerdeFundo = Color.FromArgb(225, 245, 238);
        private static readonly Color CorVerdeTexto = Color.FromArgb(15, 110, 86);
        private static readonly Color CorFundo = Color.FromArgb(245, 245, 242);
        private static readonly Color CorBorda = Color.FromArgb(210, 210, 200);
        private static readonly Color CorTextoSec = Color.FromArgb(100, 100, 95);

        public FormDarBaixa(Despesa despesa)
        {
            // ── janela ─────────────────────────────────────────────
            Text = "Dar Baixa";
            Size = new Size(400, 430);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            // ── título ──────────────────────────────────────────────
            lblTitulo = new Label
            {
                Text = "Dar Baixa no Lançamento",
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                Location = new Point(20, 18),
                Size = new Size(360, 24),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            lblDescricao = new Label
            {
                Text = despesa.Descricao,
                Font = new Font("Segoe UI", 9f),
                Location = new Point(20, 44),
                Size = new Size(360, 18),
                ForeColor = CorTextoSec
            };

            // ── painel info (valor + vencimento) ────────────────────
            pnlInfo = new Panel
            {
                Location = new Point(20, 72),
                Size = new Size(355, 52),
                BackColor = CorFundo,
            };
            pnlInfo.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(CorBorda), 0, 0, pnlInfo.Width - 1, pnlInfo.Height - 1);

            lblValorLabel = new Label
            {
                Text = "Valor",
                Font = new Font("Segoe UI", 8f),
                ForeColor = CorTextoSec,
                Location = new Point(12, 8),
                AutoSize = true
            };
            lblValor = new Label
            {
                Text = $"R$ {despesa.Valor:N2}",
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(12, 24),
                AutoSize = true
            };
            lblVencLabel = new Label
            {
                Text = "Vencimento",
                Font = new Font("Segoe UI", 8f),
                ForeColor = CorTextoSec,
                Location = new Point(190, 8),
                AutoSize = true
            };
            lblVenc = new Label
            {
                Text = despesa.Vencimento.HasValue
                                ? despesa.Vencimento.Value.ToString("dd/MM/yyyy")
                                : "Sem vencimento",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(186, 117, 23),
                Location = new Point(190, 24),
                AutoSize = true
            };

            pnlInfo.Controls.AddRange(new Control[] { lblValorLabel, lblValor, lblVencLabel, lblVenc });

            // ── data de pagamento ───────────────────────────────────
            lblDataLabel = new Label
            {
                Text = "Data do pagamento",
                Font = new Font("Segoe UI", 9f),
                ForeColor = CorTextoSec,
                Location = new Point(20, 140),
                AutoSize = true
            };
            dtpPagamento = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Location = new Point(20, 160),
                Size = new Size(355, 28)
            };

            // ── forma de pagamento ──────────────────────────────────
            lblFormaLabel = new Label
            {
                Text = "Forma de pagamento",
                Font = new Font("Segoe UI", 9f),
                ForeColor = CorTextoSec,
                Location = new Point(20, 204),
                AutoSize = true
            };

            pnlFormas = new Panel
            {
                Location = new Point(20, 224),
                Size = new Size(355, 70)
            };

            btnBoleto = CriarBotaoForma("🏦  Boleto", new Point(0, 0));
            btnCartao = CriarBotaoForma("💳  Cartão", new Point(180, 0));
            btnDinheiro = CriarBotaoForma("💵  Dinheiro", new Point(0, 38));
            btnPix = CriarBotaoForma("⚡  Pix", new Point(180, 38));

            btnBoleto.Click += (s, e) => SelecionarForma(btnBoleto, "Boleto");
            btnCartao.Click += (s, e) => SelecionarForma(btnCartao, "Cartão");
            btnDinheiro.Click += (s, e) => SelecionarForma(btnDinheiro, "Dinheiro");
            btnPix.Click += (s, e) => SelecionarForma(btnPix, "Pix");

            pnlFormas.Controls.AddRange(new Control[] { btnBoleto, btnCartao, btnDinheiro, btnPix });

            lblErroForma = new Label
            {
                Text = "Selecione a forma de pagamento.",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(163, 45, 45),
                Location = new Point(20, 300),
                AutoSize = true,
                Visible = false
            };

            // ── botões ──────────────────────────────────────────────
            btnConfirmar = new Button
            {
                Text = "✓  Confirmar Baixa",
                Location = new Point(190, 355),
                Size = new Size(185, 36),
                BackColor = CorVerde,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(20, 355),
                Size = new Size(160, 36),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(80, 80, 75),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancelar.FlatAppearance.BorderColor = CorBorda;

            // ── separador ───────────────────────────────────────────
            var separator = new Panel
            {
                Location = new Point(0, 340),
                Size = new Size(400, 1),
                BackColor = CorBorda
            };

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblDescricao, pnlInfo,
                lblDataLabel, dtpPagamento,
                lblFormaLabel, pnlFormas,
                lblErroForma,
                separator,
                btnConfirmar, btnCancelar
            });
        }

        private Button CriarBotaoForma(string texto, Point location) => new Button
        {
            Text = texto,
            Location = location,
            Size = new Size(170, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(50, 50, 45),
            Font = new Font("Segoe UI", 9.5f),
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0)
        };

        private void SelecionarForma(Button btn, string forma)
        {
            // limpa seleção anterior
            foreach (Control c in pnlFormas.Controls)
            {
                if (c is Button b)
                {
                    b.BackColor = Color.White;
                    b.ForeColor = Color.FromArgb(50, 50, 45);
                    b.FlatAppearance.BorderColor = CorBorda;
                    b.FlatAppearance.BorderSize = 1;
                }
            }
            // marca selecionado
            btn.BackColor = CorVerdeFundo;
            btn.ForeColor = CorVerdeTexto;
            btn.FlatAppearance.BorderColor = CorVerde;
            btn.FlatAppearance.BorderSize = 2;

            _formaSelecionada = btn;
            FormaPagamento = forma;
            lblErroForma.Visible = false;
        }

        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            if (_formaSelecionada == null)
            {
                lblErroForma.Visible = true;
                return;
            }
            DataPagamento = dtpPagamento.Value.Date;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}