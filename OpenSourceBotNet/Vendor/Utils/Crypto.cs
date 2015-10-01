using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace conSweetDreams.Vendor.Utils
{
    public class Crypto
    {
        public static RSACryptoServiceProvider rsa;

        public static void AssignParameter()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "SpiderContainer";
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            rsa = new RSACryptoServiceProvider(1024, cspParams);
        }

        public static string EncryptData(string data2Encrypt)
        {
            AssignParameter();
            StreamReader reader = new StreamReader("mypublickey.xml");
            string publicOnlyKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicOnlyKeyXML);
            reader.Close();

            //read plaintext, encrypt it to ciphertext

            byte[] plainbytes = System.Text.Encoding.UTF8.GetBytes(data2Encrypt);
            byte[] cipherbytes = rsa.Encrypt(plainbytes, false);
            return Convert.ToBase64String(cipherbytes);
        }

        public static string EncryptData(string data2Encrypt, string PublicKeyToUse)
        {
            AssignParameter();
            rsa.FromXmlString(PublicKeyToUse);

            //read plaintext, encrypt it to ciphertext

            byte[] plainbytes = System.Text.Encoding.UTF8.GetBytes(data2Encrypt);
            byte[] cipherbytes = rsa.Encrypt(plainbytes, false);
            return Convert.ToBase64String(cipherbytes);
        }

        public static void AssignNewKey()
        {
            AssignParameter();

            //provide public and private RSA params
            StreamWriter writer = new StreamWriter("privatekey.xml");
            string publicPrivateKeyXML = rsa.ToXmlString(true);
            writer.Write(publicPrivateKeyXML);
            writer.Close();

            //provide public only RSA params
            writer = new StreamWriter("mypublickey.xml");
            string publicOnlyKeyXML = rsa.ToXmlString(false);
            writer.Write(publicOnlyKeyXML);
            writer.Close();

        }

        public static string DecryptData(string data2Decrypt)
        {
            AssignParameter();

            byte[] getpassword = Convert.FromBase64String(data2Decrypt);

            StreamReader reader = new StreamReader("privatekey.xml");
            string publicPrivateKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicPrivateKeyXML);
            reader.Close();

            //read ciphertext, decrypt it to plaintext
            byte[] plain = rsa.Decrypt(getpassword, false);
            return System.Text.Encoding.UTF8.GetString(plain);

        }

        public static byte[] DecryptBytes2(byte[] myBytes)
        {
            AssignParameter();
            StreamReader reader = new StreamReader(Application.StartupPath + @"\privatekey.xml");
            string publicPrivateKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicPrivateKeyXML);
            reader.Close();
            byte[] plain = rsa.Decrypt(myBytes, false);
            return plain;
        }

        public static string DecryptFile(string path, string newPath)
        {
            AssignParameter();

            FileInfo fi = new FileInfo(path);
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] myBytes = new byte[fi.Length];
            br.Read(myBytes, 0, int.Parse(fi.Length.ToString()));
            br.Close();
            fs.Close();

            StreamReader reader = new StreamReader("privatekey.xml");
            string publicPrivateKeyXML = reader.ReadToEnd();
            rsa.FromXmlString(publicPrivateKeyXML);
            reader.Close();

            byte[] plain = rsa.Decrypt(myBytes, false);

            // Possibly Create Random File, and After Write Rename (For Larger Files)
            FileStream fsCreate = new FileStream(newPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fsCreate);

            bw.Write(plain);
            bw.Close();
            fsCreate.Close();

            return path;
        }

        public static string EncryptFile(string TheirPublicKey, string path, string newPath)
        {
            AssignParameter();

            FileInfo fi = new FileInfo(path);
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] myBytes = new byte[fi.Length];
            br.Read(myBytes, 0, int.Parse(fi.Length.ToString()));
            br.Close();
            fs.Close();

            rsa.FromXmlString(TheirPublicKey);
            br.Close();

            byte[] eBytes = rsa.Encrypt(myBytes, false);

            // Possibly Create Random File, and After Write Rename (For Larger Files)
            FileStream fsCreate = new FileStream(newPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fsCreate);

            bw.Write(eBytes);
            bw.Close();
            fsCreate.Close();

            return path;
        }

        public static string ReturnMyPublicKey()
        {
            StreamReader reader = new StreamReader("mypublickey.xml");
            string publicOnlyKeyXML = reader.ReadToEnd();
            reader.Close();
            return publicOnlyKeyXML;
        }

        public static byte[] EncryptBytes(byte[] myBytes, string PublicKeyToUse)
        {
            byte[] cipherbytes = myBytes;

            try
            {
                if (myBytes != null)
                {
                    AssignParameter();
                    rsa.FromXmlString(PublicKeyToUse);
                    cipherbytes = rsa.Encrypt(myBytes, false);
                }
            }
            catch (ExecutionEngineException ex)
            {
                if (ex.ToString().IndexOf("Bad Length.") > -1)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine();
                    Console.WriteLine(myBytes.Length.ToString());
                }
            }

            return cipherbytes;
        }
    }
}
