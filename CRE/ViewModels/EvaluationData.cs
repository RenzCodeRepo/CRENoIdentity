namespace CRE.ViewModels
{
    public class EvaluationData
    {
        // Common Fields for Both Templates
        public string Title { get; set; }           // Title of the study
        public string Discipline { get; set; }      // Discipline/Field of Study
        public string Proponent { get; set; }       // Name of the person or team proposing the study
        public string Institution { get; set; }     // Institution associated with the study
        public string Reviewer { get; set; }        // Name of the reviewer
        public bool AlternateMember { get; set; }   // Checkbox for Alternate Member

        public bool Recommendation_Approved { get; set; }
        public bool Recommendation_Disapproved { get; set; }
        public bool Recommendation_MajorRevisions { get; set; }
        public bool Recommendation_MinorRevisions { get; set; }
        public bool ReviewDate { get; set; }
        public bool ReviewerName { get; set; }
        public bool ReviewType_Exempt { get; set; }
        public bool ReviewType_Expedited { get; set; }
        public bool ReviewType_Full { get; set; }

        // Informed Consent Template Specific Fields
        public bool NonScientist { get; set; }      // Checkbox for Non-Scientist
        public bool NonAffiliated { get; set; }     // Checkbox for Non-Affiliated
        
        // Protocol Review Sheet Specific Fields
        public bool RegularMember { get; set; }      

        // Informed Consent Question and Choices Bookmarks
        public bool Background_No { get; set; }
        public bool Background_Yes { get; set; }
        public bool BackgroundOfStudy { get; set; }    
        public bool Benefits_No { get; set; }
        public bool Benefits_Yes { get;set; }
        public bool BenefitsToParticipants { get; set; }
        public bool Confidentiality_No { get; set; }
        public bool Confidentiality_Yes { get; set; }
        public bool Confidentiality_Extent { get; set; }
        public bool ConsentRequired_Explaination { get; set; }
        public bool ConsentRequired_No { get; set; }
        public bool ConsentRequired_Yes { get; set; }
        public bool Contact_No { get; set; }
        public bool Contact_Yes { get; set; }
        public bool ContactForQuestions { get; set; }
        public bool Cost_No { get; set; }
        public bool Cost_Yes { get; set; }
        public bool CostOfParticipation { get; set; }
        public bool InformedConsent_No { get; set; }
        public bool InformedConsent_Yes { get; set; }
        public bool LayLanguage_No { get; set; }
        public bool LayLanguage_Yes { get; set; }
        public bool LayLanguageConsent { get; set; }
        public bool OtherConcerns { get; set; }
        public bool OtherConcerns_No { get; set; }
        public bool OtherConcerns_Yes { get; set; }
        public bool Payment_No { get; set; }
        public bool Payment_Yes { get; set; }
        public bool PaymentRemuneration { get; set; }
        public bool Procedures_No { get; set; }
        public bool Procedures_Yes { get; set; }
        public bool ProceduresOfStudy { get; set; }
        public bool Purpose_No { get; set; }
        public bool Purpose_Yes { get; set; }
        public bool PurposeOfStudy { get; set; }

        public bool Recommendation_Exempt { get; set; }
        public bool RemarksInformed { get; set; }
        public bool Risk { get; set; }
        public bool Risk_No { get; set; }
        public bool Risk_Yes { get; set; }
        public bool VoluntaryConsent { get; set; }
        public bool VoluntaryProcess_No { get; set; }
        public bool VoluntaryProcess_Yes { get; set; }

        // Protocol Review Sheet Question and Choices Bookmarks

        public bool Explanation1 { get; set; }
        public bool Explanation10 { get; set; }
        public bool Explanation11 { get; set; }
        public bool Explanation12 { get; set; }
        public bool Explanation13 { get; set; }
        public bool Explanation2 { get; set; }
        public bool Explanation3 { get; set; }
        public bool Explanation4 { get; set; }
        public bool Explanation5 { get; set; }
        public bool Explanation6 { get; set; }
        public bool Explanation8 { get; set; }
        public bool Explanation9 { get; set; }
        public bool Question1 { get; set; }
        public bool Question10 { get; set; }
        public bool Question11 { get; set; }
        public bool Question12 { get; set; }
        public bool Question13 { get; set; }
        public bool Question2 { get; set; }
        public bool Question3 { get; set; }
        public bool Question4 { get; set; }
        public bool Question5 { get; set; }
        public bool Question6 { get; set; }
        public bool Question7 { get; set; }
        public bool Question8 { get; set; }
        public bool Question9 { get; set; }
        public bool RemarksProtocol { get; set; }
        public bool Response10_No { get; set; }
        public bool Response10_NotApplicable { get; set; }
        public bool Response10_UnableToAssess { get; set; }
        public bool Response10_Yes { get; set; }
        public bool Response11_No { get; set; }
        public bool Response11_NotApplicable { get; set; }
        public bool Response11_UnableToAssess { get; set; }
        public bool Response11_Yes { get; set; }
        public bool Response12_No { get; set; }
        public bool Response12_NotApplicable { get; set; }
        public bool Response12_UnableToAssess { get; set; }
        public bool Response12_Yes { get; set; }
        public bool Response1_No { get; set; }
        public bool Response1_NotApplicable { get; set; }
        public bool Response1_UnableToAssess { get; set; }
        public bool Response1_Yes { get; set; }
        public bool Response2_No { get; set; }
        public bool Response2_NotApplicable { get; set; }
        public bool Response2_UnableToAssess { get; set; }
        public bool Response2_Yes { get; set; }
        public bool Response3_No { get; set; }
        public bool Response3_NotApplicable { get; set; }
        public bool Response3_UnableToAssess { get; set; }
        public bool Response3_Yes { get; set; }
        public bool Response4_No { get; set; }
        public bool Response4_NotApplicable { get; set; }
        public bool Response4_UnableToAssess { get; set; }
        public bool Response4_Yes { get; set; }
        public bool Response5_No { get; set; }
        public bool Response5_NotApplicable { get; set; }
        public bool Response5_UnableToAssess { get; set; }
        public bool Response5_Yes { get; set; }
        public bool Response6_No { get; set; }
        public bool Response6_NotApplicable { get; set; }
        public bool Response6_UnableToAssess { get; set; }
        public bool Response6_Yes { get; set; }
        public bool Response7_No { get; set; }
        public bool Response7_NotApplicable { get; set; }
        public bool Response7_UnableToAssess { get; set; }
        public bool Response7_Yes { get; set; }
        public bool Response8_No { get; set; }
        public bool Response8_NotApplicable { get; set; }
        public bool Response8_UnableToAssess { get; set; }
        public bool Response8_Yes { get; set; }
        public bool Response9_No { get; set; }
        public bool Response9_NotApplicable { get; set; }
        public bool Response9_UnableToAssess { get; set; }
        public bool Response9_Yes { get; set; }
    }
}
