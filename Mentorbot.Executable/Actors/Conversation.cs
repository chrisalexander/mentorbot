using System.Linq;
using Akka.Actor;

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
            Receive<AugmentedMessage>(m =>
            {
                Sender.Tell(new ResponseMessage(string.Join("\r\n\r\n", m.Analysis.Select(a => a.AnalyzerId + ":\r\n" + a.Result.ToString())), m));
            });
        }
    }
}
