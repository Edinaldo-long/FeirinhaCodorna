using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;
using System.IO.Ports;

namespace FeirinhaCodorna.Forms
{
    public partial class FormCaixa : Form
    {
        private readonly BancoDados _db;
        private Cliente? _clienteSelecionado;
        private readonly List<ItemVenda> _itens = new();
        private Produto? _produtoAtual;
        private SerialPort? _balanca;
        private decimal _tara = 0m;

        public FormCaixa(BancoDados db)
        {
            InitializeComponent();
            _db = db;
            InicializarEventos();
            IniciarBalanca();
        }

        // ─────────────────────────────────────────
        // BALANÇA
        // ─────────────────────────────────────────

        private void IniciarBalanca()
        {
            try
            {
                _balanca = new SerialPort("COM5", 115200);
                _balanca.NewLine = "\n";
                _balanca.DataReceived += Balanca_DataReceived;
                _balanca.Open();
            }
            catch
            {
                // Balança não conectada — programa funciona normalmente sem ela
            }
        }

        private void Balanca_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string linha = _balanca!.ReadLine().Trim();

                if (float.TryParse(linha,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out float peso))
                {
                    decimal pesoLiquido = Math.Max(0, (decimal)peso - _tara);
                    txtPeso.Invoke(() =>
                    {
                        txtPeso.Text = pesoLiquido.ToString("0.000",
                            System.Globalization.CultureInfo.InvariantCulture);
                    });
                }
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_balanca != null && _balanca.IsOpen)
                _balanca.Close();
        }

        // ─────────────────────────────────────────
        // INICIALIZAÇÃO
        // ─────────────────────────────────────────

        private void InicializarEventos()
        {
            txtCodCliente.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { BuscarPorCodigo(); e.SuppressKeyPress = true; }
                if (e.KeyCode == Keys.Tab) { e.SuppressKeyPress = true; txtEan.Focus(); }
            };
            btnBuscarCod.Click += (s, e) => BuscarPorCodigo();
            btnBuscarNome.Click += (s, e) => AbrirBusca();
            btnLimparCli.Click += (s, e) => LimparCliente();

            txtEan.KeyDown += TxtEan_KeyDown;
            txtPeso.KeyDown += TxtPeso_KeyDown;

            btnTara.Click += (s, e) => CapturarTara();
            btnTara.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    CapturarTara();
                    txtPeso.Focus();
                }
                else if (e.KeyCode == Keys.Tab)
                {
                    e.SuppressKeyPress = true;
                    txtPeso.Focus();
                }
            };

            btnAddPesado.Click += (s, e) => ConfirmarPesado();
            btnRemover.Click += (s, e) => RemoverItem();

            // ── pagamentos ────────────────────────────────────────────
            btnDinheiro.Click += (s, e) => FinalizarVenda(FormaPagamento.Dinheiro);
            btnDebito.Click += (s, e) => FinalizarVenda(FormaPagamento.CartaoDebito);
            btnCredito.Click += (s, e) => FinalizarVenda(FormaPagamento.CartaoCredito);
            btnPix.Click += (s, e) => FinalizarVenda(FormaPagamento.Pix);
            btnFiado.Click += (s, e) => FinalizarVenda(FormaPagamento.Fiado);
            btnMisto.Click += (s, e) => AbrirPagamentoMisto();
            btnEstornar.Click += (s, e) => EstornarUltimaVenda(); // << ESTORNO
            btnCancelar.Click += (s, e) => LimparCaixa();

            grdItens.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grdItens.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 245, 242);
            };

            lblCliente.Text = "Venda à vista — sem cliente selecionado";
            lblCliente.ForeColor = Color.Gray;
            lblTotal.Text = "Total: R$ 0,00";

            VisibleChanged += (s, e) => { if (Visible) txtEan.Focus(); };
        }

        // ── EAN ───────────────────────────────────────────────────────
        private void TxtEan_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BuscarPorEan();
            }
        }

        private void BuscarPorEan()
        {
            var codigo = txtEan.Text.Trim();
            if (string.IsNullOrEmpty(codigo)) return;

            var p = _db.BuscarPorEan(codigo) ?? _db.BuscarPorCodigo(codigo);

            if (p == null)
            {
                MessageBox.Show($"Produto não encontrado: {codigo}", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEan.SelectAll();
                return;
            }

            if (p.Pesavel)
            {
                _produtoAtual = p;
                lblProdutoAtual.Text = $"▶ {p.Nome}  —  R$ {p.Preco:F2}/kg";
                lblProdutoAtual.ForeColor = Color.FromArgb(15, 110, 86);
                btnTara.Focus();
            }
            else
            {
                _produtoAtual = null;
                lblProdutoAtual.Text = "";
                decimal qtd = PerguntarQuantidade(p.Nome, p.Preco);
                if (qtd <= 0) { txtEan.SelectAll(); return; }
                AdicionarItem(p, qtd);
                txtEan.Clear();
                txtEan.Focus();
            }
        }

        // ── Peso ──────────────────────────────────────────────────────
        private void TxtPeso_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmarPesado();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                CancelarPesado();
            }
        }

        private void ConfirmarPesado()
        {
            if (_produtoAtual == null) return;

            if (!decimal.TryParse(txtPeso.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal peso) || peso <= 0)
            {
                MessageBox.Show("Informe o peso corretamente.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPeso.SelectAll();
                txtPeso.Focus();
                return;
            }

            AdicionarItem(_produtoAtual, peso);
            CancelarPesado();
        }

        private void CancelarPesado()
        {
            _produtoAtual = null;
            lblProdutoAtual.Text = "";
            _tara = 0m;
            btnTara.Text = "Tara";
            btnTara.BackColor = System.Drawing.Color.FromArgb(200, 200, 195);
            txtEan.Clear();
            txtPeso.Clear();
            txtEan.Focus();
        }

        // ── Tara ──────────────────────────────────────────────────────
        private void CapturarTara()
        {
            if (!decimal.TryParse(txtPeso.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal pesoAtual) || pesoAtual <= 0)
            {
                MessageBox.Show("Coloque o prato/recipiente na balança antes de tarar.",
                    "Tara", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _tara = pesoAtual;
            btnTara.Text = $"Tara\n{_tara:F3}kg";
            btnTara.BackColor = System.Drawing.Color.FromArgb(255, 193, 7);
            txtPeso.Text = "0,000";
            txtPeso.Focus();
        }

        // ── Janela de quantidade ──────────────────────────────────────
        private decimal PerguntarQuantidade(string nomeProduto, decimal precoUnit)
        {
            using var dlg = new Form
            {
                Text = "Quantidade",
                Size = new Size(340, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };

            var lbl = new Label
            {
                Text = $"{nomeProduto}\nR$ {precoUnit:F2} por unidade — Quantidade:",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(14, 14)
            };

            var txt = new TextBox
            {
                Text = "1",
                Font = new Font("Segoe UI", 14F),
                Location = new Point(14, 72),
                Width = 300
            };
            txt.SelectAll();

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Location = new Point(214, 112)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk });
            dlg.AcceptButton = btnOk;

            if (dlg.ShowDialog(this) != DialogResult.OK) return 0;

            if (!decimal.TryParse(txt.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal qtd) || qtd <= 0)
            {
                MessageBox.Show("Quantidade inválida.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            return qtd;
        }

        // ── Itens ─────────────────────────────────────────────────────
        private void AdicionarItem(Produto p, decimal qtd)
        {
            var item = new ItemVenda
            {
                ProdutoId = p.Id,
                ProdutoNome = p.Nome,
                Quantidade = qtd,
                PrecoUnitario = p.Preco
            };
            _itens.Add(item);

            string unidade = p.Pesavel ? "kg" : p.Unidade;
            string qtdTexto = p.Pesavel ? $"{qtd:F3} {unidade}" : $"{qtd:F0} {unidade}";
            string precoTexto = $"R$ {p.Preco:F2}/{(p.Pesavel ? "kg" : "un")}";
            string totalTexto = $"R$ {item.Subtotal:F2}";

            grdItens.Rows.Add(p.Nome, qtdTexto, precoTexto, totalTexto);
            AtualizarTotal();
        }

        private void RemoverItem()
        {
            if (grdItens.CurrentRow == null || grdItens.CurrentRow.Index < 0) return;
            int i = grdItens.CurrentRow.Index;
            _itens.RemoveAt(i);
            grdItens.Rows.RemoveAt(i);
            AtualizarTotal();
        }

        private void AtualizarTotal() =>
            lblTotal.Text = $"Total: R$ {_itens.Sum(i => i.Subtotal):F2}";

        // ── Cliente ───────────────────────────────────────────────────
        private void BuscarPorCodigo()
        {
            var texto = txtCodCliente.Text.Trim();
            if (string.IsNullOrEmpty(texto)) return;

            var cli = _db.BuscarClientePorCodigo(texto)
                      ?? _db.ListarClientes(texto).FirstOrDefault();

            if (cli == null)
            {
                MessageBox.Show("Cliente não encontrado.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelecionarCliente(cli);
            txtEan.Focus();
        }

        private void AbrirBusca()
        {
            using var dlg = new FormBuscaCliente(_db);
            if (dlg.ShowDialog() == DialogResult.OK && dlg.ClienteSelecionado != null)
                SelecionarCliente(dlg.ClienteSelecionado);
            txtEan.Focus();
        }

        private void SelecionarCliente(Cliente c)
        {
            _clienteSelecionado = c;
            txtCodCliente.Text = c.Codigo;
            lblCliente.Text = $"{c.Nome}   |   Caderneta: R$ {c.SaldoFiado:F2} / Limite: R$ {c.LimiteFiado:F2}";
            lblCliente.ForeColor = Color.FromArgb(15, 110, 86);
        }

        private void LimparCliente()
        {
            _clienteSelecionado = null;
            txtCodCliente.Clear();
            lblCliente.Text = "Venda à vista — sem cliente selecionado";
            lblCliente.ForeColor = Color.Gray;
        }

        // ─────────────────────────────────────────────────────────────
        // FINALIZAR — pagamento em forma única
        // ─────────────────────────────────────────────────────────────

        private void FinalizarVenda(FormaPagamento forma)
        {
            if (_itens.Count == 0)
            {
                MessageBox.Show("Adicione produtos ao carrinho.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (forma == FormaPagamento.Fiado)
            {
                if (_clienteSelecionado == null)
                {
                    MessageBox.Show("Selecione um cliente para caderneta.", "Atenção",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!_clienteSelecionado.PodeFiar)
                {
                    MessageBox.Show(
                        $"Cliente no limite!\nSaldo: R$ {_clienteSelecionado.SaldoFiado:F2} / Limite: R$ {_clienteSelecionado.LimiteFiado:F2}",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // ── Dinheiro: abre FormTroco (já tinha confirmação) ──────────
            if (forma == FormaPagamento.Dinheiro)
            {
                using var dlgTroco = new FormTroco(_itens.Sum(i => i.Subtotal), _itens);
                if (dlgTroco.ShowDialog(this) != DialogResult.OK)
                    return;   // cliente cancelou — itens permanecem no carrinho
            }
            // ── Débito / Crédito / Pix: aguarda confirmação do operador ──
            else if (forma == FormaPagamento.CartaoDebito ||
                     forma == FormaPagamento.CartaoCredito ||
                     forma == FormaPagamento.Pix)
            {
                string nomePagamento = forma switch
                {
                    FormaPagamento.CartaoDebito => "Débito",
                    FormaPagamento.CartaoCredito => "Crédito",
                    FormaPagamento.Pix => "Pix",
                    _ => forma.ToString()
                };

                string emoji = forma == FormaPagamento.Pix ? "📲" : "💳";

                var confirmado = MessageBox.Show(
                    $"{emoji}  Pagamento via {nomePagamento}\n\n" +
                    $"Total: R$ {_itens.Sum(i => i.Subtotal):F2}\n\n" +
                    $"{(forma == FormaPagamento.Pix ? "Confirme o Pix no celular" : "Passe o cartão na maquininha")} e clique em\n" +
                    $"✔ SIM — quando o pagamento for aprovado\n" +
                    $"✖ NÃO — para cancelar e tentar outra forma",
                    $"Aguardando {nomePagamento}",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);

                if (confirmado != DialogResult.Yes)
                    return;   // não aprovado — itens permanecem no carrinho
            }

            SalvarVendaFinalizada(
                new List<ParcialPagamento>
                {
            new() { Forma = forma, Valor = _itens.Sum(i => i.Subtotal) }
                },
                forma);
        }

        // ─────────────────────────────────────────────────────────────
        // FINALIZAR — pagamento misto (múltiplas formas)
        // ─────────────────────────────────────────────────────────────
        private void AbrirPagamentoMisto()
        {
            if (_itens.Count == 0)
            {
                MessageBox.Show("Adicione produtos ao carrinho.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new FormPagamentoMisto(_itens.Sum(i => i.Subtotal), _clienteSelecionado, _db);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            if (dlg.ClienteSelecionado != null)
                SelecionarCliente(dlg.ClienteSelecionado);

            var formaPrincipal = dlg.Pagamentos
                .OrderByDescending(p => p.Valor)
                .First().Forma;

            SalvarVendaFinalizada(dlg.Pagamentos, formaPrincipal);
        }

        // ─────────────────────────────────────────────────────────────
        // NÚCLEO — grava a venda
        // ─────────────────────────────────────────────────────────────
        private void SalvarVendaFinalizada(List<ParcialPagamento> pagamentos, FormaPagamento formaRegistro)
        {
            decimal total = _itens.Sum(i => i.Subtotal);

            var parcelaCaderneta = pagamentos.FirstOrDefault(p => p.Forma == FormaPagamento.Fiado);
            if (parcelaCaderneta != null)
            {
                _db.AtualizarSaldoFiado(_clienteSelecionado!.Id, parcelaCaderneta.Valor);
            }

            var venda = new Venda
            {
                DataHora = DateTime.Now,
                ClienteId = _clienteSelecionado?.Id,
                ClienteNome = _clienteSelecionado?.Nome ?? "",
                FormaPagamento = formaRegistro,
                Itens = new List<ItemVenda>(_itens)
            };
            _db.SalvarVenda(venda);

            var linhas = pagamentos.Select(p =>
            {
                string nome = p.Forma switch
                {
                    FormaPagamento.Dinheiro => "Dinheiro",
                    FormaPagamento.CartaoDebito => "Débito",
                    FormaPagamento.CartaoCredito => "Crédito",
                    FormaPagamento.Pix => "Pix",
                    FormaPagamento.Fiado => "Caderneta",
                    _ => p.Forma.ToString()
                };
                return $"  • {nome}: R$ {p.Valor:F2}";
            });

            MessageBox.Show(
                $"Venda finalizada!\n\nCliente: {(venda.ClienteNome == "" ? "Avulso" : venda.ClienteNome)}\nTotal: R$ {total:F2}\n\nPagamento:\n{string.Join(Environment.NewLine, linhas)}",
                "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LimparCaixa();
        }

        // ── LimparCaixa ───────────────────────────────────────────────
        private void LimparCaixa()
        {
            _itens.Clear();
            grdItens.Rows.Clear();
            AtualizarTotal();
            LimparCliente();
            CancelarPesado();
        }

        // ─────────────────────────────────────────────────────────────
        // ESTORNO
        // ─────────────────────────────────────────────────────────────
        private void EstornarUltimaVenda()
        {
            int? vendaId = _db.UltimaVendaId();

            if (vendaId == null)
            {
                MessageBox.Show("Nenhuma venda disponível para estorno.",
                    "Estorno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var venda = _db.BuscarVendaComItens(vendaId.Value);
            if (venda == null) return;

            var resumo = string.Join("\n", venda.Itens.Select(i =>
                $"  • {i.ProdutoNome} — " +
                $"{(i.Quantidade % 1 == 0 ? $"{i.Quantidade:F0}" : $"{i.Quantidade:F3}")} x " +
                $"R$ {i.PrecoUnitario:F2} = R$ {i.Subtotal:F2}"));

            string cliente = string.IsNullOrEmpty(venda.ClienteNome) ? "Avulso" : venda.ClienteNome;

            var confirm = MessageBox.Show(
                $"Confirma estorno da última venda?\n\n" +
                $"Data: {venda.DataHora:dd/MM/yyyy HH:mm}\n" +
                $"Cliente: {cliente}\n" +
                $"Total: R$ {venda.Total:F2}\n\n" +
                $"Itens:\n{resumo}\n\n" +
                $"O estoque será devolvido automaticamente.",
                "Confirmar Estorno",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            // pede o motivo
            using var dlgMotivo = new Form
            {
                Text = "Motivo do Estorno",
                Size = new Size(420, 165),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(245, 245, 242)
            };

            var lblMot = new Label
            {
                Text = "Informe o motivo do estorno:",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(14, 14)
            };

            var txtMotivo = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(14, 40),
                Width = 376,
                PlaceholderText = "Ex: Cartão recusado, Pix não confirmado, Cliente desistiu..."
            };

            var btnOkMot = new Button
            {
                Text = "Confirmar Estorno",
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 36),
                Location = new Point(210, 84)
            };
            btnOkMot.FlatAppearance.BorderSize = 0;

            dlgMotivo.Controls.AddRange(new Control[] { lblMot, txtMotivo, btnOkMot });
            dlgMotivo.AcceptButton = btnOkMot;

            if (dlgMotivo.ShowDialog(this) != DialogResult.OK) return;

            string motivo = string.IsNullOrWhiteSpace(txtMotivo.Text)
                ? "Sem motivo informado"
                : txtMotivo.Text.Trim();

            _db.EstornarVenda(vendaId.Value, motivo);

            MessageBox.Show(
                $"Estorno realizado!\n\n" +
                $"Venda #{vendaId} estornada.\n" +
                $"Estoque devolvido.\n" +
                (venda.FormaPagamento == FormaPagamento.Fiado
                    ? "Saldo da caderneta revertido.\n" : "") +
                $"\nMotivo: {motivo}",
                "Estorno Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtEan.Focus();
        }
    }
}