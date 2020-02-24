﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
	public static ManualResetEvent connectionAccepted = new ManualResetEvent(false);

	public delegate void tcpCallBack(string data);
	public event tcpCallBack OnRecieveData;
	public event tcpCallBack OnConnected;

	public string ipAddr;
	public int portNumber = 11000;
	public TCPState srvrState;
	public List<TCPState> connectionsState = new List<TCPState>();

	public delegate void StartListeningAsync();

	public void StartListening()
	{
		Socket srvr_listenersoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		IPEndPoint srvr_ep = new IPEndPoint(IPAddress.Parse(ipAddr), portNumber);
		srvr_listenersoc.Bind(srvr_ep);
		srvr_listenersoc.Listen(100);
		srvrState = new TCPState(srvr_listenersoc);
		print("server set, waiting for connectoin");

		try
		{
			while (true)
			{
				connectionAccepted.Reset();
				srvr_listenersoc.BeginAccept(new AsyncCallback(AcceptCallback), srvr_listenersoc);
				connectionAccepted.WaitOne();
				print("connection accepted by server");
				OnConnected.Invoke("");
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	private void AcceptCallback(IAsyncResult ar)
	{
		connectionAccepted.Set();

		Socket listener = (Socket)ar.AsyncState;
		Socket handler = listener.EndAccept(ar);

		TCPState conState = new TCPState(handler);
		connectionsState.Add(conState);
		StartReceiving(conState);
	}

	private void StartReceiving(TCPState state)
	{
		state.workSocket.BeginReceive(state.buffer, 0, TCPState.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
	}

	private void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			String content = String.Empty;

			TCPState state = (TCPState)ar.AsyncState;
			Socket handler = state.workSocket;

			int bytesRead = handler.EndReceive(ar);

			if (bytesRead > 0)
			{
				state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
				content = state.sb.ToString();

				int indexOfEOF = content.IndexOf("<EOF>");

				if (indexOfEOF > -1)
				{
					// A mssg is received
					content = content.Substring(0, indexOfEOF);
					OnRecieveData.Invoke(content);

					// start receiving the next message
					StartReceiving(state);
				}
				else
				{
					StartReceiving(state);
				}
			}
			else
			{
				// It's theoritically not possible to get here but I put it here just in case
				StartReceiving(state);
			}
		}
		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public void SendMessage_(string mssg)
	{
		if (connectionsState.Count > 0)
		{
			print(mssg);
			mssg = mssg + "<EOF>";
			byte[] byteData = Encoding.ASCII.GetBytes(mssg);
			connectionsState[0].workSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), connectionsState[0]);
		}
		else
		{
			Debug.LogWarning("no connections found but you're trying to broadcast a message");
		}
	}

	private void SendCallback(IAsyncResult ar)
	{
		try
		{
			TCPState state = (TCPState)ar.AsyncState;
			int bytesSent = state.workSocket.EndSend(ar);
		}
		catch(Exception e) 
		{
			Debug.LogError(e.ToString());
		}
	}

	public void ShutDown()
	{
		connectionsState[0].workSocket.Shutdown(SocketShutdown.Both);
		connectionsState[0].workSocket.Close();
		Debug.Log("socket shutdown");
	}
}