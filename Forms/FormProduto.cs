using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FeirinhaCodorna.Forms
{
    /// <summary>
    /// Dialog de criação e edição de produto.
    /// Passa prod=null para novo produto, ou um Produto existente para edição.
    /// Retorna DialogResult.OK ao salvar com sucesso.
    /// </summary>
    public class FormProduto : Form
    {
        private readonly BancoDados _db;
        private readonly Produto? _original;   // null = novo produto

        // ── Campos ──────────────────────────────────────────────────────
        private TextBox _txtEan = null!;
        private TextBox _txtCodInterno = null!;
        private TextBox _txtNome = null!;
        private TextBox _txtPreco = null!;
        private TextBox _txtPrecoCusto = null!;
        private TextBox _txtEstoque = null!;
        private TextBox _txtEstoqueMin = null!;
        private ComboBox _cmbUnidade = null!;
        private ComboBox _cmbFornecedor = null!;
        private CheckBox _chkPesavel = null!;

        // ── Botões ───────────────────────────────────────────────────────
        private Button _btnSalvar = null!;
        private Button _btnCancelar = null!;

        // ── Cores do tema ────────────────────────────────────────────────
        private static readonly Color CorFundo = Color.FromArgb(245, 245, 242);
        private static readonly Color CorPainel = Color.FromArgb(235, 235, 230);
        private static readonly Color CorRotulo = Color.FromArgb(70, 70, 65);
        private static readonly Color CorSalvar = Color.FromArgb(46, 160, 80);
        private static readonly Color CorCancelar = Color.FromArgb(130, 130, 125);
        private static readonly Color CorDestaque = Color.FromArgb(30, 120, 200);
        private static readonly Color CorAlerta = Color.FromArgb(200, 80, 0);

        public FormProduto(BancoDados db, Produto? prod)
        {
            _db = db;
            _original = prod;

            Text = prod == null ? "Novo Produto" : "Editar Produto";
            Size = new Size(560, 560);
            MinimumSize = new Size(520, 520);
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = CorFundo;
            Font = new Font("Segoe UI", 9F);

            ConstruirLayout();
            PreencherFornecedores();

            if (prod != null)
                PreencherCampos(prod);
        }

        // ────────────────────────────────────────────────────────────────
        //  Layout
        // ────────────────────────────────────────────────────────────────
        private void ConstruirLayout()
        {
            // ── Cabeçalho colorido ───────────────────────────────────────
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = _original == null ? CorSalvar : CorDestaque
            };
            var lblTitulo = new Label
            {
                Text = _original == null ? "➕  Novo Produto" : "✏️  Editar Produto",
                Font = new Font("Segoe UI Semibold", 13F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(16, 14)
            };
            header.Controls.Add(lblTitulo);
            Controls.Add(header);

            // ── Corpo de campos ──────────────────────────────────────────
            int y = 68;
            int labelW = 140;
            int fieldX = 162;
            int fieldW = 350;

            // EAN (Cód. Barras)
            AdicionarRotulo("EAN (Cód. Barras):", 16, y);
            _txtEan = Campo(fieldX, y, fieldW);
            _txtEan.MaxLength = 30;
            Controls.Add(_txtEan);
            y += 36;

            // Código Interno
            AdicionarRotulo("Código Interno:", 16, y);
            _txtCodInterno = Campo(fieldX, y, fieldW);
            _txtCodInterno.MaxLength = 20;
            Controls.Add(_txtCodInterno);
            y += 36;

            // Nome *
            AdicionarRotulo("Nome *:", 16, y);
            _txtNome = Campo(fieldX, y, fieldW);
            _txtNome.Font = new Font("Segoe UI Semibold", 10F);
            Controls.Add(_txtNome);
            y += 36;

            // Separador
            AdicionarSeparador(y); y += 16;

            // Preço de Venda
            AdicionarRotulo("Preço de Venda R$:", 16, y);
            _txtPreco = CampoNumerico(fieldX, y, 160);
            Controls.Add(_txtPreco);
            y += 36;

            // Preço de Custo
            AdicionarRotulo("Preço de Custo R$:", 16, y);
            _txtPrecoCusto = CampoNumerico(fieldX, y, 160);
            var lblCustoHint = new Label
            {
                Text = "(opcional — para relatório de margem)",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 140),
                AutoSize = true,
                Location = new Point(fieldX + 168, y + 4)
            };
            Controls.Add(lblCustoHint);
            Controls.Add(_txtPrecoCusto);
            y += 36;

            // Separador
            AdicionarSeparador(y); y += 16;

            // Estoque Atual
            AdicionarRotulo("Estoque Atual:", 16, y);
            _txtEstoque = CampoNumerico(fieldX, y, 120);
            Controls.Add(_txtEstoque);
            y += 36;

            // Estoque Mínimo
            AdicionarRotulo("Estoque Mínimo:", 16, y);
            _txtEstoqueMin = CampoNumerico(fieldX, y, 120);
            var lblMinHint = new Label
            {
                Text = "(alerta de reposição)",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = CorAlerta,
                AutoSize = true,
                Location = new Point(fieldX + 128, y + 4)
            };
            Controls.Add(lblMinHint);
            Controls.Add(_txtEstoqueMin);
            y += 36;

            // Separador
            AdicionarSeparador(y); y += 16;

            // Unidade
            AdicionarRotulo("Unidade:", 16, y);
            _cmbUnidade = new ComboBox
            {
                Location = new Point(fieldX, y),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
            _cmbUnidade.Items.AddRange(new object[] { "kg", "g", "un", "cx", "lt", "ml", "dz", "pc" });
            _cmbUnidade.SelectedIndex = 0;
            Controls.Add(_cmbUnidade);

            // Pesável — na mesma linha
            _chkPesavel = new CheckBox
            {
                Text = "Vendido por peso (pesável)",
                Location = new Point(fieldX + 118, y + 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = CorRotulo,
                Checked = true
            };
            Controls.Add(_chkPesavel);
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
            y += 48;

            // ── Rodapé com botões ────────────────────────────────────────
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

            _btnSalvar = new Button
            {
                Text = "✔  SALVAR",
                Size = new Size(120, 32),
                Location = new Point(pRodape.Width - 132, 11),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = CorSalvar,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F)
            };
            _btnSalvar.FlatAppearance.BorderSize = 0;
            _btnSalvar.Click += BtnSalvar_Click;
            pRodape.Controls.Add(_btnSalvar);

            Controls.Add(pRodape);

            CancelButton = _btnCancelar;
            AcceptButton = _btnSalvar;
        }

        // ────────────────────────────────────────────────────────────────
        //  Preenchimento
        // ────────────────────────────────────────────────────────────────
        private void PreencherFornecedores()
        {
            _cmbFornecedor.Items.Clear();
            _cmbFornecedor.Items.Add(new FornecedorItem(0, "(sem fornecedor)"));

            foreach (var f in _db.ListarFornecedores())
                _cmbFornecedor.Items.Add(new FornecedorItem(f.Id, f.Nome));

            _cmbFornecedor.SelectedIndex = 0;
        }

        private void PreencherCampos(Produto p)
        {
            _txtEan.Text = p.CodigoEan;
            _txtCodInterno.Text = p.CodigoInterno;
            _txtNome.Text = p.Nome;
            _txtPreco.Text = p.Preco.ToString("N2");
            _txtPrecoCusto.Text = p.PrecoCusto.ToString("N2");
            _txtEstoque.Text = p.Estoque.ToString("N3");
            _txtEstoqueMin.Text = p.EstoqueMinimo.ToString("N3");
            _chkPesavel.Checked = p.Pesavel;

            // Unidade
            int uIdx = _cmbUnidade.Items.IndexOf(p.Unidade);
            _cmbUnidade.SelectedIndex = uIdx >= 0 ? uIdx : 0;

            // Fornecedor
            for (int i = 0; i < _cmbFornecedor.Items.Count; i++)
            {
                if (_cmbFornecedor.Items[i] is FornecedorItem fi && fi.Id == p.FornecedorId)
                {
                    _cmbFornecedor.SelectedIndex = i;
                    break;
                }
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Salvar
        // ────────────────────────────────────────────────────────────────
        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(_txtNome.Text))
            {
                Erro("O nome do produto é obrigatório.", _txtNome);
                return;
            }

            if (!decimal.TryParse(_txtPreco.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal preco) || preco < 0)
            {
                Erro("Informe um preço de venda válido.", _txtPreco);
                return;
            }

            if (!decimal.TryParse(_txtPrecoCusto.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal precoCusto))
                precoCusto = 0;

            if (!decimal.TryParse(_txtEstoque.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal estoque) || estoque < 0)
            {
                Erro("Informe um estoque válido.", _txtEstoque);
                return;
            }

            if (!decimal.TryParse(_txtEstoqueMin.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal estoqueMin) || estoqueMin < 0)
            {
                Erro("Informe um estoque mínimo válido.", _txtEstoqueMin);
                return;
            }

            int fornecedorId = (_cmbFornecedor.SelectedItem as FornecedorItem)?.Id ?? 0;

            var prod = new Produto
            {
                Id = _original?.Id ?? 0,
                CodigoEan = _txtEan.Text.Trim(),
                CodigoInterno = _txtCodInterno.Text.Trim(),
                Nome = _txtNome.Text.Trim(),
                Preco = preco,
                PrecoCusto = precoCusto,
                Estoque = estoque,
                EstoqueMinimo = estoqueMin,
                Unidade = _cmbUnidade.SelectedItem?.ToString() ?? "kg",
                Pesavel = _chkPesavel.Checked,
                FornecedorId = fornecedorId
            };

            try
            {
                _db.SalvarProduto(prod);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar produto:\n{ex.Message}", "Erro",
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

        private static TextBox Campo(int x, int y, int w) => new TextBox
        {
            Location = new Point(x, y),
            Width = w,
            Font = new Font("Segoe UI", 9.5F)
        };

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

        private void Erro(string msg, Control? foco = null)
        {
            MessageBox.Show(msg, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foco?.Focus();
        }

        // ────────────────────────────────────────────────────────────────
        //  Classe auxiliar para o combo de fornecedores
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