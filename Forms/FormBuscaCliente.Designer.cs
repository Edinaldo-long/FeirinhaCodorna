namespace FeirinhaCodorna.Forms
{
    partial class FormBuscaCliente
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtBusca = new System.Windows.Forms.TextBox();
            this.lstClientes = new System.Windows.Forms.ListBox();
            this.btnSelecionar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.lblInfo.Location = new System.Drawing.Point(12, 10);
            this.lblInfo.Size = new System.Drawing.Size(360, 20);
            this.lblInfo.Text = "Digite o nome, código ou CPF do cliente:";

            this.txtBusca.Location = new System.Drawing.Point(12, 33);
            this.txtBusca.Size = new System.Drawing.Size(360, 23);
            this.txtBusca.PlaceholderText = "Buscar cliente...";
            this.txtBusca.TextChanged += new System.EventHandler(this.txtBusca_TextChanged);

            this.lstClientes.Location = new System.Drawing.Point(12, 65);
            this.lstClientes.Size = new System.Drawing.Size(360, 200);
            this.lstClientes.DoubleClick += new System.EventHandler(this.lstClientes_DoubleClick);

            this.btnSelecionar.Location = new System.Drawing.Point(12, 278);
            this.btnSelecionar.Size = new System.Drawing.Size(170, 35);
            this.btnSelecionar.Text = "✔ Selecionar";
            this.btnSelecionar.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnSelecionar.ForeColor = System.Drawing.Color.White;
            this.btnSelecionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelecionar.Click += new System.EventHandler(this.btnSelecionar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(202, 278);
            this.btnCancelar.Size = new System.Drawing.Size(170, 35);
            this.btnCancelar.Text = "✖ Cancelar";
            this.btnCancelar.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            this.ClientSize = new System.Drawing.Size(384, 325);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.txtBusca);
            this.Controls.Add(this.lstClientes);
            this.Controls.Add(this.btnSelecionar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buscar Cliente";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtBusca;
        private System.Windows.Forms.ListBox lstClientes;
        private System.Windows.Forms.Button btnSelecionar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblInfo;
    }
}