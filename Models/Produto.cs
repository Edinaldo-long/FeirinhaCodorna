namespace FeirinhaCodorna.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; } = "";
        public string CodigoEan { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Unidade { get; set; } = "kg";
        public bool Pesavel { get; set; } = true;
        public decimal Preco { get; set; }
        public decimal Estoque { get; set; }
        public decimal EstoqueMinimo { get; set; } = 5;
        public int FornecedorId { get; set; }
        public bool EstoqueBaixo => Estoque <= EstoqueMinimo;
        public override string ToString() => $"{Nome} ({CodigoInterno})";
        public decimal PrecoCusto { get; set; }
    }
}