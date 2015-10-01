using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSourceBotNet
{
    public class WebclientFactory
    {
        public static SocksWebClient getWebClient()
        {
            SocksWebClient wc = new SocksWebClient();
            wc.ProxyDetails = new ProxyDeets();
            wc.ProxyDetails.FullProxyAddress = "127.0.0.1:9050";
            wc.ProxyDetails.ProxyAddress = "127.0.0.1";
            wc.ProxyDetails.ProxyPort = 9050;
            wc.ProxyDetails.ProxyType = ProxyType.Socks;

            return wc;
        }

        public static SocksHttpWebRequest getWebRequest(string url)
        {
            SocksHttpWebRequest wc = (SocksHttpWebRequest)SocksHttpWebRequest.Create(url);
            wc.Proxy = new System.Net.WebProxy("127.0.0.1:9050");
            
            return wc;
        }

        public static webclient getMicroWebclient()
        {
            webclient wc = new webclient();

            return wc;
        }
    }
}
