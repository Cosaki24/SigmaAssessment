# Sigma Assessment

## About the project
This is a Web API application to manage job candidates.
It has one endpoint to add a new candidate or update if the candidate already exists.

### Assumptions
- Candidates location is Tanzania (TZ) therefore, phone numbers will be validated with Tanzanian country code (+255)
- Free text comment could be a short description about the candidate.

## Tools
- .NET v6.0
- Data store is a Microsoft SQL Server Database with EntityFrameworkCore support
- Unit Tests utilizes the NUnit Framework with FluentAssertions, FakeItEasy and InMemory database
- Swagger Documentation for Development environment

## Things to be implemented
- Addition of more endpoints to return candidate information after querying (GetAllCandidates, GetCandidateByEmail, DeleteCandidate)
- Api authentication

## Time Spent
6 hours.
  
