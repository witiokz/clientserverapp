using Akka.Actor;
using System;
using Common.Actors;
using Common.Messages;
using System.Threading;
using Common.Share;

namespace Client
{
    class Program
    {
        static string UserName = string.Empty;

        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("ClientServer"))
            {
                var balanceClient = system.ActorOf(Props.Create<BalanceClientActor>(), "Balance");

                Thread.Sleep(450);

                while (true)
                {
                    if(Preloader.Show)
                    {
                        Console.WriteLine("Please wait ...");
                        Thread.Sleep(450);
                        continue;
                    }

                    Console.WriteLine(string.IsNullOrEmpty(UserName) ? "Enter your username and press <Enter>" : 
                        "Provide amount of money to balance and press <Enter>");

                    var input = Console.ReadLine();

                    if (string.IsNullOrEmpty(UserName))
                    {
                        //Send handshake exchange message
                        balanceClient.Tell(new AuthenticateRequest()
                        {
                            Username = input
                        });

                        UserName = input;
                        Preloader.Show = true;

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(input) || !decimal.TryParse(input, out decimal amountOfMoney))
                        {
                            Console.WriteLine("Incorrect amount of money");
                            continue;
                        }

                        balanceClient.Tell(new BalanceRequest
                        {
                            MoneyAmount = amountOfMoney
                        });

                        Preloader.Show = true;
                    }
                }
            }
        }
    }
}
