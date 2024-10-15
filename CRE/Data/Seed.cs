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

                context.Database.EnsureCreated();

                var seed = new Seed(); //intance to use readfile method

                context.Database.EnsureCreated();


                List<AppUser> users = new List<AppUser>();
                if (!context.User.Any())
                {
                    var passwordHasher = new PasswordHasher<AppUser>();
                    // Initialize the list of users with Email and PasswordHash
                    users = new List<AppUser>
                {
                    new AppUser { fName = "Renz Niño", mName = "S", lName = "Baladjay Internal", type = "internal", Email = "renzbaladjay25@gmail.com" },
                    new AppUser { fName = "Renz Niño", mName = "S", lName = "Baladjay External", type = "external", Email = "baladjaygaming12@gmail.com" },
                    new AppUser { fName = "Elena", mName = "N", lName = "Fa-ed", type = "internal", Email = "elena.fa-ed@example.com" },
                    new AppUser { fName = "Carlos", mName = "E", lName = "Ramos", type = "internal", Email = "carlos.ramos@example.com"  },
                    new AppUser { fName = "David", mName = "A", lName = "Gonzales", type = "internal", Email = "david.gonzales@example.com"  },
                    new AppUser { fName = "Eva", mName = "M", lName = "Mendez", type = "internal", Email = "eva.mendez@example.com"  },
                    new AppUser { fName = "Francisco", mName = "J", lName = "Lopez", type = "internal", Email = "francisco.lopez@example.com"  },
                    new AppUser { fName = "Gina", mName = "R", lName = "Garcia", type = "internal", Email = "gina.garcia@example.com"  },
                    new AppUser { fName = "Hector", mName = "E", lName = "Bautista", type = "internal", Email = "hector.bautista@example.com"  },
                    new AppUser { fName = "Irene", mName = "C", lName = "De Guzman", type = "internal", Email = "irene.deguzman@example.com"  },
                    new AppUser { fName = "Jose", mName = "P", lName = "Morales", type = "internal", Email = "jose.morales@example.com"  },
                    new AppUser { fName = "Katrina", mName = "J", lName = "Cruz", type = "internal", Email = "katrina.cruz@example.com"  },
                    new AppUser { fName = "Leon", mName = "A", lName = "Alvarez", type = "internal", Email = "leon.alvarez@example.com"  },
                    new AppUser { fName = "Mila", mName = "I", lName = "Villanueva", type = "internal", Email = "mila.villanueva@example.com"  },
                    new AppUser { fName = "Nina", mName = "E", lName = "Tan", type = "internal", Email = "nina.tan@example.com"  },
                    new AppUser { fName = "Omar", mName = "M", lName = "Reyes", type = "internal", Email = "omar.reyes@example.com"  },
                    new AppUser { fName = "Pedro", mName = "F", lName = "Santiago", type = "internal", Email = "pedro.santiago@example.com"  },
                    new AppUser { fName = "Quincy", mName = "R", lName = "Pineda", type = "internal", Email = "quincy.pineda@example.com"  },
                    new AppUser { fName = "Ruby", mName = "A", lName = "Delos Santos", type = "internal", Email = "ruby.delossantos@example.com"  },
                    new AppUser { fName = "Samuel", mName = "D", lName = "Bacalso", type = "internal", Email = "samuel.bacalso@example.com"  },
                    new AppUser { fName = "Tina", mName = "C", lName = "Silva", type = "internal", Email = "tina.silva@example.com"  },
                    new AppUser { fName = "Ulysses", mName = "J", lName = "Lim", type = "internal", Email = "ulysses.lim@example.com"  },
                    new AppUser { fName = "Dante", mName = "R", lName = "Ponce", type = "internal", Email = "dante.ponce@example.com"  },
                    new AppUser { fName = "Liza", mName = "C", lName = "Neri", type = "internal", Email = "liza.neri@example.com"  },
                    new AppUser { fName = "Lucas", mName = "G", lName = "Martinez", type = "internal", Email = "lucas.martinez@example.com"  },
                    new AppUser { fName = "Sofia", mName = "L", lName = "Villanueva", type = "internal", Email = "sofia.villanueva@example.com"  },
                    new AppUser { fName = "Michael", mName = "K", lName = "Fernandez", type = "internal", Email = "michael.fernandez@example.com"  },
                    new AppUser { fName = "Aria", mName = "M", lName = "Palacios", type = "internal", Email = "aria.palacios@example.com"  },
                    new AppUser { fName = "Bruno", mName = "H", lName = "Diaz", type = "internal", Email = "bruno.diaz@example.com"  },
                    new AppUser { fName = "Claudia", mName = "C", lName = "Ortega", type = "internal", Email = "claudia.ortega@example.com"  },
                    new AppUser { fName = "Daniel", mName = "J", lName = "Navarro", type = "internal", Email = "daniel.navarro@example.com"  },
                    new AppUser { fName = "Elisa", mName = "F", lName = "Garcia", type = "internal", Email = "elisa.garcia@example.com"  },
                    new AppUser { fName = "Marco", mName = "A", lName = "Xavier", type = "external", Email = "marco.xavier@example.com"  },
                    new AppUser { fName = "Maya", mName = "E", lName = "Yu", type = "external", Email = "maya.yu@example.com"  },
                    new AppUser { fName = "Clara", mName = "S", lName = "Zamora", type = "external", Email = "clara.zamora@example.com"  },
                    new AppUser { fName = "Felix", mName = "Q", lName = "Torres", type = "external", Email = "felix.torres@example.com"  },
                    new AppUser { fName = "Julie Charmain", mName = "O", lName = "Bonifacio", type = "internal", Email = "julie.bonifacio@example.com"  }
                };

                    foreach (var user in users)
                    {
                        user.PasswordHash = passwordHasher.HashPassword(user, "password123"); // Replace with desired password
                    }
                    foreach (var user in users)
                    {
                        await AssignRolesBasedOnFullName(userManager, user);
                    }
                    context.User.AddRange(users);
                    context.SaveChanges(); // Save to get the IDs of the added users
                }

                if (!context.Faculty.Any())
                {
                    // Map IdentityUser users to their corresponding Faculty entries
                    var facultyMembers = new List<Faculty>
                        {
                            new Faculty { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay Internal").Id, userType = "Developer", salaryGrade = 15 },
                            new Faculty { userId = users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay External").Id, userType = "Developer", salaryGrade = 13 },
                            new Faculty { userId = users.First(u => u.fName == "Elena" && u.lName == "Fa-ed").Id, userType = "Administrative", salaryGrade = 12 },
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
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay Internal").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay External").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Carlos" && u.lName == "Ramos").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "David" && u.lName == "Gonzales").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Eva" && u.lName == "Mendez").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Francisco" && u.lName == "Lopez").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Gina" && u.lName == "Garcia").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Hector" && u.lName == "Bautista").Id).facultyId },
                            new Chairperson { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Irene" && u.lName == "De Guzman").Id).facultyId }
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
                if (!context.EthicsEvaluator.Any())
                {
                    var ethicsEvaluators = new List<EthicsEvaluator>
                        {
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay Internal").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Renz Niño" && u.lName == "Baladjay External").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Jose" && u.lName == "Morales").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Katrina" && u.lName == "Cruz").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Leon" && u.lName == "Alvarez").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Mila" && u.lName == "Villanueva").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Nina" && u.lName == "Tan").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Omar" && u.lName == "Reyes").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Pedro" && u.lName == "Santiago").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Quincy" && u.lName == "Pineda").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Ruby" && u.lName == "Delos Santos").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Samuel" && u.lName == "Bacalso").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Tina" && u.lName == "Silva").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Ulysses" && u.lName == "Lim").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Dante" && u.lName == "Ponce").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Liza" && u.lName == "Neri").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Lucas" && u.lName == "Martinez").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Sofia" && u.lName == "Villanueva").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Michael" && u.lName == "Fernandez").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Aria" && u.lName == "Palacios").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Bruno" && u.lName == "Diaz").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Claudia" && u.lName == "Ortega").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                            new EthicsEvaluator { facultyId = context.Faculty.First(f => f.userId == users.First(u => u.fName == "Daniel" && u.lName == "Navarro").Id).facultyId, completedEval = 0, pendingEval = 0, declinedAssignment = 0 }
                        };

                    context.EthicsEvaluator.AddRange(ethicsEvaluators);
                    context.SaveChanges(); // Save the ethics evaluator records
                }


                if (!context.EthicsEvaluatorExpertise.Any())
                {
                    var expertiseAssignments = new List<(int evaluatorId, List<int> expertiseIds)>
                        {
                            (1, new List<int> { 1, 2 }), // Renz Niño Internal - Education, Computer Science
                            (2, new List<int> { 3, 5 }), // Renz Niño External - Engineering, Business
                            (3, new List<int> { 4, 6 }), // Jose Morales - Humanities, Social Sciences
                            (4, new List<int> { 1, 2 }), // Katrina Cruz - Education, Computer Science
                            (5, new List<int> { 3, 4 }), // Leon Alvarez - Engineering, Humanities
                            (6, new List<int> { 5, 7 }), // Mila Villanueva - Business, Science
                            (7, new List<int> { 1, 6 }), // Nina Tan - Education, Social Sciences
                            (8, new List<int> { 2, 3 }), // Omar Reyes - Computer Science, Engineering
                            (9, new List<int> { 4, 5 }), // Pedro Santiago - Humanities, Business
                            (10, new List<int> { 1, 2 }), // Quincy Pineda - Education, Computer Science
                            (11, new List<int> { 3, 4 }), // Ruby Delos Santos - Engineering, Humanities
                            (12, new List<int> { 5, 6 }), // Samuel Bacalso - Business, Social Sciences
                            (13, new List<int> { 1, 2 }), // Tina Silva - Education, Computer Science
                            (14, new List<int> { 3, 7 }), // Ulysses Lim - Engineering, Science
                            (15, new List<int> { 4, 6 }), // Dante Ponce - Humanities, Social Sciences
                            (16, new List<int> { 1, 5 }), // Liza Neri - Education, Business
                            (17, new List<int> { 2, 4 }), // Lucas Martinez - Computer Science, Humanities
                            (18, new List<int> { 3, 1 }), // Sofia Villanueva - Engineering, Education
                            (19, new List<int> { 4, 5 }), // Michael Fernandez - Humanities, Business
                            (20, new List<int> { 6, 2 }), // Aria Palacios - Social Sciences, Computer Science
                            (21, new List<int> { 7, 1 }), // Bruno Diaz - Science, Education
                            (22, new List<int> { 2, 3 }), // Claudia Ortega - Computer Science, Engineering
                            (23, new List<int> { 5, 6 })  // Daniel Navarro - Business, Social Sciences
                        };

                    foreach (var assignment in expertiseAssignments)
                    {
                        foreach (var expertiseId in assignment.expertiseIds)
                        {
                            context.EthicsEvaluatorExpertise.Add(new EthicsEvaluatorExpertise
                            {
                                ethicsEvaluatorId = assignment.evaluatorId, // Ensure 'evaluatorId' matches your EthicsEvaluator primary key
                                expertiseId = expertiseId
                            });
                        }
                    }
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
                            ethicsFormId = "FORM-15",
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
                            ethicsFormId = "FORM-18",
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
        ("Renz Niño S Baladjay Internal", new[] { "Chief", "Faculty", "Evaluator", "Secretariat", "Chairperson" }),
        ("Renz Niño S Baladjay External", new[] { "Chief", "Faculty", "Evaluator", "Secretariat", "Chairperson" }),
        ("Elena N Fa-ed", new[] { "Faculty", "Secretariat" }),
        ("Julie Charmain O Bonifacio", new[] { "Chief" }),
        ("Carlos E Ramos", new[] { "Faculty", "Chairperson" }),
        ("David A Gonzales", new[] { "Faculty", "Chairperson" }),
        ("Eva M Mendez", new[] { "Faculty", "Chairperson" }),
        ("Francisco J Lopez", new[] { "Faculty", "Chairperson" }),
        ("Gina R Garcia", new[] { "Faculty", "Chairperson" }),
        ("Hector E Bautista", new[] { "Faculty", "Chairperson" }),
        ("Irene C De Guzman", new[] { "Faculty", "Chairperson" }),
        ("Jose P Morales", new[] { "Faculty", "Chairperson" }),
        ("Katrina J Cruz", new[] { "Faculty", "Chairperson" }),
        ("Leon A Alvarez", new[] { "Faculty", "Chairperson" }),
        ("Mila I Villanueva", new[] { "Faculty", "Chairperson" }),
        ("Nina E Tan", new[] { "Faculty", "Chairperson" }),
        ("Omar M Reyes", new[] { "Faculty", "Chairperson" }),
        ("Pedro F Santiago", new[] { "Faculty", "Chairperson" }),
        ("Quincy R Pineda", new[] { "Faculty", "Chairperson" }),
        ("Ruby A Delos Santos", new[] { "Faculty", "Chairperson" }),
        ("Samuel D Bacalso", new[] { "Faculty", "Chairperson" }),
        ("Tina C Silva", new[] { "Faculty", "Chairperson" }),
        ("Ulysses J Lim", new[] { "Faculty", "Chairperson" }),
        ("Dante R Ponce", new[] { "Faculty", "Chairperson" }),
        ("Liza C Neri", new[] { "Faculty", "Chairperson" }),
        ("Lucas G Martinez", new[] { "Faculty", "Chairperson" }),
        ("Sofia L Villanueva", new[] { "Faculty", "Chairperson" }),
        ("Michael K Fernandez", new[] { "Faculty", "Chairperson" }),
        ("Aria M Palacios", new[] { "Faculty", "Chairperson" }),
        ("Bruno H Diaz", new[] { "Faculty", "Chairperson" }),
        ("Claudia C Ortega", new[] { "Faculty", "Chairperson" }),
        ("Daniel J Navarro", new[] { "Faculty", "Chairperson" }),
        ("Elisa F Garcia", new[] { "Faculty", "Chairperson" }),
        ("Marco A Xavier", new[] { "Faculty", "Chairperson" }),
        ("Maya E Yu", new[] { "Faculty", "Chairperson" }),
        ("Clara S Zamora", new[] { "Faculty", "Chairperson" }),
        ("Felix Q Torres", new[] { "Faculty", "Chairperson" }),
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
