using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class AddChiefIdToEthicsEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ethicsEvaluatorId",
                table: "EthicsEvaluation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "chiefId",
                table: "EthicsEvaluation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluation_chiefId",
                table: "EthicsEvaluation",
                column: "chiefId");

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsEvaluation_Chief_chiefId",
                table: "EthicsEvaluation",
                column: "chiefId",
                principalTable: "Chief",
                principalColumn: "chiefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthicsEvaluation_Chief_chiefId",
                table: "EthicsEvaluation");

            migrationBuilder.DropIndex(
                name: "IX_EthicsEvaluation_chiefId",
                table: "EthicsEvaluation");

            migrationBuilder.DropColumn(
                name: "chiefId",
                table: "EthicsEvaluation");

            migrationBuilder.AlterColumn<int>(
                name: "ethicsEvaluatorId",
                table: "EthicsEvaluation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
