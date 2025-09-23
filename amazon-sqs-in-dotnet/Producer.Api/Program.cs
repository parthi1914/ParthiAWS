using Amazon.SQS;
using Amazon.SQS.Model;
using Messages;
using Producer.Api.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/register", async (IAmazonSQS sqs, ILogger<Program> logger, UserRegistrationCommand command) =>
{
    // validate the incoming request object
    // register the user into the database
    var userId = Guid.NewGuid();

    // create event to notify external services
    var userRegisteredEvent = new UserRegisteredEvent(userId, command.UserName, command.Email);

    // assume that the name of the registered queue is 'user-registered'
    var queueName = "user-registered";

    // get queue url
    // or create a new queue if it doesnt already exist
    var queueUrl = string.Empty;
    try
    {
        var response = await sqs.GetQueueUrlAsync(queueName);
        queueUrl = response.QueueUrl;
    }
    catch (QueueDoesNotExistException)
    {
        logger.LogInformation("Queue {queueName} doesn't exist. Creating...", queueName);
        var response = await sqs.CreateQueueAsync(queueName);
        queueUrl = response.QueueUrl;
    }

    // create sqs request
    var sendMessageRequest = new SendMessageRequest()
    {
        QueueUrl = queueUrl,
        MessageBody = JsonSerializer.Serialize(userRegisteredEvent)
    };
    logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", queueName, sendMessageRequest.MessageBody);

    // send sqs request
    var result = await sqs.SendMessageAsync(sendMessageRequest);
    return Results.Ok();
});

app.UseHttpsRedirection();
app.Run();