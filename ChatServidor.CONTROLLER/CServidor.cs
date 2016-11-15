using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ChatServidor.CONTROLLER
{
    // DELEGATE

    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);


    // CLASSE

    public class CServidor
    {
        //VARIÁVEIS GLOBAIS

        private string _ip;
        private bool _conectado;
        private Queue<Thread> _pilhaThreadsClientes;
        private Dictionary<string, TcpClient> _tabelaClientesSockets;
        // Pilha de threads que guarda todas as runs clientes.
        List<Thread> listaThreadsClientes = new List<Thread>();

        public string Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public bool Conectado
        {
            get { return _conectado; }
            set { _conectado = value; }
        }

        public Queue<Thread> PilhaThreadsClientes
        {
            get { return _pilhaThreadsClientes; }
            set { _pilhaThreadsClientes = value; }
        }

        public Dictionary<string, TcpClient> TabelaClientesSockets
        {
            get { return _tabelaClientesSockets; }
            set { _tabelaClientesSockets = value; }
        }


        //CONSTRUTOR

        public CServidor(string ip)
        {
            Ip = ip;
            Conectado = false;
            TabelaClientesSockets = new Dictionary<string, TcpClient>();
        }


        //EVENTOS - O evento e o seu argumento irá notificar o formulário quando um usuário se conecta, desconecta, envia uma mensagem,etc

        public static event StatusChangedEventHandler StatusChanged;


        //MÉTODOS

        // Este método é chamado quando o evento StatusChanged ocorre.
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            if (StatusChanged != null)
            {
                StatusChanged(null, e); // invoca o delegate
            }
        }

        public void Conectar(bool conectar)
        {
            Console.WriteLine("thread1 in");
            if (conectar)
            {
                Conectado = true;
                // Inicia uma nova tread que hospeda o listener
                Thread ThreadConectar = new Thread(RunReceberNovosClientes);
                ThreadConectar.Start();
            }
            else
            {
                Conectado = false;
            }
            Console.WriteLine("thread1 out");
        }

        private void RunReceberNovosClientes()
        {
            Console.WriteLine("thread2 in");
            try
            {
                // Pega o IP do primeiro dispostivo da rede.
                IPAddress ipServidor = IPAddress.Parse(Ip);
                // Cria um objeto TCP listener usando o IP do servidor e porta definidas.
                TcpListener ouvinte = new TcpListener(ipServidor, 2502);
                // Receberá o socket do novo cliente que tenta se conectar.
                TcpClient novoClienteSocket = new TcpClient();
                // Uma runnable que será incluso na pilha de runs clientes.
                Thread threadConectarNovoCliente = null;

                // Inicia o TCP listener, iniciando a escuta por novas conexões.
                ouvinte.Start();

                // Faz a verificação de novas conexões.
                while (true)
                {
                    Thread.Sleep(500);

                    // Essa condição nos auxilia na eliminação de threads existentes após desconectar.
                    if (!Conectado)
                    {
                        // Quebra as próximas iterações, saindo do while(true).
                        break;
                    }
                    // Essa condição impede que nosso código seja travado com a tentativa de obter nova conexão do listener.
                    if (ouvinte.Pending())
                    {
                        // Se o Pending() não fosse usado, o laço ficaria travado aqui, esperando por uma nova conexão.
                        novoClienteSocket = ouvinte.AcceptTcpClient();
                        // Para evitar outro travamento do programa, criaemos uma thread que gerenciará o novo cliente.
                        threadConectarNovoCliente = new Thread(() => RunConectarNovoCliente(novoClienteSocket));
                        threadConectarNovoCliente.Start();
                    }
                }
                // Para o TCP listener, parando a escuta por novas conexões.
                ouvinte.Stop();

                // Elimina todas as threads clientes existentes.
                while (listaThreadsClientes.Count > 0)
                {
                    listaThreadsClientes[listaThreadsClientes.Count - 1].Abort();
                    listaThreadsClientes.RemoveAt(listaThreadsClientes.Count - 1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.WriteLine("thread2 out");
        }

        private void RunConectarNovoCliente(TcpClient novoClienteSocket)
        {
            // Uma runnable que será incluso na pilha de runs clientes.
            Thread threadCliente = new Thread(() => RunClienteConectado(novoClienteSocket));
            threadCliente.Start();
            //listaThreadsClientes.Add(threadCliente);
            //listaThreadsClientes = null;

            // Se dentro de 2,5s não receber resposta, desconecte.
            int tempoLimite = 0;
            Console.WriteLine("COMECOU ESPERA");

            /*while (true)
            {
                if (!Conectado)
                {
                    break;
                }

                //Thread.Sleep(100);
                Thread.Sleep(100);
                tempoLimite += 100;

                if (tempoLimite >= 3000)
                {
                    break;
                }
            }*/

            Console.WriteLine("TERMINO ESPERA");
        }

        private void RunClienteConectado(TcpClient clienteSocket)
        {
            Console.WriteLine("thread3 in");
            try
            {
                StreamReader Receptor = new StreamReader(clienteSocket.GetStream());
                string resposta = "";
                string usuario = "";
                StreamWriter Transmissor = new StreamWriter(clienteSocket.GetStream());
                // Se dentro de 2,5s não receber resposta, desconecte.
                int tempoLimite = 0;

                resposta = Receptor.ReadLine(); // Foi usado Replace só para garantir que não há espaços vazios.
                // Sempre que um novo cliente se conecta, obrigatóriamente o servidor deve receber o código 01 e reenviar para confirmar a conexão.
                if (resposta.Substring(0, 2).Equals("01"))
                {
                    // O que sempre virá após o "01|" (código+pipe) será o nome de usuário.
                    usuario = resposta.Substring(3);

                    if (!usuario.Equals(""))
                    {
                        if (!TabelaClientesSockets.ContainsKey(usuario))
                        {
                            TabelaClientesSockets.Add(usuario, clienteSocket);
                            OnStatusChanged(new StatusChangedEventArgs(usuario + " se conectou."));

                            // Mantém a conexão.
                            while (true)
                            {
                                Thread.Sleep(500);

                                // Código 00: Desconexão.
                                if (!Conectado || resposta.Substring(0, 2).Equals("00"))
                                {
                                    TabelaClientesSockets.Remove(usuario);
                                    OnStatusChanged(new StatusChangedEventArgs(usuario + " se desconectou."));
                                    
                                    Transmissor.WriteLine("00");
                                    Transmissor.Flush();
                                    break;
                                }
                                // Código 10: Mensagem.
                                else if (resposta.Substring(0, 2).Equals("10"))
                                {
                                    EnviarMensagem(usuario + " diz: " + resposta.Substring(3));
                                }
                                else if (resposta.Substring(0, 2).Equals("11"))
                                {
                                    // ~~
                                }
                                else
                                {
                                    Transmissor.WriteLine("01");
                                    Transmissor.Flush();
                                }

                                resposta = Receptor.ReadLine();
                            }
                        }
                        else
                        {
                            OnStatusChanged(new StatusChangedEventArgs("Nova conexão de cliente negada. " + usuario + " já existe."));
                            Transmissor.WriteLine("00|" + usuario + " já existe.");
                            Transmissor.Flush();
                        }
                    }
                    Console.WriteLine("FIMMMMMM");
                }
                Receptor.Close();
                Transmissor.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.WriteLine("thread3 out");
        }

        // Envia mensagens de um usuário para todos os outros
        private void EnviarMensagem(string mensagem) //O parâmetro mensagem vai ser tudo que vem após o primeiro pipe da mensagem superior (resposta do cliente)
        {
            StreamWriter transmissor;

            // Primeiro exibe a mensagem na aplicação
            OnStatusChanged(new StatusChangedEventArgs(mensagem));

            // Envia a mensagem para todos os clientes conectados.
            foreach (KeyValuePair<string, TcpClient> entry in TabelaClientesSockets)
            {
                transmissor = new StreamWriter(entry.Value.GetStream());
                transmissor.WriteLine("10|" + mensagem);
                transmissor.Flush();
            }
        }
    }
}
