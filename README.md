# CyberPets 2077

## Usage

The project requires dotnetcore 3.1 and was written using Visual Studio Code.

- To run the project.

```sh
dotnet run -p CyberPets.API
```

You can access swagger documentation at http://localhost:5000/swagger to directly invoke the APIs from your browser.

- To install all the packages and compile the project

```sh
dotnet build
```

- To run the project and rebuild on file change

```sh
dotnet watch -p CyberPets.API run
```

- To run the test suite

```sh
dotnet test
```

## Solution structure

The solution is divided in 4 projects.

- CyberPets.API - the AspNetCore MVC microservice. It contains the routes and the DTOs (data transfer objects).
- CyberPets.Domain - The domain entities, the services and the repository interface.
- CyberPets.Infrastructure - The repository implementation.
- CyberPets.Test - The unit tests for all those projects.

This project structure is based on [Design a DDD-oriented microservice](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice)

## Authentication

**Note: This is not a proposed auth solution, there are better ways to do authentication and authorization, is just an hypotetical scenario!**

Let's assume that we have a very modern and secure [API Gateway](https://aws.amazon.com/api-gateway/) or an [Application Load Balancer](https://docs.aws.amazon.com/elasticloadbalancing/latest/application/introduction.html) in front of several instances of our microservice running for example on [EKS](https://aws.amazon.com/eks/).

Let's assume that the API Gateway or ALB is configured to verify the identity of the user (for example with a JWT token) and to set the header X-USER-ID with a valid user id if the user is authenticated (could be an authorizer lambda, for example). All APIs that require an authenticated user will receive a valid X-USER-ID header and will not be invoked if the user is not authenticated.

To simplify the development authorization is just executed using a bit of logic in the Controller class. One option would be to use [ASP.NET Core Identity](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/) but I believe it would be out scope for this demo.

## The APIs

The services exposes [Level-1 RESTful APIs](https://martinfowler.com/articles/richardsonMaturityModel.html).

- GET /PetKinds returns the list of available pets. This endpoint is public.
- GET /UserPets returns all of pets owned by the current user.
- GET /UserPets/{GUID} returns a single pet owned by the current user.
- POST /UserPets {kind:rabbit|cat|dog|dragon} creates a new pet of the specified kind for the current user. Returns the newly created pet.
- DELETE /UserPets/{GUID} deletes a pet. Don't worry, no pet gets harmed, will be sent to another dimension and adopted by a lovely family.
- PUT /UserPets/{GUID}/caress increase the happiness of a pet up to the maximum value.
- PUT /UserPets/{GUID}/feed decreases hunger, up to the maximum value.

**Check http://localhost:5000/swagger for details about the returned types and required arguments.**

All APIs except GET /PetKinds require the X-USER-ID header and will allow the user to crate list, view, feed and caress only owned pets.
Swagger allows to type in the X-USER-ID header as a normal field, remember to type in something there (any non empty string).

## Database

IUserPetsRepository is the interface that abstract the access to the data store.

The only implementation of this interface available at this stage is UserPetsInMemoryRepository, a thread-safe non-locking in-memory db that lives only while the service is running and is not shared between multiple instances. It should NOT be used in production, never, but is perfect for local development and unit testing.

For updating hunger and happiness of a pet, the interface is designed to be able to use [optimistic locking](https://en.wikipedia.org/wiki/Optimistic_concurrency_control) to avoid data contention and ensure an high level of parallelism.
You can see an example implementation in UserPetsInMemoryRepository that uses [Interlocked.CompareExchange](https://docs.microsoft.com/en-us/dotnet/api/system.threading.interlocked.compareexchange?view=netcore-3.1).

There are few databases that would perfectly fit the scenario, the choice depends on several factors that should be discussed, the required level of concurrency or atomicity, consistency or availability.

- SQL: Is ACID and offers the best consistency but cannot be scaled horizontally easily if the number of concurrent requests is high.
- MongoDB/DocumentDB: a good option, but can be difficult to scale horizontally.
- Redis cluster: Very fast, can be very expensive and we may face consistency issues, it may or not may be a good choice depending on the requirements.
- etcd: A distributed, reliable key-value store. Is fast and scales well.

Using an SQL repository implementation, optimistic locking update would be something like this [pseudocode]:

```sql
    UPDATE UserPets
    SET HappinessLastValue=@newHappiness, HappinessLastUpdateDateTime=@now
    WHERE HappinessLastUpdateDateTime=@lastHappinessUpdateDateTime AND HappinessLastValue=@lastHappinessLastValue AND Id=@UserPetId
```

## Timer to update the pet metric

Oh well, there is no need to use a timer or another event based solution to do that.
The solution adopted here is to use linear interpolation when computing current hunger or happiness level.

UserPetMetricValue.cs encapsulate in a value type the math required to do this.

```csharp
    public readonly int GetValue(DateTime now, int rateInSeconds) =>
        Math.Clamp(LastValue + (int)((now - LastUpdate).TotalSeconds / rateInSeconds), MinValue, MaxValue);
```

To make the code easily testable, ITimeProvider is used to get the current time and can be mocked during tests.

## Possible improvement and non functional requirements

Unfortunately the specifications were quite vague in terms of where this service would run, how many and what kind of consumers it would have.
However we could already discuss some possible improvements:

- As stated before, a proper and well thought auth system and a proper IRepository implementation with a real database.
- Refactoring, especially in the tests there is quite a lot of repetition and not everything is covered, but let's assume is enough for a demo.
- A maximum number of pets should be decided to avoid the possibility of a single user creating too many entries filling up the database or slowing down the system.
- APIs could follow [HATEOAS](https://en.wikipedia.org/wiki/HATEOAS)/[HAL](https://en.wikipedia.org/wiki/Hypertext_Application_Language) to better support versioning and remove the responsibility of the client of generating URLs.
- Integration and basic E2E testing is not present and should be implemented before going live.
- Logging, monitoring, alerts are required in any service that goes live. CloudWatch or DataDog or any other monitoring solution already adopted would work well.
- We should record all actions that a user executes in the system for security and to recover the state of the system in case of bugs or mistakes (Audit / Event sourcing). Can be as simple as logging or can be more advanced like an event SQL table or Kafka or another stream.
- To automatically scale the service and to reduce the associated maintanance costs one option would be to implement [Azure Functions](https://azure.microsoft.com/en-gb/services/functions/) or an [AWS Lambdas](https://aws.amazon.com/lambda/) based microservice. Depending on the expected number of invokations per day can even be cheaper.
- Is important to have a Continuous Integration solution with multistaged deploy (at least two environments, `test` and `live`). For example [GitHub Actions](https://github.com/features/actions).
- Documentation, is always important but is definitely out of scope for this demo.
