using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Project1._0
{
    public partial class Login : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        bool check = false;
        bool checkw = true;
        public Login()
        {
            InitializeComponent();
        }

        bool refromServer = false;
        Dashboard dashboard = new Dashboard();

        private void btn_login_Click(object sender, EventArgs e)
        {
            if (box_username.Text != "" && box_pass.Text != "")
            {
                // Kết nối đến server
                if (check == false)
                {
                    clientSocket.Connect(IPAddress.Parse("127.0.0.1"), 8080);
                    serverStream = clientSocket.GetStream();
                }

                // Gửi thông tin login đến server
                byte[] outStream = Encoding.UTF8.GetBytes("1\n" + box_username.Text + "\n" + box_pass.Text + "\n" + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                // Mở thread để nhận thông tin từ server
                CheckForIllegalCrossThreadCalls = false;
                Thread ctThread = new Thread(getMessage);
                if (check == false)
                {
                    ctThread.Start();
                }
                check = true;

                while(checkw)
                if (refromServer)
                {
                    dashboard.username = box_username.Text;
                    dashboard.clientSocket = clientSocket;
                    dashboard.serverStream = serverStream;
                    dashboard.Show();
                    this.Hide();
#pragma warning disable SYSLIB0006 // Type or member is obsolete
                    ctThread.Abort();
#pragma warning restore SYSLIB0006 // Type or member is obsolete
                        break;
                }
            }
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, inStream.Length);
                string returndata = Encoding.UTF8.GetString(inStream);
                readData = "" + returndata;
                if (readData == "True")
                {
                    refromServer = true;
                }
                if (readData == "False")
                {
                    MessageBox.Show("Sai tai khoang hoac mat khau.");
                    checkw = false;
                }
            }
        }

        private void btn_sigh_Click(object sender, EventArgs e)
        {                   
            this.Hide();
            SignUp signUp = new SignUp(clientSocket, serverStream);
            signUp.Show();
        }
    }
}
