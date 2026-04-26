namespace FeirinhaCodorna.Forms
{
    partial class FormCaixa
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.txtCodCliente = new System.Windows.Forms.TextBox();
            this.btnBuscarCod = new System.Windows.Forms.Button();
            this.btnBuscarNome = new System.Windows.Forms.Button();
            this.btnLimparCli = new System.Windows.Forms.Button();
            this.lblCliente = new System.Windows.Forms.Label();
            this.txtEan = new System.Windows.Forms.TextBox();
            this.txtPeso = new System.Windows.Forms.TextBox();
            this.btnTara = new System.Windows.Forms.Button();
            this.btnAddPesado = new System.Windows.Forms.Button();
            this.lblProdutoAtual = new System.Windows.Forms.Label();
            this.grdItens = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnRemover = new System.Windows.Forms.Button();
            this.btnDinheiro = new System.Windows.Forms.Button();
            this.btnDebito = new System.Windows.Forms.Button();
            this.btnCredito = new System.Windows.Forms.Button();
            this.btnPix = new System.Windows.Forms.Button();
            this.btnFiado = new System.Windows.Forms.Button();
            this.btnMisto = new System.Windows.Forms.Button();
            this.btnEstornar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.grdItens)).BeginInit();
            this.SuspendLayout();

            var fonteNormal = new System.Drawing.Font("Segoe UI", 13F);
            var fonteBotao = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            var fonteTotal = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            var fonteProduto = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);

            // ── Layout principal ──────────────────────────────────────
            var layoutPrincipal = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new System.Windows.Forms.Padding(8)
            };
            layoutPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            layoutPrincipal.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // ── Painel do topo ────────────────────────────────────────
            var painelTopo = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new System.Windows.Forms.Padding(0, 0, 0, 4)
            };

            // linha cliente
            var linhaCliente = new System.Windows.Forms.FlowLayoutPanel
            {
                FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 4)
            };

            this.txtCodCliente.Font = fonteNormal;
            this.txtCodCliente.Size = new System.Drawing.Size(160, 36);
            this.txtCodCliente.PlaceholderText = "Cód. cliente";
            this.txtCodCliente.Name = "txtCodCliente";
            this.txtCodCliente.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

            this.btnBuscarCod.Font = fonteBotao;
            this.btnBuscarCod.Text = "Buscar";
            this.btnBuscarCod.Size = new System.Drawing.Size(110, 38);
            this.btnBuscarCod.Name = "btnBuscarCod";
            this.btnBuscarCod.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

            this.btnBuscarNome.Font = fonteBotao;
            this.btnBuscarNome.Text = "Por nome";
            this.btnBuscarNome.Size = new System.Drawing.Size(130, 38);
            this.btnBuscarNome.Name = "btnBuscarNome";
            this.btnBuscarNome.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

            this.btnLimparCli.Font = fonteBotao;
            this.btnLimparCli.Text = "Limpar";
            this.btnLimparCli.Size = new System.Drawing.Size(110, 38);
            this.btnLimparCli.Name = "btnLimparCli";

            linhaCliente.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtCodCliente, this.btnBuscarCod, this.btnBuscarNome, this.btnLimparCli
            });

            this.lblCliente.Font = fonteNormal;
            this.lblCliente.AutoSize = true;
            this.lblCliente.Text = "Venda à vista — sem cliente selecionado";
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Margin = new System.Windows.Forms.Padding(0, 2, 0, 4);

            // linha EAN
            var linhaEan = new System.Windows.Forms.FlowLayoutPanel
            {
                FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 4)
            };

            this.txtEan.Font = fonteNormal;
            this.txtEan.Size = new System.Drawing.Size(260, 36);
            this.txtEan.PlaceholderText = "EAN / Código";
            this.txtEan.Name = "txtEan";
            this.txtEan.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

            this.btnTara.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTara.Text = "Tara";
            this.btnTara.Size = new System.Drawing.Size(72, 38);
            this.btnTara.Name = "btnTara";
            this.btnTara.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnTara.BackColor = System.Drawing.Color.FromArgb(200, 200, 195);
            this.btnTara.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnTara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTara.FlatAppearance.BorderSize = 0;
            this.btnTara.Cursor = System.Windows.Forms.Cursors.Hand;

            this.txtPeso.Font = fonteNormal;
            this.txtPeso.Size = new System.Drawing.Size(150, 36);
            this.txtPeso.PlaceholderText = "Peso (kg)";
            this.txtPeso.Name = "txtPeso";
            this.txtPeso.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);

            this.btnAddPesado.Font = fonteBotao;
            this.btnAddPesado.Text = "Adicionar";
            this.btnAddPesado.Size = new System.Drawing.Size(130, 38);
            this.btnAddPesado.Name = "btnAddPesado";

            linhaEan.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.txtEan, this.btnTara, this.txtPeso, this.btnAddPesado
            });

            this.lblProdutoAtual.Font = fonteProduto;
            this.lblProdutoAtual.ForeColor = System.Drawing.Color.FromArgb(15, 110, 86);
            this.lblProdutoAtual.AutoSize = true;
            this.lblProdutoAtual.Text = "";
            this.lblProdutoAtual.Name = "lblProdutoAtual";
            this.lblProdutoAtual.Margin = new System.Windows.Forms.Padding(0, 2, 0, 4);

            painelTopo.Controls.Add(linhaCliente);
            painelTopo.Controls.Add(this.lblCliente);
            painelTopo.Controls.Add(linhaEan);
            painelTopo.Controls.Add(this.lblProdutoAtual);

            // ── Painel inferior ───────────────────────────────────────
            var painelInferior = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            painelInferior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            painelInferior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            painelInferior.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));

            // grade
            this.grdItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdItens.AllowUserToAddRows = false;
            this.grdItens.ReadOnly = true;
            this.grdItens.Name = "grdItens";
            this.grdItens.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdItens.ColumnHeadersDefaultCellStyle.Font = fonteBotao;
            this.grdItens.DefaultCellStyle.Font = fonteNormal;
            this.grdItens.RowTemplate.Height = 42;
            this.grdItens.Columns.Add("Produto", "Produto");
            this.grdItens.Columns.Add("Qtd", "Qtd");
            this.grdItens.Columns.Add("Preco", "Preço Unit.");
            this.grdItens.Columns.Add("Subtotal", "Subtotal");

            // linha total + remover
            var linhaTotalRemover = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            linhaTotalRemover.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            linhaTotalRemover.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));

            this.lblTotal.Font = fonteTotal;
            this.lblTotal.Text = "Total: R$ 0,00";
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.btnRemover.Font = fonteBotao;
            this.btnRemover.Text = "Remover item";
            this.btnRemover.Size = new System.Drawing.Size(160, 44);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;

            linhaTotalRemover.Controls.Add(this.lblTotal, 0, 0);
            linhaTotalRemover.Controls.Add(this.btnRemover, 1, 0);

            // ── linha pagamentos (9 colunas) ──────────────────────────
            var linhaPagamentos = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 9,
                RowCount = 1
            };
            // colunas 0-6: AutoSize  |  coluna 7: espaçador  |  coluna 8: AutoSize
            for (int i = 0; i < 7; i++)
                linhaPagamentos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            linhaPagamentos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            linhaPagamentos.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));

            // botões de forma única — colunas 0-4
            System.Windows.Forms.Button[] btnsPag = { this.btnDinheiro, this.btnDebito, this.btnCredito, this.btnPix, this.btnFiado };
            string[] textosPag = { "Dinheiro", "Débito", "Crédito", "Pix", "Caderneta" };
            for (int i = 0; i < btnsPag.Length; i++)
            {
                btnsPag[i].Font = fonteBotao;
                btnsPag[i].Text = textosPag[i];
                btnsPag[i].Size = new System.Drawing.Size(130, 50);
                btnsPag[i].Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
                linhaPagamentos.Controls.Add(btnsPag[i], i, 0);
            }

            // Misto — coluna 5
            this.btnMisto.Font = fonteBotao;
            this.btnMisto.Text = "Misto";
            this.btnMisto.Size = new System.Drawing.Size(110, 50);
            this.btnMisto.Name = "btnMisto";
            this.btnMisto.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.btnMisto.BackColor = System.Drawing.Color.FromArgb(255, 165, 0);
            this.btnMisto.ForeColor = System.Drawing.Color.White;
            this.btnMisto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMisto.FlatAppearance.BorderSize = 0;
            linhaPagamentos.Controls.Add(this.btnMisto, 5, 0);

            // Estornar — coluna 6
            this.btnEstornar.Font = fonteBotao;
            this.btnEstornar.Text = "Estornar";
            this.btnEstornar.Size = new System.Drawing.Size(120, 50);
            this.btnEstornar.Name = "btnEstornar";
            this.btnEstornar.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.btnEstornar.BackColor = System.Drawing.Color.FromArgb(180, 30, 30);
            this.btnEstornar.ForeColor = System.Drawing.Color.White;
            this.btnEstornar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstornar.FlatAppearance.BorderSize = 0;
            linhaPagamentos.Controls.Add(this.btnEstornar, 6, 0);

            // Cancelar venda — coluna 8
            this.btnCancelar.Font = fonteBotao;
            this.btnCancelar.Text = "Cancelar venda";
            this.btnCancelar.Size = new System.Drawing.Size(170, 50);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            linhaPagamentos.Controls.Add(this.btnCancelar, 8, 0);

            painelInferior.Controls.Add(this.grdItens, 0, 0);
            painelInferior.Controls.Add(linhaTotalRemover, 0, 1);
            painelInferior.Controls.Add(linhaPagamentos, 0, 2);

            layoutPrincipal.Controls.Add(painelTopo, 0, 0);
            layoutPrincipal.Controls.Add(painelInferior, 0, 1);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 661);
            this.Name = "FormCaixa";
            this.Text = "Frente de Caixa";
            this.Controls.Add(layoutPrincipal);

            ((System.ComponentModel.ISupportInitialize)(this.grdItens)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TextBox txtCodCliente = null!;
        private System.Windows.Forms.Button btnBuscarCod = null!;
        private System.Windows.Forms.Button btnBuscarNome = null!;
        private System.Windows.Forms.Button btnLimparCli = null!;
        private System.Windows.Forms.Label lblCliente = null!;
        private System.Windows.Forms.TextBox txtEan = null!;
        private System.Windows.Forms.TextBox txtPeso = null!;
        private System.Windows.Forms.Button btnTara = null!;
        private System.Windows.Forms.Button btnAddPesado = null!;
        private System.Windows.Forms.Label lblProdutoAtual = null!;
        private System.Windows.Forms.DataGridView grdItens = null!;
        private System.Windows.Forms.Label lblTotal = null!;
        private System.Windows.Forms.Button btnRemover = null!;
        private System.Windows.Forms.Button btnDinheiro = null!;
        private System.Windows.Forms.Button btnDebito = null!;
        private System.Windows.Forms.Button btnCredito = null!;
        private System.Windows.Forms.Button btnPix = null!;
        private System.Windows.Forms.Button btnFiado = null!;
        private System.Windows.Forms.Button btnMisto = null!;
        private System.Windows.Forms.Button btnEstornar = null!;
        private System.Windows.Forms.Button btnCancelar = null!;
    }
}