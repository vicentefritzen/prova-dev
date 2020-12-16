using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ApplicationMonitor.Library
{
    public class StateObject
    {
        // Socket cliente
        public Socket workSocket = null;
        // Tamanho do buffer  
        public const int BufferSize = 1024;
        // Buffer
        public byte[] buffer = new byte[BufferSize];
        // String recebida
        public StringBuilder sb = new StringBuilder();
        // Identificador do cliente conectado (sequencial)
        public int clientNumber;
    }

    public class AsyncSocketServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static Dictionary<int, StateObject> Clients = new Dictionary<int, StateObject>();
        public static int connectedClient = 0;
        public const string C_SOCKET_ADDRESS = "127.0.0.1";
        public const int C_SOCKET_PORT = 9000;
        public static List<EventDTO> eventList = new List<EventDTO>();

        public AsyncSocketServer() { }

        public static void StartListening()
        {
            Byte[] bytes = new Byte[1024];

            IPAddress IP = IPAddress.Parse(C_SOCKET_ADDRESS);
            IPEndPoint EP = new IPEndPoint(IP, C_SOCKET_PORT);
            Socket listner = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listner.Bind(EP);
                listner.Listen(100);

                while (true)
                {
                    allDone.Reset();
                    listner.BeginAccept(new AsyncCallback(AcceptCallBack), listner);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void AcceptCallBack(IAsyncResult ar)
        {
            connectedClient++;
            allDone.Set();

            Socket listner = (Socket)ar.AsyncState;
            Socket handler = listner.EndAccept(ar);

            StateObject state = new StateObject();
            state.clientNumber = connectedClient;

            Clients.Add(connectedClient, state);

            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);

        }
        public static void ReadCallBack(IAsyncResult ar)
        {
            String content = String.Empty;
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                state.sb.Clear();
                Socket handler = state.workSocket;

                // Le os dados do cliente.   
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = state.sb.ToString();

                    if (content.Length == 4)
                    {
                        //to-do implementar validacao do header para exibir os alarmes solicitantes.
                    }
                    else
                    {
                        EventDTO eventDTO = null;

                        if (content.Length >= 9)
                        {
                            eventDTO = new EventDTO();

                            eventDTO.EventDate = DateTime.Now;
                            eventDTO.PanelId = connectedClient;
                            eventDTO.AccountId = BitConverter.ToUInt16(state.buffer, 1);
                            eventDTO.EventId = BitConverter.ToUInt16(state.buffer, 3);
                            eventDTO.EventDescription = ""; //to-do implementar o retorno do evento vinculado
                            eventDTO.PartitionId = state.buffer[5];
                            eventDTO.ZoneId = state.buffer[6];
                            eventDTO.UserId = state.buffer[7];

                            eventList.Add(eventDTO);

                            // to-do implementar o checksum.
                        }
                    }

                    if (isClientConnected(handler))
                        Send(handler, content);

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
                }
            }
            catch (SocketException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private static void Send(Socket handler, String data)
        {
            // Converte a string para um array de bytes usando o ascii encoding
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Inicia o envio dos dados ao requisitante da conexao
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Recebe os dados
                Socket handler = (Socket)ar.AsyncState;

                // Completa o recebimento dos dados
                int bytesSent = handler.EndSend(ar);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool isClientConnected(Socket handler)
        {
            return handler.Connected;
        }
    }
}