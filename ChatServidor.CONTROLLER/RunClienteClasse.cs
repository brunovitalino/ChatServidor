using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServidor.CONTROLLER
{
    class RunClienteClasse
    {
        private static string _novoClienteSocket;

        public static string NovoClienteSocket
        {
            get { return RunClienteClasse._novoClienteSocket; }
            set { RunClienteClasse._novoClienteSocket = value; }
        }

        public RunClienteClasse(string novoClienteSocket)
        {
            NovoClienteSocket = novoClienteSocket;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Usuário é: " + NovoClienteSocket);
        }
    }
}
