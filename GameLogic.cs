using System;
using System.Collections.Generic;
using System.Text;

namespace ChessServer
{
    class GameLoop
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}