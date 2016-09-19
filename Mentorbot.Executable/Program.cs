using System;
using Akka.Actor;
using Mentorbot.Executable.Actors;
using Mentorbot.Executable.Messages;

namespace Mentorbot.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("Mentorbot");

            var slackbot = system.ActorOf<Slackbot>("slackbot");
            slackbot.Tell(SlackbotInstruction.Start);

            Console.ReadLine();
        }
    }
}
