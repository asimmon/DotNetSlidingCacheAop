Sample application that shows how to use an thread-safe aspect to cache the result of any method invocation for a specific duration.

### How it's done

Castle DynamicProxy provides AOP. You can modify the algorithm to work with Microsoft Unity, I've done it in production.

The caching interceptor is meant to be used as a proxy of dependencies resolved with a DI container.

### Requirements to run the tests:

* .NET Core
* Visual Studio 2017, Visual Studio for Mac or .NET Core CLI as the new csproj format is used
