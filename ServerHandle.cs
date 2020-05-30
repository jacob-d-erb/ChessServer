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
                Console.WriteLine("Player " + _fromClient + " is hosting a game.");
            }
        }

        public static void JoinedGame(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            int _hostingId = _packet.ReadInt();
            Console.WriteLine("Handling game joiner.");
            
            if (ConsistencyCheck(_fromClient, _clientIdCheck))
            {
                Server.activeGames.Add(_hostingId, _fromClient);
                Server.clients[_fromClient].player.enemId = _hostingId;
                Server.clients[_hostingId].player.enemId = _fromClient;
                ServerSend.RemRoomToAll(_hostingId);
                Server.lobbyGames.Remove(_hostingId);
                ServerSend.SendJoinGame(_hostingId, _fromClient);
            }
        }

        public static void MakeMove(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            int _startRow = _packet.ReadInt();
            int _startCol = _packet.ReadInt();
            int _endRow = _packet.ReadInt();
            int _endCol = _packet.ReadInt();
            string _pawnPromote = _packet.ReadString();

            ServerSend.SendMove(Server.clients[_fromClient].player.enemId, _startRow, _startCol, _endRow, _endCol, _pawnPromote);

            Console.WriteLine("Packet containing move has been received.");
        }

        public static void EndGame(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();

            int opponentId = Server.clients[_fromClient].player.enemId;
            Server.clients[_fromClient].player.enemId = 0;
            if(opponentId != 0)
            {
                Server.clients[opponentId].player.enemId = 0;
            }

            if(Server.activeGames.ContainsKey(_fromClient))
            {
                Server.activeGames.Remove(_fromClient);
            }
            if(Server.activeGames.ContainsKey(opponentId))
            {
                Server.activeGames.Remove(opponentId);
            }
            Console.WriteLine("Ending game.");

        }

        public static void Forfeit(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            int opponentId = Server.clients[_fromClient].player.enemId;
            Server.clients[_fromClient].player.enemId = 0;
            if(opponentId != 0)
            {
                Server.clients[opponentId].player.enemId = 0;
            }

            if(Server.activeGames.ContainsKey(_fromClient))
            {
                Server.activeGames.Remove(_fromClient);
            }
            if(Server.activeGames.ContainsKey(opponentId))
            {
                Server.activeGames.Remove(opponentId);
            }

            ServerSend.SendForfeit(opponentId);
            Console.WriteLine("Ending game due to forfeit.");

        }

        public static void RemoveFromLobby(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();

            if(Server.lobbyGames.ContainsKey(_fromClient))
            {
                Server.lobbyGames.Remove(_fromClient);
                ServerSend.RemRoomToAll(_fromClient);
                Console.WriteLine("Hosted game has been removed.");
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