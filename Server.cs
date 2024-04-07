using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;
using Microsoft.VisualBasic;


class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();
    public static List<TcpClient> InGameClients = new List<TcpClient>();
    public static List<User> Users = new List<User>();

    public static void CheckUser(RegForm user, ref Message answer)
    {
        if (user.IsNewUser)
        {
            if (Users.Find(x => x.Login == user.Login) != null)
            {
                answer.IsAllOK = false;
                answer.Error = Message.Exception.UserWithTheSameLoginIsAlreadyExist;
            }
            else
            {
                user.ID = Users.Count;

                Users.Add(user);

                Json.UploadToFile(Users);
            }
        }
        else
        {
            if (Users.Find(x => x.Login == user.Login && x.Password == user.Password) == null)
            {
                answer.IsAllOK = false;
                answer.Error = Message.Exception.WrongLoginOrPassword;
            }
        }
    }

    public static async Task Game()
    {
        while (true)
        {
            List<Player> Players = new List<Player>();

            for (int i = 0; i < InGameClients.Count; i++)
            {
                if (InGameClients[i].Connected)
                {
                    Players.Add(TcpInteraction.ReadfromStream<Player>(InGameClients[i]));
                }
                else
                {
                    InGameClients.RemoveAt(i);
                }
            }

            for (int i = 0; i < Users.Count; i++)
            {
                Player player = Players.Find(x => x.ID == Users[i].ID);

                Users[i].X = player.X;
                Users[i].Y = player.Y;
            }

            Json.UploadToFile(Users);

            string players_str = Newtonsoft.Json.JsonConvert.SerializeObject(Players);

            for (int i = 0; i < InGameClients.Count; i++)
            {
                if (InGameClients[i].Connected)
                {
                    await TcpInteraction.WriteToStream(InGameClients[i], players_str);
                }
                else
                {
                    InGameClients.RemoveAt(i);
                }
            }
        }
    }

    public static async Task ProcessClient(TcpClient client)
    {
        while (true)
        {
            RegForm user = TcpInteraction.ReadfromStream<RegForm>(client);

            Message answer = new Message() { IsAllOK = true, Error = Message.Exception.None };

            CheckUser(user, ref answer);

            await TcpInteraction.WriteToStream(client, answer);

            if (answer.IsAllOK) break;
        }

        InGameClients.Add(client);
        await Game();
    }

    public static async Task Main(string[] args)
    {
        TcpListener tcp_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        tcp_listener.Start();

        Console.WriteLine("Server started...\n");


        Json.DownloadFromFile(ref Users);

        while (true)
        {
            TcpClient tcp_client = await tcp_listener.AcceptTcpClientAsync();

            await Console.Out.WriteLineAsync("Client started...\n");

            Clients.Add(tcp_client);

            _ = Task.Run(async () => await ProcessClient(Clients[Clients.Count - 1]));
        }
    }
}
