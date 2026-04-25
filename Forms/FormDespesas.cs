using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public partial class FormDespesas : Form
    {
        private readonly BancoDados _db;

        private NumericUpDown nudParcelas = new();
        private DataGridView grid = new();
        private ComboBox cboCategoria = new();
        private ComboBox cboSubcategoria = new();
        private Label lblSubcategoria = new();
        private TextBox txtDescricao = new();
        private TextBox txtValor = new();
        private DateTimePicker dtpData = new();
        private DateTimePicker dtpVencimento = new();
        private CheckBox chkSemVencimento = new();
        private Label lblTotalMes = new();
        private Button btnSalvar = new();
        private Button btnExcluir = new();
        private Button btnDarBaixa = new();
        private Button btnNovaCategoria = new();

        private List<Despesa> _despesasDoMes = new();

        private readonly List<string> _categorias = new()
        {
            "Aluguel", "Água", "Energia Elétrica", "Internet / Telefone",
            "Combustível", "Fornecedor", "Funcionário", "Manutenção",
            "Material de Limpeza", "Material de Escritório", "Impostos / Taxas", "Outros"
        };

        private readonly Dictionary<string, List<string>> _subcategorias = new()
        {
            ["Manutenção"] = new() { "Freezer / Geladeira", "Carro / Moto", "Encanamento", "Elétrica", "Computador", "Ar-condicionado", "Balança", "Caixa registradora", "Estrutura / Obra", "Outro" },
            ["Funcionário"] = new() { "Salário", "Vale transporte", "Vale alimentação", "13º Salário", "Férias", "FGTS / INSS", "Rescisão", "Outro" },
            ["Impostos / Taxas"] = new() { "Simples Nacional", "IPTU", "Alvará", "Vigilância Sanitária", "Bombeiros", "Contador", "Outro" },
            ["Material de Limpeza"] = new() { "Produtos de limpeza", "Pano / Esponja", "Lixeira / Saco de lixo", "EPI / Luva", "Outro" },
            ["Material de Escritório"] = new() { "Papel / Impressão", "Caneta / Caderno", "Etiqueta / Placa", "Outro" },
            ["Combustível"] = new() { "Carro", "Moto", "Caminhão / Van", "Maquinário", "Outro" },
            ["Outros"] = new() { "Brinde / Cortesia", "Doação", "Multa", "Taxa bancária", "Outro" },
        };

        public FormDespesas(BancoDados db)
        {
            _db = db;
            InitializeComponent();
            ConstruirLayout();
            CarregarCategorias();
            CarregarDespesas();
        }

        private void ConstruirLayout()
        {
            Text = "Despesas";
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9.5f);

            // ── Topo ──────────────────────────────────────────────────────────
            var pnlTopo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(235, 233, 228),
                Padding = new Padding(16, 0, 16, 0)
            };
            pnlTopo.Controls.Add(new Label
            {
                Text = "💸  Lançamento de Despesas",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 55),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            });

            // ── Formulário de lançamento ───────────────────────────────────────
            var pnlForm = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(16, 8, 16, 6),
                BackColor = Color.FromArgb(245, 245, 242)
            };

            int y1 = 6;

            var lblCat = Rotulo("Categoria", 0, y1);
            cboCategoria = new ComboBox
            {
                Left = 0,
                Top = y1 + 16,
                Width = 155,
                Height = 26,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f)
            };

            lblSubcategoria = Rotulo("Detalhe", 163, y1);
            cboSubcategoria = new ComboBox
            {
                Left = 163,
                Top = y1 + 16,
                Width = 150,
                Height = 26,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Visible = false
            };
            lblSubcategoria.Visible = false;

            var lblDesc = Rotulo("Descrição", 163, y1);
            txtDescricao = new TextBox
            {
                Left = 163,
                Top = y1 + 16,
                Width = 350,
                Height = 26,
                Font = new Font("Segoe UI", 9.5f),
                PlaceholderText = "Ex: Conta de energia maio"
            };

            var lblVal = Rotulo("Valor (R$)", 521, y1);
            txtValor = new TextBox
            {
                Left = 521,
                Top = y1 + 16,
                Width = 90,
                Height = 26,
                Font = new Font("Segoe UI", 9.5f),
                PlaceholderText = "0,00"
            };
            txtValor.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.' && e.KeyChar != '\b')
                    e.Handled = true;
            };

            var lblParc = Rotulo("Parcelas", 619, y1);
            nudParcelas = new NumericUpDown
            {
                Left = 619,
                Top = y1 + 16,
                Width = 55,
                Height = 26,
                Minimum = 1,
                Maximum = 48,
                Value = 1,
                Font = new Font("Segoe UI", 9.5f)
            };

            btnNovaCategoria = BotaoAcao("＋ Categoria", Color.FromArgb(100, 110, 160), 682, y1 + 12, 130, 30);
            btnNovaCategoria.Click += BtnNovaCategoria_Click;

            // ── Linha 2 ────────────────────────────────────────────────────────
            int y2 = y1 + 58;

            var lblDt = Rotulo("Dt. Lançamento", 0, y2);
            dtpData = new DateTimePicker
            {
                Left = 0,
                Top = y2 + 16,
                Width = 120,
                Height = 26,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 9.5f)
            };

            var lblVenc = Rotulo("Vencto. 1ª Parcela", 128, y2);
            dtpVencimento = new DateTimePicker
            {
                Left = 128,
                Top = y2 + 16,
                Width = 120,
                Height = 26,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(30),
                Font = new Font("Segoe UI", 9.5f)
            };

            chkSemVencimento = new CheckBox
            {
                Text = "Sem venc.",
                Left = 256,
                Top = y2 + 18,
                Width = 90,
                Height = 24,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 80, 75)
            };
            chkSemVencimento.CheckedChanged += (s, e) =>
                dtpVencimento.Enabled = !chkSemVencimento.Checked;

            btnSalvar = BotaoAcao("✔  Lançar", Color.FromArgb(80, 140, 90), 354, y2 + 12, 120, 30);
            btnSalvar.Click += BtnSalvar_Click;

            // ── Evento categoria changed ───────────────────────────────────────
            cboCategoria.SelectedIndexChanged += (s, e) =>
            {
                string cat = cboCategoria.SelectedItem?.ToString() ?? "";
                bool temDetalhe = _subcategorias.ContainsKey(cat);

                if (temDetalhe)
                {
                    cboSubcategoria.Visible = true;
                    lblSubcategoria.Visible = true;
                    lblDesc.Left = 321;
                    txtDescricao.Left = 321;
                    txtDescricao.Width = 192;
                }
                else
                {
                    cboSubcategoria.Visible = false;
                    lblSubcategoria.Visible = false;
                    lblDesc.Left = 163;
                    txtDescricao.Left = 163;
                    txtDescricao.Width = 350;
                }

                if (temDetalhe)
                {
                    cboSubcategoria.Items.Clear();
                    foreach (var sub in _subcategorias[cat])
                        cboSubcategoria.Items.Add(sub);
                    cboSubcategoria.SelectedIndex = 0;
                }

                if (cat == "Fornecedor")
                {
                    var fornecedores = _db.ListarFornecedores();
                    var source = new AutoCompleteStringCollection();
                    foreach (var f in fornecedores) source.Add(f.Nome);
                    txtDescricao.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    txtDescricao.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    txtDescricao.AutoCompleteCustomSource = source;
                    txtDescricao.PlaceholderText = "Nome do fornecedor...";
                }
                else
                {
                    txtDescricao.AutoCompleteMode = AutoCompleteMode.None;
                    txtDescricao.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                    txtDescricao.PlaceholderText = "Ex: Conta de energia maio";
                }
            };

            lblTotalMes = new Label
            {
                Left = 0,
                Top = y2 + 52,
                Width = 500,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 60, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            pnlForm.Controls.AddRange(new Control[]
            {
                lblCat, cboCategoria,
                lblSubcategoria, cboSubcategoria,
                lblDesc, txtDescricao,
                lblVal, txtValor,
                lblParc, nudParcelas,
                btnNovaCategoria,
                lblDt, dtpData,
                lblVenc, dtpVencimento, chkSemVencimento,
                btnSalvar,
                lblTotalMes
            });

            // ── Barra do grid ──────────────────────────────────────────────────
            var pnlBarra = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.FromArgb(238, 236, 230),
                Padding = new Padding(12, 4, 12, 4)
            };
            pnlBarra.Controls.Add(new Label
            {
                Text = $"Despesas de {DateTime.Now:MMMM/yyyy}",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 75),
                Dock = DockStyle.Left,
                Width = 240,
                TextAlign = ContentAlignment.MiddleLeft
            });

            // Botão Excluir
            btnExcluir = BotaoAcao("🗑  Excluir", Color.FromArgb(180, 70, 70), 0, 4, 110, 26);
            btnExcluir.Dock = DockStyle.Right;
            btnExcluir.Enabled = false;
            btnExcluir.Click += BtnExcluir_Click;

            // Botão Dar Baixa — fica à esquerda do Excluir
            btnDarBaixa = BotaoAcao("✓  Dar Baixa", Color.FromArgb(26, 122, 74), 0, 4, 120, 26);
            btnDarBaixa.Dock = DockStyle.Right;
            btnDarBaixa.Enabled = false;
            btnDarBaixa.Click += BtnDarBaixa_Click;

            // Ordem importa: adiciona Excluir antes para ficar mais à direita
            pnlBarra.Controls.Add(btnExcluir);
            pnlBarra.Controls.Add(btnDarBaixa);

            // ── Grid ──────────────────────────────────────────────────────────
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.FromArgb(225, 225, 220),
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(235, 233, 228),
                    ForeColor = Color.FromArgb(60, 60, 55),
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Padding = new Padding(6, 0, 0, 0)
                }
            };
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 220, 200);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);
            grid.RowTemplate.Height = 30;

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", FillWeight = 4, Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colData", HeaderText = "Dt. Lançamento", FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "Categoria", FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescricao", HeaderText = "Descrição", FillWeight = 35 });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colValor",
                HeaderText = "Valor (R$)",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colVencimento",
                HeaderText = "Vencimento",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSituacao",
                HeaderText = "Situação",
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            grid.CellFormatting += Grid_CellFormatting;
            grid.SelectionChanged += Grid_SelectionChanged;

            Controls.Add(grid);
            Controls.Add(pnlBarra);
            Controls.Add(pnlForm);
            Controls.Add(pnlTopo);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static Label Rotulo(string texto, int left, int top) => new Label
        {
            Text = texto,
            Left = left,
            Top = top,
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 100, 95),
            Font = new Font("Segoe UI", 8f)
        };

        private static Button BotaoAcao(string texto, Color cor, int left, int top, int w, int h)
        {
            var btn = new Button
            {
                Text = texto,
                Left = left,
                Top = top,
                Width = w,
                Height = h,
                FlatStyle = FlatStyle.Flat,
                BackColor = cor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ── Formatação do grid ─────────────────────────────────────────────────

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = grid.Rows[e.RowIndex];
            var cat = row.Cells["colCategoria"].Value?.ToString() ?? "";
            string catBase = cat.Contains(" › ") ? cat.Split(" › ")[0] : cat;

            // Cor de fundo por categoria
            var corFundo = catBase switch
            {
                "Energia Elétrica" => Color.FromArgb(255, 253, 230),
                "Água" => Color.FromArgb(230, 245, 255),
                "Aluguel" => Color.FromArgb(255, 238, 230),
                "Funcionário" => Color.FromArgb(235, 250, 235),
                "Fornecedor" => Color.FromArgb(245, 235, 255),
                "Combustível" => Color.FromArgb(255, 245, 220),
                "Manutenção" => Color.FromArgb(255, 248, 225),
                "Impostos / Taxas" => Color.FromArgb(240, 235, 255),
                _ => Color.White
            };

            if (!row.Selected)
                row.DefaultCellStyle.BackColor = corFundo;

            // Cor do vencimento
            var vencStr = row.Cells["colVencimento"].Value?.ToString();
            if (!string.IsNullOrEmpty(vencStr) && vencStr != "—" &&
                DateTime.TryParseExact(vencStr, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime vencDate))
            {
                if (e.ColumnIndex == grid.Columns["colVencimento"].Index)
                {
                    int dias = (vencDate - DateTime.Today).Days;
                    e.CellStyle!.ForeColor = dias < 0 ? Color.Red
                                           : dias <= 5 ? Color.OrangeRed
                                                        : Color.DarkGreen;
                    e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                }
            }

            // Cor da coluna Situação
            if (e.ColumnIndex == grid.Columns["colSituacao"].Index)
            {
                var sit = row.Cells["colSituacao"].Value?.ToString();
                if (sit == "Quitado")
                {
                    e.CellStyle!.BackColor = Color.FromArgb(225, 245, 238);
                    e.CellStyle.ForeColor = Color.FromArgb(15, 110, 86);
                    e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle!.BackColor = Color.FromArgb(250, 238, 218);
                    e.CellStyle.ForeColor = Color.FromArgb(133, 79, 11);
                    e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                }
            }
        }

        // ── Seleção do grid ────────────────────────────────────────────────────

        private void Grid_SelectionChanged(object? sender, EventArgs e)
        {
            bool temSeleção = grid.SelectedRows.Count > 0;

            if (!temSeleção)
            {
                btnDarBaixa.Enabled = false;
                btnExcluir.Enabled = false;
                return;
            }

            var row = grid.SelectedRows[0];
            var sit = row.Cells["colSituacao"].Value?.ToString() ?? "Pendente";

            btnExcluir.Enabled = true;
            btnDarBaixa.Enabled = sit != "Quitado";
        }

        // ── Carga de dados ─────────────────────────────────────────────────────

        private void CarregarCategorias()
        {
            cboCategoria.Items.Clear();
            foreach (var c in _categorias)
                cboCategoria.Items.Add(c);
            cboCategoria.SelectedIndex = 0;
        }

        private void CarregarDespesas()
        {
            grid.Rows.Clear();
            _despesasDoMes = _db.ListarDespesasDoMes();
            decimal total = 0;

            foreach (var d in _despesasDoMes)
            {
                string venc = d.Vencimento.HasValue
                    ? d.Vencimento.Value.ToString("dd/MM/yyyy") : "—";
                string sit = d.Situacao ?? "Pendente";

                grid.Rows.Add(
                    d.Id,
                    d.Data.ToString("dd/MM/yyyy"),
                    d.Categoria,
                    d.Descricao,
                    d.Valor.ToString("N2"),
                    venc,
                    sit
                );
                total += d.Valor;
            }

            lblTotalMes.Text = $"  Total de despesas no mês: R$ {total:N2}";
        }

        private string CategoriaFinal()
        {
            string cat = cboCategoria.SelectedItem?.ToString() ?? "Outros";
            if (cboSubcategoria.Visible && cboSubcategoria.SelectedItem != null)
            {
                string sub = cboSubcategoria.SelectedItem.ToString()!;
                if (sub != "Outro")
                    return $"{cat} › {sub}";
            }
            return cat;
        }

        // ── Handlers dos botões ────────────────────────────────────────────────

        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Informe a descrição da despesa.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescricao.Focus();
                return;
            }

            string valorStr = txtValor.Text.Trim().Replace('.', ',');
            if (!decimal.TryParse(valorStr,
                    System.Globalization.NumberStyles.Number,
                    new System.Globalization.CultureInfo("pt-BR"),
                    out decimal valorTotal) || valorTotal <= 0)
            {
                MessageBox.Show("Informe um valor válido.\nExemplo: 150,75", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return;
            }

            try
            {
                int parcelas = (int)nudParcelas.Value;
                decimal valorParcela = Math.Round(valorTotal / parcelas, 2);
                string descBase = txtDescricao.Text.Trim();
                string categoria = CategoriaFinal();
                DateTime dataLanc = dtpData.Value.Date;
                DateTime? vencBase = chkSemVencimento.Checked ? null : dtpVencimento.Value.Date;

                for (int i = 0; i < parcelas; i++)
                {
                    string desc = parcelas > 1 ? $"{descBase} ({i + 1}/{parcelas})" : descBase;
                    DateTime? venc = vencBase.HasValue ? vencBase.Value.AddMonths(i) : null;

                    _db.SalvarDespesa(new Despesa
                    {
                        Descricao = desc,
                        Valor = valorParcela,
                        Data = dataLanc,
                        Categoria = categoria,
                        Vencimento = venc
                    });
                }

                txtDescricao.Clear();
                txtValor.Clear();
                dtpData.Value = DateTime.Today;
                dtpVencimento.Value = DateTime.Today.AddDays(30);
                chkSemVencimento.Checked = false;
                nudParcelas.Value = 1;
                cboCategoria.SelectedIndex = 0;

                CarregarDespesas();
                txtDescricao.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar despesa:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDarBaixa_Click(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;

            var row = grid.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["colId"].Value);
            var sit = row.Cells["colSituacao"].Value?.ToString() ?? "Pendente";

            if (sit == "Quitado")
            {
                MessageBox.Show("Esta despesa já está quitada.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var despesa = _despesasDoMes.FirstOrDefault(d => d.Id == id);
            if (despesa == null) return;

            var frm = new FormDarBaixa(despesa);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                _db.DarBaixaDespesa(despesa.Id, frm.DataPagamento, frm.FormaPagamento);
                CarregarDespesas();
            }
        }

        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma despesa para excluir.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = grid.SelectedRows[0];
            string desc = row.Cells["colDescricao"].Value?.ToString() ?? "";
            string val = row.Cells["colValor"].Value?.ToString() ?? "";

            if (MessageBox.Show($"Excluir a despesa:\n\"{desc}\"  —  R$ {val}?",
                    "Confirmar exclusão", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;

            int id = Convert.ToInt32(row.Cells["colId"].Value);
            _db.ExcluirDespesa(id);
            CarregarDespesas();
        }

        private void BtnNovaCategoria_Click(object? sender, EventArgs e)
        {
            string nova = Microsoft.VisualBasic.Interaction.InputBox(
                "Digite o nome da nova categoria:", "Nova Categoria", "").Trim();

            if (string.IsNullOrWhiteSpace(nova)) return;

            if (!_categorias.Contains(nova))
            {
                _categorias.Insert(_categorias.Count - 1, nova);
                CarregarCategorias();
            }
            cboCategoria.SelectedItem = nova;
        }
    }
}