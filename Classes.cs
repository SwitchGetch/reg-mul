using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace Classes
{

    public class SHA
    {
        public static string ConvertToSHA256(string str) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(str)));
    }

    public class Player
    {
        public int X { get; set; }

        public int Y { get; set; }

        public char Symbol { get; set; }

        public ConsoleColor Color { get; set; }

        public string ID { get; set; }
    }

    public class User : Player
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }

    public class RegForm : User
    {
        public bool IsNewUser { get; set; }
    }

    public class Message
    {
        public bool IsAllOK { get; set; }

        public Exception error { get; set; }

        public enum Exception
        {
            None,
            UserWithTheSameLoginIsAlreadyExist,
            WrongLoginOrPassword
        }
    }


    public class TcpInteraction
    {
        public static async Task WriteToStream(TcpClient client, string message)
        {
            var stream = client.GetStream();
            message += '\0';

            byte[] bytes = Encoding.UTF8.GetBytes(message.ToArray());

            await stream.WriteAsync(bytes);
        }


        public static string ReadfromStream(TcpClient client)
        {
            var stream = client.GetStream();
            List<byte> bytes = new List<byte>();


            int bytes_read = 0;

            while ((bytes_read = stream.ReadByte()) != '\0')
            {
                bytes.Add((byte)bytes_read);
            }

            string str = Encoding.UTF8.GetString(bytes.ToArray());

            return str;
        }
    }

    public class Json
    {
        public static void UploadToFile(List<User> Users)
        {
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(Users);

            File.WriteAllText("userdata.json", str);
        }

        public static void DownloadFromFile(ref List<User> Users)
        {
            string str = File.ReadAllText("userdata.json");

            if (str.Length > 0) Users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(str);
        }
    }
}