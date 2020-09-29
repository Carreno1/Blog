namespace Portfolio_Blog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Owin.Security.Providers.Yahoo;
    using Portfolio_Blog.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Portfolio_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Portfolio_Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            var roleManager = new RoleManager<IdentityRole>(
               new RoleStore<IdentityRole>(context));
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });

            }


            #region Add User creation here

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "michaelcarrenocc@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "michaelcarrenocc@gmail.com",
                    Email = "michaelcarrenocc@gmail.com",
                    FirstName = "Michael",
                    LastName = "Carreno",
                    DisplayName = "Student",
                };

                userManager.Create(user, "Abc&123!");
                userManager.AddToRole(user.Id, "Admin");


            }

            if(!context.Users.Any(u => u.Email == "arussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "arussell@coderfoundry.com",
                    Email = "arussell@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Russell",
                    DisplayName = "Intructor",
                };
                userManager.Create(user, "Abc&123!");
                userManager.AddToRole(user.Id, "Moderator");
            }



            #endregion







        }
    }
}
