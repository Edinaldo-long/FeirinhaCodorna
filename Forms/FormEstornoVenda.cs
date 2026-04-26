using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FormEstornoVenda : Form
    {
        private readonly BancoDados _db;

        private TextBox txtBusca = null!;
        private DateTimePicker dtpDe = null!;
        private DateTimePicker dtpAte = null!;
        private Button btnBuscar = null!;
        private DataGridView grdVendas = null!;
        private DataGridView grdItens = null!;
        private Label lblResumo = null!;
        private Button btnEstornar = null!;
        private Button btnFechar = null!;

        private List<VendaResumo> _vendas = new();
        private VendaResumo? _vendaSelecionada;

        public FormEstornoVenda(BancoDados db)
        {
            _db = db;
            ConstruirInterface();
            CarregarVendas();
        }

        private void ConstruirInterface()
        {
            Text = "Estorno / Devolucao de Venda";
            Size = new Size(1100, 700);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 245, 242);
            Font = new Font("Segoe UI", 9.5f);

            var fonteNormal = new Font("Segoe UI", 11F);
            var fonteBotao = new Font("Segoe UI", 11F, FontStyle.Bold);
            var fonteTitulo = new Font("Segoe UI", 13F, FontStyle.Bold);

            var lblTitulo = new Label
            {
                Text = "Estorno / Devolucao de Venda",
                Font = fonteTitulo,
                ForeColor = Color.FromArgb(180, 30, 30),
                AutoSize = true,
                Location = new Point(12, 12)
            };

            var lblBusca = new Label
            {
                Text = "Cliente / No venda:",
                AutoSize = true,
                Font = fonteNormal,
                Location = new Point(12, 52)
            };

            txtBusca = new TextBox
            {
                Font = fonteNormal,
                Location = new Point(12, 72),
                Width = 260,
                PlaceholderText = "Nome do cliente ou numero..."
            };

            var lblDe = new Label
            {
                Text = "De:",
                AutoSize = true,
                Font = fonteNormal,
                Location = new Point(286, 52)
            };

            dtpDe = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = fonteNormal,
                Location = new Point(286, 72),
                Width = 130,
                Value = DateTime.Today.AddDays(-30)
            };

            var lblAte = new Label
            {
                Text = "Ate:",
                AutoSize = true,
                Font = fonteNormal,
                Location = new Point(428, 52)
            };

            dtpAte = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = fonteNormal,
                Location = new Point(428, 72),
                Width = 130,
                Value = DateTime.Today
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Font = fonteBotao,
                Location = new Point(572, 68),
                Size = new Size(130, 36),
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += (s, e) => CarregarVendas();
            txtBusca.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CarregarVendas(); };

            var lblVendas = new Label
            {
                Text = "Vendas encontradas:",
                AutoSize = true,
                Font = fonteNormal,
                Location = new Point(12, 120)
            };

            grdVendas = new DataGridView
            {
                Location = new Point(12, 140),
                Size = new Size(1060, 220),
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = { Font = new Font("Segoe UI", 10F, FontStyle.Bold) },
                DefaultCellStyle = { Font = fonteNormal },
                RowTemplate = { Height = 38 },
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            grdVendas.Columns.Add("Col_Id", "No Venda");
            grdVendas.Columns.Add("Col_Data", "Data/Hora");
            grdVendas.Columns.Add("Col_Cliente", "Cliente");
            grdVendas.Columns.Add("Col_Forma", "Pagamento");
            grdVendas.Columns.Add("Col_Total", "Total");
            grdVendas.Columns["Col_Id"].FillWeight = 60;
            grdVendas.Columns["Col_Data"].FillWeight = 120;
            grdVendas.Columns["Col_Total"].FillWeight = 80;
            grdVendas.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grdVendas.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 245, 242);
            };
            grdVendas.SelectionChanged += GrdVendas_SelectionChanged;

            lblResumo = new Label
            {
                Text = "Selecione uma venda acima para ver os itens",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(12, 372)
            };

            var lblItens = new Label
            {
                Text = "Itens da venda selecionada:",
                AutoSize = true,
                Font = fonteNormal,
                Location = new Point(12, 392)
            };

            grdItens = new DataGridView
            {
                Location = new Point(12, 412),
                Size = new Size(1060, 160),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = { Font = new Font("Segoe UI", 10F, FontStyle.Bold) },
                DefaultCellStyle = { Font = fonteNormal },
                RowTemplate = { Height = 36 },
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            grdItens.Columns.Add("Col_Produto", "Produto");
            grdItens.Columns.Add("Col_Qtd", "Qtd");
            grdItens.Columns.Add("Col_Preco", "Preco Unit.");
            grdItens.Columns.Add("Col_Subtotal", "Subtotal");
            grdItens.Columns["Col_Qtd"].FillWeight = 80;
            grdItens.Columns["Col_Preco"].FillWeight = 100;
            grdItens.Columns["Col_Subtotal"].FillWeight = 100;

            btnEstornar = new Button
            {
                Text = "Estornar venda selecionada",
                Font = fonteBotao,
                Size = new Size(280, 44),
                Location = new Point(12, 588),
                BackColor = Color.FromArgb(180, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnEstornar.FlatAppearance.BorderSize = 0;
            btnEstornar.Click += BtnEstornar_Click;

            btnFechar = new Button
            {
                Text = "Fechar",
                Font = fonteBotao,
                Size = new Size(120, 44),
                Location = new Point(952, 588),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFechar.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblTitulo,
                lblBusca, txtBusca,
                lblDe, dtpDe,
                lblAte, dtpAte,
                btnBuscar,
                lblVendas, grdVendas,
                lblResumo,
                lblItens, grdItens,
                btnEstornar, btnFechar
            });
        }

        private void CarregarVendas()
        {
            _vendas = _db.BuscarVendasParaEstorno(
                txtBusca.Text.Trim(),
                dtpDe.Value.Date,
                dtpAte.Value.Date);

            grdVendas.Rows.Clear();
            foreach (var v in _vendas)
            {
                string forma = v.FormaPagamento switch
                {
                    "Dinheiro" => "Dinheiro",
                    "CartaoDebito" => "Debito",
                    "CartaoCredito" => "Credito",
                    "Pix" => "Pix",
                    "Fiado" => "Caderneta",
                    _ => v.FormaPagamento
                };
                grdVendas.Rows.Add(
                    $"#{v.Id}",
                    v.DataHora.ToString("dd/MM/yyyy HH:mm"),
                    string.IsNullOrEmpty(v.ClienteNome) ? "Avulso" : v.ClienteNome,
                    forma,
                    $"R$ {v.Total:F2}");
            }

            grdItens.Rows.Clear();
            lblResumo.Text = $"{_vendas.Count} venda(s) encontrada(s). Selecione uma para ver os itens.";
            lblResumo.ForeColor = Color.FromArgb(60, 60, 60);
            btnEstornar.Enabled = false;
            _vendaSelecionada = null;
        }

        private void GrdVendas_SelectionChanged(object? sender, EventArgs e)
        {
            grdItens.Rows.Clear();
            btnEstornar.Enabled = false;
            _vendaSelecionada = null;

            if (grdVendas.CurrentRow == null || grdVendas.CurrentRow.Index < 0) return;
            int idx = grdVendas.CurrentRow.Index;
            if (idx >= _vendas.Count) return;

            var resumo = _vendas[idx];
            var venda = _db.BuscarVendaComItens(resumo.Id);
            if (venda == null) return;

            _vendaSelecionada = resumo;

            foreach (var item in venda.Itens)
            {
                string qtdTexto = item.Quantidade % 1 == 0
                    ? $"{item.Quantidade:F0}"
                    : $"{item.Quantidade:F3}";
                grdItens.Rows.Add(
                    item.ProdutoNome,
                    qtdTexto,
                    $"R$ {item.PrecoUnitario:F2}",
                    $"R$ {item.Subtotal:F2}");
            }

            lblResumo.Text =
                $"Venda #{resumo.Id}  |  " +
                $"{resumo.DataHora:dd/MM/yyyy HH:mm}  |  " +
                $"Cliente: {(string.IsNullOrEmpty(resumo.ClienteNome) ? "Avulso" : resumo.ClienteNome)}  |  " +
                $"Total: R$ {resumo.Total:F2}";
            lblResumo.ForeColor = Color.FromArgb(15, 110, 86);
            btnEstornar.Enabled = true;
        }

        private void BtnEstornar_Click(object? sender, EventArgs e)
        {
            if (_vendaSelecionada == null) return;

            using var dlg = new Form
            {
                Text = "Motivo da Devolucao",
                Size = new Size(460, 280),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };

            var lblMot = new Label
            {
                Text = "Selecione ou descreva o motivo:",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(14, 14)
            };

            string[] opcoes =
            {
                "Produto vencido / estragado",
                "Compra equivocada (item errado)",
                "Produto ja tinha em casa",
                "Necessidade financeira / credito",
                "Problema no pagamento (cartao/Pix)",
                "Outro"
            };

            var cmbMotivo = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(14, 40),
                Width = 410,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMotivo.Items.AddRange(opcoes);
            cmbMotivo.SelectedIndex = 0;

            var lblObs = new Label
            {
                Text = "Observacao adicional (opcional):",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(14, 82)
            };

            var txtObs = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(14, 102),
                Width = 410,
                PlaceholderText = "Detalhes adicionais..."
            };

            var btnConfirmar = new Button
            {
                Text = "Confirmar Estorno",
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 40),
                Location = new Point(14, 150)
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 11F),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(224, 150)
            };

            dlg.Controls.AddRange(new Control[]
                { lblMot, cmbMotivo, lblObs, txtObs, btnConfirmar, btnCancelar });
            dlg.AcceptButton = btnConfirmar;
            dlg.CancelButton = btnCancelar;

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            string motivo = cmbMotivo.SelectedItem?.ToString() ?? "Sem motivo";
            if (!string.IsNullOrWhiteSpace(txtObs.Text))
                motivo += $" - {txtObs.Text.Trim()}";

            var venda = _db.BuscarVendaComItens(_vendaSelecionada.Id);
            if (venda == null) return;

            var confirm = MessageBox.Show(
                $"Confirma o estorno da venda #{_vendaSelecionada.Id}?\n\n" +
                $"Data: {_vendaSelecionada.DataHora:dd/MM/yyyy HH:mm}\n" +
                $"Cliente: {(string.IsNullOrEmpty(_vendaSelecionada.ClienteNome) ? "Avulso" : _vendaSelecionada.ClienteNome)}\n" +
                $"Total: R$ {_vendaSelecionada.Total:F2}\n\n" +
                $"Motivo: {motivo}\n\n" +
                $"O estoque sera devolvido automaticamente.",
                "Confirmar Estorno",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _db.EstornarVenda(_vendaSelecionada.Id, motivo);

            MessageBox.Show(
                $"Estorno realizado com sucesso!\n\n" +
                $"Venda #{_vendaSelecionada.Id} estornada.\n" +
                $"Estoque devolvido.\n" +
                (venda.FormaPagamento == FormaPagamento.Fiado
                    ? "Saldo da caderneta revertido.\n" : "") +
                $"\nMotivo: {motivo}",
                "Estorno Concluido",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            CarregarVendas();
        }
    }

    public class VendaResumo
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string ClienteNome { get; set; } = "";
        public string FormaPagamento { get; set; } = "";
        public decimal Total { get; set; }
    }
}