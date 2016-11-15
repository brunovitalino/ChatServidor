using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServidor.CONTROLLER
{
    public class StatusChangedEventArgs : EventArgs
    {
        //VARIÁVEIS GLOBAIS

        private string _mensagemEvento; // Estamos interessados na mensagem descrevendo o evento

        public string MensagemEvento    // Propriedade para retornar e definir um mensagem do evento
        {
            get { return _mensagemEvento; }
            set { _mensagemEvento = value; }
        }

        //CONSTRUTOR

        public StatusChangedEventArgs(string msgEvento) // Define a mensagem do evento
        {
            MensagemEvento = msgEvento;
        }
    }
}
