
[![Build Status](https://ci.appveyor.com/api/projects/status/github/FlukeFan/Reposify?svg=true)](https://ci.appveyor.com/project/FlukeFan/Reposify) <pre>

Reposify
========

A generic C# .Net Repository with a primary focus of testability.

Building
========

Pre-requisites
--------------

* Docker;
* .NET SDK specified in global.json.

To build:

1. Open CommandPrompt.bat as administrator;
2. Type 'dcud' (docker compose up detach);
3. Type 'br' (restores NuGet packages);
4. Type 'b' to build.

Build commands:

br                                      Restore dependencies (execute this first)
b                                       Dev-build
ba                                      Build all (including slow tests and coverage)
bw                                      Watch dev-build
bt [test]                               Run tests with filter Name~[test]
btw [test]                              Watch run tests with filter Name~[test]
bc                                      Clean the build outputs

dcu                                     Docker compose up
dcud                                    Docker compose up (detach)

b /t:setApiKey /p:apiKey=[key]          Set the api key
b /t:push                               Push packages to NuGet and publish them (setApiKey before running this)
