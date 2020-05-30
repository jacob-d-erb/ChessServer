using System;
using System.Collections.Generic;
using System.Text;

namespace ChessServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendRoomToSingle(int _toClient, int _host, int _gametype)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendRoom))
            {
                _packet.Write(_host);
                _packet.Write(Server.clients[_host].player.username);
                _packet.Write(_gametype);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendRoomToAllExceptOne(int _exceptClient, int _gametype)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendRoom))
            {
                _packet.Write(_exceptClient);
                _packet.Write(Server.clients[_exceptClient].player.username);
                _packet.Write(_gametype);

                SendTCPDataToAll(_exceptClient, _packet);
            }
        }

        public static void RemRoomToAll(int _host)
        {
            using (Packet _packet = new Packet((int)ServerPackets.removeRoom))
            {
                _packet.Write(_host);

                SendTCPDataToAll(_packet);
                Console.WriteLine("Packet notifying all players that game has been removed has been sent.");
            }
        }

        public static void SendJoinGame(int _host, int _joiner)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendJoinGame))
            {
                _packet.Write(_joiner);
                _packet.Write(Server.clients[_joiner].player.username);

                SendTCPData(_host, _packet);
                Console.WriteLine("Packet notifying host of game has been sent.");
            }
        }

        public static void SendMove(int _recipient, int _startRow, int _startCol, int _endRow, int _endCol, string _pawnPromote)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendMove))
            {
                _packet.Write(_startRow);
                _packet.Write(_startCol);
                _packet.Write(_endRow);
                _packet.Write(_endCol);
                _packet.Write(_pawnPromote);

                SendTCPData(_recipient, _packet);
                Console.WriteLine("Packet containing move has been sent.");
            }
        }

        public static void SendForfeit(int _recipient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendForfeit))
            {

                SendTCPData(_recipient, _packet);
                Console.WriteLine("Packet containing forfeit has been sent.");
            }
        }
    }
}