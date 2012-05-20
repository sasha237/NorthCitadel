using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using NerZul.Core.Utils;

namespace Core.Source.Network
{
    public class TorClient
    {
        private string IP = "";
        private int port = 0;
        private string authCode = "";

        private TcpClient tcpClient;

        public TorClient(string address, string authCode)
        {
            IP = address.Split(':')[0];
            port = 9051; // Convert.ToInt32(address.Split(':')[1]);
            this.authCode = authCode;

            tcpClient = new TcpClient();
        }

        private string SendCommand(string command, bool openConnection, bool closeConnection)
        {
            //ConsoleLog.WriteLine("Tor command send:  " + command);

            try
            {
                if (openConnection)
                    tcpClient.Connect(IP, port);

                Stream tcpStream = tcpClient.GetStream();

                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(command);
                tcpStream.Write(buffer, 0, buffer.Length);
                tcpStream.Flush();

                //ConsoleLog.WriteLine("Tor command sent:  " + command);

                byte[] message = new byte[4096];
                int bytesRead;
                bytesRead = tcpStream.Read(message, 0, 4096);

                if (closeConnection)
                    tcpClient.Close(); 

                //ConsoleLog.WriteLine("Tor response received: " + System.Text.Encoding.ASCII.GetString(message, 0, bytesRead));

                return System.Text.Encoding.ASCII.GetString(message, 0, bytesRead);
            }

            catch (Exception e)
            {
                ConsoleLog.WriteLine("Tor send command failed: " + e.Message);
                return "";
            }
        }

        private bool CheckOK(string response)
        {
            try
            {
                string token = response.Substring(0, response.IndexOf(' '));
                return (Convert.ToInt32(token) == 250);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public bool GetNewIP()
        {
            string responce;

            responce = SendCommand("AUTHENTICATE \"" + authCode + "\"\r\n", true, false);
            if (!CheckOK(responce))
            {
                ConsoleLog.WriteLine("Tor authentication failed: " + responce);
                return false;
            }

            responce = SendCommand("signal NEWNYM" + "\r\n", false, true);
            if (!CheckOK(responce))
            {
                ConsoleLog.WriteLine("Tor new IP getting failed: " + responce);
                return false;
            }

            return true;
        }
    }
}
