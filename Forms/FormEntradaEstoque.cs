using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FeirinhaCodorna.Forms
{
    /// <summary>
    /// Dialog de entrada de estoque para um produto.
    /// Registra quantidade recebida, fornecedor, nota fiscal e preço de custo.
    /// Retorna DialogResult.OK ao confirmar.
    /// </summary>
    public class FormEntradaEstoque : Form
    {
        private readonly BancoDados _db;
        private readonly Produto _produto;

        // ── Campos ──────────────────────────────────────────────────────
        private Label _lblProdutoInfo = null!;
        private Label _lblEstoqueAtual = null!;
        private TextBox _txtQuantidade = null!;
        private TextBox _txtPrecoCusto = null!;
        private ComboBox _cmbFornecedor = null!;
        private TextBox _txtNotaFiscal = null!;
        private TextBox _txtObservacao = null!;
        private Label _lblPreviewNovo = null!;

        // ── Botões ───────────────────────────────────────────────────────
        private Button _btnConfirmar = null!;
        private Button _btnCancelar = null!;

        // ── Cores ────────────────────────────────────────────────────────
        private static readonly Color CorFundo = Color.FromArgb(245, 245, 242);
        private static readonly Color CorPainel = Color.FromArgb(235, 235, 230);
        private static readonly Color CorRotulo = Color.FromArgb(70, 70, 65);
        private static readonly Color CorEntrada = Color.FromArgb(30, 120, 200);
        private static readonly Color CorCancelar = Color.FromArgb(130, 130, 125);
        private static readonly Color CorVerde = Color.FromArgb(46, 160, 80);
        private static readonly Color CorAlerta = Color.FromArgb(200, 80, 0);

        public FormEntradaEstoque(BancoDados db, Produto produto)
        {
            _db = db;
            _produto = produto;

            Text = "Entrada de Estoque";
            Size = new Size(520, 540);
            MinimumSize = new Size(480, 500);
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = CorFundo;
            Font = new Font("Segoe UI", 9F);

            ConstruirLayout();
            PreencherFornecedores();
        }

        // ────────────────────────────────────────────────────────────────
        //  Layout
        // ────────────────────────────────────────────────────────────────
        private void ConstruirLayout()
        {
            // ── Cabeçalho ────────────────────────────────────────────────
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = CorEntrada
            };
            var lblTitulo = new Label
            {
                Text = "📦  Entrada de Estoque",
                Font = new Font("Segoe UI Semibold", 13F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(16, 14)
            };
            header.Controls.Add(lblTitulo);
            Controls.Add(header);

            // ── Card do produto ──────────────────────────────────────────
            var pProduto = new Panel
            {
                Location = new Point(16, 64),
                Size = new Size(470, 70),
                BackColor = Color.FromArgb(225, 235, 248),
                BorderStyle = BorderStyle.None
            };
            // borda esquerda colorida
            var borda = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(5, 70),
                BackColor = CorEntrada
            };
            pProduto.Controls.Add(borda);

            _lblProdutoInfo = new Label
            {
                Text = $"{_produto.Nome}",
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = Color.FromArgb(30, 60, 100),
                AutoSize = true,
                Location = new Point(14, 10)
            };
            pProduto.Controls.Add(_lblProdutoInfo);

            _lblEstoqueAtual = new Label
            {
                Text = $"Estoque atual: {_produto.Estoque:N3} {_produto.Unidade}   |   " +
                             $"Cód EAN: {(_produto.CodigoEan == "" ? "—" : _produto.CodigoEan)}   |   " +
                             $"Cód Interno: {(_produto.CodigoInterno == "" ? "—" : _produto.CodigoInterno)}",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(60, 90, 140),
                AutoSize = true,
                Location = new Point(14, 40)
            };
            pProduto.Controls.Add(_lblEstoqueAtual);
            Controls.Add(pProduto);

            // ── Campos ───────────────────────────────────────────────────
            int y = 148;
            int fieldX = 180;
            int fieldW = 306;

            // Quantidade
            AdicionarRotulo("Quantidade *:", 16, y);
            _txtQuantidade = CampoNumerico(fieldX, y, 140);
            _txtQuantidade.Font = new Font("Segoe UI Semibold", 11F);
            _txtQuantidade.TextChanged += AtualizarPreview;
            Controls.Add(_txtQuantidade);

            var lblUnidade = new Label
            {
                Text = _produto.Unidade,
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = CorEntrada,
                AutoSize = true,
                Location = new Point(fieldX + 148, y + 3)
            };
            Controls.Add(lblUnidade);
            y += 36;

            // Preview novo estoque
            _lblPreviewNovo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = CorVerde,
                AutoSize = true,
                Location = new Point(fieldX, y)
            };
            Controls.Add(_lblPreviewNovo);
            y += 22;

            // Separador
            AdicionarSeparador(y); y += 14;

            // Preço de Custo
            AdicionarRotulo("Preço de Custo R$:", 16, y);
            _txtPrecoCusto = CampoNumerico(fieldX, y, 140);
            if (_produto.PrecoCusto > 0)
                _txtPrecoCusto.Text = _produto.PrecoCusto.ToString("N2");
            var lblCustoHint = new Label
            {
                Text = "(atualiza o custo do produto)",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 140),
                AutoSize = true,
                Location = new Point(fieldX + 148, y + 4)
            };
            Controls.Add(lblCustoHint);
            Controls.Add(_txtPrecoCusto);
            y += 36;

            // Fornecedor
            AdicionarRotulo("Fornecedor:", 16, y);
            _cmbFornecedor = new ComboBox
            {
                Location = new Point(fieldX, y),
                Width = fieldW,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
            Controls.Add(_cmbFornecedor);
            y += 36;

            // Nota Fiscal
            AdicionarRotulo("Nota Fiscal (NF):", 16, y);
            _txtNotaFiscal = new TextBox
            {
                Location = new Point(fieldX, y),
                Width = fieldW,
                Font = new Font("Segoe UI", 9.5F),
                PlaceholderText = "Nº da NF ou referência..."
            };
            Controls.Add(_txtNotaFiscal);
            y += 36;

            // Separador
            AdicionarSeparador(y); y += 14;

            // Observação
            AdicionarRotulo("Observação:", 16, y);
            _txtObservacao = new TextBox
            {
                Location = new Point(fieldX, y),
                Size = new Size(fieldW, 52),
                Multiline = true,
                Font = new Font("Segoe UI", 9.5F),
                PlaceholderText = "Opcional — ex: lote, validade, condição..."
            };
            Controls.Add(_txtObservacao);
            y += 64;

            // ── Aviso ────────────────────────────────────────────────────
            var aviso = new Label
            {
                Text = "⚠️  A entrada de estoque não pode ser desfeita automaticamente. Use Estorno de Venda para correções.",
                Font = new Font("Segoe UI", 7.5F),
                ForeColor = CorAlerta,
                Location = new Point(16, y),
                Size = new Size(470, 28),
                AutoSize = false
            };
            Controls.Add(aviso);

            // ── Rodapé ───────────────────────────────────────────────────
            var pRodape = new Panel
            {
                Location = new Point(0, ClientSize.Height - 54),
                Size = new Size(ClientSize.Width, 54),
                BackColor = CorPainel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _btnCancelar = new Button
            {
                Text = "CANCELAR",
                Size = new Size(110, 32),
                Location = new Point(pRodape.Width - 248, 11),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = CorCancelar,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F),
                DialogResult = DialogResult.Cancel
            };
            _btnCancelar.FlatAppearance.BorderSize = 0;
            pRodape.Controls.Add(_btnCancelar);

            _btnConfirmar = new Button
            {
                Text = "📦  CONFIRMAR ENTRADA",
                Size = new Size(180, 32),
                Location = new Point(pRodape.Width - 132, 11),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = CorEntrada,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F)
            };
            _btnConfirmar.FlatAppearance.BorderSize = 0;
            _btnConfirmar.Click += BtnConfirmar_Click;
            // reposiciona para caber o texto maior
            _btnConfirmar.Location = new Point(pRodape.Width - 300, 11);
            pRodape.Controls.Add(_btnConfirmar);

            Controls.Add(pRodape);

            CancelButton = _btnCancelar;
            AcceptButton = _btnConfirmar;

            // foco inicial
            ActiveControl = _txtQuantidade;
        }

        // ────────────────────────────────────────────────────────────────
        //  Fornecedores
        // ────────────────────────────────────────────────────────────────
        private void PreencherFornecedores()
        {
            _cmbFornecedor.Items.Clear();
            _cmbFornecedor.Items.Add(new FornecedorItem(0, "(sem fornecedor)"));

            int selIdx = 0;
            foreach (var f in _db.ListarFornecedores())
            {
                int idx = _cmbFornecedor.Items.Add(new FornecedorItem(f.Id, f.Nome));
                if (f.Id == _produto.FornecedorId)
                    selIdx = idx;   // pré-seleciona o fornecedor do produto
            }

            _cmbFornecedor.SelectedIndex = selIdx;
        }

        // ────────────────────────────────────────────────────────────────
        //  Preview de novo estoque
        // ────────────────────────────────────────────────────────────────
        private void AtualizarPreview(object? sender, EventArgs e)
        {
            if (decimal.TryParse(_txtQuantidade.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal qtd) && qtd > 0)
            {
                decimal novoEstoque = _produto.Estoque + qtd;
                _lblPreviewNovo.Text =
                    $"→ Novo estoque após entrada: {novoEstoque:N3} {_produto.Unidade}";
                _lblPreviewNovo.ForeColor = novoEstoque > _produto.EstoqueMinimo
                    ? CorVerde : CorAlerta;
            }
            else
            {
                _lblPreviewNovo.Text = "";
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Confirmar
        // ────────────────────────────────────────────────────────────────
        private void BtnConfirmar_Click(object? sender, EventArgs e)
        {
            // Validar quantidade
            if (!decimal.TryParse(_txtQuantidade.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal qtd) || qtd <= 0)
            {
                MessageBox.Show("Informe uma quantidade válida e maior que zero.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtQuantidade.Focus();
                return;
            }

            // Confirmação
            string fornNome = _cmbFornecedor.SelectedItem?.ToString() ?? "(sem fornecedor)";
            string nf = string.IsNullOrWhiteSpace(_txtNotaFiscal.Text) ? "—" : _txtNotaFiscal.Text.Trim();

            var confirm = MessageBox.Show(
                $"Confirma a entrada de estoque?\n\n" +
                $"Produto:    {_produto.Nome}\n" +
                $"Quantidade: {qtd:N3} {_produto.Unidade}\n" +
                $"Fornecedor: {fornNome}\n" +
                $"NF / Ref.:  {nf}\n\n" +
                $"Estoque atual:  {_produto.Estoque:N3} {_produto.Unidade}\n" +
                $"Estoque novo:   {(_produto.Estoque + qtd):N3} {_produto.Unidade}",
                "Confirmar Entrada",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);

            if (confirm != DialogResult.Yes) return;

            try
            {
                // 1. Atualiza estoque
                _db.EntrarEstoque(_produto.Id, qtd);

                // 2. Atualiza preço de custo se informado
                if (decimal.TryParse(_txtPrecoCusto.Text.Replace(",", "."),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out decimal novoCusto) && novoCusto > 0)
                {
                    // reutiliza SalvarProduto para atualizar o custo
                    var prodAtualizado = new Produto
                    {
                        Id = _produto.Id,
                        CodigoEan = _produto.CodigoEan,
                        CodigoInterno = _produto.CodigoInterno,
                        Nome = _produto.Nome,
                        Preco = _produto.Preco,
                        PrecoCusto = novoCusto,
                        Estoque = _produto.Estoque + qtd,
                        EstoqueMinimo = _produto.EstoqueMinimo,
                        Unidade = _produto.Unidade,
                        Pesavel = _produto.Pesavel,
                        FornecedorId = _produto.FornecedorId
                    };
                    _db.SalvarProduto(prodAtualizado);
                }

                MessageBox.Show(
                    $"✅  Entrada registrada com sucesso!\n\n" +
                    $"Produto:       {_produto.Nome}\n" +
                    $"Entrada:       +{qtd:N3} {_produto.Unidade}\n" +
                    $"Novo estoque:  {(_produto.Estoque + qtd):N3} {_produto.Unidade}",
                    "Entrada Confirmada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao registrar entrada:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Helpers de layout
        // ────────────────────────────────────────────────────────────────
        private void AdicionarRotulo(string texto, int x, int y)
        {
            Controls.Add(new Label
            {
                Text = texto,
                AutoSize = true,
                Location = new Point(x, y + 4),
                ForeColor = CorRotulo,
                Font = new Font("Segoe UI", 9F)
            });
        }

        private void AdicionarSeparador(int y)
        {
            Controls.Add(new Panel
            {
                Location = new Point(16, y + 2),
                Size = new Size(ClientSize.Width - 32, 1),
                BackColor = Color.FromArgb(210, 210, 205)
            });
        }

        private static TextBox CampoNumerico(int x, int y, int w)
        {
            var t = new TextBox
            {
                Location = new Point(x, y),
                Width = w,
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = HorizontalAlignment.Right,
                Text = "0,000"
            };
            t.Enter += (s, _) => t.SelectAll();
            return t;
        }

        // ────────────────────────────────────────────────────────────────
        //  Classe auxiliar
        // ────────────────────────────────────────────────────────────────
        private class FornecedorItem
        {
            public int Id { get; }
            public string Nome { get; }
            public FornecedorItem(int id, string nome) { Id = id; Nome = nome; }
            public override string ToString() => Nome;
        }
    }
}