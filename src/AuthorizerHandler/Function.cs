using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthorizerHandler
{
    public class Function
    {
        /// <summary>
        /// A simple function that authorizes call to api gateway
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayCustomAuthorizerResponse FunctionHandler(APIGatewayCustomAuthorizerRequest input, ILambdaContext context)
        {
            context.Logger.LogLine(System.Text.Json.JsonSerializer.Serialize(input));

            var allowedUserAgent = Environment.GetEnvironmentVariable("AllowedUserAgent") ?? "";
            context.Logger.LogLine($"env variable: {allowedUserAgent}");

            var userAgent = input.RequestContext.Identity.UserAgent;
            context.Logger.LogLine($"userAgent: {userAgent}");

            var apiKey = input.Headers["x-api-key"];
            context.Logger.LogLine($"apiKey: {apiKey}");

            var policy = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>()
            };

            policy.Statement.Add(new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
            {
                Action = new HashSet<string>(new[] { "execute-api:Invoke" }),
                Effect = allowedUserAgent.Equals(userAgent) ? "Allow" : "Deny",
                Resource = new HashSet<string>(new[] { input.MethodArn })
            });

            var contextOutput = new APIGatewayCustomAuthorizerContextOutput
            {
                ["User"] = "User",
                ["Path"] = input.MethodArn
            };

            return new APIGatewayCustomAuthorizerResponse
            {
                PrincipalID = "User",
                Context = contextOutput,
                PolicyDocument = policy,
                UsageIdentifierKey = apiKey
            };
        }
    }
}
