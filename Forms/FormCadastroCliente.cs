using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormCadastroCliente : Form
    {
        public Cliente? ClienteEditado { get; private set; }

        private readonly Cliente? _clienteOriginal;

        private TextBox txtCodigo, txtNome, txtCpf, txtRg, txtEndereco,
                txtNumero, txtBairro, txtComplemento, txtTelefone, txtCelular, txtWhatsApp,
                txtAutorizado, txtLimite, txtSaldo;

        // Layout fixo: cada linha tem altura 30px, espaçamento 8px entre linhas
        private const int LabelX = 16;
        private const int LabelW = 160;
        private const int TxtX = 182;
        private const int TxtW = 320;
        private const int RowH = 30;
        private const int RowGap = 8;
        private const int StartY = 16;

        public FormCadastroCliente(Cliente? cliente)
        {
            _clienteOriginal = cliente;
            Text = cliente == null ? "Novo Cliente" : "Editar Cliente";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            AutoScroll = false;

            // 14 campos × (30 + 8) = 532 + 16 top + 16 bottom + 58 botões + 40 titlebar = ~662
            ClientSize = new Size(520, 660);

            txtCodigo = Campo(0, "Código:", maiusculo: false);
            txtNome = Campo(1, "Nome *:", maiusculo: true);
            txtCpf = Campo(2, "CPF:", maiusculo: false);
            txtRg = Campo(3, "RG:", maiusculo: false);
            txtEndereco = Campo(4, "Endereço:", maiusculo: true);
            txtNumero = Campo(5, "Número:", maiusculo: false);
            txtBairro = Campo(6, "Bairro:", maiusculo: true);
            txtComplemento = Campo(7, "Complemento:", maiusculo: true);
            txtTelefone = Campo(8, "Telefone:", maiusculo: false);
            txtCelular = Campo(9, "Celular:", maiusculo: false);
            txtWhatsApp = Campo(10, "WhatsApp:", maiusculo: false);
            txtAutorizado = Campo(11, "Autorizado caderneta:", maiusculo: true);
            txtLimite = Campo(12, "Limite caderneta R$:", maiusculo: false);
            txtSaldo = Campo(13, "Saldo devedor R$:", maiusculo: false);

            // Botões fixos no rodapé
            int btnY = ClientSize.Height - 50;

            var btnSalvar = new Button
            {
                Text = "Salvar",
                Size = new Size(110, 34),
                Location = new Point(ClientSize.Width - 126, btnY),
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnSalvar.FlatAppearance.BorderSize = 0;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(110, 34),
                Location = new Point(ClientSize.Width - 244, btnY),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 9f)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            btnSalvar.Click += (s, e) => Salvar();

            Controls.AddRange(new Control[] { btnSalvar, btnCancelar });

            // Preencher dados se editando
            if (cliente != null)
            {
                txtCodigo.Text = cliente.Codigo;
                txtNome.Text = cliente.Nome?.ToUpper();
                txtCpf.Text = cliente.Cpf;
                txtRg.Text = cliente.Rg;
                txtEndereco.Text = cliente.Endereco?.ToUpper();
                txtNumero.Text = cliente.Numero;
                txtBairro.Text = cliente.Bairro?.ToUpper();
                txtComplemento.Text = cliente.Complemento?.ToUpper();
                txtTelefone.Text = cliente.Telefone;
                txtCelular.Text = cliente.Celular;
                txtWhatsApp.Text = cliente.WhatsApp;
                txtAutorizado.Text = cliente.AutorizadoCaderneta?.ToUpper();
                txtLimite.Text = cliente.LimiteFiado.ToString("F2");
                txtSaldo.Text = cliente.SaldoFiado.ToString("F2");

                AtualizarCorSaldo(cliente.SaldoFiado);
            }
            else
            {
                txtLimite.Text = "100,00";
                txtSaldo.Text = "0,00";
                AtualizarCorSaldo(0);
            }

            txtSaldo.ReadOnly = true;
        }

        private TextBox Campo(int index, string rotulo, bool maiusculo)
        {
            int y = StartY + index * (RowH + RowGap);

            var lbl = new Label
            {
                Text = rotulo,
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(LabelX, y),
                Size = new Size(LabelW, RowH),
                Font = new Font("Segoe UI", 9f)
            };

            var txt = new TextBox
            {
                Location = new Point(TxtX, y + 2),
                Size = new Size(TxtW, RowH),
                Font = new Font("Segoe UI", 9.5f),
                CharacterCasing = maiusculo ? CharacterCasing.Upper : CharacterCasing.Normal
            };

            Controls.Add(lbl);
            Controls.Add(txt);
            return txt;
        }

        private void AtualizarCorSaldo(decimal saldo)
        {
            if (saldo > 0)
            {
                txtSaldo.BackColor = Color.FromArgb(255, 210, 210);
                txtSaldo.ForeColor = Color.FromArgb(160, 0, 0);
                txtSaldo.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
            else
            {
                txtSaldo.BackColor = Color.FromArgb(235, 235, 232);
                txtSaldo.ForeColor = Color.Black;
                txtSaldo.Font = new Font("Segoe UI", 9.5f);
            }
        }

        private void Salvar()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome do cliente é obrigatório.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }
            if (!decimal.TryParse(txtLimite.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal limite))
            {
                MessageBox.Show("Limite inválido.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }
            decimal.TryParse(txtSaldo.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal saldo);

            ClienteEditado = new Cliente
            {
                Id = _clienteOriginal?.Id ?? 0,
                Codigo = txtCodigo.Text.Trim(),
                Nome = txtNome.Text.Trim(),
                Cpf = txtCpf.Text.Trim(),
                Rg = txtRg.Text.Trim(),
                Endereco = txtEndereco.Text.Trim(),
                Numero = txtNumero.Text.Trim(),
                Bairro = txtBairro.Text.Trim(),
                Complemento = txtComplemento.Text.Trim(),
                Telefone = txtTelefone.Text.Trim(),
                Celular = txtCelular.Text.Trim(),
                WhatsApp = txtWhatsApp.Text.Trim(),
                AutorizadoCaderneta = txtAutorizado.Text.Trim(),
                LimiteFiado = limite,
                SaldoFiado = saldo
            };
        }
    }
}