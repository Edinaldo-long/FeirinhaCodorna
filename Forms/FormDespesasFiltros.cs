using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormDespesasFiltros : Form
    {
        private readonly BancoDados _db;
        private List<Despesa> _todasDespesas = new();
        private List<Despesa> _filtradas = new();

        private static readonly Color CorVerde = Color.FromArgb(26, 122, 74);
        private static readonly Color CorVerdeFundo = Color.FromArgb(225, 245, 238);
        private static readonly Color CorVerdeTexto = Color.FromArgb(15, 110, 86);
        private static readonly Color CorVermelho = Color.FromArgb(163, 45, 45);
        private static readonly Color CorAmbar = Color.FromArgb(133, 79, 11);
        private static readonly Color CorAmbarFundo = Color.FromArgb(250, 238, 218);
        private static readonly Color CorFundo = Color.FromArgb(245, 245, 242);
        private static readonly Color CorBorda = Color.FromArgb(210, 210, 200);
        private static readonly Color CorTextoSec = Color.FromArgb(100, 100, 95);
        private static readonly Color CorAzulFundo = Color.FromArgb(230, 241, 251);
        private static readonly Color CorAzulTexto = Color.FromArgb(12, 68, 124);

        private DateTimePicker dtpDe = null!;
        private DateTimePicker dtpAte = null!;
        private ComboBox cmbCategoria = null!;
        private TextBox txtBusca = null!;

        private Button btnSitTodas = null!;
        private Button btnSitPendente = null!;
        private Button btnSitQuitado = null!;
        private Button btnSitVencido = null!;

        private Button btnTipoTodos = null!;
        private Button btnTipoParcelado = null!;
        private Button btnTipoAvista = null!;
        private Button btnTipo7dias = null!;

        private Label lblTotalValor = null!;
        private Label lblTotalCount = null!;
        private Label lblPendValor = null!;
        private Label lblPendCount = null!;
        private Label lblQuitValor = null!;
        private Label lblQuitCount = null!;
        private Label lblVencValor = null!;
        private Label lblVencCount = null!;

        private DataGridView grid = null!;

        private string _situacaoFiltro = "Todas";
        private string _tipoFiltro = "Todos";

        public FormDespesasFiltros(BancoDados db)
        {
            _db = db;
            InicializarComponentes();
            AplicarFiltros();
        }

        private void InicializarComponentes()
        {
            Text = "Lançamento de Despesas";
            Size = new Size(1100, 740);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.5f);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 16, 20, 16), BackColor = Color.White };

            var lblTitulo = new Label { Text = "Lançamento de Despesas", Font = new Font("Segoe UI", 14f), ForeColor = Color.FromArgb(30, 30, 30), AutoSize = true, Location = new Point(0, 0) };

            var pnlFiltros = new Panel { Location = new Point(0, 38), Size = new Size(1040, 190), BackColor = CorFundo };
            pnlFiltros.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(CorBorda), 0, 0, pnlFiltros.Width - 1, pnlFiltros.Height - 1);

            var lblDe = Rotulo("Período — De", new Point(12, 10));
            var lblAte = Rotulo("Até", new Point(220, 10));

            dtpDe = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), Location = new Point(12, 28), Size = new Size(195, 26) };
            dtpAte = new DateTimePicker { Format = DateTimePickerFormat.Short, Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)), Location = new Point(220, 28), Size = new Size(195, 26) };

            var btnMesAtual = BotaoAtalho("Este mês", new Point(430, 28));
            var btnMesAnter = BotaoAtalho("Mês anterior", new Point(520, 28));
            var btnAno = BotaoAtalho("Este ano", new Point(630, 28));
            var btnLimpar = BotaoAtalho("Limpar tudo", new Point(716, 28));

            btnMesAtual.Click += (_, __) => { dtpDe.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpAte.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)); AplicarFiltros(); };
            btnMesAnter.Click += (_, __) => { var ant = DateTime.Today.AddMonths(-1); dtpDe.Value = new DateTime(ant.Year, ant.Month, 1); dtpAte.Value = new DateTime(ant.Year, ant.Month, DateTime.DaysInMonth(ant.Year, ant.Month)); AplicarFiltros(); };
            btnAno.Click += (_, __) => { dtpDe.Value = new DateTime(DateTime.Today.Year, 1, 1); dtpAte.Value = new DateTime(DateTime.Today.Year, 12, 31); AplicarFiltros(); };
            btnLimpar.Click += (_, __) => LimparFiltros();

            var lblCat = Rotulo("Categoria", new Point(12, 64));
            var lblBusca = Rotulo("Busca por descrição", new Point(220, 64));

            cmbCategoria = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(12, 82), Size = new Size(195, 26) };
            cmbCategoria.Items.AddRange(new object[] { "(Todas)", "Aluguel", "Combustível › Carro", "Fornecedor", "Manutenção", "Outros" });
            cmbCategoria.SelectedIndex = 0;

            txtBusca = new TextBox { PlaceholderText = "Descrição...", Location = new Point(220, 82), Size = new Size(400, 26) };

            var lblSit = Rotulo("Situação:", new Point(12, 118));
            btnSitTodas = Chip("Todas", new Point(80, 115));
            btnSitPendente = Chip("Pendentes", new Point(148, 115));
            btnSitQuitado = Chip("Quitados", new Point(232, 115));
            btnSitVencido = Chip("Vencidos/Atrasados", new Point(314, 115));

            btnSitTodas.Click += (_, __) => SelecionarSituacao("Todas");
            btnSitPendente.Click += (_, __) => SelecionarSituacao("Pendente");
            btnSitQuitado.Click += (_, __) => SelecionarSituacao("Quitado");
            btnSitVencido.Click += (_, __) => SelecionarSituacao("Vencido");
            SelecionarSituacao("Todas");

            var lblTipo = Rotulo("Tipo:", new Point(12, 155));
            btnTipoTodos = Chip("Todos", new Point(58, 152));
            btnTipoParcelado = Chip("Parcelados", new Point(122, 152));
            btnTipoAvista = Chip("À vista", new Point(206, 152));
            btnTipo7dias = Chip("Vencem em 7 dias", new Point(276, 152));

            btnTipoTodos.Click += (_, __) => SelecionarTipo("Todos");
            btnTipoParcelado.Click += (_, __) => SelecionarTipo("Parcelados");
            btnTipoAvista.Click += (_, __) => SelecionarTipo("Avista");
            btnTipo7dias.Click += (_, __) => SelecionarTipo("7dias");
            SelecionarTipo("Todos");

            dtpDe.ValueChanged += (_, __) => AplicarFiltros();
            dtpAte.ValueChanged += (_, __) => AplicarFiltros();
            cmbCategoria.SelectedIndexChanged += (_, __) => AplicarFiltros();
            txtBusca.TextChanged += (_, __) => AplicarFiltros();

            pnlFiltros.Controls.AddRange(new Control[] { lblDe, lblAte, dtpDe, dtpAte, btnMesAtual, btnMesAnter, btnAno, btnLimpar, lblCat, lblBusca, cmbCategoria, txtBusca, lblSit, btnSitTodas, btnSitPendente, btnSitQuitado, btnSitVencido, lblTipo, btnTipoTodos, btnTipoParcelado, btnTipoAvista, btnTipo7dias });

            var pnlCards = new Panel { Location = new Point(0, 240), Size = new Size(1040, 70) };
            var cards = new (string label, Color corV, Action<Label, Label> bind)[]
            {
                ("Total filtrado", Color.FromArgb(30,30,30), (v, c) => { lblTotalValor = v; lblTotalCount = c; }),
                ("Pendentes", CorAmbar, (v, c) => { lblPendValor = v; lblPendCount = c; }),
                ("Quitados", CorVerde, (v, c) => { lblQuitValor = v; lblQuitCount = c; }),
                ("Vencidos", CorVermelho, (v, c) => { lblVencValor = v; lblVencCount = c; }),
            };

            int cx = 0;
            foreach (var (label, corV, bind) in cards)
            {
                var pnl = new Panel { Location = new Point(cx, 0), Size = new Size(248, 64), BackColor = CorFundo };
                pnl.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(CorBorda), 0, 0, pnl.Width - 1, pnl.Height - 1);
                var lLbl = new Label { Text = label, Font = new Font("Segoe UI", 8f), ForeColor = CorTextoSec, Location = new Point(10, 8), AutoSize = true };
                var lVal = new Label { Text = "R$ —", Font = new Font("Segoe UI", 12f), ForeColor = corV, Location = new Point(10, 24), AutoSize = true };
                var lCount = new Label { Text = "— lançamentos", Font = new Font("Segoe UI", 8f), ForeColor = CorTextoSec, Location = new Point(10, 48), AutoSize = true };
                pnl.Controls.AddRange(new Control[] { lLbl, lVal, lCount });
                pnlCards.Controls.Add(pnl);
                bind(lVal, lCount);
                cx += 256;
            }

            grid = new DataGridView { Location = new Point(0, 322), Size = new Size(1040, 360), ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None, BackgroundColor = Color.White, GridColor = CorBorda, Font = new Font("Segoe UI", 9f), ColumnHeadersHeight = 34, RowTemplate = { Height = 28 }, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None };
            grid.ColumnHeadersDefaultCellStyle.BackColor = CorFundo;
            grid.EnableHeadersVisualStyles = false;
            grid.DefaultCellStyle.SelectionBackColor = CorAzulFundo;
            grid.DefaultCellStyle.SelectionForeColor = CorAzulTexto;

            var colunas = new[] { ("Id", "Id", 50), ("Data", "Lançamento", 100), ("Categoria", "Categoria", 150), ("Descricao", "Descrição", 280), ("Valor", "Valor (R$)", 100), ("Vencimento", "Vencimento", 100), ("Parcela", "Parcela", 75), ("Situacao", "Situação", 90) };
            foreach (var (nome, header, largura) in colunas) grid.Columns.Add(new DataGridViewTextBoxColumn { Name = nome, HeaderText = header, Width = largura });
            grid.CellFormatting += Grid_CellFormatting;

            scroll.Controls.AddRange(new Control[] { lblTitulo, pnlFiltros, pnlCards, grid });
            Controls.Add(scroll);
        }

        private void AplicarFiltros()
        {
            string? cat = cmbCategoria.SelectedIndex <= 0 ? null : cmbCategoria.Text;
            string? busca = string.IsNullOrWhiteSpace(txtBusca.Text) ? null : txtBusca.Text;
            string? sitBanco = _situacaoFiltro == "Todas" ? null : _situacaoFiltro;

            // CORREÇÃO: Removi nomes de parâmetros para evitar erro CS1739
            _todasDespesas = _db.ListarDespesasFiltradas(
                dtpDe.Value.Date,
                dtpAte.Value.Date,
                cat,
                sitBanco,
                busca,
                _tipoFiltro == "Parcelados");

            _filtradas = _tipoFiltro switch
            {
                "Avista" => _todasDespesas.Where(d => !EhParcelado(d)).ToList(),
                "7dias" => _todasDespesas.Where(d => d.Vencimento.HasValue && d.Vencimento.Value.Date >= DateTime.Today && d.Vencimento.Value.Date <= DateTime.Today.AddDays(7) && d.Situacao != "Quitado").ToList(),
                _ => _todasDespesas
            };

            AtualizarCards();
            AtualizarGrid();
        }

        private static bool EhParcelado(Despesa d)
        {
            var txt = d.Descricao.Trim();
            if (!txt.EndsWith(")")) return false;
            int a = txt.LastIndexOf('(');
            if (a < 0) return false;
            var parte = txt[(a + 1)..^1];
            return parte.Contains('/');
        }

        private void AtualizarCards()
        {
            decimal total = _filtradas.Sum(d => d.Valor);
            var pend = _filtradas.Where(d => d.Situacao == "Pendente").ToList();
            var quit = _filtradas.Where(d => d.Situacao == "Quitado").ToList();
            var venc = _filtradas.Where(d => d.Situacao == "Vencido").ToList();

            lblTotalValor.Text = $"R$ {total:N2}";
            lblTotalCount.Text = $"{_filtradas.Count} lançamentos";
            lblPendValor.Text = $"R$ {pend.Sum(d => d.Valor):N2}";
            lblPendCount.Text = $"{pend.Count} lançamentos";
            lblQuitValor.Text = $"R$ {quit.Sum(d => d.Valor):N2}";
            lblQuitCount.Text = $"{quit.Count} lançamentos";
            lblVencValor.Text = $"R$ {venc.Sum(d => d.Valor):N2}";
            lblVencCount.Text = $"{venc.Count} lançamentos";
        }

        private void AtualizarGrid()
        {
            grid.Rows.Clear();
            foreach (var d in _filtradas)
            {
                string parcela = "—";
                var txt = d.Descricao.Trim();
                if (txt.EndsWith(")"))
                {
                    int a = txt.LastIndexOf('(');
                    if (a >= 0) { var parte = txt[(a + 1)..^1]; if (parte.Contains('/')) parcela = parte; }
                }
                grid.Rows.Add(d.Id, d.Data.ToString("dd/MM/yyyy"), d.Categoria, d.Descricao, d.Valor.ToString("N2"), d.Vencimento.HasValue ? d.Vencimento.Value.ToString("dd/MM/yyyy") : "—", parcela, d.Situacao);
                grid.Rows[^1].Tag = d;
            }
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = grid.Columns[e.ColumnIndex].Name;
            if (col == "Situacao" && e.Value != null)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                (e.CellStyle.BackColor, e.CellStyle.ForeColor) = e.Value.ToString() switch { "Quitado" => (CorVerdeFundo, CorVerdeTexto), "Vencido" => (Color.FromArgb(252, 235, 235), CorVermelho), _ => (CorAmbarFundo, CorAmbar) };
            }
        }

        private void SelecionarSituacao(string valor)
        {
            _situacaoFiltro = valor;
            var todos = new[] { btnSitTodas, btnSitPendente, btnSitQuitado, btnSitVencido };
            var sels = new[] { "Todas", "Pendente", "Quitado", "Vencido" };
            for (int i = 0; i < todos.Length; i++) EstilizarChip(todos[i], sels[i] == valor);
            AplicarFiltros();
        }

        private void SelecionarTipo(string valor)
        {
            _tipoFiltro = valor;
            var todos = new[] { btnTipoTodos, btnTipoParcelado, btnTipoAvista, btnTipo7dias };
            var sels = new[] { "Todos", "Parcelados", "Avista", "7dias" };
            for (int i = 0; i < todos.Length; i++) EstilizarChip(todos[i], sels[i] == valor);
            AplicarFiltros();
        }

        private static void EstilizarChip(Button btn, bool ativo)
        {
            btn.BackColor = ativo ? Color.FromArgb(230, 241, 251) : Color.White;
            btn.ForeColor = ativo ? Color.FromArgb(12, 68, 124) : Color.FromArgb(80, 80, 75);
            btn.FlatAppearance.BorderColor = ativo ? Color.FromArgb(133, 183, 235) : Color.FromArgb(210, 210, 200);
        }

        private void LimparFiltros()
        {
            dtpDe.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpAte.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            cmbCategoria.SelectedIndex = 0;
            txtBusca.Clear();
            SelecionarSituacao("Todas");
            SelecionarTipo("Todos");
        }

        private static Label Rotulo(string texto, Point loc) => new Label { Text = texto, Font = new Font("Segoe UI", 8f), ForeColor = Color.FromArgb(100, 100, 95), Location = loc, AutoSize = true };
        private static Button BotaoAtalho(string texto, Point loc) => new Button { Text = texto, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = Color.FromArgb(60, 60, 55), Font = new Font("Segoe UI", 8.5f), Cursor = Cursors.Hand, Height = 26, FlatAppearance = { BorderColor = Color.FromArgb(210, 210, 200) } };
        private static Button Chip(string texto, Point loc) => new Button { Text = texto, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = Color.FromArgb(80, 80, 75), Font = new Font("Segoe UI", 8.5f), Cursor = Cursors.Hand, Height = 26, FlatAppearance = { BorderColor = Color.FromArgb(210, 210, 200) } };
    }
}