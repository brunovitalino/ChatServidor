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

        private string _ip = "";
        private bool _conectado = false;
        private Queue<Thread> _pilhaThreadsClientes = new Queue<Thread>();
        private Dictionary<string, TcpClient> _tabelaClientesSockets = new Dictionary<string, TcpClient>();

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

        public CServidor()
        {
        }

        public CServidor(string ip)
        {
            Ip = ip;
        }


        //EVENTOS - O evento e o seu argumento irá notificar o formulário quando um usuário se conecta, desconecta, envia uma mensagem,etc

        public static event StatusChangedEventHandler StatusChanged;


        //MÉTODOS

        // Este método é chamado quando o evento StatusChanged ocorre.
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e); // invoca o delegate
            }
            /*if (StatusChanged != null)
            {
                StatusChanged(null, e); // invoca o delegate
            }*/
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
                IPAddress IpFormatado = IPAddress.Parse(Ip);
                // Cria um objeto TCP listener usando o IP do servidor e porta definidas.
                TcpListener Ouvinte = new TcpListener(IpFormatado, 2502);
                // Inicia o TCP listener e escuta as conexões.
                Ouvinte.Start();
                // Receberá o socket do novo cliente que tenta se conectar.
                TcpClient novoClienteSocket = new TcpClient();
                // Auxiliar para incluir uma nova thread cliente na lista de threads que gerencia todos os clientes.
                Thread ThreadCliente = null;

                // Faz a verificação de novas conexões.
                while (true)
                {
                    Thread.Sleep(500);
                    Console.WriteLine("Conectado: " + Conectado);

                    // Essa condição nos auxilia na eliminação de threads existentes após desconectar.
                    if (!Conectado)
                    {
                        // Quebra as próximas iterações, saindo do while(true).
                        break;
                    }
                    // Essa condição impede que nosso código seja travado com a tentativa de obter nova conexão do listener.
                    if (Ouvinte.Pending())
                    {
                        // Se o Pending() não fosse usado, o laço ficaria travado aqui, esperando por uma nova conexão.
                        novoClienteSocket = Ouvinte.AcceptTcpClient();
                        // Para evitar outro travamento do programa, criaemos uma thread que gerenciará o novo cliente.
                        ThreadCliente = new Thread(() => RunServidor(novoClienteSocket));
                        ThreadCliente.Start();
                        PilhaThreadsClientes.Enqueue(ThreadCliente);
                    }
                }

                Ouvinte.Stop();
                // Elimina todas as threads de clientes existentes.
                while (PilhaThreadsClientes.Count > 0)
                {
                    PilhaThreadsClientes.Dequeue().Abort();
                }
                novoClienteSocket.Close();
                PilhaThreadsClientes = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.WriteLine("thread2 out");
        }

        private void RunServidor(TcpClient clienteSocket)
        {
            Console.WriteLine("thread3 in");
            try
            {
                StreamReader Receptor = new StreamReader(clienteSocket.GetStream());
                string resposta = "";
                string usuario = "";
                StreamWriter Transmissor = new StreamWriter(clienteSocket.GetStream());

                resposta = Receptor.ReadLine().Replace(" ", ""); // Foi usado Replace só para garantir que não há espaços vazios.
                // Sempre que um novo cliente se conecta, obrigatóriamente o servidor deve receber o código 01 e reenviar para confirmar a conexão.
                if (resposta.Substring(0, 2).Equals("01"))
                {
                    // O que sempre virá após o "01|" (código+pipe) será o nome de usuário.
                    usuario = resposta.Substring(3);

                    if (!usuario.Equals(""))
                    {
                        OnStatusChanged(new StatusChangedEventArgs(usuario + " entrou."));
                    }
                    /*
                    usuario = resposta.Substring(3);
                    Console.WriteLine("usuario: " + usuario);
                    if (!usuario.Equals(""))
                    {
                        TabelaClientesSockets.Add(usuario, clienteSocket);
                        if (!TabelaClientesSockets.ContainsKey(usuario))
                        {
                            //Console.WriteLine("!TabelaClientesSockets.ContainsKey(" + usuario + ")");
                            //TabelaClientesSockets.Add(usuario, clienteSocket);
                            OnStatusChanged(new StatusChangedEventArgs(usuario + " entrou na sala."));
                            //while ((resposta = Receptor.ReadLine()) != "00")
                            //{

                            //}
                        }
                        else
                        {
                            //Console.WriteLine("TabelaClientesSockets.ContainsKey(" + usuario + ")");
                            //OnStatusChanged(new StatusChangedEventArgs(usuario + " já existe."));
                        }
                    }*/
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
        private void EnviarMensagem(string origem, string mensagem) //O parâmetro mensagem vai ser tudo que vem após o primeiro pipe da mensagem superior (resposta do cliente)
        {
            //StreamWriter swTransmissor;

            // Primeiro exibe a mensagem na aplicação
            StatusChangedEventArgs E = new StatusChangedEventArgs(origem + " disse : " + mensagem);
            OnStatusChanged(E);
        }
    }
}
