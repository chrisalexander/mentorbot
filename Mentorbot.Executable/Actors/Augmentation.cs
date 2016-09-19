using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using MargieBot.Models;
using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;

namespace Mentorbot.Executable.Actors
{
    class Augmentation : ReceiveActor
    {
        private readonly LinguisticsClient client = new LinguisticsClient(File.ReadAllText("CognitiveServicesApiKey.txt"));

        public Augmentation()
        {
            this.Initialise();
        }

        private void Initialise()
        {
            Receive<ResponseContext>(r =>
            {
                this.Process(r).PipeTo(Sender);
            });
        }

        private async Task<AugmentedMessage> Process(ResponseContext r)
        {
            var analysers = await this.client.ListAnalyzersAsync();

            var result = await this.client.AnalyzeTextAsync(new AnalyzeTextRequest { Text = r.Message.Text, Language = "en", AnalyzerIds = analysers.Select(a => a.Id).ToArray() });

            return new AugmentedMessage(r, result); 
        }
    }

    class AugmentedMessage
    {
        public AugmentedMessage(ResponseContext context, AnalyzeTextResult[] analysisResult)
        {
            this.Context = context;
            this.Analysis = analysisResult;
        }

        public ResponseContext Context { get; }

        public AnalyzeTextResult[] Analysis { get; }
    }
}
