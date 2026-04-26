using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeirinhaCodorna.Forms
{
    /// <summary>
    /// Relatórios completos: Vendas por período, Despesas, Estornos e Resumo financeiro.
    /// </summary>
    public class FormRelatorio : Form
    {
        private readonly BancoDados _db;

        private TabControl _tabs = null!;
        private DateTimePicker _dtDe = null!, _dtAte = null!;
        private Button _btnAtualizar = null!;

        // ── Aba Vendas ───────────────────────────────────────────────────
        private DataGridView _gridVendas = null!;
        private Label _lblTotalVendas = null!;

        // ── Aba Despesas ─────────────────────────────────────────────────
        private DataGridView _gridDespesas = null!;
        private Label _lblTotalDespesas = null!;

        // ── Aba Estornos ─────────────────────────────────────────────────
        private DataGridView _gridEstornos = null!;
        private Label _lblTotalEstornos = null!;

        // ── Aba Resumo ───────────────────────────────────────────────────
        private Panel _painelResumo = null!;

        public FormRelatorio(BancoDados db)
        {
            _db = db;
            Text = "Relatórios";
            Size = new Size(960, 660);
            MinimumSize = new Size(860, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9F);

            ConstruirLayout();
            AtualizarRelatorios();
        }

        // ────────────────────────────────────────────────────────────────
        //  Layout principal
        // ────────────────────────────────────────────────────────────────
        private void ConstruirLayout()
        {
            // Título
            Controls.Add(new Label
            {
                Text = "📊  Relatórios",
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(60, 60, 55),
                AutoSize = true,
                Location = new Point(16, 14)
            });

            // Painel de período
            var pFiltro = new Panel
            {
                Location = new Point(16, 46),
                Size = new Size(916, 44),
                BackColor = Color.FromArgb(235, 235, 230)
            };
            Controls.Add(pFiltro);

            pFiltro.Controls.Add(Rotulo("Período — De:", 8, 13));
            _dtDe = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(108, 10),
                Width = 110,
                Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
            };
            pFiltro.Controls.Add(_dtDe);

            pFiltro.Controls.Add(Rotulo("Até:", 228, 13));
            _dtAte = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(256, 10),
                Width = 110,
                Value = DateTime.Today
            };
            pFiltro.Controls.Add(_dtAte);

            _btnAtualizar = Botao("🔄 Atualizar", 380, 8, 120,
                Color.FromArgb(70, 130, 180));
            _btnAtualizar.Click += (s, e) => AtualizarRelatorios();
            pFiltro.Controls.Add(_btnAtualizar);

            var btnHoje = Botao("Hoje", 510, 8, 70, Color.FromArgb(100, 140, 100));
            btnHoje.Click += (s, e) => { _dtDe.Value = _dtAte.Value = DateTime.Today; AtualizarRelatorios(); };
            pFiltro.Controls.Add(btnHoje);

            var btnMes = Botao("Este mês", 588, 8, 90, Color.FromArgb(100, 140, 100));
            btnMes.Click += (s, e) =>
            {
                _dtDe.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                _dtAte.Value = DateTime.Today;
                AtualizarRelatorios();
            };
            pFiltro.Controls.Add(btnMes);

            var btnExportar = Botao("💾 Exportar CSV", 700, 8, 130, Color.FromArgb(130, 90, 160));
            btnExportar.Click += BtnExportar_Click;
            pFiltro.Controls.Add(btnExportar);

            // TabControl
            _tabs = new TabControl
            {
                Location = new Point(16, 100),
                Size = new Size(916, 510),
                Font = new Font("Segoe UI", 9.5F)
            };
            Controls.Add(_tabs);

            CriarAbaVendas();
            CriarAbaDespesas();
            CriarAbaEstornos();
            CriarAbaResumo();
        }

        // ────────────────────────────────────────────────────────────────
        //  Aba Vendas
        // ────────────────────────────────────────────────────────────────
        private void CriarAbaVendas()
        {
            var tab = new TabPage("🛒  Vendas");
            _tabs.TabPages.Add(tab);

            _gridVendas = CriarGrid();
            _gridVendas.Size = new Size(892, 410);
            tab.Controls.Add(_gridVendas);

            _gridVendas.Columns.Add(Col("Id", "Nº", 60));
            _gridVendas.Columns.Add(Col("DataHora", "Data/Hora", 130));
            _gridVendas.Columns.Add(Col("Cliente", "Cliente", 200));
            _gridVendas.Columns.Add(Col("Pagamento", "Pagamento", 120));
            _gridVendas.Columns.Add(ColDinheiro("Total", "Total (R$)", 100));

            _lblTotalVendas = RodapeTotalizador(tab, 424);
        }

        // ────────────────────────────────────────────────────────────────
        //  Aba Despesas
        // ────────────────────────────────────────────────────────────────
        private void CriarAbaDespesas()
        {
            var tab = new TabPage("💸  Despesas");
            _tabs.TabPages.Add(tab);

            _gridDespesas = CriarGrid();
            _gridDespesas.Size = new Size(892, 410);
            tab.Controls.Add(_gridDespesas);

            _gridDespesas.Columns.Add(Col("Data", "Data", 90));
            _gridDespesas.Columns.Add(Col("Descricao", "Descrição", 250));
            _gridDespesas.Columns.Add(Col("Categoria", "Categoria", 130));
            _gridDespesas.Columns.Add(Col("Situacao", "Situação", 90));
            _gridDespesas.Columns.Add(ColDinheiro("Valor", "Valor (R$)", 100));

            _lblTotalDespesas = RodapeTotalizador(tab, 424);
        }

        // ────────────────────────────────────────────────────────────────
        //  Aba Estornos
        // ────────────────────────────────────────────────────────────────
        private void CriarAbaEstornos()
        {
            var tab = new TabPage("🔄  Estornos");
            _tabs.TabPages.Add(tab);

            _gridEstornos = CriarGrid();
            _gridEstornos.Size = new Size(892, 410);
            tab.Controls.Add(_gridEstornos);

            _gridEstornos.Columns.Add(Col("Id", "Nº Cupom", 70));
            _gridEstornos.Columns.Add(Col("DataHora", "Data/Hora", 130));
            _gridEstornos.Columns.Add(Col("Cliente", "Cliente", 200));
            _gridEstornos.Columns.Add(Col("Motivo", "Motivo", 250));
            _gridEstornos.Columns.Add(ColDinheiro("Total", "Total (R$)", 100));

            _lblTotalEstornos = RodapeTotalizador(tab, 424);
        }

        // ────────────────────────────────────────────────────────────────
        //  Aba Resumo
        // ────────────────────────────────────────────────────────────────
        private void CriarAbaResumo()
        {
            var tab = new TabPage("📈  Resumo Financeiro");
            _tabs.TabPages.Add(tab);

            _painelResumo = new Panel
            {
                Location = new Point(4, 4),
                Size = new Size(892, 460),
                AutoScroll = true,
                BackColor = Color.White
            };
            tab.Controls.Add(_painelResumo);
        }

        // ────────────────────────────────────────────────────────────────
        //  Atualizar todos os dados
        // ────────────────────────────────────────────────────────────────
        private void AtualizarRelatorios()
        {
            DateTime de = _dtDe.Value.Date;
            DateTime ate = _dtAte.Value.Date;

            var vendas = CarregarVendas(de, ate);
            var despesas = CarregarDespesas(de, ate);
            var estornos = CarregarEstornos(de, ate);

            AtualizarResumo(vendas, despesas, estornos, de, ate);
        }

        // ── Vendas ───────────────────────────────────────────────────────
        private List<(int Id, DateTime Dt, string Cliente, string Pag, decimal Total)>
            CarregarVendas(DateTime de, DateTime ate)
        {
            _gridVendas.Rows.Clear();
            var lista = _db.BuscarVendasParaEstorno("", de, ate); // reutiliza a query existente
            var resultado = new List<(int, DateTime, string, string, decimal)>();

            foreach (var v in lista)
            {
                _gridVendas.Rows.Add(
                    v.Id,
                    v.DataHora.ToString("dd/MM/yyyy HH:mm"),
                    string.IsNullOrWhiteSpace(v.ClienteNome) ? "(sem cliente)" : v.ClienteNome,
                    v.FormaPagamento,
                    v.Total);
                resultado.Add((v.Id, v.DataHora, v.ClienteNome, v.FormaPagamento, v.Total));
            }

            decimal totalGeral = resultado.Sum(x => x.Item5);
            _lblTotalVendas.Text =
                $"Total de vendas no período: R$ {totalGeral:N2}   |   {resultado.Count} venda(s)";

            return resultado;
        }

        // ── Despesas ─────────────────────────────────────────────────────
        private List<Despesa> CarregarDespesas(DateTime de, DateTime ate)
        {
            _gridDespesas.Rows.Clear();
            var lista = _db.ListarDespesasFiltradas(de, ate, null, null, null, false);

            foreach (var d in lista)
                _gridDespesas.Rows.Add(
                    d.Data.ToString("dd/MM/yyyy"),
                    d.Descricao,
                    d.Categoria,
                    d.Situacao,
                    d.Valor);

            decimal total = lista.Sum(d => d.Valor);
            _lblTotalDespesas.Text =
                $"Total de despesas no período: R$ {total:N2}   |   {lista.Count} lançamento(s)";

            return lista;
        }

        // ── Estornos ─────────────────────────────────────────────────────
        private List<(int Id, DateTime Dt, string Cliente, string Motivo, decimal Total)>
            CarregarEstornos(DateTime de, DateTime ate)
        {
            _gridEstornos.Rows.Clear();
            var resultado = new List<(int, DateTime, string, string, decimal)>();

            // Busca vendas ESTORNADAS no período
            // Reusa BuscarVendasParaEstorno mas com filtro invertido
            var estornos = _db.BuscarVendasEstornadas(de, ate);

            foreach (var v in estornos)
            {
                // motivo fica em FormaPagamento como "ESTORNADA:motivo"
                string motivo = v.FormaPagamento.StartsWith("ESTORNADA:")
                    ? v.FormaPagamento[10..]
                    : v.FormaPagamento;

                _gridEstornos.Rows.Add(
                    v.Id,
                    v.DataHora.ToString("dd/MM/yyyy HH:mm"),
                    string.IsNullOrWhiteSpace(v.ClienteNome) ? "(sem cliente)" : v.ClienteNome,
                    motivo,
                    v.Total);

                resultado.Add((v.Id, v.DataHora, v.ClienteNome, motivo, v.Total));
            }

            decimal total = resultado.Sum(x => x.Item5);
            _lblTotalEstornos.Text =
                $"Total estornado no período: R$ {total:N2}   |   {resultado.Count} estorno(s)";

            return resultado;
        }

        // ── Resumo financeiro ────────────────────────────────────────────
        private void AtualizarResumo(
            List<(int Id, DateTime Dt, string Cliente, string Pag, decimal Total)> vendas,
            List<Despesa> despesas,
            List<(int Id, DateTime Dt, string Cliente, string Motivo, decimal Total)> estornos,
            DateTime de, DateTime ate)
        {
            _painelResumo.Controls.Clear();

            decimal totalVendas = vendas.Sum(v => v.Total);
            decimal totalEstornos = estornos.Sum(e => e.Item5);
            decimal totalDespesas = despesas.Sum(d => d.Valor);
            decimal receitaLiquida = totalVendas - totalEstornos;
            decimal resultado = receitaLiquida - totalDespesas;

            // ─ por forma de pagamento ─
            var porForma = vendas
                .GroupBy(v => v.Pag)
                .Select(g => (Forma: g.Key, Total: g.Sum(x => x.Total), Qtd: g.Count()))
                .OrderByDescending(x => x.Total)
                .ToList();

            int y = 14;
            int cx = 20; // coluna esquerda

            Cartao(_painelResumo, ref y, cx, "📅  Período",
                $"{de:dd/MM/yyyy} a {ate:dd/MM/yyyy}");

            Cartao(_painelResumo, ref y, cx, "🛒  Total Bruto de Vendas",
                $"R$ {totalVendas:N2}  ({vendas.Count} vendas)",
                Color.FromArgb(30, 120, 60));

            Cartao(_painelResumo, ref y, cx, "🔄  Total Estornado",
                $"R$ {totalEstornos:N2}  ({estornos.Count} estornos)",
                Color.FromArgb(180, 60, 60));

            Cartao(_painelResumo, ref y, cx, "✅  Receita Líquida",
                $"R$ {receitaLiquida:N2}",
                Color.FromArgb(30, 100, 180));

            Cartao(_painelResumo, ref y, cx, "💸  Total Despesas",
                $"R$ {totalDespesas:N2}  ({despesas.Count} lançamentos)",
                Color.FromArgb(150, 90, 0));

            Color corResultado = resultado >= 0
                ? Color.FromArgb(20, 140, 60)
                : Color.FromArgb(180, 30, 30);
            string icone = resultado >= 0 ? "🟢" : "🔴";
            Cartao(_painelResumo, ref y, cx, $"{icone}  Resultado do Período",
                $"R$ {resultado:N2}",
                corResultado, bold: true);

            // ─ Por forma de pagamento ─
            y += 10;
            var lblFormas = new Label
            {
                Text = "Vendas por Forma de Pagamento:",
                Font = new Font("Segoe UI Semibold", 10F),
                AutoSize = true,
                Location = new Point(cx, y),
                ForeColor = Color.FromArgb(60, 60, 55)
            };
            _painelResumo.Controls.Add(lblFormas);
            y += 24;

            foreach (var (forma, total, qtd) in porForma)
            {
                var lblF = new Label
                {
                    Text = $"   • {forma,-18}  R$ {total,10:N2}   ({qtd} venda{(qtd > 1 ? "s" : "")})",
                    Font = new Font("Consolas", 9.5F),
                    AutoSize = true,
                    Location = new Point(cx + 10, y),
                    ForeColor = Color.FromArgb(70, 70, 65)
                };
                _painelResumo.Controls.Add(lblF);
                y += 20;
            }
        }

        // ────────────────────────────────────────────────────────────────
        //  Exportar CSV (aba ativa)
        // ────────────────────────────────────────────────────────────────
        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            DataGridView? grid = _tabs.SelectedIndex switch
            {
                0 => _gridVendas,
                1 => _gridDespesas,
                2 => _gridEstornos,
                _ => null
            };

            if (grid == null)
            {
                MessageBox.Show("Selecione uma aba de dados (Vendas, Despesas ou Estornos) para exportar.",
                    "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"relatorio_{_tabs.SelectedTab!.Text.Trim()}_{DateTime.Today:yyyyMMdd}.csv"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();

            // cabeçalho
            var headers = new List<string>();
            foreach (DataGridViewColumn col in grid.Columns)
                headers.Add($"\"{col.HeaderText}\"");
            sb.AppendLine(string.Join(";", headers));

            // linhas
            foreach (DataGridViewRow row in grid.Rows)
            {
                var cells = new List<string>();
                foreach (DataGridViewCell cell in row.Cells)
                    cells.Add($"\"{cell.Value?.ToString()?.Replace("\"", "\"\"") ?? ""}\"");
                sb.AppendLine(string.Join(";", cells));
            }

            System.IO.File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"✅ Arquivo exportado com sucesso!\n{dlg.FileName}",
                "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ────────────────────────────────────────────────────────────────
        //  Helpers visuais
        // ────────────────────────────────────────────────────────────────
        private static DataGridView CriarGrid() => new DataGridView
        {
            Location = new Point(4, 4),
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private static DataGridViewTextBoxColumn Col(string name, string header, int weight) =>
            new DataGridViewTextBoxColumn { Name = name, HeaderText = header, FillWeight = weight };

        private static DataGridViewTextBoxColumn ColDinheiro(string name, string header, int weight) =>
            new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                FillWeight = weight,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            };

        private static Label RodapeTotalizador(TabPage tab, int y)
        {
            var lbl = new Label
            {
                Location = new Point(4, y),
                Size = new Size(880, 24),
                Font = new Font("Segoe UI Semibold", 9.5F),
                ForeColor = Color.FromArgb(50, 80, 130),
                Text = "Carregando...",
                TextAlign = ContentAlignment.MiddleRight
            };
            tab.Controls.Add(lbl);
            return lbl;
        }

        private static Label Rotulo(string texto, int x, int y) => new Label
        {
            Text = texto,
            AutoSize = true,
            Location = new Point(x, y),
            ForeColor = Color.FromArgb(80, 80, 75)
        };

        private static Button Botao(string texto, int x, int y, int largura, Color cor) => new Button
        {
            Text = texto,
            Location = new Point(x, y),
            Size = new Size(largura, 28),
            BackColor = cor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F)
        };

        private void Cartao(Panel painel, ref int y, int x,
            string titulo, string valor,
            Color? cor = null, bool bold = false)
        {
            var p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(600, 36),
                BackColor = Color.FromArgb(248, 248, 246),
                BorderStyle = BorderStyle.FixedSingle
            };

            p.Controls.Add(new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 95),
                AutoSize = true,
                Location = new Point(8, 4)
            });

            p.Controls.Add(new Label
            {
                Text = valor,
                Font = bold
                    ? new Font("Segoe UI Semibold", 11F)
                    : new Font("Segoe UI", 10F),
                ForeColor = cor ?? Color.FromArgb(60, 60, 55),
                AutoSize = true,
                Location = new Point(8, 17)
            });

            painel.Controls.Add(p);
            y += 44;
        }
    }
}