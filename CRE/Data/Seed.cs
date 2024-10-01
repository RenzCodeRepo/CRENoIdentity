using CRE.Models;
using Microsoft.EntityFrameworkCore;

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
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('User', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Faculty', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Chief', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Chairperson', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Secretariat', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Expertise', RESEED, 1);");
                context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('EthicsEvaluator', RESEED, 1);");

                context.Database.EnsureCreated();

                var seed = new Seed(); //intance to use readfile method

                context.Database.EnsureCreated();
                // Seed 26 User records
                if (!context.User.Any())
                {
                    context.User.AddRange(
                        new User { fName = "Renz Niño", mName = "S", lName = "Baladjay External", type = "external" }, // Developer ID for external
                        new User { fName = "Renz Niño", mName = "S", lName = "Baladjay Internal", type = "internal" }, // Developer ID for external
                        new User { fName = "Alice", mName = "A", lName = "Anderson", type = "internal" },
                        new User { fName = "Bob", mName = "B", lName = "Bennett", type = "internal" },
                        new User { fName = "Charlie", mName = "C", lName = "Carter", type = "internal" },
                        new User { fName = "David", mName = "D", lName = "Dawson", type = "internal" },
                        new User { fName = "Eva", mName = "E", lName = "Evans", type = "internal" },
                        new User { fName = "Frank", mName = "F", lName = "Fisher", type = "internal" },
                        new User { fName = "Grace", mName = "G", lName = "Garcia", type = "internal" },
                        new User { fName = "Henry", mName = "H", lName = "Harrison", type = "internal" },
                        new User { fName = "Ivy", mName = "I", lName = "Irwin", type = "internal" },
                        new User { fName = "Jack", mName = "J", lName = "Johnson", type = "internal" },
                        new User { fName = "Kara", mName = "K", lName = "Knight", type = "internal" },
                        new User { fName = "Liam", mName = "L", lName = "Lewis", type = "internal" },
                        new User { fName = "Mia", mName = "M", lName = "Mason", type = "internal" },
                        new User { fName = "Nina", mName = "N", lName = "Nelson", type = "internal" },
                        new User { fName = "Oliver", mName = "O", lName = "Owens", type = "internal" },
                        new User { fName = "Parker", mName = "P", lName = "Peterson", type = "internal" },
                        new User { fName = "Quinn", mName = "Q", lName = "Quincy", type = "internal" },
                        new User { fName = "Ruby", mName = "R", lName = "Robinson", type = "internal" },
                        new User { fName = "Samuel", mName = "S", lName = "Stewart", type = "internal" },
                        new User { fName = "Tina", mName = "T", lName = "Turner", type = "internal" },
                        new User { fName = "Uma", mName = "U", lName = "Underwood", type = "internal" },
                        new User { fName = "Vince", mName = "V", lName = "Vaughn", type = "internal" },
                        new User { fName = "Will", mName = "W", lName = "Wilson", type = "internal" },
                        new User { fName = "Xander", mName = "X", lName = "Xavier", type = "external" }, // External user
                        new User { fName = "Yara", mName = "Y", lName = "Young", type = "external" }, // External user
                        new User { fName = "Zoe", mName = "Z", lName = "Zimmerman", type = "external" } // External user
                       
                        );
                    context.SaveChanges();
                }
                if (!context.Faculty.Any())
                {
                    context.Faculty.AddRange(
                        new Faculty { userId = 3, userType = "Professor", salaryGrade = 15 },
                        new Faculty { userId = 4, userType = "Associate Professor", salaryGrade = 13 },
                        new Faculty { userId = 5, userType = "Assistant Professor", salaryGrade = 12 },
                        new Faculty { userId = 6, userType = "Instructor", salaryGrade = 10 },
                        new Faculty { userId = 7, userType = "Professor", salaryGrade = 15 },
                        new Faculty { userId = 8, userType = "Instructor", salaryGrade = 10 },
                        new Faculty { userId = 9, userType = "Assistant Professor", salaryGrade = 12 },
                        new Faculty { userId = 10, userType = "Associate Professor", salaryGrade = 13 },
                        new Faculty { userId = 11, userType = "Professor", salaryGrade = 15 },
                        new Faculty { userId = 12, userType = "Instructor", salaryGrade = 10 },
                        new Faculty { userId = 13, userType = "Assistant Professor", salaryGrade = 12 },
                        new Faculty { userId = 14, userType = "Professor", salaryGrade = 15 },
                        new Faculty { userId = 15, userType = "Associate Professor", salaryGrade = 13 },
                        new Faculty { userId = 16, userType = "Instructor", salaryGrade = 10 }
                    );
                    context.SaveChanges();
                }
                // Seed 1 Chief record linked to a user that is not in the Faculty table
                if (!context.Chief.Any())
                {
                    context.Chief.Add(
                        new Chief { userId = 17, center = "CRE" }
                    );
                }
                // Seed 8 Chairperson records using the 7 Faculty records
                if (!context.Chairperson.Any())
                {
                    context.Chairperson.AddRange(
                        new Chairperson { facultyId = 1 },
                        new Chairperson { facultyId = 2 },
                        new Chairperson { facultyId = 3 },
                        new Chairperson { facultyId = 4 },
                        new Chairperson { facultyId = 5 },
                        new Chairperson { facultyId = 6 },
                        new Chairperson { facultyId = 7 }
                    );
                    context.SaveChanges();
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
                    context.EthicsEvaluator.AddRange(
                        new EthicsEvaluator { facultyId = 8, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 9, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 10, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 11, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 12, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 13, completedEval = 0, pendingEval = 0, declinedAssignment = 0 },
                        new EthicsEvaluator { facultyId = 14, completedEval = 0, pendingEval = 0, declinedAssignment = 0 }
                    );
                    context.SaveChanges();
                }
                // Seed E_Evaluator_Expertise records manually
                if (!context.EthicsEvaluatorExpertise.Any())
                {
                    context.EthicsEvaluatorExpertise.AddRange(
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 1, expertiseId = 1 }, // Expertise: Education
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 1, expertiseId = 2 }, // Expertise: Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 2, expertiseId = 3 }, // Expertise: Engineering
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 2, expertiseId = 5 }, // Expertise: Business
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 3, expertiseId = 4 }, // Expertise: Humanities
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 3, expertiseId = 6 }, // Expertise: Social Sciences
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 4, expertiseId = 1 }, // Expertise: Education
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 4, expertiseId = 2 }, // Expertise: Computer Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 5, expertiseId = 3 }, // Expertise: Engineering
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 5, expertiseId = 4 }, // Expertise: Humanities
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 6, expertiseId = 5 }, // Expertise: Business
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 6, expertiseId = 7 }, // Expertise: Science
                        new EthicsEvaluatorExpertise { ethicsEvaluatorId = 7, expertiseId = 1 }    // Expertise: Education
                    );
                    context.SaveChanges();
                }
                // Seed EthicsForms
                if (!context.EthicsForm.Any())
                {
                    context.EthicsForm.AddRange(
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-9",
                            formName = "Application for Ethics Review of New Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-9-Application for Ethics Review of New Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-10",
                            formName = "Research Study Protocol",
                            formDescription = "This form is required to be accomplished by all of the applicants.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10-Research-Study Protocol.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-10.1",
                            formName = "Research Study Protocol",
                            formDescription = "This form is for non-human research/es.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-11",
                            formName = "Informed Consent Form",
                            formDescription = "This form should be used for studies that requires human respondents.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-10.1-NON-HUMAN-DETERMINANT-TEMPLATE.docx")
                        },
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-12",
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
                        new EthicsForm
                        {
                            ethicsFormId = "FORM-16",
                            formName = "Application for Cancellation of Ethics Review",
                            formDescription = "This form should be used in the event of cancellation of ethics clearance application.",
                            file = seed.ReadFileToByteArray("FormFiles\\FORM-16-Application for Cancellation of Ethics Review.docx")
                        },
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
                        }
                        //would ask the front end if needed as downloadable form
                        //new EthicsForm
                        //{
                        //    ethicsFormId = "LI",
                        //    formName = "Letter of Intent",
                        //    file = seed.ReadFileToByteArray("FormFiles\\LETTER OF INTENT.docx")
                        //}
                    );
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
        } }
}
