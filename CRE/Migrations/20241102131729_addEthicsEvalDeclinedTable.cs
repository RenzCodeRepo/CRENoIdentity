using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class addEthicsEvalDeclinedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthicsEvaluationDeclined",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    evaluationId = table.Column<int>(type: "int", nullable: false),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    reasonForDecline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    declineDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsEvaluationDeclined", x => x.id);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluationDeclined_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluationDeclined_EthicsEvaluation_evaluationId",
                        column: x => x.evaluationId,
                        principalTable: "EthicsEvaluation",
                        principalColumn: "evaluationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluationDeclined_evaluationId",
                table: "EthicsEvaluationDeclined",
                column: "evaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluationDeclined_urecNo",
                table: "EthicsEvaluationDeclined",
                column: "urecNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthicsEvaluationDeclined");
        }
    }
}
