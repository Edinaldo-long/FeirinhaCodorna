using System.Security.Cryptography;
using System.Text;

namespace FeirinhaCodorna.Utils
{
    public static class Seguranca
    {
        public static string GerarHash(string senha)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();

                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        public static bool VerificarSenha(string senhaDigitada, string hashBanco)
        {
            return GerarHash(senhaDigitada) == hashBanco;
        }
    }
}