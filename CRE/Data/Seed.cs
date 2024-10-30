using CRE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CRE.Extensions;

namespace CRE.Data
{
    public class Seed
    {
        public byte[] ReadFileToByteArray(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (IOException ex)
            {
                // Log the exception or handle it as necessary
                Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
                return null; // Or handle the error according to your application's needs
            }
        }
        public static async Task SeedDataAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure roles exist
                foreach (var role in new[] { UserRoles.Chief, UserRoles.Faculty, UserRoles.Evaluator, UserRoles.Secretariat, UserRoles.Chairperson })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Faculty', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Chief', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Chairperson', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Secretariat', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Expertise', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('EthicsEvaluator', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('EthicsApplicationForms', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('EvaluationForms', RESEED, 1);");

                context.Database.EnsureCreated();

                var seed = new Seed(); //intance to use readfile method

                context.Database.EnsureCreated();

                List<AppUser> users = new List<AppUser>();
                if (!context.AppUser.Any()) // Ensure this matches your DbSet name
                {
                    var passwordHasher = new PasswordHasher<AppUser>();

                    users = new List<AppUser>
                    {
                        new AppUser { fName = "Renz Niño", mName = "S", lName = "Baladjay Internal", type = "internal", Email = "renzbaladjay25@gmail.com", UserName = "renzbaladjay25@gmail.com", NormalizedUserName = "RENZBALADJAY25@GMAIL.COM", NormalizedEmail = "RENZBALADJAY25@GMAIL.COM" },
                        new AppUser { fName = "Renz Niño", mName = "S", lName = "Baladjay External", type = "external", Email = "baladjaygaming12@gmail.com", UserName = "baladjaygaming12@gmail.com", NormalizedUserName = "BALADJAYGAMING12@GMAIL.COM", NormalizedEmail = "BALADJAYGAMING12@GMAIL.COM" },
                        new AppUser { fName = "Elena", mName = "N", lName = "Fa-ed", type = "internal", Email = "secretariatCre@example.com", UserName = "secretariatCre@example.com", NormalizedUserName = "SECRETARIATCRE@EXAMPLE.COM", NormalizedEmail = "SECRETARIATCRE@EXAMPLE.COM" },
                        new AppUser { fName = "Carlos", mName = "E", lName = "Ramos", type = "internal", Email = "eduChair@example.com", UserName = "eduChair@example.com", NormalizedUserName = "EDUCHAIR@EXAMPLE.COM", NormalizedEmail = "EDUCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "David", mName = "A", lName = "Gonzales", type = "internal", Email = "csistChair@example.com", UserName = "csistChair@example.com", NormalizedUserName = "CSISTCHAIR@EXAMPLE.COM", NormalizedEmail = "CSISTCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Eva", mName = "M", lName = "Mendez", type = "internal", Email = "eadChair@example.com", UserName = "eadChair@example.com", NormalizedUserName = "EADCHAIR@EXAMPLE.COM", NormalizedEmail = "EADCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Francisco", mName = "J", lName = "Lopez", type = "internal", Email = "hlcChair@example.com", UserName = "hlcChair@example.com", NormalizedUserName = "HLCCHAIR@EXAMPLE.COM", NormalizedEmail = "HLCCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Gina", mName = "R", lName = "Garcia", type = "internal", Email = "busChair@example.com", UserName = "busChair@example.com", NormalizedUserName = "BUSCHAIR@EXAMPLE.COM", NormalizedEmail = "BUSCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Hector", mName = "E", lName = "Bautista", type = "internal", Email = "sosciChair@example.com", UserName = "sosciChair@example.com", NormalizedUserName = "SOSCICHAIR@EXAMPLE.COM", NormalizedEmail = "SOSCICHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Irene", mName = "C", lName = "De Guzman", type = "internal", Email = "smsChair@example.com", UserName = "smsChair@example.com", NormalizedUserName = "SMSCHAIR@EXAMPLE.COM", NormalizedEmail = "SMSCHAIR@EXAMPLE.COM" },
                        new AppUser { fName = "Jose", mName = "P", lName = "Morales", type = "internal", Email = "jose.morales@example.com", UserName = "jose.morales@example.com", NormalizedUserName = "JOSE.MORALES@EXAMPLE.COM", NormalizedEmail = "JOSE.MORALES@EXAMPLE.COM" },
                        new AppUser { fName = "Katrina", mName = "J", lName = "Cruz", type = "internal", Email = "katrina.cruz@example.com", UserName = "katrina.cruz@example.com", NormalizedUserName = "KATRINA.CRUZ@EXAMPLE.COM", NormalizedEmail = "KATRINA.CRUZ@EXAMPLE.COM" },
                        new AppUser { fName = "Leon", mName = "A", lName = "Alvarez", type = "internal", Email = "leon.alvarez@example.com", UserName = "leon.alvarez@example.com", NormalizedUserName = "LEON.ALVAREZ@EXAMPLE.COM", NormalizedEmail = "LEON.ALVAREZ@EXAMPLE.COM" },
                        new AppUser { fName = "Mila", mName = "I", lName = "Villanueva", type = "internal", Email = "mila.villanueva@example.com", UserName = "mila.villanueva@example.com", NormalizedUserName = "MILA.VILLANUEVA@EXAMPLE.COM", NormalizedEmail = "MILA.VILLANUEVA@EXAMPLE.COM" },
                        new AppUser { fName = "Nina", mName = "E", lName = "Tan", type = "internal", Email = "nina.tan@example.com", UserName = "nina.tan@example.com", NormalizedUserName = "NINA.TAN@EXAMPLE.COM", NormalizedEmail = "NINA.TAN@EXAMPLE.COM" },
                        new AppUser { fName = "Omar", mName = "M", lName = "Reyes", type = "internal", Email = "omar.reyes@example.com", UserName = "omar.reyes@example.com", NormalizedUserName = "OMAR.REYES@EXAMPLE.COM", NormalizedEmail = "OMAR.REYES@EXAMPLE.COM" },
                        new AppUser { fName = "Pedro", mName = "F", lName = "Santiago", type = "internal", Email = "pedro.santiago@example.com", UserName = "pedro.santiago@example.com", NormalizedUserName = "PEDRO.SANTIAGO@EXAMPLE.COM", NormalizedEmail = "PEDRO.SANTIAGO@EXAMPLE.COM" },
                        new AppUser { fName = "Quincy", mName = "R", lName = "Pineda", type = "internal", Email = "quincy.pineda@example.com", UserName = "quincy.pineda@example.com", NormalizedUserName = "QUINCY.PINEDA@EXAMPLE.COM", NormalizedEmail = "QUINCY.PINEDA@EXAMPLE.COM" },
                        new AppUser { fName = "Ruby", mName = "A", lName = "Delos Santos", type = "internal", Email = "ruby.delossantos@example.com", UserName = "ruby.delossantos@example.com", NormalizedUserName = "RUBY.DELOSSANTOS@EXAMPLE.COM", NormalizedEmail = "RUBY.DELOSSANTOS@EXAMPLE.COM" },
                        new AppUser { fName = "Samuel", mName = "D", lName = "Bacalso", type = "internal", Email = "samuel.bacalso@example.com", UserName = "samuel.bacalso@example.com", NormalizedUserName = "SAMUEL.BACALSO@EXAMPLE.COM", NormalizedEmail = "SAMUEL.BACALSO@EXAMPLE.COM" },
                        new AppUser { fName = "Tina", mName = "C", lName = "Silva", type = "internal", Email = "tina.silva@example.com", UserName = "tina.silva@example.com", NormalizedUserName = "TINA.SILVA@EXAMPLE.COM", NormalizedEmail = "TINA.SILVA@EXAMPLE.COM" },
                        new AppUser { fName = "Ulysses", mName = "J", lName = "Lim", type = "internal", Email = "ulysses.lim@example.com", UserName = "ulysses.lim@example.com", NormalizedUserName = "ULYSSES.LIM@EXAMPLE.COM", NormalizedEmail = "ULYSSES.LIM@EXAMPLE.COM" },
                        new AppUser { fName = "Dante", mName = "R", lName = "Ponce", type = "internal", Email = "dante.ponce@example.com", UserName = "dante.ponce@example.com", NormalizedUserName = "DANTE.PONCE@EXAMPLE.COM", NormalizedEmail = "DANTE.PONCE@EXAMPLE.COM" },
                        new AppUser { fName = "Liza", mName = "C", lName = "Neri", type = "internal", Email = "liza.neri@example.com", UserName = "liza.neri@example.com", NormalizedUserName = "LIZA.NERI@EXAMPLE.COM", NormalizedEmail = "LIZA.NERI@EXAMPLE.COM" },
                        new AppUser { fName = "Lucas", mName = "G", lName = "Martinez", type = "internal", Email = "lucas.martinez@example.com", UserName = "lucas.martinez@example.com", NormalizedUserName = "LUCAS.MARTINEZ@EXAMPLE.COM", NormalizedEmail = "LUCAS.MARTINEZ@EXAMPLE.COM" },
                        new AppUser { fName = "Sofia", mName = "L", lName = "Villanueva", type = "internal", Email = "sofia.villanueva@example.com", UserName = "sofia.villanueva@example.com", NormalizedUserName = "SOFIA.VILLANUEVA@EXAMPLE.COM", NormalizedEmail = "SOFIA.VILLANUEVA@EXAMPLE.COM" },
                        new AppUser { fName = "Michael", mName = "K", lName = "Fernandez", type = "internal", Email = "michael.fernandez@example.com", UserName = "michael.fernandez@example.com", NormalizedUserName = "MICHAEL.FERNANDEZ@EXAMPLE.COM", NormalizedEmail = "MICHAEL.FERNANDEZ@EXAMPLE.COM" },
                        new AppUser { fName = "Aria", mName = "M", lName = "Palacios", type = "internal", Email = "aria.palacios@example.com", UserName = "aria.palacios@example.com", NormalizedUserName = "ARIA.PALACIOS@EXAMPLE.COM", NormalizedEmail = "ARIA.PALACIOS@EXAMPLE.COM" },
                        new AppUser { fName = "Bruno", mName = "H", lName = "Diaz", type = "internal", Email = "bruno.diaz@example.com", UserName = "bruno.diaz@example.com", NormalizedUserName = "BRUNO.DIAZ@EXAMPLE.COM", NormalizedEmail = "BRUNO.DIAZ@EXAMPLE.COM" },
                        new AppUser { fName = "Claudia", mName = "C", lName = "Ortega", type = "internal", Email = "claudia.ortega@example.com", UserName = "claudia.ortega@example.com", NormalizedUserName = "CLAUDIA.ORTEGA@EXAMPLE.COM", NormalizedEmail = "CLAUDIA.ORTEGA@EXAMPLE.COM" },
                        new AppUser { fName = "Daniel", mName = "J", lName = "Navarro", type = "internal", Email = "daniel.navarro@example.com", UserName = "daniel.navarro@example.com", NormalizedUserName = "DANIEL.NAVARRO@EXAMPLE.COM", NormalizedEmail = "DANIEL.NAVARRO@EXAMPLE.COM" },
                        new AppUser { fName = "Elisa", mName = "F", lName = "Garcia", type = "internal", Email = "elisa.garcia@example.com", UserName = "elisa.garcia@example.com", NormalizedUserName = "ELISA.GARCIA@EXAMPLE.COM", NormalizedEmail = "ELISA.GARCIA@EXAMPLE.COM" },
                        new AppUser { fName = "Marco", mName = "A", lName = "Xavier", type = "external", Email = "marco.xavier@example.com", UserName = "marco.xavier@example.com", NormalizedUserName = "MARCO.XAVIER@EXAMPLE.COM", NormalizedEmail = "MARCO.XAVIER@EXAMPLE.COM" },
                        new AppUser { fName = "Maya", mName = "E", lName = "Yu", type = "external", Email = "maya.yu@example.com", UserName = "maya.yu@example.com", NormalizedUserName = "MAYA.YU@EXAMPLE.COM", NormalizedEmail = "MAYA.YU@EXAMPLE.COM" },
                        new AppUser { fName = "Clara", mName = "S", lName = "Zamora", type = "external", Email = "clara.zamora@example.com", UserName = "clara.zamora@example.com", NormalizedUserName = "CLARA.ZAMORA@EXAMPLE.COM", NormalizedEmail = "CLARA.ZAMORA@EXAMPLE.COM" },
                        new AppUser { fName = "Felix", mName = "Q", lName = "Torres", type = "external", Email = "felix.torres@example.com", UserName = "felix.torres@example.com", NormalizedUserName = "FELIX.TORRES@EXAMPLE.COM", NormalizedEmail = "FELIX.TORRES@EXAMPLE.COM" },
                        new AppUser { fName = "Julie Charmain", mName = "O", lName = "Bonifacio", type = "internal", Email = "chiefCre@example.com", UserName = "chiefCre@example.com", NormalizedUserName = "CHIEFCRE@EXAMPLE.COM", NormalizedEmail = "CHIEFCRE@EXAMPLE.COM" }
                    };

                    // Hash passwords for all users
                    foreach (var user in users)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, "password123"); // Set password
                    }

                    // Save users to the database
                    context.AppUser.AddRange(users);
                    context.SaveChanges(); // Save to get the IDs of the added users

                    // Assign roles after saving the users
                    foreach (var user in users)
                    {
                        await AssignRolesBasedOnFullName(userManager, user); // Ensure this method is defined correctly
                    }
                }

                if (!context.Faculty.Any())
                {
                    // Map IdentityUser users to their corresponding Faculty entries
                    var facultyMembers = new List<Faculty>
                        {
                            new Faculty { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay Internal").Id, userType = "Developer", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay External").Id, userType = "Developer", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Carlos" && u.lName == "Ramos").Id, userType = "Administrative", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "David" && u.lName == "Gonzales").Id, userType = "Administrative", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Eva" && u.lName == "Mendez").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Francisco" && u.lName == "Lopez").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Gina" && u.lName == "Garcia").Id, userType = "Associate Professor", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Hector" && u.lName == "Bautista").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Irene" && u.lName == "De Guzman").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Jose" && u.lName == "Morales").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Katrina" && u.lName == "Cruz").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Leon" && u.lName == "Alvarez").Id, userType = "Associate Professor", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Mila" && u.lName == "Villanueva").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Nina" && u.lName == "Tan").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Omar" && u.lName == "Reyes").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Pedro" && u.lName == "Santiago").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Quincy" && u.lName == "Pineda").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Ruby" && u.lName == "Delos Santos").Id, userType = "Associate Professor", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Samuel" && u.lName == "Bacalso").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Tina" && u.lName == "Silva").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Ulysses" && u.lName == "Lim").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Dante" && u.lName == "Ponce").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Liza" && u.lName == "Neri").Id, userType = "Associate Professor", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Lucas" && u.lName == "Martinez").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Sofia" && u.lName == "Villanueva").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Michael" && u.lName == "Fernandez").Id, userType = "Assistant Professor", salaryGrade = 12 },
                            new Faculty { userId = users.First(u => u.fName == "Aria" && u.lName == "Palacios").Id, userType = "Associate Professor", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Bruno" && u.lName == "Diaz").Id, userType = "Professor", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Claudia" && u.lName == "Ortega").Id, userType = "Instructor", salaryGrade = 10 },
                            new Faculty { userId = users.First(u => u.fName == "Daniel" && u.lName == "Navarro").Id, userType = "Assistant Professor", salaryGrade = 12 }
                        };

                    context.Faculty.AddRange(facultyMembers);
                    context.SaveChanges(); // Save the faculty records
                }
                if (!context.Secretariat.Any())
                {
                    var secretariats = new List<Secretariat>
                    {
                        new Secretariat { userId = users.First(u => u.fName == "Elena" && u.lName == "Fa-ed").Id}
                    };
                    context.Secretariat.AddRange(secretariats);
                    context.SaveChanges();
                }

                if (!context.Chief.Any())
                {
                    // Add Chief records by assigning users with "Chief" role
                    var chiefs = new List<Chief>
                        {
                            new Chief { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay Internal").Id, center = "CRE" },
                            new Chief { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay External").Id, center = "CRE" },
                            new Chief { userId = users.First(u => u.fName == "Julie Charmain" && u.lName == "Bonifacio").Id, center = "CRE" }
                        };

                    context.Chief.AddRange(chiefs);
                    context.SaveChanges(); // Save the Chief records
                }

                if (!context.Chairperson.Any())
                {
                    var chairpersons = new List<Chairperson>
                        {
                            new Chairperson { facultyId = 1, fieldOfStudy = "Computer Science, Information Systems, and Technology"},
                            new Chairperson { facultyId = 2, fieldOfStudy = "Computer Science, Information Systems, and Technology"},
                            new Chairperson { facultyId = 3, fieldOfStudy = "Education" },
                            new Chairperson { facultyId = 4, fieldOfStudy = "Computer Science, Information Systems, and Technology"},
                            new Chairperson { facultyId = 5, fieldOfStudy = "Engineering, Architecture, and Design"},
                            new Chairperson { facultyId = 6, fieldOfStudy = "Humanities, Language, and Communication"},
                            new Chairperson { facultyId = 7, fieldOfStudy = "Business"},
                            new Chairperson { facultyId = 8, fieldOfStudy = "Social Sciences"},
                            new Chairperson { facultyId = 9, fieldOfStudy = "Science, Mathematics, and Statistics"},
                        };

                    context.Chairperson.AddRange(chairpersons);
                    context.SaveChanges(); // Save the chairperson records
                }

                // Seed Expertise
                if (!context.Expertise.Any())
                {
                    context.Expertise.AddRange(
                        new Expertise { expertiseName = "Education" },
                        new Expertise { expertiseName = "Computer Science, Information Systems, and Technology" },
                        new Expertise { expertiseName = "Engineering, Architecture, and Design" },
                        new Expertise { expertiseName = "Humanities, Language, and Communication" },
                        new Expertise { expertiseName = "Business" },
                        new Expertise { expertiseName = "Social Sciences" },
                        new Expertise { expertiseName = "Science, Mathematics, and Statistics" }
                    );
                    context.SaveChanges();
                }

                // Seed Ethics Evaluators
                if (!context.EthicsEvaluator.Any())
                {
                    var ethicsEvaluators = new List<EthicsEvaluator>
                {
                    new EthicsEvaluator { facultyId = 1, completedEval = 5, pendingEval = 3, declinedAssignment = 2 },
                    new EthicsEvaluator { facultyId = 2, completedEval = 7, pendingEval = 1, declinedAssignment = 0 },
                    new EthicsEvaluator { facultyId = 10, completedEval = 4, pendingEval = 5, declinedAssignment = 1 },
                    new EthicsEvaluator { facultyId = 11, completedEval = 2, pendingEval = 7, declinedAssignment = 3 },
                    new EthicsEvaluator { facultyId = 12, completedEval = 0, pendingEval = 2, declinedAssignment = 5 },
                    new EthicsEvaluator { facultyId = 13, completedEval = 6, pendingEval = 0, declinedAssignment = 4 },
                    new EthicsEvaluator { facultyId = 14, completedEval = 1, pendingEval = 6, declinedAssignment = 3 },
                    new EthicsEvaluator { facultyId = 15, completedEval = 3, pendingEval = 4, declinedAssignment = 2 },
                    new EthicsEvaluator { facultyId = 16, completedEval = 8, pendingEval = 3, declinedAssignment = 1 },
                    new EthicsEvaluator { facultyId = 17, completedEval = 4, pendingEval = 2, declinedAssignment = 6 },
                    new EthicsEvaluator { facultyId = 18, completedEval = 10, pendingEval = 0, declinedAssignment = 1 },
                    new EthicsEvaluator { facultyId = 19, completedEval = 2, pendingEval = 8, declinedAssignment = 2 },
                    new EthicsEvaluator { facultyId = 20, completedEval = 1, pendingEval = 6, declinedAssignment = 3 },
                    new EthicsEvaluator { facultyId = 21, completedEval = 9, pendingEval = 5, declinedAssignment = 0 },
                    new EthicsEvaluator { facultyId = 22, completedEval = 5, pendingEval = 1, declinedAssignment = 2 },
                    new EthicsEvaluator { facultyId = 23, completedEval = 3, pendingEval = 4, declinedAssignment = 2 },
                    new EthicsEvaluator { facultyId = 24, completedEval = 2, pendingEval = 2, declinedAssignment = 4 },
                    new EthicsEvaluator { facultyId = 25, completedEval = 7, pendingEval = 1, declinedAssignment = 3 },
                    new EthicsEvaluator { facultyId = 26, completedEval = 4, pendingEval = 5, declinedAssignment = 1 },
                    new EthicsEvaluator { facultyId = 27, completedEval = 6, pendingEval = 2, declinedAssignment = 3 },
                    new EthicsEvaluator { facultyId = 28, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                    new EthicsEvaluator { facultyId = 29, completedEval = 1, pendingEval = 1, declinedAssignment = 1 },
                    new EthicsEvaluator { facultyId = 30, completedEval = 2, pendingEval = 2, declinedAssignment = 5 }
                };

                    context.EthicsEvaluator.AddRange(ethicsEvaluators);
                    context.SaveChanges(); // Save the ethics evaluator records
                }
                // Seed EvaluationForms
                if (!context.EvaluationForms.Any())
                {
                    context.EvaluationForms.AddRange(
                        new EvaluationForms
                        {
                            evalFormName = "Informed Consent Form",
                            evalFormFile = seed.ReadFileToByteArray("EvaluationTemplates\\Informed-Consent-Form-Evaluation-Sheet-TEMPLATE.docx")
                        },
                        new EvaluationForms
                        {
                            evalFormName = "Protocol Review Sheet",
                            evalFormFile = seed.ReadFileToByteArray("EvaluationTemplates\\ProtocolReviewSheet-TEMPLATE.docx")
                        }
                    );
                    await context.SaveChangesAsync(); // Save changes to the context
                }
                // Seed EthicsEvaluatorExpertise with specific expertise assignments
                if (!context.EthicsEvaluatorExpertise.Any())
                {
                    var ethicsEvaluatorExpertises = new List<EthicsEvaluatorExpertise> {
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 1, expertiseId = 1 }, // Faculty 2 -> Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 2, expertiseId = 2 }, // Faculty 2 -> Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 3, expertiseId = 3 }, // Faculty 11 -> Engineering
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 4, expertiseId = 4 }, // Faculty 12 -> Humanities
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 5, expertiseId = 5 }, // Faculty 13 -> Business
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 6, expertiseId = 6 }, // Faculty 14 -> Social Sciences
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 7, expertiseId = 7 }, // Faculty 15 -> Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 8, expertiseId = 1 }, // Faculty 16 -> Education
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 9, expertiseId = 2 }, // Faculty 17 -> Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 10, expertiseId = 3 }, // Faculty 18 -> Engineering
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 11, expertiseId = 4 }, // Faculty 19 -> Humanities
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 12, expertiseId = 5 }, // Faculty 20 -> Business
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 13, expertiseId = 6 }, // Faculty 21 -> Social Sciences
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 14, expertiseId = 7 }, // Faculty 22 -> Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 15, expertiseId = 1 }, // Faculty 23 -> Education
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 16, expertiseId = 2 }, // Faculty 24 -> Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 17, expertiseId = 3 }, // Faculty 25 -> Engineering
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 18, expertiseId = 4 }, // Faculty 26 -> Humanities
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 19, expertiseId = 5 }, // Faculty 27 -> Business
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 20, expertiseId = 6 }, // Faculty 28 -> Social Sciences
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 21, expertiseId = 7 }, // Faculty 29 -> Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 22, expertiseId = 1 }, // Faculty 30 -> Education
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 23, expertiseId = 2 }, // Faculty 30 -> Education
                    };
                    context.EthicsEvaluatorExpertise.AddRange(ethicsEvaluatorExpertises);
                    context.SaveChanges(); // Save the expertise relationships
                }
                // Seed EthicsForms
                if (!context.EthicsForm.Any())
                {
                    context.EthicsForm.AddRange(
                        new EthicsForm
                        {
                            ethicsFormId = "FORM9",
                            formName = "Application for Ethics Review of New Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-9-Application for Ethics Review of New Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM10",
                            formName = "Research Study Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10-Research-Study Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM10_1",
                            formName = "Research Study Protocol",
                            formDescription = "This form is for non-human research/es.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM11",
                            formName = "Informed Consent Form",
                            formDescription = "This form should be used for studies that requires human respondents.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM12",
                            formName = "Assent Form",
                            formDescription = "This form should be used for studies with minor respondents.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-12-Assent Form for Minors-Participants.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM15",
                            formName = "Application for Ethics Review of Amendments",
                            formDescription = "This form should be used for detail changes and addition with regards to the application.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-15-Application for Ethics Review of Amendments.docx")
                        },
                        //Form 16 is disabled due to the interview 
                        //new EthicsForm
                        //{
                        //    ethicsFormId = "FORM-16",
                        //    formName = "Application for Cancellation of Ethics Review",
                        //    formDescription = "This form should be used in the event of cancellation of ethics clearance application.",
                        //    file = seed.ReadFileToByteArray("FormFiles\\FORM-16-Application for Cancellation of Ethics Review.docx")
                        //},
                        new EthicsForm
                        {
                            ethicsFormId = "FORM18",
                            formName = "Terminal Report Template",
                            formDescription = "This form should be submitted upon completion of the study.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-18-Terminal-Report-Template.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "CAA",
                            formName = "Co-Authorship Agreement",
                            formDescription = "This document is required for those studies with co-author/s.",
                            file = seed.ReadFileToByteArray("FormFiles\\CO-AUTHORSHIP-AGREEMENT.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "RCV",
                            formName = "Researcher Curriculum Vitae",
                            formDescription = "This document is required to be submitted by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\RESEARCHER CURRICULUM VITAE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "CV",
                            formName = "Certificate of Validity",
                            formDescription = "This document should be submitted the study instrument is researcher-made or adapted but modified.",
                            file = seed.ReadFileToByteArray("FormFiles\\CERTIFICATE OF VALIDITY.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "LI",
                            formName = "Letter of Intent",
                            formDescription = "This document is to be submitted to the Office of the VPRED",
                            file = seed.ReadFileToByteArray("FormFiles\\LETTER OF INTENT.docx")
                        }
                    );
                    context.SaveChanges();
                }

            }
        }
        // Method to assign roles based on full name
        private static async Task AssignRolesBasedOnFullName(UserManager<AppUser> userManager, AppUser user)
        {
            var userRoles = new (string FullName, string[] Roles)[]
            {
        ("Renz Niño S Baladjay Internal", new[] { "Researcher", "Chief", "Faculty", "Evaluator", "Secretariat", "Chairperson" }),
        ("Renz Niño S Baladjay External", new[] { "Researcher", "Chief", "Faculty", "Evaluator", "Secretariat", "Chairperson" }),
        ("Elena N Fa-ed", new[] { "Researcher", "Secretariat" }),
        ("Julie Charmain O Bonifacio", new[] { "Researcher", "Chief" }),
        ("Carlos E Ramos", new[] { "Researcher", "Faculty", "Chairperson" }),
        ("David A Gonzales", new[] { "Researcher", "Faculty", "Chairperson" }),
        ("Eva M Mendez", new[] { "Researcher", "Faculty", "Chairperson" }),
        ("Francisco J Lopez", new[] {"Researcher", "Faculty", "Chairperson" }),
        ("Gina R Garcia", new[] {"Researcher", "Faculty", "Chairperson" }),
        ("Hector E Bautista", new[] {"Researcher", "Faculty", "Chairperson" }),
        ("Irene C De Guzman", new[] {"Researcher", "Faculty", "Chairperson" }),
        ("Jose P Morales", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Katrina J Cruz", new[] {"Researcher", "Faculty", "Evaluator" }),
        ("Leon A Alvarez", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Mila I Villanueva", new[] {"Researcher", "Faculty", "Evaluator" }),
        ("Nina E Tan", new[] {"Researcher", "Faculty", "Evaluator" }),
        ("Omar M Reyes", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Pedro F Santiago", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Quincy R Pineda", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Ruby A Delos Santos", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Samuel D Bacalso", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Tina C Silva", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Ulysses J Lim", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Dante R Ponce", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Liza C Neri", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Lucas G Martinez", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Sofia L Villanueva", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Michael K Fernandez", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Aria M Palacios", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Bruno H Diaz", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Claudia C Ortega", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Daniel J Navarro", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Elisa F Garcia", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Marco A Xavier", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Maya E Yu", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Clara S Zamora", new[] { "Researcher", "Faculty", "Evaluator" }),
        ("Felix Q Torres", new[] { "Researcher", "Faculty", "Evaluator" }),
            };

            foreach (var userRole in userRoles)
            {
                if (user.FullName() == userRole.FullName) // Assuming FullName is a method to concatenate fName, mName, lName
                {
                    foreach (var role in userRole.Roles)
                    {
                        if (!await userManager.IsInRoleAsync(user, role))
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                    }
                    break; // Exit after assigning roles
                }
            }
        }

    }
}
