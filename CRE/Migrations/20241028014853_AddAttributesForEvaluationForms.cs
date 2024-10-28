using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributesForEvaluationForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "protocolReviewSheet",
                table: "EthicsEvaluation",
                newName: "ProtocolReviewSheet");

            migrationBuilder.RenameColumn(
                name: "informedConsentForm",
                table: "EthicsEvaluation",
                newName: "InformedConsentForm");

            migrationBuilder.RenameColumn(
                name: "remarks",
                table: "EthicsEvaluation",
                newName: "ProtocolRemarks");

            migrationBuilder.RenameColumn(
                name: "recommendation",
                table: "EthicsEvaluation",
                newName: "ProtocolRecommendation");

            migrationBuilder.AddColumn<string>(
                name: "ConsentRecommendation",
                table: "EthicsEvaluation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsentRemarks",
                table: "EthicsEvaluation",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentRecommendation",
                table: "EthicsEvaluation");

            migrationBuilder.DropColumn(
                name: "ConsentRemarks",
                table: "EthicsEvaluation");

            migrationBuilder.RenameColumn(
                name: "ProtocolReviewSheet",
                table: "EthicsEvaluation",
                newName: "protocolReviewSheet");

            migrationBuilder.RenameColumn(
                name: "InformedConsentForm",
                table: "EthicsEvaluation",
                newName: "informedConsentForm");

            migrationBuilder.RenameColumn(
                name: "ProtocolRemarks",
                table: "EthicsEvaluation",
                newName: "remarks");

            migrationBuilder.RenameColumn(
                name: "ProtocolRecommendation",
                table: "EthicsEvaluation",
                newName: "recommendation");
        }
    }
}
