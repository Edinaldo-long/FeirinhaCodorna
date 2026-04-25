using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public partial class FormBuscaCliente : Form
    {
        private readonly BancoDados _db;
        public Cliente? ClienteSelecionado { get; private set; }

        public FormBuscaCliente(BancoDados db)
        {
            _db = db;
            InitializeComponent();
            CarregarClientes("");
        }

        private void CarregarClientes(string filtro)
        {
            var clientes = _db.ListarClientes(filtro);
            lstClientes.DataSource = clientes;
            lstClientes.DisplayMember = "Nome";
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            CarregarClientes(txtBusca.Text.Trim());
        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (lstClientes.SelectedItem is Cliente cliente)
            {
                ClienteSelecionado = cliente;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Selecione um cliente.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lstClientes_DoubleClick(object sender, EventArgs e)
        {
            btnSelecionar_Click(sender, e);
        }
    }
}