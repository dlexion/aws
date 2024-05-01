using Amazon.CDK;
using Amazon.CDK.AWS.CodeDeploy;
using Amazon.CDK.AWS.Lambda;

using System;
using Constructs;

namespace Pipeline
{
    public class LambdaStack : Stack
    {
        public readonly CfnParametersCode LambdaCode;

        public LambdaStack(Construct scope, string id, StackProps props = null) :
            base(scope, id, props)
        {
            LambdaCode = Code.FromCfnParameters();

            var func = new Function(this, "Lambda", new FunctionProps
            {
                Code = LambdaCode,
                Handler = "ApiEventHandler::ApiEventHandler.Functions::Handle",
                Runtime = Runtime.DOTNET_6,
                Description = "Function generated at " + DateTime.Now.ToString("s")
            });

            var version = func.CurrentVersion;
            var alias = new Alias(this, "LambdaAlias", new AliasProps
            {
                AliasName = "Prod",
                Version = version
            });

            new LambdaDeploymentGroup(this, "DeploymentGroup", new LambdaDeploymentGroupProps
            {
                Alias = alias,
                DeploymentConfig = LambdaDeploymentConfig.ALL_AT_ONCE
            });
        }
    }
}