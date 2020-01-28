# AutoSME (.Net Core Web API part)

**Note:** Configure your own ```:appsettings.json```
API is making requests to z3k API and processes shift data to create a list of teams and distribute them among SMEs.

## Structure
**Folders:** 
```
Controllers/        
Mapping/
Model/
SheduleHelper/
```

## Usage

Create distribution - heavy function reading the data from /schedule and caching it to the database for faster data loading.
Get distribution - getting the saved distribution from the database.
Get distribution info - getting additional info.
Get shiftInfo - getting shift time, name etc.
Get positions - getting a list of positions and corresponding number of people.


## Security

Kestrel web server is used as reverse proxy behind NGINX which processes and filters incoming requests.
