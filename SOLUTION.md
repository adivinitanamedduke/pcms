Solution Design and trade-offs
API (Backend Architecture) (.NET API)
Hybrid Data Storage Strategy
Dual-Repository Strategy to meet the "in-memory collection" requirement.
	Products (EF Core In-Memory): Used to demonstrate proficiency with Entity Framework. It provides a relational-like experience (ID generation, LINQ support) without needing a SQL Server instance.
	Categories (Dictionary): Implemented using a ConcurrentDictionary<int, Category>. For thread-safe collection management. 

Generic Repository Pattern
To maintain clean code, a Generic Repository Pattern was implemented: 
	Trade-off: While it adds an abstraction layer, it allows the ProductService to remain agnostic of whether the data is in a real DB or RAM.
	Mapping: The IMappable interface was used to decouple Database Entities from Domain Models, ensuring that internal DB structures don't leak into the API responses. 

API Versioning & Routing
Constraint: Lowercase URLs were enforced globally via RouteOptions for REST consistency as required.
Versioning: A query-string based versioning (?api-version=1) was implemented.
Trade-off: Query-string versioning is easier for quick testing in Swagger compared to Header-based versioning. 
