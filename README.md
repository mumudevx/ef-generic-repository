# ef-generic-repository
This repository contains Entity Framework (EF 8) generic repository with various data operation methods.

# Features / Methods
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
- [x] OrderBy Extension for dynamic ordering on front-end call (like: Server request when DataTable ordering change event fired: 'orderBy: name asc' or 'orderBy: name desc')
