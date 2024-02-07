using System.Configuration;
using DalleImageBase64;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;

// Get configuration settings from App.config
string _endpoint = ConfigurationManager.AppSettings["endpoint"]!;
string _apiKey = ConfigurationManager.AppSettings["api-key"]!;
string _dalleDeployment = ConfigurationManager.AppSettings["dalle-deployment"]!;
string _gptDeployment = ConfigurationManager.AppSettings["gpt-deployment"]!;

// Create a kernel builder
var builder = Kernel.CreateBuilder(); 
 
// Add OpenAI services to the kernel
builder.AddAzureOpenAITextToImage(_dalleDeployment, _endpoint, _apiKey);
builder.AddAzureOpenAIChatCompletion(_gptDeployment, _endpoint, _apiKey); 
 
// Build the kernel
var kernel = builder.Build();

// Get AI service instance used to generate images
var dallE = kernel.GetRequiredService<ITextToImageService>();

// create execution settings for the prompt
var prompt = @"
Think about an artificial object that represents {{$input}}.";

var executionSettings = new OpenAIPromptExecutionSettings {
    MaxTokens = 256,
    Temperature = 1
};

// create a semantic function from the prompt
var genImgFunction = kernel.CreateFunctionFromPrompt(prompt, executionSettings);

// Get a phrase from the user
Console.WriteLine("Enter a phrase to generate an image from: ");
string? phrase = Console.ReadLine();
if (string.IsNullOrEmpty(phrase)) {
    Console.WriteLine("No phrase entered.");
    return;
}

// Invoke the semantic function to generate an image description
var imageDescResult = await kernel.InvokeAsync(genImgFunction, new() { ["input"] = phrase });
var imageDesc = imageDescResult.ToString();

// Use DALL-E 3 to generate an image. 
// In this case, OpenAI returns a URL (though you can ask to return a base64 image)
var imageUrl = await dallE.GenerateImageAsync(imageDesc.Trim(), 1024, 1024);

// Display the image URL
Console.WriteLine($"Image URL:\n\n{imageUrl}");

// generate a random number between 0 and 200 to be used for filename
var random = new Random().Next(0, 200);

// use SkiaUtils class to save the image as a .png file
string filename = await SkiaUtils.SaveImageToFile(imageUrl, 1024, 1024, $"{random}-image.png");

// use SkiaUtils class to get base64 string representation of the image
var base64Image = await SkiaUtils.GetImageToBase64String(imageUrl, 1024, 1024);

// save base64 string representation of the image to a text file
File.WriteAllText($"{random}-base64image.txt", base64Image);