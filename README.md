# Clear The Grid code, Solver and GUI

This is the software used to solve the codecompetitions' puzzles. It contains parts of the original starterkit for level import and move validation and creation, and a completely new application using a genetic algoritm, a Database for storage of solutions and settings/parameters and a display/graph for performance review.

!!!IMPORTANT!!!

Before running the application, please ensure that the connectionString used in the DAL project is correct to be used with SQL Server.

Example:
optionsBuilder.UseSqlServer(@"Data Source=localhost\SqlExpress01;Trusted_Connection=True;TrustServerCertificate=True;Initial Catalog=ClearTheGridDB;");

After this, it is possible to either build the database in the DAL as startup project using the NuGet package manager with:
update-database

Or restore the attached backup of the database using SQLManagement studio.

In the bin folder after building:
Make sure the 'Levels' folder exists with the level subfolders containing the maps as supplied in the StarterKit.

When the steps above are done, the program can be run using the GUI project as startup project
