using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using FeirinhaCodorna.Data;
using FeirinhaCodorna.Models;

namespace FeirinhaCodorna.Forms
{
    public class FrmGerenciarFuncionarios : Form
    {
        private DataGridView dgvFuncionarios;
        private BancoDados banco = new BancoDados();

        // controla se está em modo edição (evita que o grid sobrescreva os campos)
        private bool _modoEdicao = false;

        // ── Aba Dados Pessoais ──────────────────────────────────────────
        private TextBox txtNome, txtRG, txtFuncao;
        private TextBox txtEndereco, txtNumero, txtBairro, txtCidade, txtEstado;
        private MaskedTextBox mskCpf, mskTelefone, mskCEP, mskNascimento, mskAdmissao;

        // ── Aba Emergência — Contato 1 ──────────────────────────────────
        private TextBox txtContato1Nome, txtContato1Parentesco;
        private MaskedTextBox mskContato1Fixo, mskContato1Cel;

        // ── Aba Emergência — Contato 2 ──────────────────────────────────
        private TextBox txtContato2Nome, txtContato2Parentesco;
        private MaskedTextBox mskContato2Fixo, mskContato2Cel;

        // botões que precisamos referenciar fora do construtor
        private Button btnSalvar, btnEditar, btnExcluir, btnLimpar, btnEmergencia;

        public FrmGerenciarFuncionarios()
        {
            this.Text = "Gerenciamento de Funcionários";
            this.Size = new Size(940, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Font = new Font("Segoe UI", 9.5f);

            // ============================================================
            // TABS
            // ============================================================
            var tabs = new TabControl
            {
                Location = new Point(15, 12),
                Size = new Size(895, 260),
                Font = new Font("Segoe UI", 9.5f)
            };

            var tabDados = new TabPage("📋  Dados Pessoais");
            var tabEmerg = new TabPage("🚨  Contatos de Emergência");

            // ============================================================
            // ABA DADOS PESSOAIS
            // ============================================================
            int lx = 12, ly = 18, lh = 42;

            // Linha 1 — Nome + Função
            tabDados.Controls.Add(MkLabel("Nome completo:", lx, ly));
            txtNome = MkTextBox(lx + 112, ly, 310); tabDados.Controls.Add(txtNome);
            tabDados.Controls.Add(MkLabel("Função:", lx + 440, ly));
            txtFuncao = MkTextBox(lx + 500, ly, 160); tabDados.Controls.Add(txtFuncao);

            ly += lh;
            // Linha 2 — RG + CPF + Telefone
            tabDados.Controls.Add(MkLabel("RG:", lx, ly));
            txtRG = MkTextBox(lx + 112, ly, 130); tabDados.Controls.Add(txtRG);
            tabDados.Controls.Add(MkLabel("CPF:", lx + 258, ly));
            mskCpf = new MaskedTextBox { Mask = "000.000.000-00", Location = new Point(lx + 295, ly), Size = new Size(130, 25) };
            tabDados.Controls.Add(mskCpf);
            tabDados.Controls.Add(MkLabel("Telefone:", lx + 442, ly));
            mskTelefone = new MaskedTextBox { Mask = "(00) 00000-0000", Location = new Point(lx + 510, ly), Size = new Size(150, 25) };
            tabDados.Controls.Add(mskTelefone);

            ly += lh;
            // Linha 3 — Endereço + Número
            tabDados.Controls.Add(MkLabel("Endereço:", lx, ly));
            txtEndereco = MkTextBox(lx + 112, ly, 390); tabDados.Controls.Add(txtEndereco);
            tabDados.Controls.Add(MkLabel("Número:", lx + 518, ly));
            txtNumero = MkTextBox(lx + 578, ly, 82); tabDados.Controls.Add(txtNumero);

            ly += lh;
            // Linha 4 — Bairro + Cidade
            tabDados.Controls.Add(MkLabel("Bairro:", lx, ly));
            txtBairro = MkTextBox(lx + 112, ly, 210); tabDados.Controls.Add(txtBairro);
            tabDados.Controls.Add(MkLabel("Cidade:", lx + 338, ly));
            txtCidade = MkTextBox(lx + 395, ly, 210); tabDados.Controls.Add(txtCidade);

            ly += lh;
            // Linha 5 — Estado + CEP + Nascimento + Admissão
            tabDados.Controls.Add(MkLabel("Estado:", lx, ly));
            txtEstado = MkTextBox(lx + 112, ly, 55); tabDados.Controls.Add(txtEstado);
            tabDados.Controls.Add(MkLabel("CEP:", lx + 183, ly));
            mskCEP = new MaskedTextBox { Mask = "00000-000", Location = new Point(lx + 220, ly), Size = new Size(100, 25) };
            tabDados.Controls.Add(mskCEP);
            tabDados.Controls.Add(MkLabel("Nascimento:", lx + 336, ly));
            mskNascimento = new MaskedTextBox { Mask = "00/00/0000", Location = new Point(lx + 420, ly), Size = new Size(110, 25) };
            tabDados.Controls.Add(mskNascimento);
            tabDados.Controls.Add(MkLabel("Admissão:", lx + 545, ly));
            mskAdmissao = new MaskedTextBox { Mask = "00/00/0000", Location = new Point(lx + 618, ly), Size = new Size(110, 25) };
            tabDados.Controls.Add(mskAdmissao);

            // ============================================================
            // ABA EMERGÊNCIA
            // ============================================================
            var lblAviso = new Label
            {
                Text = "⚠  Cadastre até 2 pessoas para contato em caso de emergência.",
                Location = new Point(10, 8),
                Size = new Size(860, 20),
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            tabEmerg.Controls.Add(lblAviso);

            // Contato 1
            var grp1 = new GroupBox { Text = "Contato 1", Location = new Point(10, 32), Size = new Size(860, 82), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            grp1.Controls.Add(MkLabel("Nome:", 8, 22));
            txtContato1Nome = MkTextBox(65, 20, 220); grp1.Controls.Add(txtContato1Nome);
            grp1.Controls.Add(MkLabel("Parentesco:", 300, 22));
            txtContato1Parentesco = MkTextBox(390, 20, 130); grp1.Controls.Add(txtContato1Parentesco);
            grp1.Controls.Add(MkLabel("Tel. fixo:", 535, 22));
            mskContato1Fixo = new MaskedTextBox { Mask = "(00) 0000-0000", Location = new Point(600, 20), Size = new Size(130, 25) };
            grp1.Controls.Add(mskContato1Fixo);
            grp1.Controls.Add(MkLabel("Celular/WhatsApp:", 8, 52));
            mskContato1Cel = new MaskedTextBox { Mask = "(00) 00000-0000", Location = new Point(148, 50), Size = new Size(140, 25) };
            grp1.Controls.Add(mskContato1Cel);
            tabEmerg.Controls.Add(grp1);

            // Contato 2
            var grp2 = new GroupBox { Text = "Contato 2", Location = new Point(10, 122), Size = new Size(860, 82), Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
            grp2.Controls.Add(MkLabel("Nome:", 8, 22));
            txtContato2Nome = MkTextBox(65, 20, 220); grp2.Controls.Add(txtContato2Nome);
            grp2.Controls.Add(MkLabel("Parentesco:", 300, 22));
            txtContato2Parentesco = MkTextBox(390, 20, 130); grp2.Controls.Add(txtContato2Parentesco);
            grp2.Controls.Add(MkLabel("Tel. fixo:", 535, 22));
            mskContato2Fixo = new MaskedTextBox { Mask = "(00) 0000-0000", Location = new Point(600, 20), Size = new Size(130, 25) };
            grp2.Controls.Add(mskContato2Fixo);
            grp2.Controls.Add(MkLabel("Celular/WhatsApp:", 8, 52));
            mskContato2Cel = new MaskedTextBox { Mask = "(00) 00000-0000", Location = new Point(148, 50), Size = new Size(140, 25) };
            grp2.Controls.Add(mskContato2Cel);
            tabEmerg.Controls.Add(grp2);

            tabs.TabPages.Add(tabDados);
            tabs.TabPages.Add(tabEmerg);

            // ============================================================
            // BOTÕES
            // ============================================================
            btnSalvar = new Button
            {
                Text = "💾  Salvar",
                Location = new Point(15, 282),
                Size = new Size(110, 34),
                BackColor = Color.FromArgb(144, 210, 144),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Enabled = false
            };
            btnSalvar.FlatAppearance.BorderSize = 0;

            btnEditar = new Button
            {
                Text = "✏️  Editar",
                Location = new Point(135, 282),
                Size = new Size(110, 34),
                BackColor = Color.FromArgb(135, 185, 230),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Enabled = false
            };
            btnEditar.FlatAppearance.BorderSize = 0;

            btnExcluir = new Button
            {
                Text = "🗑  Excluir",
                Location = new Point(255, 282),
                Size = new Size(110, 34),
                BackColor = Color.FromArgb(240, 128, 128),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                Enabled = false
            };
            btnExcluir.FlatAppearance.BorderSize = 0;

            btnLimpar = new Button
            {
                Text = "🧹  Novo",
                Location = new Point(375, 282),
                Size = new Size(110, 34),
                BackColor = Color.FromArgb(210, 210, 210),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f)
            };
            btnLimpar.FlatAppearance.BorderSize = 0;

            btnEmergencia = new Button
            {
                Text = "🚨  Ver Contato de Emergência",
                Location = new Point(530, 282),
                Size = new Size(255, 34),
                BackColor = Color.FromArgb(200, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Enabled = false
            };
            btnEmergencia.FlatAppearance.BorderSize = 0;

            btnSalvar.Click += BtnSalvar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnExcluir.Click += BtnExcluir_Click;
            btnLimpar.Click += (s, e) => IniciarNovo();
            btnEmergencia.Click += BtnEmergencia_Click;

            // ============================================================
            // GRID
            // ============================================================
            dgvFuncionarios = new DataGridView
            {
                Location = new Point(15, 328),
                Size = new Size(895, 335),
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.WhiteSmoke,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                MultiSelect = false
            };
            dgvFuncionarios.EnableHeadersVisualStyles = false;
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(210, 208, 200);
            dgvFuncionarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvFuncionarios.SelectionChanged += DgvFuncionarios_SelectionChanged;
            dgvFuncionarios.CellDoubleClick += DgvFuncionarios_CellDoubleClick;

            var lblDica = new Label
            {
                Text = "💡 Selecione um funcionário e clique em ✏️ Editar — ou dê duplo clique na linha.",
                Location = new Point(15, 672),
                Size = new Size(895, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic)
            };

            this.Controls.AddRange(new Control[]
            {
                tabs, btnSalvar, btnEditar, btnExcluir, btnLimpar, btnEmergencia,
                dgvFuncionarios, lblDica
            });

            DefinirCamposEditaveis(false);
            CarregarFuncionarios();
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private Label MkLabel(string text, int x, int y) =>
            new Label { Text = text, Location = new Point(x, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f) };

        private TextBox MkTextBox(int x, int y, int w) =>
            new TextBox { Location = new Point(x, y), Size = new Size(w, 25) };

        private void DefinirCamposEditaveis(bool editavel)
        {
            Control[] campos = {
                txtNome, txtRG, txtFuncao, txtEndereco, txtNumero,
                txtBairro, txtCidade, txtEstado,
                mskCpf, mskTelefone, mskCEP, mskNascimento, mskAdmissao,
                txtContato1Nome, txtContato1Parentesco, mskContato1Fixo, mskContato1Cel,
                txtContato2Nome, txtContato2Parentesco, mskContato2Fixo, mskContato2Cel
            };
            foreach (var c in campos)
                c.Enabled = editavel;

            btnSalvar.Enabled = editavel;
        }

        // ============================================================
        // CARREGAR GRID
        // ============================================================
        private void CarregarFuncionarios()
        {
            var lista = banco.ListarFuncionarios();
            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Nome");
            dt.Columns.Add("CPF");
            dt.Columns.Add("Telefone");
            dt.Columns.Add("Função");
            dt.Columns.Add("Bairro");
            dt.Columns.Add("Cidade");
            dt.Columns.Add("Admissão");
            dt.Columns.Add("Contato 1");
            dt.Columns.Add("Cel. Contato 1");
            dt.Columns.Add("Contato 2");
            dt.Columns.Add("Cel. Contato 2");

            foreach (var f in lista)
                dt.Rows.Add(
                    f.Id, f.Nome, f.CPF, f.Telefone, f.Funcao,
                    f.Bairro, f.Cidade,
                    f.DataAdmissao > DateTime.MinValue ? f.DataAdmissao.ToString("dd/MM/yyyy") : "",
                    f.ContatoEmergencia, f.CelularEmergencia,
                    f.ContatoEmergencia2, f.CelularEmergencia2);

            dgvFuncionarios.DataSource = dt;

            if (dgvFuncionarios.Columns["Id"] != null)
                dgvFuncionarios.Columns["Id"].Visible = false;
        }

        // ============================================================
        // SELEÇÃO NO GRID — apenas habilita botões, NÃO preenche campos
        // ============================================================
        private void DgvFuncionarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (_modoEdicao) return; // em edição, ignora mudanças de seleção
            bool temSelecao = dgvFuncionarios.SelectedRows.Count > 0;
            btnEditar.Enabled = temSelecao;
            btnExcluir.Enabled = temSelecao;
            btnEmergencia.Enabled = temSelecao;
        }

        // Duplo clique = atalho para Editar
        private void DgvFuncionarios_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                CarregarParaEdicao();
        }

        // ============================================================
        // INICIAR NOVO CADASTRO
        // ============================================================
        private void IniciarNovo()
        {
            _modoEdicao = true;
            LimparCampos();
            DefinirCamposEditaveis(true);
            dgvFuncionarios.ClearSelection();
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnEmergencia.Enabled = false;
            txtNome.Focus();
        }

        // ============================================================
        // BOTÃO EDITAR — carrega dados e libera campos
        // ============================================================
        private void BtnEditar_Click(object? sender, EventArgs e) => CarregarParaEdicao();

        private void CarregarParaEdicao()
        {
            if (dgvFuncionarios.SelectedRows.Count == 0) return;
            if (!int.TryParse(dgvFuncionarios.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;

            var f = banco.BuscarFuncionario(id);
            if (f == null) return;

            _modoEdicao = true;
            DefinirCamposEditaveis(true);
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;

            PreencherCampos(f);
            txtNome.Focus();
        }

        private void PreencherCampos(Funcionario f)
        {
            txtNome.Text = f.Nome;
            txtRG.Text = f.RG;
            txtFuncao.Text = f.Funcao;
            txtEndereco.Text = f.Endereco;
            txtNumero.Text = f.Numero;
            txtBairro.Text = f.Bairro;
            txtCidade.Text = f.Cidade;
            txtEstado.Text = f.Estado;

            mskCpf.Text = f.CPF;
            mskTelefone.Text = f.Telefone;
            mskCEP.Text = f.CEP;
            mskNascimento.Text = f.DataNascimento > DateTime.MinValue ? f.DataNascimento.ToString("ddMMyyyy") : "";
            mskAdmissao.Text = f.DataAdmissao > DateTime.MinValue ? f.DataAdmissao.ToString("ddMMyyyy") : "";

            txtContato1Nome.Text = f.ContatoEmergencia;
            txtContato1Parentesco.Text = f.ParentescoEmergencia;
            mskContato1Fixo.Text = f.TelFixoEmergencia;
            mskContato1Cel.Text = f.CelularEmergencia;

            txtContato2Nome.Text = f.ContatoEmergencia2;
            txtContato2Parentesco.Text = f.ParentescoEmergencia2;
            mskContato2Fixo.Text = f.TelFixoEmergencia2;
            mskContato2Cel.Text = f.CelularEmergencia2;
        }

        // ============================================================
        // SALVAR
        // ============================================================
        private void BtnSalvar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Preencha o nome do funcionário!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = 0;
            if (dgvFuncionarios.SelectedRows.Count > 0)
                int.TryParse(dgvFuncionarios.SelectedRows[0].Cells["Id"].Value?.ToString(), out id);

            DateTime ParseData(MaskedTextBox m) =>
                DateTime.TryParseExact(m.Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var d) ? d : DateTime.MinValue;

            var f = new Funcionario
            {
                Id = id,
                Nome = txtNome.Text.Trim(),
                RG = txtRG.Text.Trim(),
                CPF = mskCpf.Text.Trim(),
                Telefone = mskTelefone.Text.Trim(),
                Funcao = txtFuncao.Text.Trim(),
                Endereco = txtEndereco.Text.Trim(),
                Numero = txtNumero.Text.Trim(),
                Bairro = txtBairro.Text.Trim(),
                Cidade = txtCidade.Text.Trim(),
                Estado = txtEstado.Text.Trim(),
                CEP = mskCEP.Text.Trim(),
                DataNascimento = ParseData(mskNascimento),
                DataAdmissao = ParseData(mskAdmissao),

                ContatoEmergencia = txtContato1Nome.Text.Trim(),
                ParentescoEmergencia = txtContato1Parentesco.Text.Trim(),
                TelFixoEmergencia = mskContato1Fixo.Text.Trim(),
                CelularEmergencia = mskContato1Cel.Text.Trim(),

                ContatoEmergencia2 = txtContato2Nome.Text.Trim(),
                ParentescoEmergencia2 = txtContato2Parentesco.Text.Trim(),
                TelFixoEmergencia2 = mskContato2Fixo.Text.Trim(),
                CelularEmergencia2 = mskContato2Cel.Text.Trim(),
            };

            banco.SalvarFuncionario(f);
            MessageBox.Show("Funcionário salvo com sucesso!", "Sucesso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            _modoEdicao = false;
            DefinirCamposEditaveis(false);
            LimparCampos();
            CarregarFuncionarios();
        }

        // ============================================================
        // EXCLUIR
        // ============================================================
        private void BtnExcluir_Click(object? sender, EventArgs e)
        {
            if (dgvFuncionarios.SelectedRows.Count == 0) return;

            string nome = dgvFuncionarios.SelectedRows[0].Cells["Nome"].Value?.ToString() ?? "";
            if (MessageBox.Show($"Excluir o funcionário '{nome}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int.TryParse(dgvFuncionarios.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id);
                banco.ExcluirFuncionario(id);
                _modoEdicao = false;
                DefinirCamposEditaveis(false);
                LimparCampos();
                CarregarFuncionarios();
            }
        }

        // ============================================================
        // BOTÃO EMERGÊNCIA — lê direto do banco, sem depender dos campos
        // ============================================================
        private void BtnEmergencia_Click(object? sender, EventArgs e)
        {
            if (dgvFuncionarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um funcionário na lista primeiro.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(dgvFuncionarios.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            var f = banco.BuscarFuncionario(id);
            if (f == null) return;

            bool sem1 = string.IsNullOrWhiteSpace(f.ContatoEmergencia) && string.IsNullOrWhiteSpace(f.CelularEmergencia);
            bool sem2 = string.IsNullOrWhiteSpace(f.ContatoEmergencia2) && string.IsNullOrWhiteSpace(f.CelularEmergencia2);

            if (sem1 && sem2)
            {
                MessageBox.Show(
                    $"Nenhum contato de emergência cadastrado para '{f.Nome}'.\n\nSelecione, clique em ✏️ Editar e preencha a aba '🚨 Contatos de Emergência'.",
                    "Sem dados", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string Linha(string label, string val) =>
                $"  {label,-18} {(string.IsNullOrWhiteSpace(val) ? "Não informado" : val)}\n";

            string msg =
                $"🚨  CONTATO DE EMERGÊNCIA\n" +
                $"  Funcionário: {f.Nome}\n" +
                $"{"─────────────────────────────────────────",45}\n\n";

            if (!sem1)
            {
                msg += "  👤 CONTATO 1\n";
                msg += Linha("Familiar:", f.ContatoEmergencia);
                msg += Linha("Parentesco:", f.ParentescoEmergencia);
                msg += Linha("Tel. fixo:", f.TelFixoEmergencia);
                msg += Linha("Celular/WhatsApp:", f.CelularEmergencia);
            }

            if (!sem2)
            {
                msg += "\n  👤 CONTATO 2\n";
                msg += Linha("Familiar:", f.ContatoEmergencia2);
                msg += Linha("Parentesco:", f.ParentescoEmergencia2);
                msg += Linha("Tel. fixo:", f.TelFixoEmergencia2);
                msg += Linha("Celular/WhatsApp:", f.CelularEmergencia2);
            }

            MessageBox.Show(msg, "🚨 Emergência — " + f.Nome,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ============================================================
        // LIMPAR CAMPOS
        // ============================================================
        private void LimparCampos()
        {
            txtNome.Clear(); txtRG.Clear(); txtFuncao.Clear();
            txtEndereco.Clear(); txtNumero.Clear(); txtBairro.Clear();
            txtCidade.Clear(); txtEstado.Clear();

            mskCpf.Clear(); mskTelefone.Clear(); mskCEP.Clear();
            mskNascimento.Clear(); mskAdmissao.Clear();

            txtContato1Nome.Clear(); txtContato1Parentesco.Clear();
            mskContato1Fixo.Clear(); mskContato1Cel.Clear();

            txtContato2Nome.Clear(); txtContato2Parentesco.Clear();
            mskContato2Fixo.Clear(); mskContato2Cel.Clear();

            dgvFuncionarios.ClearSelection();
            btnEditar.Enabled = false;
            btnExcluir.Enabled = false;
            btnEmergencia.Enabled = false;
        }
    }
}