using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace PalBot.Core {
	public static class TorClient {
		private static string SendCommand(TcpClient tcpClient, string command) {
			//ConsoleLog.WriteLine("Tor command send:  " + command);
			try {
				Stream tcpStream = tcpClient.GetStream();

				byte[] buffer = System.Text.Encoding.ASCII.GetBytes(command);
				tcpStream.Write(buffer, 0, buffer.Length);
				tcpStream.Flush();

				//ConsoleLog.WriteLine("Tor command sent:  " + command);

				byte[] message = new byte[4096];
				int bytesRead;
				bytesRead = tcpStream.Read(message, 0, 4096);

				//ConsoleLog.WriteLine("Tor response received: " + System.Text.Encoding.ASCII.GetString(message, 0, bytesRead));

				return System.Text.Encoding.ASCII.GetString(message, 0, bytesRead);
			} catch (Exception e) {
				MainForm.SetStatus("Tor send command failed: " + e.Message);
				return "";
			}
		}

		private static bool CheckOK(string response) {
			try {
				string token = response.Substring(0, response.IndexOf(' '));
				return (Convert.ToInt32(token) == 250);
			} catch (Exception) {
				return false;
			}
		}


		public static bool GetNewIP() {
			try {
				TcpClient tcpClient = new TcpClient();
				tcpClient.Connect("127.0.0.1", 9051);
				string responce;
				responce = SendCommand(tcpClient, "AUTHENTICATE \"\"\r\n");
				if (!CheckOK(responce)) {
					MainForm.SetStatus("Tor authentication failed: " + responce);
					return false;
				}
				responce = SendCommand(tcpClient, "signal NEWNYM\r\n");
				if (!CheckOK(responce)) {
					MainForm.SetStatus("Tor new IP getting failed: " + responce);
					return false;
				}
				tcpClient.Close();
				return true;
			} catch (Exception ex) {
				MainForm.SetStatus("Error calling TOR: " + ex.Message);
				return false;
			}
		}
	}
}
