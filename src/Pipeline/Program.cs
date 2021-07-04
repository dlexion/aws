using Amazon.CDK;

namespace Pipeline
{
    class Program
    {
        const string CodecommitRepoName = "aws-mentoring-repo";

        static void Main(string[] args)
        {
            var app = new App();

            var lambdaStack = new LambdaStack(app, "LambdaStack");
            new PipelineStack(app, "PipelineDeployingLambdaStack", new PipelineStackProps
            {
                LambdaCode = lambdaStack.LambdaCode,
                RepoName = CodecommitRepoName
            });

            app.Synth();
        }
    }
}