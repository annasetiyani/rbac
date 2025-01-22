# Role Based Access Control (RBAC) API 
Simple role based access control API, not using open auth or microsoft generated authentication. 

## Prerequisites
* Install **CLI** for Entity Framework Core 9
   ```
   Run `dotnet tool install --global dotnet-ef --version 9.0.1` in the console.
   ```

* Update **CLI** to Entity Framework Core 9
   ```
   Run `dotnet tool update --global dotnet-ef --version 9.0.1` in the console.
   ```
   
## Solution Structure
    ```
    Rbac Example Solution
    ├ /src
    ├── /Rbac.Api
    ├──── /Controllers
    ├──── appsettings.json // containing application settings including database connection
    ├──── rbac.db          // SQLite Db used in the project
    ├──── Program.cs       // Application configuration
    ├── /Rbac.Core
    ├──── /Data
    ├────── /Entities      //Entity classes
    ├────── /Models        //Model classes
    ├──── /Migrations      //Migration files
    ├──── /Services        //Dependency injection classes
    ├────── /Interfaces    //Interface classes
    ├──────/Providers     //Provider classes
    ├Rbac.sln
    ```
## How to set up local development database
* Paste the setting below in **secrets.json** and replace **[dbName]** with the .db file in the root project, then save the file.
```JSON
{
  "ConnectionStrings": {
    "RbacDbConnection": "Data Source=./[dbName]"
  }
}
```
* Add JWT settings in **secrets.json**. The full JWT settings are in [here](https://docs.google.com/document/d/1wvkmcVsb8eivLv95ENLl16KEnO-9IKIKiMA1idOQfC8/edit?usp=sharing).
* Run the command below in the console to create the database.
```Text
dotnet ef database update -s ..\Rbac.Api\Rbac.Api.csproj
```

# How to run the API
* Clone the repository to your local machine.
* Use any IDE of your choice to open the solution
* Set the **Rbac.Api** project as the startup project.
* Run the command below in the console to start the API.
```Text
dotnet run -p .\Rbac.Api\Rbac.Api.csproj
```
* Open the browser and navigate to `https://localhost:7072/swagger/index.html` to view the API documentation.
* Use the **POST** endpoint to create data.
* Use the **GET** endpoint to get data.
* Use the **UPDATE** endpoint to update data.
* Use the **DELETE** endpoint to delete data.

## Assumptions
1. Roles are predefined and cannot be created dynamically.
2. Permissions are predefined and cannot be created dynamically.
3. Passwords have to be: 
    - At least 8 characters long.
    - At most 20 characters long.
    - Contain at least one uppercase letter.
    - Contain at least one lowercase letter.
    - Contain at least one number.
    - Contain at least one special character.
4. The authorization is not applied during login. 
5. The authorization is not yet applied to all endpoints.
6. Login token is not yet saved in cache or cookies.
7. Logout is not yet implemented.
8. Security is not yet implemented.
9. A proper error handling is not yet implemented.
10. A proper logging is not yet implemented.
11. A proper login process is not yet implemented.