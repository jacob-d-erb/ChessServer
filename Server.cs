using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ChessServer
{
    class Server
    {
        public static int MaxPlayers {get; private set; }
        public static int Port { get; private set;}
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static Dictionary<int, int> lobbyGames = new Dictionary<int, int>(); //Hosting ID, gametype.
        public static Dictionary<int, int> activeGames = new Dictionary<int, int>(); //White ID, Black ID
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        private static TcpListener tcpListener;
        
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting Server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} Failed to connect! Server is full.");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.hostingGame, ServerHandle.HostingGame},
                { (int)ClientPackets.joinedGame, ServerHandle.JoinedGame},
                { (int)ClientPackets.makeMove, ServerHandle.MakeMove},
                { (int)ClientPackets.endGame, ServerHandle.EndGame},
                { (int)ClientPackets.forfeit, ServerHandle.Forfeit},
                { (int)ClientPackets.removeFromLobby, ServerHandle.RemoveFromLobby},
            };

            Console.WriteLine("Initialized packets.");
        }
    }
}