# SamrtFridge Interface Implmentation

## Questions
1. Is there a preference in the testing framework I should use?
1. Should I include persistent storage or should the system reset every time?
1. Should I include a UI as well as implmentation of the interface?
1. The interface is in Java but the job is for .NET which language should I use?
1. The return value for `GetItems` is not clear, what should the inner array contain?

## Assumptions
1. No preference in testing framework. I chose to use MSTest and Fluent Assertions.
1. No need for persistent storage. I implemented the repository pattern to allow for swaping out the storage if needed at a later time.
1. No need for a UI.
1. Implementation should be in C#
1. For `GetItems` the array should have
   1. Type
   2. Fill Factor
   3. All Items with that Type

## Next Steps
1. Add additional repositories for persistant storage (file, database, web service)
1. Add a UI
1. Add a set of integration tests to make sure the whole system works together not just as parts

## Unit Tests/Code coverage
Currently there are 38 unit test with 100% code coverage of non unit test code
