# TweetStream
A Sample Twitter Streaming app with in-memory implementation and calculating statistics

## Solution Structure
The solution is divided into different projects for API, Service Layer, Data/Repository Layer, Consumer service and Producer Service.
It uses an in-memory singleton object as a data store and C#'s ConcurrentQueue for the producer and consumer adding and consuming tweets.

**TweetStream.API** : REST API endpoints for Retrieving the Stats when needed.

**TweetStream.Service**: Service/Business Logic Layer of the application.

**TweetStream.Data**: Data/Repository Layer of the application that retrieves/saves tweet data.

**TweetStream.Contracts**: The contracts that can get nugetized and are publicly available for other services to consume this REST API. It defines the response structure. Can contain SDK methods too that can be used to call the API endpoints.

**TweetStream.Producer**: The Background service that talk to Twitter's streaming API, receives data, and adds it to the Queue. (Should ideally be a seperate app posting to a central Queue like Service Bus).

**TweetStream.Consumer**: The Background service that removes data from the Queue in FIFO and processes it (Including Saving throught Service/Data layers). (Should ideally be a seperate app consuming from a central Queue like Service Bus).

There are also Unit Test Projects for each of the above projects.


## Steps to Run

In order to run the application, Please update the Twitter bearer token in the API project's appsettings.json.

**Twitter.token**

And then run  TweetStream.API project. 

It will open a Console window that shows the Producer/Consumer processing the data.

Visit the Swagger UI to Calculate and Get the Statistics of the Tweets so far:

https://localhost:7204/swagger/index.html

Or use the enclosed **Postman collection json** to hit it and get statistics.

The application was built as a sample, and to be quick and easy to demo.

### Hope you like it :)