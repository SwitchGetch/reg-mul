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

        public int ID { get; set; }
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

        public Exception Error { get; set; }

        public enum Exception
        {
            None,
            UserWithTheSameLoginIsAlreadyExist,
            WrongLoginOrPassword
        }
    }

    public class TcpInteraction
    {
        public static async Task WriteToStream<T>(TcpClient client, T message)
        {
            var stream = client.GetStream();
            string str_message = Newtonsoft.Json.JsonConvert.SerializeObject(message) + '\0';

            byte[] bytes = Encoding.UTF8.GetBytes(str_message.ToArray());

            await stream.WriteAsync(bytes);
        }


        public static T ReadfromStream<T>(TcpClient client)
        {
            var stream = client.GetStream();
            List<byte> bytes = new List<byte>();


            int bytes_read = 0;

            while ((bytes_read = stream.ReadByte()) != '\0')
            {
                bytes.Add((byte)bytes_read);
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes.ToArray()));
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

    public class Block : Player
    {
        public BlockType Type { get; set; }

        public enum BlockType
        {
            Empty,
            Wall
        }
    }

    public class Map
    {
        public static List<List<Block>> map = new List<List<Block>>();

        public static void Create(int l, int h)
        {
            for (int i = 0; i < l; i++)
            {
                List<Block> Line = new List<Block>();

                for (int j = 0; j < h; j++)
                {
                    if (i == 0 || j == 0 || i == l - 1 || j == h - 1)
                    {
                        Line.Add(new Block() { Type = Block.BlockType.Wall, Color = ConsoleColor.Red, Symbol = '#', X = i, Y = j });
                    }
                    else
                    {
                        Line.Add(new Block() { Type = Block.BlockType.Empty, Color = ConsoleColor.Black, Symbol = ' ', X = i, Y = j });
                    }
                }

                map.Add(Line);
            }
        }

        public static void Output(List<Player> Players)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    Console.SetCursorPosition(map[i][j].X, map[i][j].Y);
                    Console.ForegroundColor = map[i][j].Color;
                    Console.Write(map[i][j].Symbol);
                }
            }

            for (int i = 0; i < Players.Count; i++)
            {
                Console.SetCursorPosition(Players[i].X, Players[i].Y);
                Console.ForegroundColor = Players[i].Color;
                Console.Write(Players[i].Symbol);
            }
        }
    }
}
