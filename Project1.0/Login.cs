﻿using System;
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
        string readData = null;
        bool check = false;
        bool checkw;
        public Login()
        {
            InitializeComponent();
        }

        bool refromServer = false;
        Dashboard dashboard = new Dashboard();

        private void btn_login_Click(object sender, EventArgs e)
        {
            checkw = true;
            if (box_username.Text != "" && box_pass.Text != "")
            {
                // Gửi thông tin login đến server
                byte[] outStream = Encoding.UTF8.GetBytes("1\n" + box_username.Text + "\n" + box_pass.Text + "\n" + "$");
                connectServer.serverStream.Write(outStream, 0, outStream.Length);
                connectServer.serverStream.Flush();

                // Mở thread để nhận thông tin từ server
                CheckForIllegalCrossThreadCalls = false;
                Thread ctThread = new Thread(getMessage);
                if (check == false)
                {
                    ctThread.Start();
                }
                check = true;

                while (checkw)
                {
                    if (refromServer)
                    {
                        dashboard.username = box_username.Text;
                        dashboard.clientSocket = clientSocket;
                        dashboard.serverStream = connectServer.serverStream;
                        dashboard.Show();
                        this.Hide();
                        #pragma warning disable SYSLIB0006 // Type or member is obsolete
                        ctThread.Interrupt();
                        #pragma warning restore SYSLIB0006 // Type or member is obsolete
                        break;
                    }
                }
            }
        }

        private void getMessage()
        {
            while (true)
            {
                byte[] inStream = new byte[10025];
                connectServer.serverStream.Read(inStream, 0, inStream.Length);
                string returndata = Encoding.UTF8.GetString(inStream).Replace("\0", string.Empty);
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
            SignUp signUp = new SignUp(clientSocket, connectServer.serverStream);
            signUp.Show();
        }
    }
}
