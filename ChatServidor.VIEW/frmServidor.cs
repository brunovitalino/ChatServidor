using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using ChatServidor.CONTROLLER;
using System.Threading;

namespace ChatServidor.VIEW
{
    // DELEGATE

    public delegate void AtualizaLogCallback(string strMensagem);


    // CLASSE

    public partial class frmServidor : Form
    {
        //VARIÁVEIS GLOBAIS

        private bool _conectado = false;
        private CServidor _servidor;

        public bool Conectado
        {
            get { return _conectado; }
            set { _conectado = value; }
        }

        public CServidor Servidor
        {
            get { return _servidor; }
            set { _servidor = value; }
        }


        //CONSTRUTOR

        public frmServidor()
        {
            InitializeComponent();
        }


        //EVENTOS

        private void frmServidor_Load(object sender, EventArgs e)
        {
            //mskIp.ValidatingType = typeof(System.Net.IPAddress);
        }

        private void mskIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Conectar();
            }
        }

        //Isso fará com que o "cursor" (ponto de inserção) se mova para a lacuna seguinte, que se inicia após o próximo ponto.
        //Cada vez que a tecla . (ponto) do teclado é apertada, esse salto será realizado.
        private void mskIp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('.'))
            {
                for (int i = mskIp.SelectionStart; i < mskIp.MaskedTextProvider.Length; i++)
                {
                    if (!mskIp.MaskedTextProvider.IsEditPosition(i))
                    {
                        mskIp.SelectionStart = i;
                        // Após realizado o salto, não queremos que continue pelos campos seguintes.
                        // Então cancelamos as próximas iterações.
                        break;
                    }
                }
            }
        }

        private void mskIp_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            toolTip1.ToolTipTitle = "Entrada inválida";
            toolTip1.Show("Desculpe, somente digitos (0-9) são permitidos.", mskIp, mskIp.Location, 3000);
        }

        private void mskIp_MouseHover(object sender, EventArgs e)
        {
            // Caso o evento MaskInputRejected tenha sido disparado, o toolTip1 voltará ao padrão com o código abaixo.
            toolTip1.ToolTipTitle = "";
        }

        /*private void mskIp_Leave(object sender, EventArgs e)
        {
            // Reseta o cursor quando deixamos o maskedtextbox
            mskIp.SelectionStart = 0;
            // Habilita a propriedade TabStop assim nós podemos circular através dos form controls novamente  
            foreach (Control c in this.Controls)
            {
                c.TabStop = true;
            }
        }*/

        private void btnConectar_Click(object sender, EventArgs e)
        {
            Conectar();
        }

        public void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.Invoke(new AtualizaLogCallback(this.AtualizaLog), new object[] { e.MensagemEvento }); // Chama o método que atualiza o formulário
        }

        private void frmServidor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Conectado)
            {
                Servidor.Conectar(false);
            }
            Application.Exit();
        }


        // MÉTODOS

        //Validação MaskedTextBox do mskIp
        //Se todas as lacunas do campo possui ao menos 1 posição preenchida cada, então o maskedtextbox estará validado.
        private bool isMascaraValidada()
        {
            // À cada 3 posições editáveis, significa que uma lacuna entre os pontos foi validada.
            int contador = 0;
            // Verifica se ao menos 1 posição, da lacuna que está sendo verificada, foi preenchida.
            bool espacoPreenchido = false;

            for (int i = 0; i < mskIp.MaskedTextProvider.Length; i++)
            {
                // Com essa condição, toda a lógica será baseada sobre os campos editáveis apenas.
                if (mskIp.MaskedTextProvider.IsEditPosition(i))
                {
                    // Atribuindo no inicio, fará com que o contador na sua primeira iteração já inicie com 1.
                    contador++;

                    // Verifica se esse espaço da lacuna não está vazio.
                    if (!mskIp.GetCharFromPosition(mskIp.GetPositionFromCharIndex(i)).Equals('_'))  // Temos que usar underline, pois a propriedade HidePrompOnLeave que torna os underlines em espaços em branco está quebrada.
                    {                                                                               // Pois fica preenchendo as últimas posições (que vêm após a última noEditPosition) com o dígito da primeira posição.
                        espacoPreenchido = true;
                    }

                    // Então verifique se algo foi digitado na lacuna.
                    if (contador >= 3)
                    {
                        // Se não houve nenhum espaço da lacuna preenchido, então:
                        if (!espacoPreenchido)
                        {
                            toolTip1.ToolTipTitle = "Endereço IP inválido";
                            toolTip1.Show("Desculpe, mas o endereço digitado não é válido. Digite um endereço válido.", mskIp, 2000);
                            return false;
                        }
                        else
                        {
                            // Retornamos as variáveis aos valores padrões para realizarmos novas verificações.
                            contador = 0;
                            espacoPreenchido = false; //Como só é alterado por último, então não está disparando o ERRO acima que vem antes. Pois a iteração acaba PRIMEIRO
                        }
                    }
                }
            }

            // Os '9's seriam substituídos por espaços em branco. Mas é inútil, pois com a propriedade HidePromptOnLeave desativada os underlines continuam aparecendo.
            /*for (int i = 0; i < mskIp.MaskedTextProvider.Length; i++)
            {
                if (mskIp.MaskedTextProvider.IsEditPosition(i))
                {
                    if (mskIp.GetCharFromPosition(mskIp.GetPositionFromCharIndex(i)).Equals('_'))
                    {
                        if (mskIp.Text.Length >= (i + 1))
                        {
                            if (i == 0)
                            {
                                mskIp.Text = '9' + mskIp.Text.Substring(i + 1);
                            }
                            else if (i < (mskIp.MaskedTextProvider.EditPositionCount - 1))
                            {
                                mskIp.Text = mskIp.Text.Substring(0, i) + '9' + mskIp.Text.Substring(i + 1);
                            }
                            else
                            {
                                mskIp.Text = mskIp.Text.Substring(0, i) + '9';
                            }
                        }
                        // Para os últimos dígitos, como a propriedade Text ignora os dígitos vazios que se seguem após a última POSÍÇÃO NÃO EDITÁVEL,
                        // então apenas acrescentamos os dígitos que queremos ao final.
                        else
                        {
                            mskIp.Text = mskIp.Text + "9";
                        }
                    }
                }
                Console.WriteLine("|"+mskIp.Text.Substring(11)+"|");
            }*/
            //OTIMIZANDO
            // Os '9's seriam substituídos por espaços em branco. Mas é inútil, pois com a propriedade HidePromptOnLeave desativada os underlines continuam aparecendo.
            /*mskIp.Text = mskIp.Text.Replace(' ', '9');
            if (mskIp.MaskedTextProvider.Length > mskIp.Text.Length)
            {
                for (int i = mskIp.Text.Length; i < mskIp.MaskedTextProvider.Length; i++)
                {
                    mskIp.Text = mskIp.Text + '9';
                }
            }*/

            return true;
        }

        // Método que fará o servidor ficar online.
        private void Conectar()
        {
            if (isMascaraValidada())
            {
                // Tenta se Conectar.
                if (!Conectado)
                {
                    // Conectando...
                    btnConectar.Enabled = false;
                    btnConectar.Text = "Conectando...";
                    btnConectar.BackColor = Color.Silver;
                    Servidor = new CServidor(mskIp.Text.Replace(" ", ""));
                    // Vincula o tratamento de evento StatusChanged à nossoServidor_StatusChanged
                    CServidor.StatusChanged += new StatusChangedEventHandler(OnStatusChanged);                    
                    Servidor.Conectar(true);
                }
                else
                {
                    // Desconectando...
                    btnConectar.Enabled = false;
                    btnConectar.Text = "Desconectando...";
                    btnConectar.BackColor = Color.Silver;
                    CServidor.StatusChanged -= OnStatusChanged;
                    Servidor.Conectar(false);
                    Servidor = null;
                }

                Console.WriteLine("Conectado antes:" + Conectado);
                Thread t = new Thread(RunDelay);
                t.Start();
                t.Join(); //Espera a finalização da thread t para que o restante do algoritmo possa dar prosseguimento.
                // Atualiza o valor de Conectado da camada VIEW.
                if (Servidor != null)
                {
                    Conectado = Servidor.Conectado;
                }
                Console.WriteLine("Conectado dpois:" + Conectado);

                // Resultado da tentativa de conexão.
                if (Servidor!=null && Servidor.Conectado)
                {
                    Conectado = true;
                    btnConectar.Text = "Conectado";
                    btnConectar.ForeColor = Color.Black;
                    btnConectar.BackColor = Color.White;
                    txtLog.AppendText("Servidor online.\r\n");
                }
                else
                {
                    Conectado = false;
                    Servidor = null;
                    btnConectar.Text = "Conectar";
                    btnConectar.ForeColor = Color.White;
                    btnConectar.BackColor = Color.Black;
                    txtLog.AppendText("Servidor offline.\r\n");
                }
                Console.WriteLine("Conectado final:" + Conectado);
                btnConectar.Enabled = true;
                //MessageBox.Show("Erro de conexão : " + e.Message);
            }
        }

        private void AtualizaLog(string strMensagem)
        {
            txtLog.AppendText(strMensagem + "\r\n");
        }

        //
        private static void RunDelay()
        {
            Thread.Sleep(1500);
        }



    }
}
