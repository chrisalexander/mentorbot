using Akka.Actor;
using Mentorbot.Executable.Messages;

namespace Mentorbot.Executable.Actors
{
    class Slackbot : ReceiveActor
    {
        public Slackbot()
        {
            Receive<SlackbotInstruction>(i => this.ProcessInstruction(i));
        }

        private void ProcessInstruction(SlackbotInstruction i)
        {
            
        }
    }
}
