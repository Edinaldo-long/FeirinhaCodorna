using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public partial class FormFornecedores : Form
    {
        private readonly BancoDados _db;
        private DataGridView grid = new();
        private List<Fornecedor> _lista = new();


        public FormFornecedores(BancoDados db)
        {
            _db = db;
            ConstruirTela();
            CarregarDados();
        }

        private void ConstruirTela()
        {
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9.5f);

            // ── Cabeçalho ──────────────────────────────────────────
            var titulo = new Label
            {
                Text = "Fornecedores",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Height = 50,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };

            // ── Barra de ferramentas ───────────────────────────────
            var barra = new Panel
            {
                Height = 46,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(235, 233, 228),
                Padding = new Padding(8, 6, 8, 6)
            };

            var btnNovo = CriarBotao("+ Novo Fornecedor", Color.FromArgb(80, 140, 80));
            btnNovo.Click += (s, e) => AbrirCadastro(null);

            var btnEditar = CriarBotao("✏ Editar", Color.FromArgb(90, 120, 160));
            btnEditar.Left = btnNovo.Right + 8;
            btnEditar.Click += (s, e) => EditarSelecionado();

            var btnExcluir = CriarBotao("🗑 Excluir", Color.FromArgb(180, 80, 80));
            btnExcluir.Left = btnEditar.Right + 8;
            btnExcluir.Click += (s, e) => ExcluirSelecionado();

            barra.Controls.AddRange(new Control[] { btnNovo, btnEditar, btnExcluir });

            // ── Grid ───────────────────────────────────────────────
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(245, 245, 242),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(225, 223, 218);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            grid.CellDoubleClick += (s, e) => EditarSelecionado();

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nome", HeaderText = "Nome", FillWeight = 30 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CnpjCpf", HeaderText = "CNPJ/CPF", FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefone", HeaderText = "Telefone", FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Endereco", HeaderText = "Endereço", FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ativo", HeaderText = "Ativo", FillWeight = 10 });

            Controls.Add(grid);
            Controls.Add(barra);
            Controls.Add(titulo);
        }

        private Button CriarBotao(string texto, Color cor)
        {
            return new Button
            {
                Text = texto,
                Height = 30,
                Width = 150,
                FlatStyle = FlatStyle.Flat,
                BackColor = cor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void CarregarDados()
        {
            _lista = _db.ListarFornecedores();
            grid.Rows.Clear();
            foreach (var f in _lista)
                grid.Rows.Add(f.Nome, f.CnpjCpf, f.Telefone, f.Endereco, f.Ativo ? "Sim" : "Não");
        }

        private void AbrirCadastro(Fornecedor? fornecedor)
        {
            var form = new FormCadastroFornecedor(_db, fornecedor);
            if (form.ShowDialog() == DialogResult.OK)
                CarregarDados();
        }

        private void EditarSelecionado()
        {
            if (grid.SelectedRows.Count == 0) return;
            var idx = grid.SelectedRows[0].Index;
            AbrirCadastro(_lista[idx]);
        }

        private void ExcluirSelecionado()
        {
            if (grid.SelectedRows.Count == 0) return;
            var f = _lista[grid.SelectedRows[0].Index];
            if (MessageBox.Show($"Excluir fornecedor \"{f.Nome}\"?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _db.ExcluirFornecedor(f.Id);
                CarregarDados();
            }
        }
    }
}