# NewsWebsite
A News Website that uses ASP.NET Core MVC 3.1 and Docker

# Setup
The solution uses Docker with Windows Containers.
* `DockerDevelopmentMode` is set to Regular (yes it's much slower than the Fast Mode) in order to prevent the .NET Docker optimization tools - for maximum encapsulation (https://docs.microsoft.com/en-us/visualstudio/containers/docker-compose-properties?view=vs-2019).
* That means that, in order to run the solution on your PC, you only need Visual Studio (tested with Community 2019 Version 16.5.3 version) and Docker (tested with Docker version 19.03.8, build afacb8b).
* The projects also uses NPM (internally in the containers). However, Visual Studio by default, restores the npm packages if it finds a reference to NPM in your solution. In our case we use the packages only in the containers, not outside of them. That's why we need to disable the automatic download of the npm packages just as explained here: https://stackoverflow.com/questions/31965425/how-to-disable-visual-studio-2015-and-above-automatic-bower-install-on-solution/42064360#42064360.

* The current code will run automatically all the migrations on the start of the project and will seed some sample data by the following method:
```
       private void MigrateAndSeed(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                NewsDbContext newsDbContext = serviceScope.ServiceProvider.GetService<NewsDbContext>();
                while (true) // Try to connect until a successful connection, since Docker has some timing issues when initializing the database container.
                {
                    try
                    {
                        newsDbContext.Database.Migrate();
                        break; // if the migrate is successful, the code flow will reach this break
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex.ToString());
                        Thread.Sleep(15000);
                    }
                }

                this.Seed(newsDbContext);
            }
        }
```
(it's located in the `Startup.cs` file in the `NewsWebsite` project)
If you've completed the steps above. The project should be good and running.

# Development
## Connecting to the database container
1. `docker container ls` - will list all the containers. Grab the Container Id that uses the `microsoft/mssql-server-windows-express:1709` image.
2. `docker container inspect <containerId>` - will show info specific to the container. Grab the `IPAddress`' value
3. Connect to the database container just like you would connect to a database on your PC. For example, you can use Microsoft SQL Server Management Studio (tested with version 18). Use SQL Server Authentication, get the password from the `.env` file (located in the `docker-compose` project, `saUserPassword` environmental variable) and the `DatabaseUser` environmental variable from the `docker-compose.override.yml`. You may also need to specify the port - again from the `.env` file (`newsDatabasePort` environmental variable), however at the moment we are using port `1433` (the default database port), so no specifying is needed.

## Migrations
Since we are using the .NET integrated tools for Docker and another assembly for the migrations, the migrations commands have to be more explicit.
### Creating a migration
`dotnet ef migrations add <MigrationName> --startup-project .\NewsWebsite --project .\DataAccess` - This will create the migration in the DataAccess assembly.

## Logging
The custom exception logger uses Docker volumes, so that even if the container is removed, the logs will stay.
### Activating the cusom logger (Serilog) and using a user-friendly error message
By default (depending on your configuration) .NET Core will log the errors to various places (console, file, etc). However, we may need to save the state of the HTTP request, when the error occurred. For example, will the following line of code `logger.LogError(exceptionHandlerPathFeature.Error, "An exception with the following input occured: HttpContext.Request.Form: {@Form}, HttpContext.Request.Query: {@Query} at the following path: {@Path}", JsonConvert.SerializeObject(HttpContext.Request.Form), JsonConvert.SerializeObject(HttpContext.Request.Query), exceptionHandlerPathFeature.Path);` we will log some of the input values supplied by the user (for easier debugging afterwards).
That's why we use centralized exception handling that redirects to the `HomeController class, Error Action method` when an unhandled exception occurs. The `Error` action method logs the error will additional info supplied by us, sets a generic user-friendly error message for the front-end and redirects to the `Index` action method.
Obviously, we can't use the user-friendly error page and the Developer error page at once. To activate our custom error logging and the user-friendly page, uncomment the following line in the in the `Startup.cs file, Configure method`: `app.UseExceptionHandler("/Home/Error");` and comment out the following line `app.UseDeveloperExceptionPage();`. If you want to get back your Developer error page, do the opposite.
### Locating the custom logs on the host PC
1. `docker container ls` - will list all the containers. Grab the Container Id that uses the image created by the Docker for the current project. (The image name should look like dockercompose13174763213979636238_newswebsite and the container name should be NewsWebsite)
2. `docker container inspect <containerId>` - will show info specific to the container.  In the Mounts section look for a Mount with  "Type": "volume"

Should look something like this:
```
            {
                "Type": "volume",
                "Name": "8fe40fd0acb6c9555a06ed0a5231fa1673db228506b60998685177e1faf42fcc",
                "Source": "C:\\ProgramData\\Docker\\volumes\\8fe40fd0acb6c9555a06ed0a5231fa1673db228506b60998685177e1faf42fcc\\_data",
                "Destination": "c:\\logs",
                "Driver": "local",
                "Mode": "",
                "RW": true,
                "Propagation": ""
            },
```
Get the `Source` path (if you are navigating to it by using the Windows Explorer you will need to remove the double slashes and leave only one slash as a delimiter).
You will see the `NewsWebsiteLogs.txt` that contains the logged information.
The log name can be specified from the `appsettings.Development.json` file, located in the `NewsWebsite` project.
