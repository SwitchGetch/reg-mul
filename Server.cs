using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;
using Microsoft.VisualBasic;


class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();
    public static List<User> Users = new List<User>();

    public static void CheckUser(RegForm user, ref Message answer)
    {
        if (user.IsNewUser)
        {
            if (Users.Find(x => x.Login == user.Login) != null)
            {
                answer.IsAllOK = false;
                answer.error = Message.Exception.UserWithTheSameLoginIsAlreadyExist;
            }
            else
            {
                User userdata = user;

                Users.Add(userdata);

                Json.UploadToFile(Users);
            }
        }
        else
        {
            if (Users.Find(x => x.Login == user.Login && x.Password == user.Password) == null)
            {
                answer.IsAllOK = false;
                answer.error = Message.Exception.WrongLoginOrPassword;
            }
        }
    }

    public static async Task ProcessClient(TcpClient client)
    {
        while (true)
        {
            RegForm user = Newtonsoft.Json.JsonConvert.DeserializeObject<RegForm>(TcpInteraction.ReadfromStream(client));

            Message answer = new Message() { IsAllOK = true };

            CheckUser(user, ref answer);

            await TcpInteraction.WriteToStream(client, Newtonsoft.Json.JsonConvert.SerializeObject(answer));
        }
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