using FeirinhaCodorna.Data;

namespace FeirinhaCodorna.Forms
{
    public partial class FormRelatorio : Form
    {
        public FormRelatorio(BancoDados db)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Relatório";
        }
    }
}