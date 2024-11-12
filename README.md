# Installation

To get started with the Hacker News App, follow these steps:

Assuming you already extracted the provided archive: simply CD in you're favorite command line utility to the location of extraction.

```shell
CD C:\HackerNews
```

Restore the NuGet packages:

```shell
dotnet restore
```

Build the project:

```shell
dotnet build
```

Run the application:

```shell
dotnet run
```

Observe the CMD log for any errors/warnings. You should receive none, and the line "Now listening on: http://localhost:5090" should appear.
This means that you can send HTTP requests now.

```bash
curl --request GET \
  --url 'http://localhost:5090/HackerNews/GetBestStories?count=5'
```

which will return you 5 "top stories" from the hacker-news.firebaseio.com site.

# Assumptions/possible improvements:

- Assumption during JSON parsing: 
  
  - if Id or returned Item different from provided in the request - ignoring such item
  
  - Considering that documentation states that the "ask" item also has type "story" - distinguish between the to by checking "url" field as well: it should be non-empty for the "story"

- Only top-level "stories" are processed - "kids" are ignored

- Assuming that 200 seconds (1 request per second limit) wait time is acceptable - no asynchronous processing (request => get task ID => check task ID for results) implemented.

- Assuming that a single request to the API completes several times faster than a second - a simple rate limiter "max requests number per second" is implemented.

- Assuming that each Item itself changed rarely - simple "static memory caching" is implemented.

- Error handling/reporting is very basic: either Success of Failure reported to the user.

- No heavy load is expected to this Service: thus no containerization/scaling is implemented.

- Simply "retry on error with increasing pause" error handling strategy implemented in http-client using Polly.

## possible improvements

- if "kids" of each story expected to be processed - internal Scheduler with background processing of the stories graph should be implemented
- use Redis memory cache with more robust caching strategy.
- if heavy load is expected: at least Docker with multiple instances and Load-balancer (like nginx) should be used.
- in case of distributed system - Redis should be used as a stand-alone service.