using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;
using Microsoft.VisualBasic;


class Client
{
    public static bool GetAnswer(TcpClient client)
    {
        string server_answer = TcpInteraction.ReadfromStream(client);

        Message answer = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(server_answer);

        if (answer.IsAllOK)
        {
            Console.WriteLine("\n\nyou are in!\n");

            return true;
        }
        else
        {
            Console.WriteLine("\nError: " + answer.error.ToString());

            return false;
        }
    }

    public static async Task Autentification(TcpClient client)
    {
        await Console.Out.WriteAsync("\nyou want to:\n1) register\n2) log in\nyour choice: ");

        ConsoleKeyInfo choice = Console.ReadKey();


        RegForm user = new RegForm();

        await Console.Out.WriteAsync("\n\nLogin: ");
        user.Login = Console.ReadLine();

        await Console.Out.WriteAsync("Password: ");
        user.Password = SHA.ConvertToSHA256(Console.ReadLine());

        user.IsNewUser = choice.Key == ConsoleKey.D1;


        await TcpInteraction.WriteToStream(client, Newtonsoft.Json.JsonConvert.SerializeObject(user));
    }

    public static async Task Main(string[] args)
    {
        TcpClient tcp_client = new TcpClient();

        await tcp_client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 12345);
        await Console.Out.WriteLineAsync("Connected...\n");


        while (true)
        {
            await Autentification(tcp_client);

            if (GetAnswer(tcp_client))
            {
                break;
            }
        }

    }
}