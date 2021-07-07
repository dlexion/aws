# Welcome to your CDK C# project!

This is a blank project for C# development with CDK.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET Core CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template

## Steps to deploy

1. cdk bootstrap --profile personal
2. cdk deploy PipelineDeployingLambdaStack --profile personal

## Steps to delete
1. cdk destroy LambdaStack --profile personal
2. cdk destroy PipelineDeployingLambdaStack --profile personal