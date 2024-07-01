---
languages:
- csharp
products:
- dotnet
- dotnet-orleans
- dotnet-aspire
page_type: sample
name: "Orleans Batch Processing sample app on Aspire"
urlFragment: "orleans-voting-sample-app-on-aspire"
description: "An Orleans sample demonstrating a batch processing system on Aspire."
---

# .NET Aspire Orleans batch processing sample app

This is a simple .NET app that shows how to use Orleans to perform batch processing

## Demonstrates

- How to use Orleans to perform managed batch processing
- How to use a "Governor" to throttle requests
    -  This currently only throttles the number of concurrent larger processes but in the future could include more fine grain throttling
- *NOTE* - This example incorporates a set of "services" that simulates processing time using Delays.  Future iterations will include actual data integration

## Future
- Add Management API to control the Govenor
    - Change Max Capacity on the fly

- Add data intregration
    - Change the processing engine to use "real" data
    - Incorporate retry, restart, persisted run state management, etc.

- Add Cluster Scaling
    - Add a mechanisim to scale a depoyment.  While the Govenor could limit the number of processes and help manage this, it would be good to be able to increase the scale and allow the Govenor to play a part in that

## Sample prerequisites

This sample is written in C# and targets .NET 8.0. It requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

## Building the sample

To download and run the sample, follow these steps:

1. Clone the `https://github.com/scott-wilkos/orleans-batch-processing-on-aspire` repository.
2. In Visual Studio (2022 or later):
    1. On the menu bar, choose **File** > **Open** > **Project/Solution**.
    2. Navigate to the folder that holds the sample code, and open the solution (.sln) file.
    3. Right click the _BatchProcessing.AppHost_ project in the solution explore and choose it as the startup project.
    4. Choose the <kbd>F5</kbd> key to run with debugging, or <kbd>Ctrl</kbd>+<kbd>F5</kbd> keys to run the project without debugging.
3. From the command line:
   1. Navigate to the folder that holds the sample code.
   2. At the command line, type [`dotnet run`](https://docs.microsoft.com/dotnet/core/tools/dotnet-run).

To run the game, run the .NET Aspire app by executing the following at the command prompt (opened to the base directory of the sample):

``` bash
dotnet run --project BatchProcessing.AppHost
```

1. On the **Resources** page, click on one of the endpoints for the **webfrontend** project. This launches the simple .NET app.
2. In the .NET app:
    1. Navigate to the **Engine Simulator** menu.
    2. Enter a value in for the number of "records" to process and press **Simulate Batch Run**
    3. The batch job will begin running and the status will be updated in the grid below

For more information about using Orleans, see the [Orleans documentation](https://learn.microsoft.com/dotnet/orleans).