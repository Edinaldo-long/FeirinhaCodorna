namespace FeirinhaCodorna.Models
{
    public class TurnoCaixa
    {
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal TrocoInicial { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal TotalSangrias { get; set; }
        public decimal ValorContado { get; set; }
        public decimal Diferenca { get; set; }
        public string Status { get; set; } = "Aberto";

        // Calculado — não vem do banco
        public decimal SaldoEsperado => TrocoInicial + TotalVendas - TotalSangrias;
    }

    public class MovimentacaoCaixa
    {
        public int Id { get; set; }
        public int TurnoId { get; set; }
        public string Tipo { get; set; } = "";   // "Sangria" | "Reforço" | "Abertura"
        public decimal Valor { get; set; }
        public string Motivo { get; set; } = "";
        public DateTime DataHora { get; set; }
    }
}