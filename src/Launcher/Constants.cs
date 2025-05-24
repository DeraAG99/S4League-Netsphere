using Launcher;
using System.Net;

namespace WPFS4Launcher
{
    internal class Constants
    {
        public static bool KRClient = false;

        public static MainWindow LoginWindow;
        public static IPEndPoint ConnectEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 28001);
    }
}
