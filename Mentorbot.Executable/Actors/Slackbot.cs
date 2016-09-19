using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using MargieBot;
using MargieBot.Models;

namespace Mentorbot.Executable.Actors
{
    class Slackbot : ReceiveActor
    {
        private Bot bot;

        private ConcurrentDictionary<string, IActorRef> conversations = new ConcurrentDictionary<string, IActorRef>();

        private IActorRef augmenter;

        public Slackbot()
        {
            Initialise();
        }

        private void Initialise()
        {
            this.augmenter = Context.ActorOf<Augmentation>();

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

                this.augmenter.Tell(r);
            });

            Receive<AugmentedMessage>(m =>
            {
                var conversation = this.conversations.GetOrAdd(
                                                        m.Context.Message.ChatHub.ID,
                                                        (s) => Context.ActorOf(Props.Create<Conversation>(s)));

                conversation.Tell(m);
            });

            Receive<ResponseMessage>(r =>
            {
                this.bot.Say(new BotMessage { Text = r.Message, ChatHub = r.AugmentedMessage.Context.Message.ChatHub }).PipeTo(Self);
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

    class ResponseMessage
    {
        public ResponseMessage(string message, AugmentedMessage inputMessage)
        {
            this.Message = message;
            this.AugmentedMessage = inputMessage;
        }

        public string Message { get; }

        public AugmentedMessage AugmentedMessage { get; }
    }
}
