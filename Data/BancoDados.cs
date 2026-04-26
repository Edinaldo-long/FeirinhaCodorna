using FeirinhaCodorna.Forms;
using FeirinhaCodorna.Models;
using FeirinhaCodorna.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeirinhaCodorna.Data
{
    public class BancoDados
    {
        private readonly string _conexao;

        public BancoDados()
        {
            _conexao = "Data Source=feirinha.db";
            CriarTabelas();
            MigrarTabelas();
            SeedUsuarioAdmin();
        }

        private void SeedUsuarioAdmin()
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();

            var check = con.CreateCommand();
            check.CommandText = "SELECT COUNT(*) FROM Usuarios";
            long count = (long)check.ExecuteScalar()!;
            if (count > 0) return;

            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO Usuarios (Login, SenhaCriptografada, Perfil) VALUES ($login, $senha, $perfil)";
            cmd.Parameters.AddWithValue("$login", "admin");
            cmd.Parameters.AddWithValue("$senha", Seguranca.GerarHash("123456"));
            cmd.Parameters.AddWithValue("$perfil", "Administrador");
            cmd.ExecuteNonQuery();
        }

        private void CriarTabelas()
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();

            var tabelas = new[]
            {
                @"CREATE TABLE IF NOT EXISTS Funcionarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    RG TEXT DEFAULT '',
                    CPF TEXT DEFAULT '',
                    Endereco TEXT DEFAULT '',
                    Numero TEXT DEFAULT '',
                    Bairro TEXT DEFAULT '',
                    Cidade TEXT DEFAULT '',
                    Estado TEXT DEFAULT '',
                    CEP TEXT DEFAULT '',
                    Telefone TEXT DEFAULT '',
                    ContatoEmergencia TEXT DEFAULT '',
                    ParentescoEmergencia TEXT DEFAULT '',
                    TelFixoEmergencia TEXT DEFAULT '',
                    CelularEmergencia TEXT DEFAULT '',
                    ContatoEmergencia2 TEXT DEFAULT '',
                    ParentescoEmergencia2 TEXT DEFAULT '',
                    TelFixoEmergencia2 TEXT DEFAULT '',
                    CelularEmergencia2 TEXT DEFAULT '',
                    Funcao TEXT DEFAULT '',
                    DataNascimento TEXT DEFAULT '',
                    DataAdmissao TEXT DEFAULT '',
                    DataDemissao TEXT DEFAULT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    SenhaCriptografada TEXT NOT NULL,
                    Perfil TEXT NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS Fornecedores (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    CnpjCpf TEXT,
                    Telefone TEXT,
                    Endereco TEXT DEFAULT '',
                    Bairro TEXT DEFAULT '',
                    Produtos TEXT,
                    Ativo INTEGER DEFAULT 1
                )",
                @"CREATE TABLE IF NOT EXISTS Produtos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CodigoInterno TEXT,
                    CodigoEan TEXT,
                    Nome TEXT NOT NULL,
                    Unidade TEXT DEFAULT 'kg',
                    Pesavel INTEGER DEFAULT 1,
                    Preco REAL DEFAULT 0,
                    Estoque REAL DEFAULT 0,
                    EstoqueMinimo REAL DEFAULT 5,
                    FornecedorId INTEGER DEFAULT 0
                )",
                @"CREATE TABLE IF NOT EXISTS Clientes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Codigo TEXT,
                    Nome TEXT NOT NULL,
                    Telefone TEXT DEFAULT '',
                    Cpf TEXT DEFAULT '',
                    LimiteFiado REAL DEFAULT 100,
                    SaldoFiado REAL DEFAULT 0,
                    Celular TEXT DEFAULT '',
                    WhatsApp TEXT DEFAULT '',
                    Rg TEXT DEFAULT '',
                    Endereco TEXT DEFAULT '',
                    Bairro TEXT DEFAULT '',
                    AutorizadoCaderneta TEXT DEFAULT '',
                    Numero TEXT DEFAULT '',
                    Complemento TEXT DEFAULT ''
                )",
                @"CREATE TABLE IF NOT EXISTS Vendas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DataHora TEXT,
                    ClienteId INTEGER,
                    ClienteNome TEXT,
                    FormaPagamento TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS ItensVenda (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    VendaId INTEGER,
                    ProdutoId INTEGER,
                    ProdutoNome TEXT,
                    Quantidade REAL,
                    PrecoUnitario REAL
                )",
                @"CREATE TABLE IF NOT EXISTS Despesas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Descricao TEXT,
                    Valor REAL,
                    Data TEXT,
                    Categoria TEXT DEFAULT 'Geral'
                )",
                @"CREATE TABLE IF NOT EXISTS TurnosCaixa (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DataAbertura TEXT NOT NULL,
                    DataFechamento TEXT,
                    TrocoInicial REAL DEFAULT 0,
                    TotalVendas REAL DEFAULT 0,
                    TotalSangrias REAL DEFAULT 0,
                    ValorContado REAL DEFAULT 0,
                    Diferenca REAL DEFAULT 0,
                    Status TEXT DEFAULT 'Aberto'
                )",
                @"CREATE TABLE IF NOT EXISTS MovimentacoesCaixa (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TurnoId INTEGER NOT NULL,
                    Tipo TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    Motivo TEXT,
                    DataHora TEXT NOT NULL
                )"
            };

            foreach (var sql in tabelas)
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private void MigrarTabelas()
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var novasColunas = new[]
            {
                "ALTER TABLE Clientes ADD COLUMN Celular TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN WhatsApp TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN Rg TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN Endereco TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN Bairro TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN AutorizadoCaderneta TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN Numero TEXT DEFAULT ''",
                "ALTER TABLE Clientes ADD COLUMN Complemento TEXT DEFAULT ''",
                "ALTER TABLE Produtos ADD COLUMN PrecoCusto REAL DEFAULT 0",
                "ALTER TABLE Fornecedores ADD COLUMN Endereco TEXT DEFAULT ''",
                "ALTER TABLE Fornecedores ADD COLUMN Numero TEXT DEFAULT ''",
                "ALTER TABLE Fornecedores ADD COLUMN Cidade TEXT DEFAULT ''",
                "ALTER TABLE Fornecedores ADD COLUMN Estado TEXT DEFAULT ''",
                "ALTER TABLE Fornecedores ADD COLUMN Cep TEXT DEFAULT ''",
                "ALTER TABLE Despesas ADD COLUMN Vencimento TEXT DEFAULT NULL",
                "ALTER TABLE Despesas ADD COLUMN Situacao TEXT DEFAULT 'Pendente'",
                "ALTER TABLE Despesas ADD COLUMN DataPagamento TEXT DEFAULT NULL",
                "ALTER TABLE Despesas ADD COLUMN FormaPagamentoBaixa TEXT DEFAULT NULL",
                "ALTER TABLE Funcionarios ADD COLUMN RG TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN Endereco TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN Numero TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN Bairro TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN Cidade TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN Estado TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN CEP TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN ContatoEmergencia TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN ParentescoEmergencia TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN TelFixoEmergencia TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN CelularEmergencia TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN ContatoEmergencia2 TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN ParentescoEmergencia2 TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN TelFixoEmergencia2 TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN CelularEmergencia2 TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN DataNascimento TEXT DEFAULT ''",
                "ALTER TABLE Funcionarios ADD COLUMN DataDemissao TEXT DEFAULT NULL"
            };
            foreach (var sql in novasColunas)
            {
                try
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }

        // ==================== USUÁRIOS ====================
        public void SalvarUsuario(string login, string senha, string perfil)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO Usuarios (Login, SenhaCriptografada, Perfil) VALUES ($login, $senha, $perfil)";
            cmd.Parameters.AddWithValue("$login", login);
            cmd.Parameters.AddWithValue("$senha", Seguranca.GerarHash(senha));
            cmd.Parameters.AddWithValue("$perfil", perfil);
            cmd.ExecuteNonQuery();
        }

        public string? ValidarLogin(string login, string senha)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT SenhaCriptografada, Perfil FROM Usuarios WHERE Login = $login";
            cmd.Parameters.AddWithValue("$login", login);

            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                string senhaDb = r.GetString(0);
                if (Seguranca.VerificarSenha(senha, senhaDb))
                    return r.GetString(1);
            }
            return null;
        }

        public List<(string Login, string Perfil)> ListarUsuarios()
        {
            var lista = new List<(string, string)>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Login, Perfil FROM Usuarios";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((r.GetString(0), r.GetString(1)));
            return lista;
        }

        public void ExcluirUsuario(string login)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Usuarios WHERE Login = $login";
            cmd.Parameters.AddWithValue("$login", login);
            cmd.ExecuteNonQuery();
        }

        // ==================== FUNCIONÁRIOS ====================
        public void SalvarFuncionario(Funcionario f)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            if (f.Id == 0)
            {
                cmd.CommandText = @"INSERT INTO Funcionarios
                    (Nome, RG, CPF, Endereco, Numero, Bairro, Cidade, Estado, CEP, Telefone,
                     ContatoEmergencia, ParentescoEmergencia, TelFixoEmergencia, CelularEmergencia,
                     ContatoEmergencia2, ParentescoEmergencia2, TelFixoEmergencia2, CelularEmergencia2,
                     Funcao, DataNascimento, DataAdmissao, DataDemissao)
                    VALUES
                    ($nome, $rg, $cpf, $end, $num, $bairro, $cidade, $estado, $cep, $tel,
                     $ctemp, $par, $telf, $celemer,
                     $ctemp2, $par2, $telf2, $celemer2,
                     $func, $nasc, $adm, $dem)";
            }
            else
            {
                cmd.CommandText = @"UPDATE Funcionarios SET
                    Nome=$nome, RG=$rg, CPF=$cpf,
                    Endereco=$end, Numero=$num, Bairro=$bairro, Cidade=$cidade, Estado=$estado, CEP=$cep,
                    Telefone=$tel,
                    ContatoEmergencia=$ctemp, ParentescoEmergencia=$par,
                    TelFixoEmergencia=$telf, CelularEmergencia=$celemer,
                    ContatoEmergencia2=$ctemp2, ParentescoEmergencia2=$par2,
                    TelFixoEmergencia2=$telf2, CelularEmergencia2=$celemer2,
                    Funcao=$func, DataNascimento=$nasc, DataAdmissao=$adm, DataDemissao=$dem
                    WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", f.Id);
            }
            cmd.Parameters.AddWithValue("$nome", f.Nome);
            cmd.Parameters.AddWithValue("$rg", f.RG);
            cmd.Parameters.AddWithValue("$cpf", f.CPF);
            cmd.Parameters.AddWithValue("$end", f.Endereco);
            cmd.Parameters.AddWithValue("$num", f.Numero);
            cmd.Parameters.AddWithValue("$bairro", f.Bairro);
            cmd.Parameters.AddWithValue("$cidade", f.Cidade);
            cmd.Parameters.AddWithValue("$estado", f.Estado);
            cmd.Parameters.AddWithValue("$cep", f.CEP);
            cmd.Parameters.AddWithValue("$tel", f.Telefone);
            cmd.Parameters.AddWithValue("$ctemp", f.ContatoEmergencia);
            cmd.Parameters.AddWithValue("$par", f.ParentescoEmergencia);
            cmd.Parameters.AddWithValue("$telf", f.TelFixoEmergencia);
            cmd.Parameters.AddWithValue("$celemer", f.CelularEmergencia);
            cmd.Parameters.AddWithValue("$ctemp2", f.ContatoEmergencia2);
            cmd.Parameters.AddWithValue("$par2", f.ParentescoEmergencia2);
            cmd.Parameters.AddWithValue("$telf2", f.TelFixoEmergencia2);
            cmd.Parameters.AddWithValue("$celemer2", f.CelularEmergencia2);
            cmd.Parameters.AddWithValue("$func", f.Funcao);
            cmd.Parameters.AddWithValue("$nasc", f.DataNascimento.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$adm", f.DataAdmissao.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$dem", f.DataDemissao.HasValue ? f.DataDemissao.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void ExcluirFuncionario(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Funcionarios WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Funcionario> ListarFuncionarios()
        {
            var lista = new List<Funcionario>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Funcionarios ORDER BY Nome";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(LerFuncionario(r));
            return lista;
        }

        public Funcionario? BuscarFuncionario(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Funcionarios WHERE Id = $id LIMIT 1";
            cmd.Parameters.AddWithValue("$id", id);
            using var r = cmd.ExecuteReader();
            return r.Read() ? LerFuncionario(r) : null;
        }

        private Funcionario LerFuncionario(SqliteDataReader r)
        {
            int o(string col) => r.GetOrdinal(col);
            string s(string col) => r.IsDBNull(o(col)) ? "" : r.GetString(o(col));
            DateTime parseDate(string col)
            {
                var val = s(col);
                return DateTime.TryParse(val, out var d) ? d : DateTime.MinValue;
            }

            return new Funcionario
            {
                Id = r.GetInt32(o("Id")),
                Nome = s("Nome"),
                RG = s("RG"),
                CPF = s("CPF"),
                Endereco = s("Endereco"),
                Numero = s("Numero"),
                Bairro = s("Bairro"),
                Cidade = s("Cidade"),
                Estado = s("Estado"),
                CEP = s("CEP"),
                Telefone = s("Telefone"),
                ContatoEmergencia = s("ContatoEmergencia"),
                ParentescoEmergencia = s("ParentescoEmergencia"),
                TelFixoEmergencia = s("TelFixoEmergencia"),
                CelularEmergencia = s("CelularEmergencia"),
                ContatoEmergencia2 = s("ContatoEmergencia2"),
                ParentescoEmergencia2 = s("ParentescoEmergencia2"),
                TelFixoEmergencia2 = s("TelFixoEmergencia2"),
                CelularEmergencia2 = s("CelularEmergencia2"),
                Funcao = s("Funcao"),
                DataNascimento = parseDate("DataNascimento"),
                DataAdmissao = parseDate("DataAdmissao"),
                DataDemissao = r.IsDBNull(o("DataDemissao")) ? null :
                               DateTime.TryParse(r.GetString(o("DataDemissao")), out var dd) ? dd : null
            };
        }

        // ==================== PRODUTOS ====================
        public void SalvarProduto(Produto p)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            if (p.Id == 0)
                cmd.CommandText = @"INSERT INTO Produtos (CodigoInterno, CodigoEan, Nome, Unidade, Pesavel, Preco, PrecoCusto, Estoque, EstoqueMinimo, FornecedorId) VALUES ($ci, $ean, $nome, $un, $pes, $preco, $pCusto, $est, $estMin, $forn)";
            else
            {
                cmd.CommandText = @"UPDATE Produtos SET CodigoInterno=$ci, CodigoEan=$ean, Nome=$nome, Unidade=$un, Pesavel=$pes, Preco=$preco, PrecoCusto=$pCusto, Estoque=$est, EstoqueMinimo=$estMin, FornecedorId=$forn WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", p.Id);
            }
            cmd.Parameters.AddWithValue("$ci", p.CodigoInterno);
            cmd.Parameters.AddWithValue("$ean", p.CodigoEan);
            cmd.Parameters.AddWithValue("$nome", p.Nome);
            cmd.Parameters.AddWithValue("$un", p.Unidade);
            cmd.Parameters.AddWithValue("$pes", p.Pesavel ? 1 : 0);
            cmd.Parameters.AddWithValue("$preco", p.Preco);
            cmd.Parameters.AddWithValue("$pCusto", p.PrecoCusto);
            cmd.Parameters.AddWithValue("$est", p.Estoque);
            cmd.Parameters.AddWithValue("$estMin", p.EstoqueMinimo);
            cmd.Parameters.AddWithValue("$forn", p.FornecedorId);
            cmd.ExecuteNonQuery();
        }

        public void ExcluirProduto(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Produtos WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void EntrarEstoque(int id, decimal quantidade)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Produtos SET Estoque = Estoque + $qtd WHERE Id = $id";
            cmd.Parameters.AddWithValue("$qtd", quantidade);
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Produto> ListarProdutos()
        {
            var lista = new List<Produto>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Produtos ORDER BY Nome";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(LerProduto(r));
            return lista;
        }

        public Produto? BuscarPorEan(string ean)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Produtos WHERE CodigoEan = $ean LIMIT 1";
            cmd.Parameters.AddWithValue("$ean", ean);
            using var r = cmd.ExecuteReader();
            return r.Read() ? LerProduto(r) : null;
        }

        public Produto? BuscarPorCodigo(string codigo)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Produtos WHERE CodigoInterno = $cod LIMIT 1";
            cmd.Parameters.AddWithValue("$cod", codigo);
            using var r = cmd.ExecuteReader();
            return r.Read() ? LerProduto(r) : null;
        }

        private Produto LerProduto(SqliteDataReader r) => new Produto
        {
            Id = r.GetInt32(0),
            CodigoInterno = r.GetString(1),
            CodigoEan = r.GetString(2),
            Nome = r.GetString(3),
            Unidade = r.GetString(4),
            Pesavel = r.GetInt32(5) == 1,
            Preco = (decimal)r.GetDouble(6),
            Estoque = (decimal)r.GetDouble(7),
            EstoqueMinimo = (decimal)r.GetDouble(8),
            FornecedorId = r.GetInt32(9),
            PrecoCusto = r.IsDBNull(10) ? 0 : (decimal)r.GetDouble(10)
        };

        // ==================== CLIENTES ====================
        public void SalvarCliente(Cliente c)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            if (c.Id == 0)
                cmd.CommandText = @"INSERT INTO Clientes (Codigo, Nome, Telefone, Cpf, LimiteFiado, SaldoFiado, Celular, WhatsApp, Rg, Endereco, Bairro, AutorizadoCaderneta, Numero, Complemento) VALUES ($cod, $nome, $tel, $cpf, $lim, $saldo, $cel, $wpp, $rg, $end, $bairro, $aut, $num, $comp)";
            else
            {
                cmd.CommandText = @"UPDATE Clientes SET Codigo=$cod, Nome=$nome, Telefone=$tel, Cpf=$cpf, LimiteFiado=$lim, SaldoFiado=$saldo, Celular=$cel, WhatsApp=$wpp, Rg=$rg, Endereco=$end, Bairro=$bairro, AutorizadoCaderneta=$aut, Numero=$num, Complemento=$comp WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", c.Id);
            }
            cmd.Parameters.AddWithValue("$cod", c.Codigo);
            cmd.Parameters.AddWithValue("$nome", c.Nome);
            cmd.Parameters.AddWithValue("$tel", c.Telefone);
            cmd.Parameters.AddWithValue("$cpf", c.Cpf);
            cmd.Parameters.AddWithValue("$lim", c.LimiteFiado);
            cmd.Parameters.AddWithValue("$saldo", c.SaldoFiado);
            cmd.Parameters.AddWithValue("$cel", c.Celular);
            cmd.Parameters.AddWithValue("$wpp", c.WhatsApp);
            cmd.Parameters.AddWithValue("$rg", c.Rg);
            cmd.Parameters.AddWithValue("$end", c.Endereco);
            cmd.Parameters.AddWithValue("$bairro", c.Bairro);
            cmd.Parameters.AddWithValue("$aut", c.AutorizadoCaderneta);
            cmd.Parameters.AddWithValue("$num", c.Numero);
            cmd.Parameters.AddWithValue("$comp", c.Complemento);
            cmd.ExecuteNonQuery();
        }

        public void ExcluirCliente(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Clientes WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Cliente> ListarClientes(string filtro = "")
        {
            var lista = new List<Cliente>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            if (string.IsNullOrWhiteSpace(filtro))
                cmd.CommandText = "SELECT * FROM Clientes ORDER BY Nome";
            else
            {
                cmd.CommandText = "SELECT * FROM Clientes WHERE Nome LIKE $filtro ORDER BY Nome";
                cmd.Parameters.AddWithValue("$filtro", $"%{filtro}%");
            }
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(LerCliente(r));
            return lista;
        }

        public Cliente? BuscarClientePorCodigo(string codigo)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Clientes WHERE Codigo = $cod LIMIT 1";
            cmd.Parameters.AddWithValue("$cod", codigo);
            using var r = cmd.ExecuteReader();
            return r.Read() ? LerCliente(r) : null;
        }

        public void AtualizarSaldoFiado(int clienteId, decimal valor)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Clientes SET SaldoFiado = SaldoFiado + $val WHERE Id = $id";
            cmd.Parameters.AddWithValue("$val", valor);
            cmd.Parameters.AddWithValue("$id", clienteId);
            cmd.ExecuteNonQuery();
        }

        private Cliente LerCliente(SqliteDataReader r)
        {
            int ord(string col) => r.GetOrdinal(col);
            return new Cliente
            {
                Id = r.GetInt32(ord("Id")),
                Codigo = r.IsDBNull(ord("Codigo")) ? "" : r.GetString(ord("Codigo")),
                Nome = r.IsDBNull(ord("Nome")) ? "" : r.GetString(ord("Nome")),
                Telefone = r.IsDBNull(ord("Telefone")) ? "" : r.GetString(ord("Telefone")),
                Cpf = r.IsDBNull(ord("Cpf")) ? "" : r.GetString(ord("Cpf")),
                LimiteFiado = r.IsDBNull(ord("LimiteFiado")) ? 100 : (decimal)r.GetDouble(ord("LimiteFiado")),
                SaldoFiado = r.IsDBNull(ord("SaldoFiado")) ? 0 : (decimal)r.GetDouble(ord("SaldoFiado")),
                Celular = r.IsDBNull(ord("Celular")) ? "" : r.GetString(ord("Celular")),
                WhatsApp = r.IsDBNull(ord("WhatsApp")) ? "" : r.GetString(ord("WhatsApp")),
                Rg = r.IsDBNull(ord("Rg")) ? "" : r.GetString(ord("Rg")),
                Endereco = r.IsDBNull(ord("Endereco")) ? "" : r.GetString(ord("Endereco")),
                Bairro = r.IsDBNull(ord("Bairro")) ? "" : r.GetString(ord("Bairro")),
                AutorizadoCaderneta = r.IsDBNull(ord("AutorizadoCaderneta")) ? "" : r.GetString(ord("AutorizadoCaderneta")),
                Numero = r.IsDBNull(ord("Numero")) ? "" : r.GetString(ord("Numero")),
                Complemento = r.IsDBNull(ord("Complemento")) ? "" : r.GetString(ord("Complemento")),
            };
        }

        // ==================== FORNECEDORES ====================
        public void SalvarFornecedor(Fornecedor f)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            if (f.Id == 0)
                cmd.CommandText = @"INSERT INTO Fornecedores (Nome, CnpjCpf, Telefone, Endereco, Numero, Cidade, Estado, Cep, Produtos, Ativo) VALUES ($nome, $cnpj, $tel, $end, $num, $cid, $est, $cep, $prod, $ativo)";
            else
            {
                cmd.CommandText = @"UPDATE Fornecedores SET Nome=$nome, CnpjCpf=$cnpj, Telefone=$tel, Endereco=$end, Numero=$num, Cidade=$cid, Estado=$est, Cep=$cep, Produtos=$prod, Ativo=$ativo WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", f.Id);
            }
            cmd.Parameters.AddWithValue("$nome", f.Nome);
            cmd.Parameters.AddWithValue("$cnpj", f.CnpjCpf);
            cmd.Parameters.AddWithValue("$tel", f.Telefone);
            cmd.Parameters.AddWithValue("$end", f.Endereco);
            cmd.Parameters.AddWithValue("$num", f.Numero);
            cmd.Parameters.AddWithValue("$cid", f.Cidade);
            cmd.Parameters.AddWithValue("$est", f.Estado);
            cmd.Parameters.AddWithValue("$cep", f.Cep);
            cmd.Parameters.AddWithValue("$prod", f.Produtos);
            cmd.Parameters.AddWithValue("$ativo", f.Ativo ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void ExcluirFornecedor(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Fornecedores WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Fornecedor> ListarFornecedores()
        {
            var lista = new List<Fornecedor>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Fornecedores ORDER BY Nome";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                int ord(string col) => r.GetOrdinal(col);
                lista.Add(new Fornecedor
                {
                    Id = r.GetInt32(ord("Id")),
                    Nome = r.IsDBNull(ord("Nome")) ? "" : r.GetString(ord("Nome")),
                    CnpjCpf = r.IsDBNull(ord("CnpjCpf")) ? "" : r.GetString(ord("CnpjCpf")),
                    Telefone = r.IsDBNull(ord("Telefone")) ? "" : r.GetString(ord("Telefone")),
                    Endereco = r.IsDBNull(ord("Endereco")) ? "" : r.GetString(ord("Endereco")),
                    Numero = r.IsDBNull(ord("Numero")) ? "" : r.GetString(ord("Numero")),
                    Cidade = r.IsDBNull(ord("Cidade")) ? "" : r.GetString(ord("Cidade")),
                    Estado = r.IsDBNull(ord("Estado")) ? "" : r.GetString(ord("Estado")),
                    Cep = r.IsDBNull(ord("Cep")) ? "" : r.GetString(ord("Cep")),
                    Produtos = r.IsDBNull(ord("Produtos")) ? "" : r.GetString(ord("Produtos")),
                    Ativo = r.GetInt32(ord("Ativo")) == 1
                });
            }
            return lista;
        }

        // ==================== VENDAS ====================

        /// <summary>
        /// Busca vendas ativas (não estornadas) por período e filtro de cliente/nº cupom.
        /// Usada tanto pelo FormEstorno quanto pelo FormRelatorio.
        /// </summary>
        public List<VendaResumo> BuscarVendasParaEstorno(string filtro, DateTime de, DateTime ate)
        {
            var lista = new List<VendaResumo>();
            using var con = new SqliteConnection(_conexao);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT v.Id, v.DataHora, v.ClienteNome, v.FormaPagamento,
                       COALESCE(SUM(i.Quantidade * i.PrecoUnitario), 0) AS Total
                FROM Vendas v
                LEFT JOIN ItensVenda i ON i.VendaId = v.Id
                WHERE v.FormaPagamento NOT LIKE 'ESTORNADA%'
                  AND date(v.DataHora) >= $de
                  AND date(v.DataHora) <= $ate
                  AND (
                      v.ClienteNome LIKE $filtro
                      OR CAST(v.Id AS TEXT) LIKE $filtro
                  )
                GROUP BY v.Id
                ORDER BY v.DataHora DESC";

            cmd.Parameters.AddWithValue("$de", de.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$ate", ate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$filtro", string.IsNullOrEmpty(filtro) ? "%" : $"%{filtro}%");

            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new VendaResumo
                {
                    Id = r.GetInt32(0),
                    DataHora = DateTime.Parse(r.GetString(1)),
                    ClienteNome = r.IsDBNull(2) ? "" : r.GetString(2),
                    FormaPagamento = r.IsDBNull(3) ? "" : r.GetString(3),
                    Total = (decimal)r.GetDouble(4)
                });

            return lista;
        }

        /// <summary>
        /// Busca vendas que já foram estornadas no período informado.
        /// Usada pelo FormRelatorio (aba Estornos).
        /// O motivo do estorno fica em FormaPagamento no formato "ESTORNADA:motivo".
        /// </summary>
        public List<VendaResumo> BuscarVendasEstornadas(DateTime de, DateTime ate)
        {
            var lista = new List<VendaResumo>();
            using var con = new SqliteConnection(_conexao);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
                SELECT v.Id, v.DataHora, v.ClienteNome, v.FormaPagamento,
                       COALESCE(SUM(i.Quantidade * i.PrecoUnitario), 0) AS Total
                FROM Vendas v
                LEFT JOIN ItensVenda i ON i.VendaId = v.Id
                WHERE v.FormaPagamento LIKE 'ESTORNADA%'
                  AND date(v.DataHora) >= $de
                  AND date(v.DataHora) <= $ate
                GROUP BY v.Id
                ORDER BY v.DataHora DESC";

            cmd.Parameters.AddWithValue("$de", de.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$ate", ate.ToString("yyyy-MM-dd"));

            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new VendaResumo
                {
                    Id = r.GetInt32(0),
                    DataHora = DateTime.Parse(r.GetString(1)),
                    ClienteNome = r.IsDBNull(2) ? "" : r.GetString(2),
                    FormaPagamento = r.IsDBNull(3) ? "" : r.GetString(3), // "ESTORNADA:motivo"
                    Total = (decimal)r.GetDouble(4)
                });

            return lista;
        }

        /// <summary>
        /// Busca uma venda completa com todos os seus itens pelo Id.
        /// </summary>
        public Venda? BuscarVendaComItens(int vendaId)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM Vendas WHERE Id = $id LIMIT 1";
            cmd.Parameters.AddWithValue("$id", vendaId);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var venda = new Venda
            {
                Id = r.GetInt32(0),
                DataHora = DateTime.Parse(r.GetString(1)),
                ClienteId = r.IsDBNull(2) ? null : r.GetInt32(2),
                ClienteNome = r.IsDBNull(3) ? "" : r.GetString(3),
                FormaPagamento = Enum.Parse<FormaPagamento>(r.GetString(4))
            };
            r.Close();

            var ci = con.CreateCommand();
            ci.CommandText = "SELECT * FROM ItensVenda WHERE VendaId = $vid";
            ci.Parameters.AddWithValue("$vid", vendaId);
            using var ri = ci.ExecuteReader();
            while (ri.Read())
                venda.Itens.Add(new ItemVenda
                {
                    Id = ri.GetInt32(0),
                    ProdutoId = ri.GetInt32(2),
                    ProdutoNome = ri.GetString(3),
                    Quantidade = (decimal)ri.GetDouble(4),
                    PrecoUnitario = (decimal)ri.GetDouble(5)
                });

            return venda;
        }

        /// <summary>
        /// Estorna uma venda: devolve estoque, reverte fiado se houver, marca como estornada.
        /// Funciona para qualquer venda independente da data.
        /// </summary>
        public void EstornarVenda(int vendaId, string motivo)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();

            // busca itens para devolver ao estoque
            var ci = con.CreateCommand();
            ci.CommandText = "SELECT ProdutoId, Quantidade FROM ItensVenda WHERE VendaId = $vid";
            ci.Parameters.AddWithValue("$vid", vendaId);
            using var ri = ci.ExecuteReader();
            var itens = new List<(int ProdutoId, decimal Qtd)>();
            while (ri.Read())
                itens.Add((ri.GetInt32(0), (decimal)ri.GetDouble(1)));
            ri.Close();

            // devolve ao estoque
            foreach (var (prodId, qtd) in itens)
            {
                var ce = con.CreateCommand();
                ce.CommandText = "UPDATE Produtos SET Estoque = Estoque + $qtd WHERE Id = $pid";
                ce.Parameters.AddWithValue("$qtd", qtd);
                ce.Parameters.AddWithValue("$pid", prodId);
                ce.ExecuteNonQuery();
            }

            // reverte fiado se a venda era caderneta
            var cf = con.CreateCommand();
            cf.CommandText = "SELECT ClienteId, FormaPagamento FROM Vendas WHERE Id = $id";
            cf.Parameters.AddWithValue("$id", vendaId);
            using var rf = cf.ExecuteReader();
            if (rf.Read() && !rf.IsDBNull(0))
            {
                int clienteId = rf.GetInt32(0);
                string forma = rf.GetString(1);
                rf.Close();

                if (forma == "Fiado")
                {
                    var ct = con.CreateCommand();
                    ct.CommandText = "SELECT COALESCE(SUM(Quantidade * PrecoUnitario), 0) FROM ItensVenda WHERE VendaId = $vid";
                    ct.Parameters.AddWithValue("$vid", vendaId);
                    decimal total = (decimal)Convert.ToDouble(ct.ExecuteScalar()!);

                    var cu = con.CreateCommand();
                    cu.CommandText = "UPDATE Clientes SET SaldoFiado = SaldoFiado - $val WHERE Id = $id";
                    cu.Parameters.AddWithValue("$val", total);
                    cu.Parameters.AddWithValue("$id", clienteId);
                    cu.ExecuteNonQuery();
                }
            }
            else rf.Close();

            // marca venda como estornada — motivo gravado no campo FormaPagamento
            var cm = con.CreateCommand();
            cm.CommandText = "UPDATE Vendas SET FormaPagamento = $forma WHERE Id = $id";
            cm.Parameters.AddWithValue("$forma", $"ESTORNADA:{motivo}");
            cm.Parameters.AddWithValue("$id", vendaId);
            cm.ExecuteNonQuery();
        }

        /// <summary>
        /// Retorna o Id da última venda não estornada.
        /// </summary>
        public int? UltimaVendaId()
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id FROM Vendas WHERE FormaPagamento NOT LIKE 'ESTORNADA%' ORDER BY Id DESC LIMIT 1";
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? null : (int)(long)result;
        }

        public void SalvarVenda(Venda v)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO Vendas (DataHora, ClienteId, ClienteNome, FormaPagamento) VALUES ($dt, $cliId, $cliNome, $forma)";
            cmd.Parameters.AddWithValue("$dt", v.DataHora.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$cliId", v.ClienteId.HasValue ? v.ClienteId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("$cliNome", v.ClienteNome);
            cmd.Parameters.AddWithValue("$forma", v.FormaPagamento.ToString());
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT last_insert_rowid()";
            cmd.Parameters.Clear();
            int vendaId = (int)(long)cmd.ExecuteScalar()!;
            foreach (var item in v.Itens)
            {
                var ci = con.CreateCommand();
                ci.CommandText = @"INSERT INTO ItensVenda (VendaId, ProdutoId, ProdutoNome, Quantidade, PrecoUnitario) VALUES ($vid, $pid, $pnome, $qtd, $preco)";
                ci.Parameters.AddWithValue("$vid", vendaId);
                ci.Parameters.AddWithValue("$pid", item.ProdutoId);
                ci.Parameters.AddWithValue("$pnome", item.ProdutoNome);
                ci.Parameters.AddWithValue("$qtd", item.Quantidade);
                ci.Parameters.AddWithValue("$preco", item.PrecoUnitario);
                ci.ExecuteNonQuery();
                var ce = con.CreateCommand();
                ce.CommandText = "UPDATE Produtos SET Estoque = Estoque - $qtd WHERE Id = $pid";
                ce.Parameters.AddWithValue("$qtd", item.Quantidade);
                ce.Parameters.AddWithValue("$pid", item.ProdutoId);
                ce.ExecuteNonQuery();
            }
        }

        public List<Venda> ListarVendasDoMes()
        {
            var lista = new List<Venda>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            string mesAtual = DateTime.Now.ToString("yyyy-MM");
            cmd.CommandText = "SELECT * FROM Vendas WHERE DataHora LIKE $mes ORDER BY DataHora DESC";
            cmd.Parameters.AddWithValue("$mes", $"{mesAtual}%");
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Venda
                {
                    Id = r.GetInt32(0),
                    DataHora = DateTime.Parse(r.GetString(1)),
                    ClienteId = r.IsDBNull(2) ? null : r.GetInt32(2),
                    ClienteNome = r.IsDBNull(3) ? "" : r.GetString(3),
                    FormaPagamento = Enum.Parse<FormaPagamento>(r.GetString(4))
                });
            return lista;
        }

        // ==================== DESPESAS ====================
        public void SalvarDespesa(Despesa d)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO Despesas (Descricao, Valor, Data, Categoria, Vencimento, Situacao) VALUES ($desc, $val, $data, $cat, $venc, 'Pendente')";
            cmd.Parameters.AddWithValue("$desc", d.Descricao);
            cmd.Parameters.AddWithValue("$val", (double)d.Valor);
            cmd.Parameters.AddWithValue("$data", d.Data.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$cat", d.Categoria);
            cmd.Parameters.AddWithValue("$venc", d.Vencimento.HasValue ? d.Vencimento.Value.ToString("yyyy-MM-dd") : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void ExcluirDespesa(int id)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM Despesas WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public void DarBaixaDespesa(int id, DateTime dataPagamento, string formaPagamento)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"UPDATE Despesas SET Situacao = 'Quitado', DataPagamento = $dtPgto, FormaPagamentoBaixa = $forma WHERE Id = $id";
            cmd.Parameters.AddWithValue("$dtPgto", dataPagamento.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$forma", formaPagamento);
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Despesa> ListarDespesasDoMes()
        {
            var lista = new List<Despesa>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            string mesAtual = DateTime.Now.ToString("yyyy-MM");
            cmd.CommandText = "SELECT * FROM Despesas WHERE Data LIKE $mes ORDER BY Data DESC";
            cmd.Parameters.AddWithValue("$mes", $"{mesAtual}%");
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(LerDespesa(r));
            return lista;
        }

        public List<Despesa> ListarDespesasFiltradas(DateTime? de, DateTime? ate, string? cat, string? sit, string? busca, bool parc)
        {
            var lista = new List<Despesa>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            var where = new StringBuilder("WHERE 1=1");
            if (de.HasValue) { where.Append(" AND Data >= $de"); cmd.Parameters.AddWithValue("$de", de.Value.ToString("yyyy-MM-dd")); }
            if (ate.HasValue) { where.Append(" AND Data <= $ate"); cmd.Parameters.AddWithValue("$ate", ate.Value.ToString("yyyy-MM-dd")); }
            if (!string.IsNullOrWhiteSpace(cat)) { where.Append(" AND Categoria = $cat"); cmd.Parameters.AddWithValue("$cat", cat); }
            if (sit == "Vencido")
            {
                where.Append(" AND Situacao = 'Pendente' AND Vencimento IS NOT NULL AND Vencimento < $hoje");
                cmd.Parameters.AddWithValue("$hoje", DateTime.Today.ToString("yyyy-MM-dd"));
            }
            else if (!string.IsNullOrWhiteSpace(sit)) { where.Append(" AND Situacao = $sit"); cmd.Parameters.AddWithValue("$sit", sit); }
            if (!string.IsNullOrWhiteSpace(busca)) { where.Append(" AND Descricao LIKE $busca"); cmd.Parameters.AddWithValue("$busca", $"%{busca}%"); }
            if (parc) where.Append(" AND Descricao LIKE '%(%/%)'");
            cmd.CommandText = $"SELECT * FROM Despesas {where} ORDER BY Data DESC, Id DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var d = LerDespesa(r);
                if (d.Situacao == "Pendente" && d.Vencimento.HasValue && d.Vencimento.Value.Date < DateTime.Today)
                    d.Situacao = "Vencido";
                lista.Add(d);
            }
            return lista;
        }

        private Despesa LerDespesa(SqliteDataReader r)
        {
            var d = new Despesa
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Descricao = r.IsDBNull(r.GetOrdinal("Descricao")) ? "" : r.GetString(r.GetOrdinal("Descricao")),
                Valor = (decimal)r.GetDouble(r.GetOrdinal("Valor")),
                Data = DateTime.Parse(r.GetString(r.GetOrdinal("Data"))),
                Categoria = r.IsDBNull(r.GetOrdinal("Categoria")) ? "Geral" : r.GetString(r.GetOrdinal("Categoria")),
            };
            try { int i = r.GetOrdinal("Vencimento"); d.Vencimento = r.IsDBNull(i) ? null : DateTime.Parse(r.GetString(i)); } catch { }
            try { int i = r.GetOrdinal("Situacao"); d.Situacao = r.IsDBNull(i) ? "Pendente" : r.GetString(i); } catch { d.Situacao = "Pendente"; }
            try { int i = r.GetOrdinal("DataPagamento"); d.DataPagamento = r.IsDBNull(i) ? null : DateTime.Parse(r.GetString(i)); } catch { }
            try { int i = r.GetOrdinal("FormaPagamentoBaixa"); d.FormaPagamentoBaixa = r.IsDBNull(i) ? null : r.GetString(i); } catch { }
            return d;
        }

        // ==================== CAIXA / TURNO ====================
        public TurnoCaixa? ObterTurnoAberto()
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM TurnosCaixa WHERE Status = 'Aberto' LIMIT 1";
            using var r = cmd.ExecuteReader();
            return r.Read() ? LerTurno(r) : null;
        }

        public int AbrirTurno(decimal trocoInicial)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO TurnosCaixa (DataAbertura, TrocoInicial, Status) VALUES ($dt, $troco, 'Aberto')";
            cmd.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$troco", trocoInicial);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT last_insert_rowid()";
            cmd.Parameters.Clear();
            return (int)(long)cmd.ExecuteScalar()!;
        }

        public void FecharTurno(int turnoId, decimal valorContado)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();

            cmd.CommandText = "SELECT COALESCE(SUM(iv.Quantidade * iv.PrecoUnitario), 0.0) FROM Vendas v JOIN ItensVenda iv ON iv.VendaId = v.Id WHERE v.DataHora >= (SELECT DataAbertura FROM TurnosCaixa WHERE Id = $id)";
            cmd.Parameters.AddWithValue("$id", turnoId);
            decimal totalVendas = (decimal)Convert.ToDouble(cmd.ExecuteScalar()!);

            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(SUM(Valor), 0.0) FROM MovimentacoesCaixa WHERE TurnoId = $id AND Tipo = 'Sangria'";
            cmd.Parameters.AddWithValue("$id", turnoId);
            decimal totalSangrias = (decimal)Convert.ToDouble(cmd.ExecuteScalar()!);

            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(SUM(Valor), 0.0) FROM MovimentacoesCaixa WHERE TurnoId = $id AND Tipo = 'Reforço'";
            cmd.Parameters.AddWithValue("$id", turnoId);
            decimal totalReforcos = (decimal)Convert.ToDouble(cmd.ExecuteScalar()!);

            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT TrocoInicial FROM TurnosCaixa WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", turnoId);
            decimal trocoInicial = (decimal)Convert.ToDouble(cmd.ExecuteScalar()!);

            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT COALESCE(SUM(iv.Quantidade * iv.PrecoUnitario), 0.0) FROM Vendas v JOIN ItensVenda iv ON iv.VendaId = v.Id WHERE v.DataHora >= (SELECT DataAbertura FROM TurnosCaixa WHERE Id = $id) AND v.FormaPagamento = 'Dinheiro'";
            cmd.Parameters.AddWithValue("$id", turnoId);
            decimal vendasDinheiro = (decimal)Convert.ToDouble(cmd.ExecuteScalar()!);

            decimal saldoEsperado = trocoInicial + vendasDinheiro + totalReforcos - totalSangrias;
            decimal diferenca = valorContado - saldoEsperado;

            cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE TurnosCaixa SET DataFechamento = $dt, TotalVendas = $tv, TotalSangrias = $ts, ValorContado = $vc, Diferenca = $dif, Status = 'Fechado' WHERE Id = $id";
            cmd.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$tv", (double)totalVendas);
            cmd.Parameters.AddWithValue("$ts", (double)totalSangrias);
            cmd.Parameters.AddWithValue("$vc", (double)valorContado);
            cmd.Parameters.AddWithValue("$dif", (double)diferenca);
            cmd.Parameters.AddWithValue("$id", turnoId);
            cmd.ExecuteNonQuery();
        }

        public void RegistrarMovimentacao(int turnoId, string tipo, decimal valor, string motivo)
        {
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO MovimentacoesCaixa (TurnoId, Tipo, Valor, Motivo, DataHora) VALUES ($tid, $tipo, $val, $mot, $dt)";
            cmd.Parameters.AddWithValue("$tid", turnoId);
            cmd.Parameters.AddWithValue("$tipo", tipo);
            cmd.Parameters.AddWithValue("$val", valor);
            cmd.Parameters.AddWithValue("$mot", motivo);
            cmd.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }

        public List<MovimentacaoCaixa> ListarMovimentacoes(int turnoId)
        {
            var lista = new List<MovimentacaoCaixa>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM MovimentacoesCaixa WHERE TurnoId = $id ORDER BY DataHora DESC";
            cmd.Parameters.AddWithValue("$id", turnoId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new MovimentacaoCaixa
                {
                    Id = r.GetInt32(0),
                    TurnoId = r.GetInt32(1),
                    Tipo = r.GetString(2),
                    Valor = (decimal)r.GetDouble(3),
                    Motivo = r.IsDBNull(4) ? "" : r.GetString(4),
                    DataHora = DateTime.Parse(r.GetString(5))
                });
            return lista;
        }

        public List<TurnoCaixa> ListarTurnosRecentes()
        {
            var lista = new List<TurnoCaixa>();
            using var con = new SqliteConnection(_conexao);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM TurnosCaixa ORDER BY Id DESC LIMIT 30";
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(LerTurno(r));
            return lista;
        }

        private TurnoCaixa LerTurno(SqliteDataReader r) => new TurnoCaixa
        {
            Id = r.GetInt32(0),
            DataAbertura = DateTime.Parse(r.GetString(1)),
            DataFechamento = r.IsDBNull(2) ? null : DateTime.Parse(r.GetString(2)),
            TrocoInicial = (decimal)r.GetDouble(3),
            TotalVendas = (decimal)r.GetDouble(4),
            TotalSangrias = (decimal)r.GetDouble(5),
            ValorContado = (decimal)r.GetDouble(6),
            Diferenca = (decimal)r.GetDouble(7),
            Status = r.GetString(8)
        };
    }
}