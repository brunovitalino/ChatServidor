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
        private int i = 0;

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

        public int I
        {
            get { return i; }
            set { i = value; }
        }


        //CONSTRUTOR

        public CServidor()
        {
        }


        //EVENTOS - O evento e o seu argumento irá notificar o formulário quando um usuário se conecta, desconecta, envia uma mensagem,etc

        public static event StatusChangedEventHandler StatusChanged;


        //MÉTODOS

        // Este método é chamado quando o evento StatusChanged ocorre.
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            /*if (StatusChanged != null)
                StatusChanged(this, e);*/


            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e); // invoca o delegate
            }
        }

        public void Conectar(string ip, bool conectar)
        {
            Ip = ip;

            if (conectar)
            {
                // Inicia uma nova tread que hospeda o listener
                Thread ThreadConectar = new Thread(RunConectar);
                ThreadConectar.Start();
                Conectado = true;
            }
            else
            {
                Conectado = false;
            }

            //
            Console.WriteLine("thread1 in");
            Thread t = new Thread(RunDelay);
            t.Start();
            t.Join();
            Console.WriteLine("thread1 out");
        }

        private void RunConectar()
        {
            Console.WriteLine("thread2 in");
            Thread ThreadAguardarNovosClientes = new Thread(RunAguardarNovosClientes);
            ThreadAguardarNovosClientes.Start();
            Console.WriteLine("thread2 out");

            // Foi colocado true para garantir sempre a execução da condição interna do laço.
            // Obs: utilizando a variável Conectado há uma chance em microssegundos de essa checagem não ocorrer.
            /*while (true)
            {
                Thread.Sleep(3000);
                Console.WriteLine(i + " thread Conectado:" + Conectado);
                if (!Conectado)
                {
                    Console.WriteLine("TERMINAR RunConectar()");
                    ThreadAguardarNovosClientes.Abort();
                    break;
                }
                i++;
            }*/
        }

        private void RunAguardarNovosClientes()
        {
            Console.WriteLine("thread3 in");
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
                // Conectado!!
                Conectado = true;

                // Faz a verificação de novas conexões.
                while (true)
                {
                    Thread.Sleep(500);
                    // Essa condição nos auxilia na eliminação de threads existentes após desconectar.
                    if (!Conectado)
                    {
                        Ouvinte.Stop();
                        // Elimina todas as threads de clientes existentes.
                        if (PilhaThreadsClientes.Count > 0)
                        {
                            while (PilhaThreadsClientes.Count > 0)
                            {
                                PilhaThreadsClientes.Dequeue().Abort();
                            }
                        }
                        // Quebra as próximas iterações, saindo do while(true).
                        break;
                    }
                    // Essa condição impede que nosso código seja travado com a tentativa de obter nova conexão do listener.
                    if (Ouvinte.Pending())
                    {
                        // Se o Pending() não fosse usado, o laço ficaria travado aqui, esperando por uma nova conexão.
                        novoClienteSocket = Ouvinte.AcceptTcpClient();
                        // Para evitar outro travamento do programa, criaemos uma thread que gerenciará o novo cliente.
                        ThreadCliente = new Thread(() => RunCliente(novoClienteSocket));
                        ThreadCliente.Start();
                        PilhaThreadsClientes.Enqueue(ThreadCliente);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.WriteLine("thread3 out");
        }

        public void RunCliente(TcpClient clienteSocket)
        {
            Console.WriteLine("thread4 in");
            string usuario = "";
            string resposta = "";
            StreamReader Receptor = new StreamReader(clienteSocket.GetStream());
            StreamWriter Transmissor = new StreamWriter(clienteSocket.GetStream());

            resposta = Receptor.ReadLine();
            EnviarMensagem(resposta.Substring(3), "entrou!");
            /*
            try
            {
                resposta = Receptor.ReadLine();
                // Sempre que um novo cliente se conecta, obrigatóriamente o servidor deve receber o código 01 e reenviar para confirmar a conexão.
                if (resposta.Substring(0, 2).Equals("01"))
                {
                    usuario = resposta.Substring(3).Replace(" ", "");
                    Console.WriteLine("usuario: " + usuario);
                    if (!usuario.Equals(""))
                    {
                        usuario = "Bruno";
                        TabelaClientesSockets.Add(usuario, clienteSocket);
                        if (!TabelaClientesSockets.ContainsKey(usuario))
                        {
                            Console.WriteLine("!TabelaClientesSockets.ContainsKey(" + usuario + ")");
                            TabelaClientesSockets.Add(usuario, clienteSocket);
                            OnStatusChanged(new StatusChangedEventArgs(usuario + " entrou na sala."));
                            //while ((resposta = Receptor.ReadLine()) != "00")
                            //{

                            //}
                        }
                        else
                        {
                            Console.WriteLine("TabelaClientesSockets.ContainsKey(" + usuario + ")");
                            OnStatusChanged(new StatusChangedEventArgs(usuario + " já existe."));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Receptor.Close();
                Transmissor.Close();
                Console.WriteLine("Eliminando thread!!");
                // Vai abortar essa própria thread.
            }*/
            Console.WriteLine("thread4 out");

        }

        // Envia mensagens de um usuário para todos os outros
        public void EnviarMensagem(string origem, string mensagem) //O parâmetro mensagem vai ser tudo que vem após o primeiro pipe da mensagem superior (resposta do cliente)
        {
            //StreamWriter swTransmissor;

            // Primeiro exibe a mensagem na aplicação
            StatusChangedEventArgs E = new StatusChangedEventArgs(origem + " disse : " + mensagem);
            OnStatusChanged(E);
        }

        //
        private void RunDelay()
        {
            Thread.Sleep(1000);
        }
    }
}
