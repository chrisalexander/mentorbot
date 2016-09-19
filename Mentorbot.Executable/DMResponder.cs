using System;
using System.Threading.Tasks;
using MargieBot.Models;
using MargieBot.Responders;

namespace Mentorbot.Executable
{
    class DMNotifier : IResponder
    {
        private readonly Action<ResponseContext> notify;

        public DMNotifier(Action<ResponseContext> notify)
        {
            this.notify = notify;
        }

        public bool CanRespond(ResponseContext context)
        {
            return
                    !context.BotHasResponded                                // Must be new response
                    && context.Message.ChatHub.Type == SlackChatHubType.DM; // Must be a DM
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            notify(context);
            return new BotMessage();
        }
    }
}
