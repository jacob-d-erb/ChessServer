using System;
using System.Collections.Generic;
using System.Text;


namespace ChessServer
{
    class Player
    {
        public int id;
        public string username;
        public int enemId = 0;
        public Player(int _id, string _username)
        {
            id = _id;
            username = _username;
        }

    }

    
}