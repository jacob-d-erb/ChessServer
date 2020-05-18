using System;
using System.Collections.Generic;
using System.Text;

namespace ChessServer
{
    class Constants
    {
        public const int TICKS_PER_SEC = 10;
        public const int MS_PER_TICK = 1000/TICKS_PER_SEC;
        public const int PORT = 1313;
        public const int MAXPLAYERS = 100;
        public const int MAXROOMS = MAXPLAYERS/2;
    }
}