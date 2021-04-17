# Foosball Api ⚽

`Foosball Api`, A REST api written in C# `.NET CORE WEB API 3.1` and uses `Postgresql` database.  

We use `Entity framework Core` for our ORM.

[![forthebadge](http://forthebadge.com/images/badges/built-with-love.svg)](http://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](http://forthebadge.com) [![forthebadge](http://forthebadge.com/images/badges/makes-people-smile.svg)](http://forthebadge.com)
</div>

# Development

Run the project with the following command
```sh
dotnet run
```

The project run on port `5001` -->  `https://localhost:5001/swagger/index.html`  

If the project is run in `Visual Studio` by clicking the play button the project runs at port `44329` --> `https://localhost:44329/swagger/`

## Entity Framework Core

```sh
dotnet ef migrations add InitialMigration
dotnet ef database update
```

When adding new tables to the database.


1. Delete migrations folder

2. dotnet ef migrations add InitialMigration

3. Comment out the tables in _initialMigration.cs file that already exist. Same with foreign keys that already exist.

4. dotnet ef database update

## Env variables

Secrets are added to `secrets.json` file with `dotnet user-secrets`

To run the project. The following variables are needed

```json
{
  "SmtpUser": "",
  "SmtpPort": "",
  "SmtpHost": "",
  "SmtpEmailFrom": "",
  "JwtSecret": "",
  "DBUserId": "",
  "DBServer": "",
  "DBPort": "",
  "DBPooling": "",
  "DBPassword": "",
  "DBDatabase": "",
  "SmtpPass": ""
}
```

## Technology

[.net core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0)
[EntityFrameworkCore] (https://docs.microsoft.com/en-us/ef/core/)
[postgresql](https://www.postgresql.org/)

## Code Rules

To come.
Remember to format the code using  `dotnet-format`

First install `dotnet-format` globally on your machine:

```sh
dotnet tool install -g dotnet-format
```

Then format the code using: 

```sh
dotnet format
```

## PATCH REQUESTS

when doing patch requests. Have the body something like this:

```json
[
  {
    "op": "replace",
		"path": "/Name",
		"value": "Some new name"
  }
]
```

## Thanks

**Foosball** © 2021+, Mossfellsbær City. Released under the [MIT License].<br>
Authored and maintained by Daniel Freyr Sigurdsson. With help from [contributors].

<!-- > [ricostacruz.com](http://ricostacruz.com) &nbsp;&middot;&nbsp;
> GitHub [@rstacruz](https://github.com/rstacruz) &nbsp;&middot;&nbsp;
> Twitter [@rstacruz](https://twitter.com/rstacruz)

[mit license]: https://mit-license.org/
[contributors]: https://github.com/rstacruz/hicat/contributors
[highlight.js]: https://highlightjs.org -->