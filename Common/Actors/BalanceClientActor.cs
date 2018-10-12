using Akka.Actor;
using Common.Messages;
using Common.Share;
using System;

namespace Common.Actors
{
    public class BalanceClientActor : ReceiveActor
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://ClientServer@localhost:8081/user/BalanceServer");

        public BalanceClientActor()
        {
            Receive<AuthenticateRequest>(request =>
            {
                Console.WriteLine("Connecting....");
                _server.Tell(request);
            });

            Receive<AuthenticateResponse>(response =>
            {
                Console.WriteLine("Connected!");
                Console.WriteLine(response.Message);

                Preloader.Show = false;
            });


            Receive<BalanceRequest>(request =>
            {
                Console.WriteLine("Sending money ....");
                _server.Tell(request);
            });

            Receive<BalanceResponse>(response =>
            {
                Console.WriteLine(response.Message);
            });

            Preloader.Show = false;
        }
    }
}
