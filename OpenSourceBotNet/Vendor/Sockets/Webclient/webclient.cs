using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace OpenSourceBotNet
{
    public class webclient
    {
        public string post_data(string url, string postData)
        {
            //Our postvars
            byte[] buffer = Encoding.ASCII.GetBytes(postData);
            //Initialization, we use localhost, change if applicable
            SocksHttpWebRequest WebReq = WebclientFactory.getWebRequest(url);
            //Our method is post, otherwise the buffer (postvars) would be useless
            WebReq.Method = "POST";
            //We use form contentType, for the postvars.
            WebReq.ContentType = "application/x-www-form-urlencoded";
            //The length of the buffer (postvars) is used as contentlength.
            WebReq.ContentLength = buffer.Length;
            //We open a stream for writing the postvars
            Stream PostData = WebReq.GetRequestStream();
            //Now we write, and afterwards, we close. Closing is always important!
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            //Get the response handle, we have no true response yet!
            SocksHttpWebResponse WebResp = (SocksHttpWebResponse)WebReq.GetResponse();
            
            //Now, we read the response (the string), and output it.
            Stream Answer = WebResp.GetResponseStream();
            StreamReader _Answer = new StreamReader(Answer);
            string answer = _Answer.ReadToEnd();
            Console.WriteLine(answer);

            //Congratulations, you just requested your first POST page, you
            //can now start logging into most login forms, with your application
            //Or other examples.

            return answer;
        } 
    }
}
