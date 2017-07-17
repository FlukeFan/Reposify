
Reposify
========

A generic C# .Net Repository with a primary focus of testability.

Building
========

To build, open CommandPrompt.bat, and type 'b'.

Build commands:

b                               : build
b /t:clean                      : clean
b /t:RestorePackages            : Restore NuGet packages
b /t:setApiKey /p:apiKey=[key]  : set the api key
b /t:push                       : Push packages to NuGet and publish them (setApiKey before running this)
