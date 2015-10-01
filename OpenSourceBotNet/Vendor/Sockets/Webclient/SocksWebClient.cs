using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

public class SocksWebClient : WebClient
{
    public IProxyDetails ProxyDetails { get; set; }
    public string UserAgent { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest result = null;

        if (ProxyDetails != null)
        {
            if (ProxyDetails.ProxyType == ProxyType.Proxy)
            {
                result = (HttpWebRequest)WebRequest.Create(address);
                result.Proxy = new WebProxy(ProxyDetails.FullProxyAddress);
                if (!string.IsNullOrEmpty(UserAgent))
                    ((HttpWebRequest)result).UserAgent = UserAgent;
            }
            else if (ProxyDetails.ProxyType == ProxyType.Socks)
            {
                result = SocksHttpWebRequest.Create(address);
                result.Proxy = new WebProxy(ProxyDetails.FullProxyAddress);
                //TODO: implement user and password

            }
            else if (ProxyDetails.ProxyType == ProxyType.None)
            {
                result = (HttpWebRequest)WebRequest.Create(address);
                if (!string.IsNullOrEmpty(UserAgent))
                    ((HttpWebRequest)result).UserAgent = UserAgent;
            }
        }
        else
        {
            result = (HttpWebRequest)WebRequest.Create(address);
            if (!string.IsNullOrEmpty(UserAgent))
                ((HttpWebRequest)result).UserAgent = UserAgent;
        }


        return result;
    }

}

public class ProxyDeets : IProxyDetails
{

    public ProxyType ProxyType { get; set; }
    /// <summary>
    /// adress and port
    /// </summary>
    public string FullProxyAddress { get; set; }
    public string ProxyAddress { get; set; }
    public int ProxyPort { get; set; }
    public string ProxyUserName { get; set; }
    public string ProxyPassword { get; set; }
}

public interface IProxyDetails
{
    ProxyType ProxyType { get; set; }
    /// <summary>
    /// adress and port
    /// </summary>
    string FullProxyAddress { get; set; }
    string ProxyAddress { get; set; }
    int ProxyPort { get; set; }
    string ProxyUserName { get; set; }
    string ProxyPassword { get; set; }
}
public enum ProxyType
	    {
	        None=0,
	        Proxy=1,
	        Socks=2
	    }