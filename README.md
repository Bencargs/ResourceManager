# ResourceManager Library

<p align="center">
  <img width="256" height="256" src="logo.png">
</p>

## Overview
The ResourceManager library provides a robust solution for managing resource locks within multi-threaded applications. It features a safe and efficient locking mechanism with built-in support for backoff strategies to handle high contention scenarios gracefully.

## Background
When I was a fresh software grad my first job I had the trask of solving a race condition that had lead to a deadlock in oursoftware.
Being totally out of my depth and unable to understand how our resources were be acquired by the program,
I came up with this clever quick and dirty fix.
Classes would register their contentious resource resource requests with a ResourceManager, if it could not acquire a lock it would release all its resources, and retry acquiring them one at a time at a random interval in the future.
It was a pretty nieve apprach, but it was innovative and I was proud of it, so I've kept it here for prosperity.

## Features
- **Lock Acquisition**: Acquire locks on resources with the ability to specify a timeout.
- **Automatic Retries**: Automatically retries lock acquisition with exponential backoff and jitter to reduce collision probabilities under high contention.
- **Thread Safety**: Ensures that resource acquisition and release are thread-safe.

## Getting Started

### Prerequisites
- .NET Framework or .NET Core
- NUnit for running unit tests (if you plan to run the included tests)

### Installing
Clone the repository to your local machine:
```bash
git clone https://github.com/Bencargs/ResourceManager.git
```

Navigate to the project directory:
```powershell
cd ResourceManager
```

Build the project (using dotnet CLI):
```powershell
dotnet build
```

### Usage Example
```csharp
var resources = new[] { "Resource1", "Resource2" };
var resourceManager = new ResourceManager(resources);

// Attempt to lock Resource1
if (resourceManager.TryLock("Resource1", 5, 500)) 
{
    Console.WriteLine("Resource locked successfully.");
} 
else 
{
    Console.WriteLine("Failed to lock the resource.");
}
resourceManager.Release("Resource1");
```

## Contributing
# How to Contribute
Fork the Project
Create your Feature Branch (git checkout -b feature/AmazingFeature)
Commit your Changes (git commit -m 'Add some AmazingFeature')
Push to the Branch (git push origin feature/AmazingFeature)
Open a Pull Request


## License
Distributed under the MIT License. See LICENSE for more information.

## Contact
Ben Cargill - Benjamin.d.cargill@gmail.com
Project Link: https://github.com/bencargs/ResourceManager