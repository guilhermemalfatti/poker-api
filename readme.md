# Poker-Api

The API it's a basic implementation of hand evaluation process, poker hand has a plethora of possibilities and so it becomes a complex task to to evaluate all possibilities.

The primary functions of the API include the creation of a new game through a POST request, card drawing via a GET request, and retrieving game results. All operational data is stored in a relational database, with SQLite chosen for simplicity.

A comprehensive documentation for all API endpoints is available within the Swagger documentation.

The service API was created using Microsoft Entity FrameworkCore using net7.0.

### TODOs
- Add unit tests
- Support auto API controller verisioning
- For future DB improvements use EF Migrations feature

### Api documentation

The api documentation can be found through the url `http://localhost:5037` while running the service in development mode.

![image](https://github.com/guilhermemalfatti/poker-api/assets/2362515/2c159495-e805-4417-b794-3c2c509031f0)
