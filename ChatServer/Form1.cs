using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int PORT = 5678;

        private void btnStart_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(StartListener);
            th.Start();
            PrintMsg("Server has started on port " + PORT);
            btnStart.Enabled = false;
        }

        List<ClientHandler> clients = new List<ClientHandler>();

        private void StartListener()
        {
            //laukia prisijungimu
            TcpListener tcpListener = new TcpListener(PORT);

            //paleidziam serveri
            tcpListener.Start();

            while (true)
            {
                //kai gauna prisijungima ji issaugo i client. Laukia prisijungimo
                TcpClient client = tcpListener.AcceptTcpClient();

                ClientHandler.MessagePasser messagePasser = PrintMsg;
                ClientHandler.DisconnectListeners disconnect = DisconnectClient;

                clients.Add(new ClientHandler(client, messagePasser, disconnect));
                PrintMsg("Client has connected");
            }
        }

        private void DisconnectClient(ClientHandler client)
        {
            clients.Remove(client);
        }

        delegate void Messenger(string msg);

        private void PrintMsg(string msg)
        {
            if (this.InvokeRequired)
            {
                Messenger function = new Messenger(PrintMsg);
                this.Invoke(function, new object[] { msg });
                return;
            }

            int marker = 0;
            for (int i = 1; i < msg.Length; i++)
            {
                if (msg[i] == '\0')
                {
                    marker = i;
                    break;
                }
            }

            if(msg[marker] == '\0')
            {
                msg = msg.Substring(0, marker);
            }

            rtMain.Text += msg + Environment.NewLine;

            foreach (var client in clients)
            {
                client.SendMessage(msg);
            }

        }
    }
}
