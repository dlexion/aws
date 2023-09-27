using System;
using System.Collections.Generic;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ApiEventHandler
{
    public class Functions
    {
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
        }

        /// <summary>
        /// A Lambda function to respond to HTTP calls from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The API Gateway response.</returns>
        public APIGatewayProxyResponse Handle(System.Text.Json.JsonElement request, ILambdaContext context)
        {
            context.Logger.LogLine("ApiEventHandler lambda");

            using var client = new AmazonDynamoDBClient();
            var table = Table.LoadTable(client, "EmailRequests");

            context.Logger.LogLine($"body: {request}");
            var emailRequest = Document.FromJson(request.ToString());

            emailRequest["Id"] = Guid.NewGuid().ToString();
            emailRequest["Status"] = "Pending";

            table.PutItemAsync(emailRequest).Wait();
            context.Logger.LogLine("Successfully put request to DynamoDB");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "Success",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            context.Logger.Log(response.Body);

            return response;
        }
    }
}
