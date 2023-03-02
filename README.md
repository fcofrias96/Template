# Template

To start up the project:

1. Sure to set the `Api project` as startup project.

2. Go to Tools > NuGet Package Manager > Package Manager Console.

3. Then in Package Manager Console sure to set `Template.Infrastructure.Identity` as Default project.

4. On the console type `add-migration start -context IdentityContext`, then press Enter.

5. On the console type `Update-database -context IdentityContext`, then press Enter.

6. Then in Package Manager Console sure to set `Template.Infrastructure.Persistence` as Default project.

7. On the console type `add-migration start -context ApplicationDbContext`, then press Enter.

8. On the console type `Update-database -context ApplicationDbContext`, then press Enter.

9. Done!, just run the project.



## Project Credentials

**Super User**
* username: superuser
* password: 123Pa$$word
	
**Admin**
* username: admin
* password: 123Pa$$word

**Moderator**
* username: moderator
* password: 123Pa$$word
	
**Basic User**
* username: basicuser
* password: 123Pa$$word
