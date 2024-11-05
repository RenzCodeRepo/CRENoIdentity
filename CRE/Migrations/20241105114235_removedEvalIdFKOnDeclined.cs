using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class removedEvalIdFKOnDeclined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthicsEvaluationDeclined_EthicsEvaluation_evaluationId",
                table: "EthicsEvaluationDeclined");

            migrationBuilder.DropIndex(
                name: "IX_EthicsEvaluationDeclined_evaluationId",
                table: "EthicsEvaluationDeclined");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluationDeclined_evaluationId",
                table: "EthicsEvaluationDeclined",
                column: "evaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsEvaluationDeclined_EthicsEvaluation_evaluationId",
                table: "EthicsEvaluationDeclined",
                column: "evaluationId",
                principalTable: "EthicsEvaluation",
                principalColumn: "evaluationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
