using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace AniSharp.API.Transport
{
    /// <summary>
    /// Defines a class, that can send and receive Strings
    /// </summary>
    public interface UdpAdapter
    {
        void send(String s);

        String receive();

        void shutdown();
    }
}
