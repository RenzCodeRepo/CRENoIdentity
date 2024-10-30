using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class AddNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthicsApplication_AspNetUsers_userId",
                table: "EthicsApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_EthicsEvaluation_EthicsApplication_urecNo",
                table: "EthicsEvaluation");

            migrationBuilder.AlterColumn<string>(
                name: "urecNo",
                table: "EthicsEvaluation",
                type: "nvarchar(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "EthicsApplication",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NotificationTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationStatus = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsApplication_AspNetUsers_userId",
                table: "EthicsApplication",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsEvaluation_EthicsApplication_urecNo",
                table: "EthicsEvaluation",
                column: "urecNo",
                principalTable: "EthicsApplication",
                principalColumn: "urecNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthicsApplication_AspNetUsers_userId",
                table: "EthicsApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_EthicsEvaluation_EthicsApplication_urecNo",
                table: "EthicsEvaluation");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.AlterColumn<string>(
                name: "urecNo",
                table: "EthicsEvaluation",
                type: "nvarchar(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "EthicsApplication",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsApplication_AspNetUsers_userId",
                table: "EthicsApplication",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EthicsEvaluation_EthicsApplication_urecNo",
                table: "EthicsEvaluation",
                column: "urecNo",
                principalTable: "EthicsApplication",
                principalColumn: "urecNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
