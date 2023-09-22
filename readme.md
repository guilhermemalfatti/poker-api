# Poker-Api

The API represents a MVP implementation of the poker game, Evaluating poker hands involves dealing with a lot of possibilities, making it a challenging task to cover everything.

The core functions of the API are creating a new game through a POST request, conducting card draws via a GET request, and retrieving the game result. The data is stored in a relational database, with the choice of SQLite for its simplicity.

A documentation for all API endpoints can be found within the Swagger documentation. The service API has been developed using Microsoft Entity Framework Core within the .NET 7.0 framework.

### TODOs
- Add more unit tests coverage, cover all hands possibilities
- Support auto API controller verisioning
- For future DB improvements use EF Migrations feature

### Api documentation

The api documentation can be found through the url `http://localhost:5037` while running the service in development mode.

![image](https://github.com/guilhermemalfatti/poker-api/assets/2362515/2c159495-e805-4417-b794-3c2c509031f0)
