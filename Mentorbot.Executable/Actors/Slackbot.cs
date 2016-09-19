using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using MargieBot;
using MargieBot.Models;

namespace Mentorbot.Executable.Actors
{
    class Slackbot : ReceiveActor
    {
        private Bot bot;

        private ConcurrentDictionary<string, IActorRef> conversations = new ConcurrentDictionary<string, IActorRef>();

        public Slackbot()
        {
            Initialise();
        }

        private void Initialise()
        {
            Receive<StartSlackbot>(i =>
            {
                CreateBot().PipeTo(Self);
            });

            Receive<Bot>(bot =>
            {
                this.bot = bot;

                Become(ConnectedBot);
            });
        }

        private void ConnectedBot()
        {
            // TODO upgrade Margie when the proxy issue is sorted
            //Receive<StartTyping>(r => this.bot.SendIsTyping(r.Hub));

            this.bot.Responders.Add(new DMNotifier(Self.Tell));

            Receive<ResponseContext>(r =>
            {
                Self.Tell(new StartTyping(r.Message.ChatHub));

                var conversation = this.conversations.GetOrAdd(
                                                        r.Message.ChatHub.ID,
                                                        (s) => Context.ActorOf(Props.Create<Conversation>(s)));

                conversation.Tell(r);
            });

            Receive<ContextMessage>(r =>
            {
                this.bot.Say(new BotMessage { Text = r.Message, ChatHub = r.Context.Message.ChatHub }).PipeTo(Self);
            });
        }

        private async Task<Bot> CreateBot()
        {
            var bot = new Bot();

            var apiKey = File.ReadLines("SlackApiKey.txt").First();

            await bot.Connect(apiKey);

            bot.Responders.Add(new GeneralResponder());

            return bot;
        }
    }

    class StartSlackbot
    {
    }

    class StartTyping
    {
        public StartTyping(SlackChatHub hub)
        {
            this.Hub = hub;
        }

        public SlackChatHub Hub { get; }
    }

    class ContextMessage
    {
        public ContextMessage(string message, ResponseContext context)
        {
            this.Message = message;
            this.Context = context;
        }

        public string Message { get; }

        public ResponseContext Context { get; }
    }
}
