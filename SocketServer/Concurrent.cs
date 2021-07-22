// Patryk Kulik
// 0989317
// INF2D

using Sequential;
using System;
//todo [Assignment]: add required namespaces
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Concurrent
{
    public class ConcurrentServer : SequentialServer
    {
        // todo [Assignment]: implement required attributes specific for concurrent server
        public Thread Thread;

        public String[] VotingOptions;
        public int VoteA = 0;
        public int VoteB = 0;
        public int VoteC = 0;
        public int VoteD = 0;

        public ConcurrentServer(Setting settings) : base(settings)
        {
            // todo [Assignment]: implement required code
            this.settings = settings;
            this.ipAddress = IPAddress.Parse(settings.serverIPAddress);

            this.VotingOptions = settings.votingList.Split(settings.commands_sep, 4);
        }

        public override void prepareServer()
        {
            // todo [Assignment]: implement required code
            Console.WriteLine("[Server] is ready to start ...");
            try
            {
                localEndPoint = new IPEndPoint(this.ipAddress, settings.serverPortNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(settings.serverListeningQueue);

                

                while (settings.serverListeningQueue != 0)
                {
                    Console.WriteLine("Waiting for incoming connections ... ");
                    Socket connection = listener.Accept();
                    Thread = new Thread(() => this.handleClient(connection));

                    this.numOfClients++;
                    Thread.Start();
                }
                Thread.Join();

            }catch (Exception e){ Console.Out.WriteLine("[Server] Preparation: {0}",e.Message); }
        }

        
        public override string processMessage(String msg)
        {
            // todo [Assignment]: implement required code
            Thread.Sleep(settings.serverProcessingTime);
            string replyMsg = Message.confirmed;

            try
            {
                switch (msg)
                {
                    case Message.terminate:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();
                        Console.WriteLine("[Server] END : number of clients communicated -> {0} ", this.numOfClients);

                        Console.WriteLine("Total votes:" + (this.VoteA + this.VoteB + this.VoteC + this.VoteD));
                        Console.WriteLine(this.VoteA + " Voted for: " + this.VotingOptions[0]);
                        Console.WriteLine(this.VoteB + " Voted for: " + this.VotingOptions[1]);
                        Console.WriteLine(this.VoteC + " Voted for: " + this.VotingOptions[2]);
                        Console.WriteLine(this.VoteD + " Voted for: " + this.VotingOptions[3]);

                        // This launches the command that won.
                        // Since I am not able to test the commands given in the assigment. I replaced them with cmd commands.
                        // The whole counting and vote checking works WITHOUT changing anything in the JSON file.
                        // The only thing that is changed is simply the command that is launched.
                        
                        if(this.VoteA >= this.VoteB && this.VoteA >= this.VoteC && this.VoteA >= this.VoteD)
                        {
                            Console.WriteLine("\n" + this.VoteA + " Voted for: " + this.VotingOptions[0] + "\nLaunching Windows cmd command: hostname");
                            System.Diagnostics.Process.Start("cmd.exe", "/C hostname");
                        }
                        if (this.VoteB >= this.VoteA && this.VoteB >= this.VoteC && this.VoteB >= this.VoteD)
                        {
                            Console.WriteLine("\n" + this.VoteB + " Voted for: " + this.VotingOptions[1] + "\nLaunching Windows cmd command: ipconfig");
                            System.Diagnostics.Process.Start("cmd.exe", "/C ipconfig");
                        }
                        if (this.VoteC >= this.VoteA && this.VoteC >= this.VoteB && this.VoteC >= this.VoteD)
                        {
                            Console.WriteLine("\n" + this.VoteC + " Voted for: " + this.VotingOptions[2] + "\nLaunching Windows cmd command: tree");
                            System.Diagnostics.Process.Start("cmd.exe", "/C tree");
                        }
                        if (this.VoteD >= this.VoteA && this.VoteD >= this.VoteB && this.VoteD >= this.VoteC)
                        {
                            Console.WriteLine("\n" + this.VoteD + " Voted for: " + this.VotingOptions[3] + "\nLaunching Windows cmd command: systeminfo");
                            System.Diagnostics.Process.Start("cmd.exe", "/C systeminfo");
                        }


                        // Resetting votes for next voting round
                        this.numOfClients = 0;
                        this.VoteA = 0;
                        this.VoteB = 0;
                        this.VoteC = 0;
                        this.VoteD = 0;

                        break;
                    default:
                        replyMsg = Message.confirmed;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("[Server] received from the client -> {0} ", msg);
                        Console.ResetColor();

                        String[] Vote = msg.Split(settings.command_msg_sep, 2);
                      
                        lock (this)
                        {
                            if (Vote[1] == this.VotingOptions[0]) { this.VoteA++; }
                            else if (Vote[1] == this.VotingOptions[1]) { this.VoteB++; }
                            else if (Vote[1] == this.VotingOptions[2]) { this.VoteC++; }
                            else if (Vote[1] == this.VotingOptions[3]) { this.VoteD++; }
                        }
                        
                        break;
                }
            }
            catch (Exception e) { Console.Out.WriteLine("[Server] Process Message {0}", e.Message); }
            return replyMsg;
        }
    }
}