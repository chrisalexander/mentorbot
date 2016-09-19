using Akka.Actor;
using MargieBot.Models;

namespace Mentorbot.Executable.Actors
{
    class Conversation : ReceiveActor
    {
        public Conversation(string id)
        {
            this.Initialise();
        }

        private void Initialise()
        {
            Receive<ResponseContext>(r =>
            {
                Sender.Tell(new ContextMessage("Received", r));
            });
        }
    }
}
