using System;

namespace FeirinhaCodorna.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string RG { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Endereço
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;

        // Emergência — Contato 1
        public string ContatoEmergencia { get; set; } = string.Empty;
        public string ParentescoEmergencia { get; set; } = string.Empty;
        public string TelFixoEmergencia { get; set; } = string.Empty;
        public string CelularEmergencia { get; set; } = string.Empty;

        // Emergência — Contato 2
        public string ContatoEmergencia2 { get; set; } = string.Empty;
        public string ParentescoEmergencia2 { get; set; } = string.Empty;
        public string TelFixoEmergencia2 { get; set; } = string.Empty;
        public string CelularEmergencia2 { get; set; } = string.Empty;

        // Dados RH
        public string Funcao { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
    }
}