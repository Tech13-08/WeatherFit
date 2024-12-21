using GroqNet;
using GroqNet.ChatCompletions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WeatherFit.Services
{
    public class GroqService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string ApiKey = "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";

        public async Task<string> GetGroqAnalysis(string weatherData, string closetData)
        {
            var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddGroqClient(ApiKey, GroqModel.LLaMA3_8b);
    }).Build();

            var groqClient = host.Services.GetRequiredService<GroqClient>();
            var content = "";
            List<string> clothes_list = closetData.Split(",").ToList();

            List<string> requests = new List<string>
        {
            "top wear", "bottom wear", "footwear", "outer wear (if none needed say none)", "headwear (if none needed say none)"
        };

            for (int i = 0; i < requests.Count; i++)
            {
                var response = await groqClient.GetChatCompletionsAsync(GeneratePrompt(weatherData, closetData, requests[i]));
                var newContent = response.Choices.First().Message.Content.Trim().ToLower();
                while (!clothes_list.Contains(newContent))
                {
                    response = await groqClient.GetChatCompletionsAsync(GeneratePrompt(weatherData, closetData, requests[i]));
                    newContent = response.Choices.First().Message.Content.Trim().ToLower();
                    if (newContent.Contains("none") || clothes_list.Count == 0)
                    {
                        return content;
                    }
                }
                clothes_list.RemoveAll(r => r == newContent);
                content = content + newContent + ", ";
            }
            return content;
        }

        private GroqChatHistory GeneratePrompt(string weatherData, string closetData, string request)
        {
            var prompt = new GroqChatHistory { new($"You are a weather and fashion expert. I will provide you the current local weather (all in metric units) and current closet inventory for the user. I will then ask you to pick a certain type of clothing from the inventory and you need to pick one that best matches the following: - This item is ideal for the given weather (consider material, color, coverage, layering) Your response should consist of only the one item. No other text, labels, explanations, jargon at all. Weather: {weatherData}  Closet:{closetData} Question: Pick the ideal {request}") };
            return prompt;
        }
    }
}