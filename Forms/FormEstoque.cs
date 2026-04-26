using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FeirinhaCodorna.Forms
{
    public class FormEstoque : Form
    {
        private readonly BancoDados _db;

        private TextBox _txtBusca = null!;
        private DataGridView _grid = null!;
        private Button _btnNovo = null!;
        private Button _btnEntrada = null!;
        private Button _btnEditar = null!;
        private Button _btnExcluir = null!;

        // lista em memória para facilitar recuperar o produto selecionado
        private List<Produto> _lista = new();

        public FormEstoque(BancoDados db)
        {
            _db = db;
            Text = "Estoque / Produtos";
            Size = new Size(1100, 520);
            MinimumSize = new Size(900, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9F);

            ConstruirLayout();
            CarregarProdutos();
        }

        // ────────────────────────────────────────────────────────────────
        //  Layout
        // ────────────────────────────────────────────────────────────────
        private void ConstruirLayout()
        {
            // ── Barra de topo ────────────────────────────────────────────
            var pTopo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(245, 245, 242),
                Padding = new Padding(8, 10, 8, 6)
            };
            Controls.Add(pTopo);

            _txtBusca = new TextBox
            {
                PlaceholderText = "BUSCAR POR NOME OU CÓDIGO...",
                Location = new Point(8, 13),
                Width = 300,
                Height = 28,
                Font = new Font("Segoe UI", 10F)
            };
            _txtBusca.TextChanged += (s, e) => CarregarProdutos();
            pTopo.Controls.Add(_txtBusca);

            _btnNovo = BotaoCor("+ NOVO", Color.FromArgb(46, 160, 80), 320, 11, 100);
            _btnNovo.Click += BtnNovo_Click;
            pTopo.Controls.Add(_btnNovo);

            _btnEntrada = BotaoCor("ENTRADA", Color.FromArgb(30, 120, 200), 428, 11, 100);
            _btnEntrada.Click += BtnEntrada_Click;
            pTopo.Controls.Add(_btnEntrada);

            _btnEditar = BotaoCor("EDITAR", Color.FromArgb(100, 100, 100), 536, 11, 100);
            _btnEditar.Click += BtnEditar_Click;
            pTopo.Controls.Add(_btnEditar);

            _btnExcluir = BotaoCor("EXCLUIR", Color.FromArgb(200, 50, 50), 644, 11, 100);
            _btnExcluir.Click += BtnExcluir_Click;
            pTopo.Controls.Add(_btnExcluir);

            // ── Grid ─────────────────────────────────────────────────────
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32
            };
            _grid.DoubleClick += (s, e) => BtnEditar_Click(s, e);
            Controls.Add(_grid);

            ConfigurarColunas();
        }

        private void ConfigurarColunas()
        {
            _grid.Columns.Clear();

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EAN",
                HeaderText = "EAN",
                FillWeight = 80
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nome",
                HeaderText = "NOME",
                FillWeight = 280
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Unidade",
                HeaderText = "UNIDADE",
                FillWeight = 70
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Preco",
                HeaderText = "PREÇO",
                FillWeight = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estoque",
                HeaderText = "ESTOQUE",
                FillWeight = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N3"
                }
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstoqueMin",
                HeaderText = "MÍN.",
                FillWeight = 70,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N3"
                }
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Situacao",
                HeaderText = "SITUAÇÃO",
                FillWeight = 75
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Pesavel",
                HeaderText = "PESÁVEL",
                FillWeight = 65
            });
        }

        // ────────────────────────────────────────────────────────────────
        //  Carga de dados
        // ────────────────────────────────────────────────────────────────
        private void CarregarProdutos()
        {
            _grid.Rows.Clear();

            // ListarProdutos() já existe no BancoDados; filtramos em memória pelo texto
            _lista = _db.ListarProdutos();

            string filtro = _txtBusca.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filtro))
                _lista = _lista.FindAll(p =>
                    p.Nome.ToLower().Contains(filtro) ||
                    p.CodigoEan.Contains(filtro) ||
                    p.CodigoInterno.ToLower().Contains(filtro));

            foreach (var p in _lista)
            {
                string situacao = p.EstoqueBaixo ? "BAIXO" : "OK";
                string pesavel = p.Pesavel ? "SIM" : "NÃO";
                string ean = string.IsNullOrWhiteSpace(p.CodigoEan) ? p.CodigoInterno : p.CodigoEan;

                int idx = _grid.Rows.Add(
                    ean,
                    p.Nome,
                    p.Unidade,
                    p.Preco,
                    p.Estoque,
                    p.EstoqueMinimo,
                    situacao,
                    pesavel);

                if (p.EstoqueBaixo)
                    _grid.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(180, 60, 0);
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Produto selecionado no grid
        // ────────────────────────────────────────────────────────────────
        private Produto? ProdutoSelecionado()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um produto na lista.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            int rowIdx = _grid.SelectedRows[0].Index;
            if (rowIdx < 0 || rowIdx >= _lista.Count) return null;
            return _lista[rowIdx];
        }

        // ────────────────────────────────────────────────────────────────
        //  Ações dos botões
        // ────────────────────────────────────────────────────────────────
        private void BtnNovo_Click(object? sender, EventArgs e)
        {
            using var form = new FormProduto(_db, null);
            if (form.ShowDialog() == DialogResult.OK)
                CarregarProdutos();
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            var prod = ProdutoSelecionado();
            if (prod == null) return;

            using var form = new FormProduto(_db, prod);
            if (form.ShowDialog() == DialogResult.OK)
                CarregarProdutos();
        }

        private void BtnEntrada_Click(object? sender, EventArgs e)
        {
            var prod = ProdutoSelecionado();
            if (prod == null) return;

            // Diálogo simples de entrada de estoque
            using var dlg = new FormEntradaEstoque(_db, prod);
            if (dlg.ShowDialog() == DialogResult.OK)
                CarregarProdutos();
        }

        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            var prod = ProdutoSelecionado();
            if (prod == null) return;

            var confirm = MessageBox.Show(
                $"Excluir o produto \"{prod.Nome}\"?\nEsta ação não pode ser desfeita.",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _db.ExcluirProduto(prod.Id);   // ExcluirProduto(int id) — BancoDados linha ~281
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Helper visual
        // ────────────────────────────────────────────────────────────────
        private static Button BotaoCor(string texto, Color cor, int x, int y, int largura) =>
            new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(largura, 28),
                BackColor = cor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F),
                FlatAppearance = { BorderSize = 0 }
            };
    }
}