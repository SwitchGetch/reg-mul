using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;

class Client
{
    public static bool GetAnswer(TcpClient client)
    {
        Message answer = TcpInteraction.ReadfromStream<Message>(client);

        if (answer.IsAllOK)
        {
            Console.WriteLine("\n\nyou are in!\n");

            return true;
        }
        else
        {
            Console.WriteLine("\nError: " + answer.Error.ToString());

            return false;
        }
    }

    public static async Task Autentification(TcpClient client)
    {
        while (true)
        {
            Console.Write("\nyou want to:\n1) register\n2) log in\nyour choice: ");

            ConsoleKeyInfo choice = Console.ReadKey();


            RegForm user = new RegForm();

            Console.Write("\n\nLogin: ");
            user.Login = Console.ReadLine();

            Console.Write("Password: ");
            user.Password = SHA.ConvertToSHA256(Console.ReadLine());

            user.IsNewUser = choice.Key == ConsoleKey.D1;

            if (user.IsNewUser)
            {
                Console.Write("Symbol: ");
                user.Symbol = Console.ReadLine()[0];

                Console.Write("Color: ");
                user.Color = (ConsoleColor)Convert.ToInt32(Console.ReadLine());
            }

            await TcpInteraction.WriteToStream(client, user);

            if (GetAnswer(client))
            {
                Map.Create(20, 20);

                _ = Task.Run(async () => await Output(client));

                await Movement(client, user);

                break;
            }
        }
    }

    public static async Task Movement(TcpClient client, Player player)
    {
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.W:

                    if (Map.map[player.X][player.Y - 1].Type != Block.BlockType.Wall)
                    {
                        player.Y--;
                    }

                    break;

                case ConsoleKey.A:

                    if (Map.map[player.X - 1][player.Y].Type != Block.BlockType.Wall)
                    {
                        player.X--;
                    }

                    break;

                case ConsoleKey.S:

                    if (Map.map[player.X][player.Y + 1].Type != Block.BlockType.Wall)
                    {
                        player.Y++;
                    }

                    break;

                case ConsoleKey.D:

                    if (Map.map[player.X + 1][player.Y].Type != Block.BlockType.Wall)
                    {
                        player.X++;
                    }

                    break;
            }

            await TcpInteraction.WriteToStream(client, player);
        }
    }

    public static async Task Output(TcpClient client)
    {
        while (true)
        {
            List<Player> Players = TcpInteraction.ReadfromStream<List<Player>>(client);

            Map.Output(Players);
        }
    }

    public static async Task Main(string[] args)
    {
        TcpClient tcp_client = new TcpClient();
        Player player = new Player();

        await tcp_client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 12345);
        await Console.Out.WriteLineAsync("Connected...\n");

        await Autentification(tcp_client);
    }
}
