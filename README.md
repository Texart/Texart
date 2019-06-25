# Texart

[![Build Status](https://dev.azure.com/Texart/Texart/_apis/build/status/Texart.Texart?branchName=master)](https://dev.azure.com/Texart/Texart/_build/latest?definitionId=1&branchName=master)

![Demo](/doc/demo.gif)

[Browse the documentation here](doc/index.md)

## Building

This project uses .NET Core 3.0 and C# 8.0. At the time of writing (05/30/2019), these are only released in preview. You should:

* Install the latest [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/) (`16.1`+). [JetBrains Rider](https://www.jetbrains.com/rider/) _may_ also work.
* Install [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0) (`3.0.100-preview6-012264` at the time of writing).
* Enable .NET Core 3.0 preview if necessary. In Visual Studio 2019, ensure `Tools` > `Options` > `Environment` > `Preview Features` > `Use Previews of the .NET Core SDK` is checked.
* Open `Texart.sln` in Visual Studio 2019 and you are good to go!
