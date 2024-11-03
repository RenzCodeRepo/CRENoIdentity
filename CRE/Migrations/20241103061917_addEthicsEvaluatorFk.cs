using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class addEthicsEvaluatorFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluationDeclined_ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined",
                column: "ethicsEvaluatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsEvaluationDeclined_EthicsEvaluator_ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined",
                column: "ethicsEvaluatorId",
                principalTable: "EthicsEvaluator",
                principalColumn: "ethicsEvaluatorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthicsEvaluationDeclined_EthicsEvaluator_ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined");

            migrationBuilder.DropIndex(
                name: "IX_EthicsEvaluationDeclined_ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined");

            migrationBuilder.DropColumn(
                name: "ethicsEvaluatorId",
                table: "EthicsEvaluationDeclined");
        }
    }
}
