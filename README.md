# ef-generic-repository
This repository contains Entity Framework (EF 8) generic repository with various data operation and extension methods.

# Repository Features / Methods
- [x] Queryable Fetch
- [x] Get
- [x] Get All
- [x] Get Paginated Queryable
- [x] Get Paginated Response
- [x] Add
- [x] Multiple/Range Add
- [x] Update
- [x] Multiple/Range Update
- [x] Delete
- [x] Multiple/Range Delete
- [ ] Change Detection and ExecuteUpdate (Need to update for UpdateRange: https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete)
- [ ] ExecuteDelete (Need to update for RemoveRange: https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete)
- [ ] Application of extensions on generic repository methods.

# Extensions Features
- [x] OrderBy Extension for dynamic ordering on front-end call (like: Server request when DataTable ordering change event fired: 'orderBy: name asc' or 'orderBy: name desc')
- [x] FilterBy Extension for dynamic filtering with operators on front-end call (like: Server request when DataTable search event fired: 'filterBy: Name Equal B' or 'filterBy: Id GreaterThan 5')
  - Supported Operators: ```Equal```, ```NotEqual```, ```GreaterThan```, ```GreaterThanOrEqual```, ```LessThan```, ```LessThanOrEqual```, ```Contains```, ```StartsWith```, ```EndsWith```
  - Supported Data Types: ```String```, ```Bool```, ```Int```, ```Float```, ```Decimal```, ```DateTime```, ```List<int>```
- [x] Exclude Extension for excluding fields from entity object. (Example using: ```queryable.ExcludeProperties("CreatedAt", "Password")```)
- [x] Support for x-level nested properties on filterBy and orderBy.
- [x] Support for multiple contains for int arrays. (Example: ```queryable.FilterBy("Id Contains [1,2]")``` or ```queryable.FilterBy("SubEntity.Id Contains [1,2]")```)
- [ ] Support access property of collection or list of entity object on filterBy and orderBy.
- [x] Support multiple params for filterBy. (Example: ```queryable.FilterBy("Id Contains [1,2]", "SubEntity.Age Equal 22", "IsActive Equal true"))```)
- [x] Support for and/or operator in filter expression even with complex ones. 
  - ```queryable.FilterBy("Id Equal 1 AND IsActive Equal True", "Amount GreaterThan 10")```
  - ```queryable.FilterBy("Id Equal 1 OR Id Equal 2", "Amount GreaterThan 19 OR SubEntity.Age GreaterThan 20")```

# Roadmap

| Milestone                  | Features/Tasks                                                                                                                                                   | Priority   | Status     | Estimated Completion |
|----------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------|------------|-----------------------|
| **Core Repository Methods** |                                                                                                                                                                 |            |            |                       |
| Implement `Change Detection` and `ExecuteUpdate` | Add support for detecting changes in entities and updating ranges efficiently.                                                                          | High       | Pending    | Q1 2025              |
| Implement `ExecuteDelete`                     | Enable bulk deletion using `ExecuteDelete` for removing ranges of entities.                                                                             | High       | Pending    | Q1 2025              |
| **Extensions Enhancements** |                                                                                                                                                                 |            |            |                       |
| Access Nested Collections                   | Add support for accessing properties of collections or lists on `filterBy` and `orderBy`.                                                              | Medium     | Pending    | Q2 2025              |
| Optimize FilterBy Expressions               | Enhance `filterBy` to support complex filters with nested entities, collections, and operators seamlessly.                                              | High       | In Progress| Q1 2025              |
| Add Application of Extensions              | Apply reusable extensions to generic repository methods (e.g., custom validations, pre/post processing).                                                | Medium     | Pending    | Q2 2025              |
| **Performance and Testing**  |                                                                                                                                                                |            |            |                       |
| Optimize Queryable Fetch Methods            | Refactor query methods to ensure performance and scalability for large datasets.                                                                        | High       | In Progress| Q1 2025              |
| Unit Testing and Benchmarking               | Create comprehensive unit tests and performance benchmarks for repository methods and extensions.                                                       | High       | In Progress| Q1 2025              |
| **Documentation and Usability** |                                                                                                                                                             |            |            |                       |
| Developer Guide                              | Provide detailed usage examples and a developer guide for `filterBy`, `orderBy`, and other extensions.                                                  | Medium     | In Progress| Q1 2025              |
| API Documentation                            | Generate API documentation for repository methods and extension features.                                                                               | Medium     | Pending    | Q1 2025              |

- **Priority**: High, Medium, Low
- **Status**: Pending, In Progress, Completed
