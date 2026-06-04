using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FeirinhaCodorna
{
    public partial class FrmSplash : Form
    {
        private ProgressBar pbStatus;
        private Label lblStatus;
        private PictureBox pbLogo;

        public FrmSplash()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 400);
            this.BackColor = Color.White;

            pbStatus = new ProgressBar { Size = new Size(400, 20), Location = new Point(50, 330), Style = ProgressBarStyle.Continuous };
            lblStatus = new Label { Size = new Size(400, 30), Location = new Point(50, 360), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Regular), Text = "Inicializando..." };

            string logoPath = Path.Combine(Application.StartupPath, "Logo_Feirinha_do_codorna.png");

            pbLogo = new PictureBox
            {
                Size = new Size(300, 300),
                Location = new Point(100, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = File.Exists(logoPath) ? Image.FromFile(logoPath) : null
            };

            this.Controls.Add(pbStatus);
            this.Controls.Add(lblStatus);
            this.Controls.Add(pbLogo);
            this.Load += FrmSplash_Load;
        }

        private async void FrmSplash_Load(object sender, EventArgs e)
        {
            pbStatus.Value = 20;
            lblStatus.Text = "Conectando-se à Feirinha...";
            await Task.Delay(1000);
            pbStatus.Value = 60;
            lblStatus.Text = "Verificando estoque...";
            await Task.Delay(1000);
            pbStatus.Value = 100;
            lblStatus.Text = "Bem-vindo!";
            await Task.Delay(500);
            this.Close();
        }
    }
}
