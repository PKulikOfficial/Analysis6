using System;
using System.Threading;
using Sequential;

namespace Concurrent
{
    public class ConcurrentClient : SimpleClient
    {
        public Thread workerThread;

        public ConcurrentClient(int id, Setting settings) : base(id, settings)
        {
            // todo [Assignment]: implement required code
            workerThread = new Thread(() => run());


        }
        public void run()
        {
            // todo [Assignment]: implement required code
            prepareClient();
            communicate();

        }
    }
    public class ConcurrentClientsSimulator : SequentialClientsSimulator
    {
        private ConcurrentClient[] clients;

        public ConcurrentClientsSimulator() : base()
        {
            Console.Out.WriteLine("\n[ClientSimulator] Concurrent simulator is going to start with {0}", settings.experimentNumberOfClients);
            clients = new ConcurrentClient[settings.experimentNumberOfClients];
            for (int i = 0; i < settings.experimentNumberOfClients; i++)
            {
                clients[i] = new ConcurrentClient(i + 1, settings); // id>0 means this is not a terminating client
            }
        }

        public void ConcurrentSimulation()
        {
            try
            {
                // todo [Assignment]: implement required code

                for(int i = 0; i < settings.experimentNumberOfClients; i++)
                {
                    clients[i].workerThread.Start();
                }

                for (int i = 0; i < settings.experimentNumberOfClients; i++)
                {
                    clients[i].workerThread.Join();
                }

                Console.Out.WriteLine("\n[ClientSimulator] All clients finished with their communications ... ");
                Thread.Sleep(settings.delayForTermination);

                ConcurrentClient endClient = new ConcurrentClient(-1, settings);
                endClient.run();


            }
            catch (Exception e)
            { Console.Out.WriteLine("[Concurrent Simulator] {0}", e.Message); }
        }
    }
}
