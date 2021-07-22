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
            var a = input.Headers.TryGetValue("User-Agent", out var agent);

            context.Logger.LogLine(a ? agent : "no agent");

            var principalId = input.RequestContext.Authorizer["principalId"];
            context.Logger.LogLine($"principal: {principalId}");

            var apiKey = input.RequestContext.Authorizer["key"];
            context.Logger.LogLine($"apiKey: {apiKey}");

            var apiKeyInHeader = input.Headers["x-api-key"];
            context.Logger.LogLine($"apiKeyInHeader: {apiKeyInHeader}");

            var keys = input.RequestContext.Authorizer.Keys;
            context.Logger.LogLine($"keys: {string.Join('|', keys)}");

            APIGatewayCustomAuthorizerPolicy policy = new APIGatewayCustomAuthorizerPolicy
            {
                Version = "2012-10-17",
                Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>()
            };

            policy.Statement.Add(new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
            {
                Action = new HashSet<string>(new string[] { "execute-api:Invoke" }),
                Effect = "Allow",
                Resource = new HashSet<string>(new string[] { input.MethodArn })

            });

            APIGatewayCustomAuthorizerContextOutput contextOutput = new APIGatewayCustomAuthorizerContextOutput();
            contextOutput["User"] = "User";
            contextOutput["Path"] = input.MethodArn;

            return new APIGatewayCustomAuthorizerResponse
            {
                PrincipalID = "User",
                Context = contextOutput,
                PolicyDocument = policy,
                UsageIdentifierKey = "123"//input.Headers["x-api-key"]
            };
        }
    }
}
