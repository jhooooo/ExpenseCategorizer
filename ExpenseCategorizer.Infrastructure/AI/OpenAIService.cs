using Azure.Identity;
using ExpenseCategorizer.Application;
using ExpenseCategorizer.Domain;
using OpenAI;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text.Json;
using System;

namespace ExpenseCategorizer.Infrastructure
{
    public class OpenAIService : IAIService
    {
        public Task<ExpenseAnalysis> CategorizeAsync(ExpenseCategorizeRequest request)
        {
#pragma warning disable OPENAI001


            const string deploymentName = "gpt-4.1-mini";
            // get endpoint and api key and fail fast if not provided
            string? endpointEnv = Environment.GetEnvironmentVariable("API_ENDPOINT");
            if (string.IsNullOrWhiteSpace(endpointEnv)) throw new InvalidOperationException("API_ENDPOINT environment variable is not set.");
            string endpoint = endpointEnv;

            string? apiKeyEnv = Environment.GetEnvironmentVariable("API_KEY");
            if (string.IsNullOrWhiteSpace(apiKeyEnv)) throw new InvalidOperationException("API_KEY environment variable is not set.");
            string apiKey = apiKeyEnv;

            ResponsesClient client = new(
                credential: new ApiKeyCredential(apiKey),
                options: new ResponsesClientOptions()
                {
                    Endpoint = new Uri(endpoint),
                });

            const string systemPrompt =
                "You are an expense categorization assistant.\r\n\r\n" +
                "You will receive a sentence or phrase describing an expense. Your task is to analyze the input and return a concise summary of the expense in JSON format.\r\n\r\n" +
                "Determine:\r\n\r\n" +
                "* **category**: The most appropriate expense category.\r\n" +
                "* **title**: A short, descriptive title for the expense.\r\n" +
                "* **price**: The expense amount, if provided. If no price is mentioned, return `null`.\r\n" +
                "* **confidence**: Your confidence in the categorization, expressed as a number between `0` and `1`.\r\n\r\n" +
                "Return **JSON only**. Do not include explanations, markdown, or additional text.\r\n" +
                "Use this format:\r\n\r\n{\r\n\"category\": \"string\",\r\n\"title\": \"string\",\r\n\"price\": 0.00,\r\n\"confidence\": 0.00\r\n}\r\n";

            CreateResponseOptions options = new()
            {
                Model = deploymentName,
                InputItems =
                {
                    ResponseItem.CreateUserMessageItem(systemPrompt),
                    ResponseItem.CreateUserMessageItem(request.Description ?? string.Empty)
                }
            };

            ResponseResult response = client.CreateResponse(options);

            string output = response.GetOutputText();
            Console.WriteLine($"[ASSISTANT]: {output}");

            var analysis = new ExpenseAnalysis();

            try
            {
                using var doc = JsonDocument.Parse(output);
                var root = doc.RootElement;

                if (root.TryGetProperty("category", out var categoryProp) && categoryProp.ValueKind == JsonValueKind.String)
                {
                    analysis.Category = categoryProp.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("title", out var titleProp) && titleProp.ValueKind == JsonValueKind.String)
                {
                    analysis.SubCategory = titleProp.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("price", out var priceProp))
                {
                    if (priceProp.ValueKind == JsonValueKind.Number && priceProp.TryGetDecimal(out var price))
                    {
                        analysis.Amount = price;
                    }
                    else
                    {
                        analysis.Amount = 0m;
                    }
                }

                if (root.TryGetProperty("confidence", out var confProp))
                {
                    if (confProp.ValueKind == JsonValueKind.Number && confProp.TryGetDecimal(out var conf))
                    {
                        analysis.Confidence = conf;
                    }
                }
            }
            catch (JsonException)
            {
                analysis.Category = "Other";
                analysis.SubCategory = output?.Trim() ?? string.Empty;
                analysis.Amount = 0m;
                analysis.Confidence = 0m;
            }

            return Task.FromResult(analysis);
        }
    }
}
