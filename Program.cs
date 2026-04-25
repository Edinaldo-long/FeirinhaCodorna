using System;
using System.Windows.Forms;
using FeirinhaCodorna.Data; // Importante para ter acesso ao banco e criar o admin

namespace FeirinhaCodorna
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Splash Screen
            using (FrmSplash splash = new FrmSplash())
            {
                splash.ShowDialog();
            }

            // 2. Login
            using (FrmLogin login = new FrmLogin())
            {
                // Verifica se já existe algum usuário, se não, cria o Admin automaticamente
                // (Isso evita que você fique travado fora do sistema na primeira vez)
                var banco = new BancoDados();
                var usuarios = banco.ListarUsuarios();
                if (usuarios.Count == 0)
                {
                    banco.SalvarUsuario("admin", "123456", "Administrador");
                }

                // Abre a tela de login
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 3. Só abre o Form1 se o login for bem-sucedido
                    Application.Run(new Form1());
                }
            }
        }
    }
}