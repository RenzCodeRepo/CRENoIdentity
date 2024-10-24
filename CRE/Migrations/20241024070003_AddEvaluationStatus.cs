using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "evaluationStatus",
                table: "EthicsEvaluation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "evaluationStatus",
                table: "EthicsEvaluation");
        }
    }
}
