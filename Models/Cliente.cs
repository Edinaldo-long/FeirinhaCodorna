namespace FeirinhaCodorna.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Celular { get; set; } = "";
        public string WhatsApp { get; set; } = "";
        public string Cpf { get; set; } = "";
        public string Rg { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Numero { get; set; } = "";

        public string Bairro { get; set; } = "";

        public string Complemento { get; set; } = "";
        public string AutorizadoCaderneta { get; set; } = "";
        public decimal LimiteFiado { get; set; } = 100;
        public decimal SaldoFiado { get; set; } = 0;
        public bool PodeFiar => SaldoFiado < LimiteFiado;
        public decimal SaldoDisponivel => LimiteFiado - SaldoFiado;
        public override string ToString() => $"{Nome} (#{Codigo})";
    }
}