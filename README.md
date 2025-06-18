A Dynamics 365 Finance and Operations developer faces lot of struggle when connecting to sandbox to debug any critical issue.
Once he gets all the critical information to connect with sandbox, he needs to update the web config file manually which is a text file
using the credentials from lcs and then he need to copy paste the update web config in the dev web root location prior which
the developer preserves the original somewhere in known location.

Once above tasks are done, he has to restart IIS and then continue with this development or debug task. If for some reason he need
to do a full synchronization of his database in dev, he need to re update the original to the current sandbox file and then again
do a restart of IIS. All these thing will are so exhaustive.

As a developer I faced these issues and felt to provide a better solution and here it is. The console application which can hanlde all
the file movement, web config file updation and doing iis restart within a single application.
