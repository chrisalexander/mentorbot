using System;
using MargieBot.Models;
using MargieBot.Responders;

namespace Mentorbot.Executable
{
    class GeneralResponder : IResponder
    {
        public GeneralResponder()
        {
        }

        public bool CanRespond(ResponseContext context)
        {
            return
                    !context.BotHasResponded                                // Must be new response
                    && context.Message.ChatHub.Type != SlackChatHubType.DM; // Must not be a DM
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage { Text = "Sorry, I only talk in DMs! Please send me a private message to get started" };
        }
    }
}
