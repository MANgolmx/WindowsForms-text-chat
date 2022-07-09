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

namespace WindowsForms_Text_Chat
{
    public partial class Form1 : Form
    {
        static Socket listenSocket;
        static Form1 mainForm;

        public Form1()
        {
            mainForm = this;
            InitializeComponent();

            IPEndPoint myIP = new IPEndPoint(IPAddress.Parse("192.168.2.123"), 51000);

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(myIP);

            listenSocket.Listen(10);

            TimerCallback tm = new TimerCallback(CheckConnections);
            System.Threading.Timer timer = new System.Threading.Timer(tm, 0, 0, 100);
        }

        public static void CheckConnections(object obj)
        {
            Socket handler = listenSocket.Accept();

            StringBuilder sb = new StringBuilder();
            int bytes = 0;
            byte[] data = new byte[1024];

            do
            {
                bytes = handler.Receive(data);
                sb.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (handler.Available > 0);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

            string message = sb.ToString();

            mainForm.textBoxChat.Invoke(new MethodInvoker(delegate { 
               mainForm.textBoxChat.Text += message + "\r\n";
            }));

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            IPEndPoint recieverIP = new IPEndPoint(IPAddress.Parse(textBoxIP.Text), 51000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            socket.Connect(recieverIP);
            string message = "[" + textBoxName.Text + "] " + textBoxMassage.Text;
            byte[] data = Encoding.Unicode.GetBytes(message);
            socket.Send(data);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            textBoxChat.Text += "[I] " + message + "\r\n";
        }
    }
}
