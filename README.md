1. Follow the [AWS MSK gettings started Guide](https://docs.aws.amazon.com/msk/latest/developerguide/getting-started.html) to create the MSK cluster and role.
2. If testing locally, modify the MSK cluster to create a Public Endpoint, still IAM.
3. Update the Boostrap Servers in Program.cs `configMSK`
4. dotnet restore
5. Create the `streams-plaintext-input` topic.
6. dotnet run
7. Send 'hello:world' to the topic `streams-plaintext-input` topic.

Nothing is printed from the console app when sent to the topic. If authentication is changed to using SASL/SCRAM and proper ACLs are setup, then it works.

From the log, it shows that the it doesn't connect to a broker.