using System;
using System.Collections.Generic;
using System.Linq;

namespace FeirinhaCodorna.Models
{
    public enum FormaPagamento { Dinheiro, CartaoDebito, CartaoCredito, Pix, Fiado }

    public class Venda
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public int? ClienteId { get; set; }
        public string ClienteNome { get; set; } = "";
        public FormaPagamento FormaPagamento { get; set; }
        public List<ItemVenda> Itens { get; set; } = new();
        public decimal Total => Itens.Sum(i => i.Subtotal);
    }
}