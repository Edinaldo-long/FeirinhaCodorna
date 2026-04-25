namespace FeirinhaCodorna.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = "";
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string Categoria { get; set; } = "Geral";
        public DateTime? Vencimento { get; set; }

        // --- campos novos para baixa ---
        public string Situacao { get; set; } = "Pendente";          // "Pendente" | "Quitado"
        public DateTime? DataPagamento { get; set; }
        public string? FormaPagamentoBaixa { get; set; }
    }
}