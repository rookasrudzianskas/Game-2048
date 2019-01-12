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

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int PORT = 5678;
        TcpClient client;
        NetworkStream ns;
        string serverKey; 

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();

                client.Connect(txtIP.Text, PORT);
                ns = client.GetStream();

                //sending username
                byte[] username = Encoding.ASCII.GetBytes(txtUsername.Text);
                //ns.Write(username, 0, username.Length);

                //crypto
                CryptoClass.GenerateRSAkeys();
                byte[] publicKeyBytes = Encoding.ASCII.GetBytes( CryptoClass.GetPublicKey());

                byte[] finalPackage = new byte[username.Length + publicKeyBytes.Length + sizeof(int) * 2 ];

                int lastIndex = 0;
                Array.Copy(BitConverter.GetBytes(username.Length), finalPackage, sizeof(int));
                lastIndex += sizeof(int);
                Array.Copy(username, 0, finalPackage, lastIndex, username.Length);
                lastIndex += username.Length;
                Array.Copy(BitConverter.GetBytes(publicKeyBytes.Length), 0, finalPackage, lastIndex, sizeof(int));
                lastIndex += sizeof(int);
                Array.Copy(publicKeyBytes, 0, finalPackage, lastIndex, publicKeyBytes.Length);

                ns.Write(finalPackage, 0, finalPackage.Length);

                byte[] buffer = new byte[1024];
                ns.Read(buffer, 0, buffer.Length);

                string enServerKey = Encoding.ASCII.GetString(buffer);

                int marker = 0;
                for (int i = 1; i < enServerKey.Length; i++)
                {
                    if (enServerKey[i] == '\0')
                    {
                        marker = i;
                        break;
                    }
                }

                if (enServerKey[marker] == '\0')
                {
                    enServerKey = enServerKey.Substring(0, marker);
                }

                serverKey = CryptoClass.RSAdecodeString(enServerKey);

               // MessageBox.Show(serverKey);

                Thread td = new Thread(ClientListener);
                td.Start();

                PrintMsg("Connected to " + txtIP.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                PrintMsg("Failed connect to server: " + txtIP.Text);
            }
            
        }

        private void ClientListener()
        {
            while (true)
            {
                //MessageBox.Show("as:"+serverKey);
                byte[] buffer = new byte[1024];
                ns.Read(buffer, 0, buffer.Length);
                string text = Encoding.ASCII.GetString(buffer);

                int marker = 0;
                for (int i = 1; i < text.Length; i++)
                {
                    if (text[i] == '\0')
                    {
                        marker = i;
                        break;
                    }
                }

                if (text[marker] == '\0')
                {
                    text = text.Substring(0, marker);
                }

                text = CryptoClass.TDESdecodeString(text, serverKey);
                PrintMsg(text);
            }
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

            if (msg[marker] == '\0')
            {
                msg = msg.Substring(0, marker);
            }

            rtMain.Text += msg + Environment.NewLine;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(txtMessage.Text);
                ns.Write(buffer, 0, buffer.Length);
                //PrintMsg(txtMessage.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not send message");
            }
            
        }
    }
}
