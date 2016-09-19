using System;
using Akka.Actor;
using Mentorbot.Executable.Actors;

namespace Mentorbot.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("Mentorbot");

            var slackbot = system.ActorOf<Slackbot>("slackbot");
            slackbot.Tell(new StartSlackbot());

            Console.ReadLine();
        }
    }
}
