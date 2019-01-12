using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatServer
{
    class ClientHandler
    {
        Thread lisThread;
        TcpClient client;
        NetworkStream ns;
        string username;
        string key;

        public delegate void MessagePasser(string msg);
        MessagePasser listeners;
        public delegate void DisconnectListeners(ClientHandler client);
        DisconnectListeners dListeners;

        public ClientHandler(TcpClient client, MessagePasser lis, DisconnectListeners dl)
        {
            listeners += lis;
            dListeners += dl;
            this.client = client;
            ns = client.GetStream();

            //pradedam laukti kitu msg
            lisThread = new Thread(GetMessage);
            lisThread.Start();
        }

        private void GetAuth()
        {
            //kanalas per kuri bus komunikuojama
            byte[] buffer = new byte[1024];
            //nuskaitom info is rysio
            ns.Read(buffer, 0, buffer.Length);

            int usernameLength = BitConverter.ToInt32(buffer, 0);
            username = Encoding.ASCII.GetString(buffer, sizeof(int), usernameLength);

            int publicKeyLength = BitConverter.ToInt32(buffer, sizeof(int) + usernameLength);

            string publicKey = Encoding.ASCII.GetString(buffer, sizeof(int) *2 + usernameLength, publicKeyLength );


            DialogResult result = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // priimti clienta
                key = CryptoClass.RandomString(20);

                string encryptedKey = CryptoClass.RSAencodeString(key, publicKey);

                byte[] encryptedKeyBytes = Encoding.ASCII.GetBytes(encryptedKey);
                ns.Write(encryptedKeyBytes, 0, encryptedKeyBytes.Length);
               // MessageBox.Show("s: "+ key);

            }
            else
            {
                dListeners(this);
                ns.Close();
                client.Close();
            }

        }

        private void GetMessage()
        {
            try
            {
                //gaunam username
                GetAuth();


                //kanalas per kuri bus komunikuojama
                byte[] buffer;

                while (client.Connected)
                {
                    buffer = new byte[1024];
                    //nuskaitom info is rysio
                    ns.Read(buffer, 0, buffer.Length);

                    string msg = Encoding.ASCII.GetString(buffer);

                    if(msg[0] == '\0')
                    {
                        client.Close();
                        break;
                    }

                    listeners( "[" + username + "]: " + msg);
                }
                listeners("[" + username + "]: " + "Client has disconnected");
                dListeners(this);
            }
            catch (Exception ex)
            {
                listeners("[" + username + "]: " + "Client has disconnected");
                dListeners(this);
            }
            
        }

        public void SendMessage(string msg)
        {
            if(key == null)
            {
                return;
            }
            string enMsg = CryptoClass.TDESencodeString(msg, key);
            byte[] endMsg = Encoding.ASCII.GetBytes(enMsg);
            try
            {
                ns.Write(endMsg, 0, endMsg.Length);
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
