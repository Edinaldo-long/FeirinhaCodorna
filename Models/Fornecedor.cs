namespace FeirinhaCodorna.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string CnpjCpf { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Cep { get; set; } = "";
        public string Produtos { get; set; } = "";
        public bool Ativo { get; set; } = true;
    }
}