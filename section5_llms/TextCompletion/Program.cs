using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;


//get credentials from user secrets
IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw

new InvalidOperationException("GitHubModels:Token not found in user secrets"))
{ };

var endpoint = "https://models.github.ai/inference";

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri(endpoint)
};

//create a chat client
IChatClient client = new OpenAIClient(credential, options)
.GetChatClient("openai/gpt-5-mini")
.AsIChatClient();

#region Basic Text Completion

//send prompt and get response
// string prompt = "Quel est la capitale de la Belgique ? expliquez en 10 mots maximum.";

// ChatResponse response = await client
// .GetResponseAsync(prompt);

// Console.WriteLine($"user ==> {prompt}");
// Console.WriteLine($"assistant ==> {response}");
// Console.WriteLine($"Token used: {response.Usage?.InputTokenCount}, output = {response.Usage?.OutputTokenCount}");

#endregion

#region Streaming Text Completion
//send prompt and get response
// string prompt = "Quel sont les problèmes et la bonnes choses du Brazil? expliquez en 200 mots maximum.";
// Console.WriteLine($"user ==> {prompt}");

// var responseStream = client.GetStreamingResponseAsync(prompt);
// await foreach (var message in responseStream)
// {
//     Console.Write(message.Text);
// }

#endregion

#region Classification

// var classificationPrompt = """
//         Please classify the following sentence into one of the categories:
//         - 'complaint'
//         - 'praise'
//         - 'suggestion'
//         - 'other'.

//         1) "I love the new layout!"
//         2) "You should add a night mode."
//         3) "When I try to log in, it keeps failing."
//         4) "This app is decent."
// """;

// Console.WriteLine($"\nuser ==> {classificationPrompt}");

// ChatResponse classificationResponse = await client
//     .GetResponseAsync(classificationPrompt);

// Console.WriteLine($"assistant ==> \n{classificationResponse}");

#endregion

#region Summarization
var summarizationPrompt = """

Summarize the following blog in 1 concise sentence:

"Microservice architecture has revolutionized the way we build and deploy applications. By breaking down applications into smaller, independent services, developers can work on different components simultaneously, leading to faster development cycles and more scalable solutions. Each microservice can be developed, deployed, and scaled independently, allowing for greater flexibility and resilience. This approach also facilitates continuous integration and continuous deployment (CI/CD) practices, enabling teams to deliver updates and new features more frequently. However, adopting microservices also comes with challenges such as increased complexity in managing inter-service communication, data consistency, and monitoring. Overall, microservice architecture offers significant benefits for modern application development when implemented thoughtfully."

""";
Console.WriteLine($"\nuser ==> {summarizationPrompt}");

ChatResponse summarizationResponse = await client.GetResponseAsync(summarizationPrompt);

Console.WriteLine($"assistant ==> \n{summarizationResponse}");

#endregion

#region Sentiment Analysis

#endregion