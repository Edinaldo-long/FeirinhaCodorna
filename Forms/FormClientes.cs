using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormClientes : Form
    {
        private readonly BancoDados _db;
        private DataGridView grid = new();
        private TextBox txtBusca = new();

        public FormClientes(BancoDados db)
        {
            _db = db;
            Text = "Clientes";
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 245, 242);

            var pnlTopo = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };

            txtBusca = new TextBox
            {
                PlaceholderText = "Buscar cliente...",
                Font = new Font("Segoe UI", 10f),
                Width = 300,
                Location = new Point(8, 12)
            };
            txtBusca.TextChanged += (s, e) => Carregar(txtBusca.Text);

            var btnNovo = new Button
            {
                Text = "NOVO CLIENTE",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 34,
                Width = 140,
                Location = new Point(320, 8)
            };
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Click += BtnNovo_Click;

            pnlTopo.Controls.AddRange(new Control[] { txtBusca, btnNovo });

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f)
            };
            grid.DoubleClick += Grid_DoubleClick;

            Controls.AddRange(new Control[] { grid, pnlTopo });
            Carregar("");
        }

        private void Carregar(string filtro)
        {
            var lista = _db.ListarClientes(filtro);
            grid.DataSource = lista;
        }

        private void BtnNovo_Click(object? sender, EventArgs e)
        {
            using var form = new FormCadastroCliente(null);
            if (form.ShowDialog() == DialogResult.OK && form.ClienteEditado != null)
            {
                _db.SalvarCliente(form.ClienteEditado);
                Carregar(txtBusca.Text);
            }
        }

        private void Grid_DoubleClick(object? sender, EventArgs e)
        {
            if (grid.CurrentRow?.DataBoundItem is not Cliente c) return;
            using var form = new FormCadastroCliente(c);
            if (form.ShowDialog() == DialogResult.OK && form.ClienteEditado != null)
            {
                _db.SalvarCliente(form.ClienteEditado);
                Carregar(txtBusca.Text);
            }
        }
    }
}