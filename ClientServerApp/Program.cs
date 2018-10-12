using Akka.Actor;
using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("ClientServer"))
            {
                system.ActorOf(Props.Create(() => new BalanceServerActor()), "BalanceServer");

                Console.ReadKey();
            }
        }

        class BalanceServerActor : ReceiveActor
        {
            private readonly Dictionary<IActorRef,decimal> Clients = new Dictionary<IActorRef, decimal>();

            public BalanceServerActor()
            {
                //Read handshake message
                Receive<AuthenticateRequest>(message =>
                {
                    if(!Clients.Keys.Contains(Sender))
                    {
                        Clients.Add(Sender, 0);
                    }

                    //Response to client on sucessfull handshake
                    Sender.Tell(new AuthenticateResponse
                    {
                        Message = string.Format("Hello {0}", message.Username),
                    }, Self);
                });

                Receive<BalanceRequest>(message =>
                {
                    //Server should receive balance update message and read it
                    var MoneyAmount = message.MoneyAmount;

                    if (Clients.Keys.Contains(Sender))
                    {
                        Clients[Sender] += message.MoneyAmount;
                    }

                    //Server should should perform balance update operation and inform client about it
                    Sender.Tell(new BalanceResponse
                    {
                        Message = string.Format("For user {0} current money amount is {1}", message.UserName, Clients[Sender])
                    }, Self);

                });
            }
        }
    }
}
