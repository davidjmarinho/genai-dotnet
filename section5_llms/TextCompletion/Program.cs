using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Text.Json.Serialization;


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
// var summarizationPrompt = """

// Summarize the following blog in 1 concise sentence:

// "Microservice architecture has revolutionized the way we build and deploy applications. By breaking down applications into smaller, independent services, developers can work on different components simultaneously, leading to faster development cycles and more scalable solutions. Each microservice can be developed, deployed, and scaled independently, allowing for greater flexibility and resilience. This approach also facilitates continuous integration and continuous deployment (CI/CD) practices, enabling teams to deliver updates and new features more frequently. However, adopting microservices also comes with challenges such as increased complexity in managing inter-service communication, data consistency, and monitoring. Overall, microservice architecture offers significant benefits for modern application development when implemented thoughtfully."

// """;
// Console.WriteLine($"\nuser ==> {summarizationPrompt}");

// ChatResponse summarizationResponse = await client.GetResponseAsync(summarizationPrompt);

// Console.WriteLine($"assistant ==> \n{summarizationResponse}");

#endregion

#region Sentiment Analysis
// var analysisPrompt = """
//     You will analyze the sentiment of the following product reviews.
//     Each line is its own revieww. Output the sentiment of each review in bullet points as either 'positive', 'negative', or 'neutral'.

//     I bought this product last week and it works great! I love it.
//     This product is terrible, it broke after one use.
//     I'm not sure how I feel about this product.
//     I found this product based on the reviews. It worked for a bit, and then stopped working.
//     """;

// Console.WriteLine($"\nuser ==> {analysisPrompt}");
// ChatResponse analysisResponse = await client.GetResponseAsync(analysisPrompt);
// Console.WriteLine($"assistant ==> \n{analysisResponse}");
#endregion

#region Structured Output

var carListings = new[]
{
        "Check out this 2015 Toyota Camry for $15,000 with 50,000 miles.",
        "Selling my 2018 Honda Accord, only $18,500 and has 30,000 miles.",
        "2017 Ford Fusion available for $14,500, driven 40,000 miles.",
        "Brand new 2020 Tesla Model 3, priced at $35,000 with just 10,000 miles.",
        "Selling a 2016 Chevrolet Malibu for $13,000, has 60,000 miles.",
        "Lease my 2019 BMW 3 Series, $40,000, only 20,000 miles.",
        "Lease a 2021 Audi A4 for $42,000, with just 15,000 miles.",
};

foreach (var listingText in carListings)
{
    var response = await client.GetResponseAsync<CarDetails>(
        $"""
        Convert the following car listing into a JSON object matching this C# class with the properties:
        Condition: "New" or "Used"
        Make: (car manufacturer)
        Model: (car model)
        Year: (four-digit-year)
        ListingType: "Sale" or "Lease"
        Price: (integer only, no currency symbol)
        Features: (array of short strings, e.g., "Bluetooth", "Backup Camera", etc.)
        TenWordSummary:  exactly ten words to summarize this listing

        Here is the listing: "{listingText}"
        """);

    if (response.TryGetResult(out var info))
    {
        Console.WriteLine($"Listing: {listingText}");
        Console.WriteLine($"Parsed JSON: {System.Text.Json.JsonSerializer.Serialize(
            info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine($"Failed to parse listing: {listingText}");
    }

}

class CarDetails
{
    public required string Condition { get; set; } //e.g., "Used" or "New"
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required int Year { get; set; }
    public CarListingType ListingType { get; set; }
    public required int Price { get; set; }
    public required string[] Features { get; set; }
    public required string TenWordSummary { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
enum CarListingType
{
    Sale,
    Lease
}

#endregion