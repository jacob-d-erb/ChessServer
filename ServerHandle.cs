using System;
using System.Collections.Generic;
using System.Text;

namespace ChessServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            Console.WriteLine($"Player name is {_username}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong Client ID ({_clientIdCheck})!");
            }

            Server.clients[_fromClient].SendIntoLobby(_username);

            foreach(KeyValuePair<int, int> entry in Server.lobbyGames)
            {
                ServerSend.SendRoomToSingle(_fromClient, entry.Key, entry.Value);
            }
        }

        public static void HostingGame(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            int _gameType = _packet.ReadInt();

            if (ConsistencyCheck(_fromClient, _clientIdCheck))
            {
                Server.lobbyGames.Add(_fromClient, _gameType);
                ServerSend.SendRoomToAllExceptOne(_fromClient, _gameType);
            }
        }

        public static void JoinedGame(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            int _hostingId = _packet.ReadInt();
            
            if (ConsistencyCheck(_fromClient, _clientIdCheck))
            {
                Server.activeGames.Add(_hostingId, _fromClient);
                Server.clients[_fromClient].player.enemId = _hostingId;
                Server.clients[_hostingId].player.enemId = _fromClient;
                ServerSend.RemRoomToAll(_hostingId);
                Server.lobbyGames.Remove(_hostingId);
            }
        }

        public static bool ConsistencyCheck(int _fromClient, int _clientIdCheck)
        {
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{Server.clients[_fromClient].player.username}\" (ID: {_fromClient}) has assumed the wrong Client ID ({_clientIdCheck})!");
                return false;
            }
            else if (Server.lobbyGames.ContainsKey(_fromClient))
            {
                Console.WriteLine($"Player \"{Server.clients[_fromClient].player.username}\" (ID: {_fromClient}) is already hosting a game!");
                return false;
            }
            else if (Server.activeGames.ContainsKey(_fromClient) || Server.activeGames.ContainsValue(_fromClient))
            {
                Console.WriteLine($"Player \"{Server.clients[_fromClient].player.username}\" (ID: {_fromClient}) is already in an active game!");
                return false;
            }
            return true;
        }
    }

}