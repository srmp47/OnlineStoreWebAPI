# Integration Tests

This folder contains integration tests for the OnlineStoreWebAPI project. Integration tests verify that multiple components work together correctly, including the full HTTP pipeline from controller to database.

## Structure

```
Integration/
├── TestWebApplicationFactory.cs    # Custom factory for test setup
├── TestDataSeeder.cs              # Seeds test data in in-memory database
├── BaseIntegrationTest.cs         # Base class with common functionality
├── Controllers/                   # Integration tests for controllers
│   ├── ProductControllerIntegrationTests.cs
│   ├── UserControllerIntegrationTests.cs
│   ├── OrderControllerIntegrationTests.cs
│   └── OrderItemControllerIntegrationTests.cs
└── README.md                      # This file
```

## Key Components

### TestWebApplicationFactory
- Configures the application for testing
- Replaces SQLite database with in-memory database
- Seeds test data automatically

### TestDataSeeder
- Provides test data for all entities (Users, Products, Orders, OrderItems)
- Includes methods to clear and reset test data
- Ensures consistent test state

### BaseIntegrationTest
- Abstract base class for all integration tests
- Provides HTTP client and database context
- Includes helper methods for HTTP operations
- Manages test data lifecycle

## How Integration Tests Work

1. **Setup**: Each test class inherits from `BaseIntegrationTest`
2. **Database**: Uses in-memory Entity Framework database
3. **HTTP Pipeline**: Tests go through the full HTTP pipeline (Controller → Service → Repository → Database)
4. **Data Isolation**: Each test starts with fresh seeded data
5. **Assertions**: Verifies HTTP status codes, response content, and database state

## Running Integration Tests

```bash
# Run all integration tests
dotnet test --filter "Category=Integration"

# Run specific integration test class
dotnet test --filter "FullyQualifiedName~ProductControllerIntegrationTests"

# Run with verbose output
dotnet test --verbosity normal
```

## Best Practices

1. **Test Naming**: Use descriptive names that explain the scenario and expected outcome
2. **Arrange-Act-Assert**: Follow the AAA pattern for test structure
3. **Data Isolation**: Each test should be independent and not rely on other tests
4. **HTTP Status Codes**: Always verify the correct HTTP status code is returned
5. **Response Content**: Verify the response content matches expectations
6. **Database State**: Verify that database operations work correctly

## Example Test Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedOutcome()
{
    // Arrange - Set up test data and conditions
    
    // Act - Perform the action being tested
    
    // Assert - Verify the results
}
```

## Adding New Integration Tests

1. Create a new test class inheriting from `BaseIntegrationTest`
2. Add test methods following the naming convention
3. Use the helper methods from `BaseIntegrationTest` for HTTP operations
4. Add appropriate assertions using FluentAssertions
5. Consider adding test data to `TestDataSeeder` if needed

## Benefits of Integration Tests

- **End-to-End Testing**: Tests the complete flow from HTTP request to database
- **Real Behavior**: Tests actual application behavior, not mocked components
- **Regression Detection**: Catches issues that unit tests might miss
- **API Contract Validation**: Ensures API endpoints work as expected
- **Database Integration**: Verifies database operations and relationships 