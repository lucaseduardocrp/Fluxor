<img src="https://raw.githubusercontent.com/lucaseduardocrp/Fluxor/refs/heads/master/assets/fluxor-icon.png" alt="NexumPack Fluxor Logo" width="200px" />

# Fluxor

A lightweight and extensible library for managing data flow and events in .NET applications, inspired by MediatR. Ideal for **CQRS-based architectures**, promoting separation of concerns and enhancing maintainability and scalability.

| Package            | Version                                                                                                          | Popularity                                                                                                        |
| ------------------ | ---------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| `NexumPack.Fluxor` | [![NuGet](https://img.shields.io/nuget/v/NexumPack.Fluxor.svg)](https://www.nuget.org/packages/NexumPack.Fluxor) | [![Nuget](https://img.shields.io/nuget/dt/NexumPack.Fluxor.svg)](https://www.nuget.org/packages/NexumPack.Fluxor) |

---

## ⭐ Give it a star

If you find this project helpful, consider giving it a ⭐ on [GitHub](https://github.com/lucaseduardocrp/Fluxor) to support the development.

---

## Features

- ✅ Lightweight and modular
- ✅ Supports command, event, and query handlers
- ✅ DI-ready with automatic registration
- ✅ Clear separation of concerns
- ✅ Based on `Microsoft.Extensions.DependencyInjection`

---

## 🚀 Getting Started

### Installation

Via .NET CLI:

```bash
dotnet add package NexumPack.Fluxor
```

### Using Contracts-Only Package

To reference only the contracts for Fluxor, which includes:

- `IRequest` (including generic variants)
  - Represents a command or query that expects a single response
- `INotification`
  - Represents an event broadcast to multiple handlers (if any)

### Advanced Usage: Request + Notification

This example demonstrates how to combine a `Request` (command/query) and a `Notification` (event) in a real-world use case.

> ✅ This scenario uses only `Microsoft.Extensions.DependencyInjection.Abstractions` for DI registration — no framework-specific packages.

---

#### 1. Define the Request and Notification

```csharp
public class CreateCustomerCommand : IRequest<string>
{
    public string Name { get; set; }
}

public class CustomerCreatedEvent : INotification
{
    public Guid CustomerId { get; }

    public CustomerCreatedEvent(Guid customerId)
    {
        CustomerId = customerId;
    }
}
```

---

#### 2. Implement the Handlers

```csharp
public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, string>
{
    private readonly IMediator _mediator;

    public CreateCustomerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        // Simulate persistence...

        // Publish event
        await _mediator.Publish(new CustomerCreatedEvent(id), cancellationToken);

        return $"Customer '{request.Name}' created with ID {id}";
    }
}

public class SendWelcomeEmailHandler : INotificationHandler<CustomerCreatedEvent>
{
    public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending welcome email to customer {notification.CustomerId}");
        return Task.CompletedTask;
    }
}
```

---

#### 3. Register the Handlers (Dependency Injection)

You can register everything manually if you want full control:

```csharp
services.AddSingleton<IMediator, Mediator>();

services.AddTransient<IRequestHandler<CreateCustomerCommand, string>, CreateCustomerHandler>();
services.AddTransient<INotificationHandler<CustomerCreatedEvent>, SendWelcomeEmailHandler>();
```

Or use assembly scanning with:

```csharp
services.AddFluxor();
```

---

#### 4. Execute the Flow

```csharp
public class CustomerAppService
{
    private readonly IMediator _mediator;

    public CustomerAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> CreateCustomer(string name)
    {
        return await _mediator.Send(new CreateCustomerCommand { Name = name });
    }
}
```

---

When the `CreateCustomer` method is called:

1. `CreateCustomerHandler` handles the request
2. It creates and persists the customer (simulated)
3. It publishes a `CustomerCreatedEvent`
4. `SendWelcomeEmailHandler` handles the event

This structure cleanly separates **commands** (which change state and return a result) from **notifications** (which communicate to the rest of the system that something happened).

## Features

- **Lightweight**: Minimal dependencies and straightforward setup.
- **In-Process Messaging**: Facilitates in-process communication between components.
- **Handler Registration**: Automatically registers handlers from specified assemblies.

## Compatibility

NexumPack.Fluxor targets .NET Standard 2.1, and is compatible with .NET Core 3.1+, .NET 5+, .NET 6+, .NET 7+, .NET 8, NET 9 and newer versions of the .NET runtime.

## About

NexumPack.Fluxor was developed by [Lucas Eduardo](https://www.linkedin.com/in/lucascrp/) under the MIT license.
