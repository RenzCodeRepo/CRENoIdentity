using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompletionCertificate",
                columns: table => new
                {
                    completionCertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    issuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    file = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletionCertificate", x => x.completionCertId);
                });

            migrationBuilder.CreateTable(
                name: "EthicsForm",
                columns: table => new
                {
                    ethicsFormId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    formName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    formDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsForm", x => x.ethicsFormId);
                });

            migrationBuilder.CreateTable(
                name: "Expertise",
                columns: table => new
                {
                    expertiseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    expertiseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expertise", x => x.expertiseId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    mName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    lName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Chief",
                columns: table => new
                {
                    chiefId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    center = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chief", x => x.chiefId);
                    table.ForeignKey(
                        name: "FK_Chief_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EthicsApplication",
                columns: table => new
                {
                    urecNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    submissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    dtsNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsApplication", x => x.urecNo);
                    table.ForeignKey(
                        name: "FK_EthicsApplication_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faculty",
                columns: table => new
                {
                    facultyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    userType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salaryGrade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculty", x => x.facultyId);
                    table.ForeignKey(
                        name: "FK_Faculty_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompletionReport",
                columns: table => new
                {
                    completionReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    submissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    terminalReport = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletionReport", x => x.completionReportId);
                    table.ForeignKey(
                        name: "FK_CompletionReport_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo");
                });

            migrationBuilder.CreateTable(
                name: "EthicsApplicationForms",
                columns: table => new
                {
                    ethicsApplicationFormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    ethicsFormId = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    dateUploaded = table.Column<DateOnly>(type: "date", nullable: false),
                    file = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsApplicationForms", x => x.ethicsApplicationFormId);
                    table.ForeignKey(
                        name: "FK_EthicsApplicationForms_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthicsApplicationForms_EthicsForm_ethicsFormId",
                        column: x => x.ethicsFormId,
                        principalTable: "EthicsForm",
                        principalColumn: "ethicsFormId");
                });

            migrationBuilder.CreateTable(
                name: "EthicsApplicationLog",
                columns: table => new
                {
                    logId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    changeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsApplicationLog", x => x.logId);
                    table.ForeignKey(
                        name: "FK_EthicsApplicationLog_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthicsApplicationLog_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "EthicsClearance",
                columns: table => new
                {
                    ethicsClearanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    issuedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    expirationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    file = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsClearance", x => x.ethicsClearanceId);
                    table.ForeignKey(
                        name: "FK_EthicsClearance_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptInfo",
                columns: table => new
                {
                    receiptNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    amountPaid = table.Column<float>(type: "real", nullable: false),
                    datePaid = table.Column<DateOnly>(type: "date", nullable: false),
                    scanReceipt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptInfo", x => x.receiptNo);
                    table.ForeignKey(
                        name: "FK_ReceiptInfo_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chairperson",
                columns: table => new
                {
                    chairpersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chairperson", x => x.chairpersonId);
                    table.ForeignKey(
                        name: "FK_Chairperson_Faculty_facultyId",
                        column: x => x.facultyId,
                        principalTable: "Faculty",
                        principalColumn: "facultyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EthicsEvaluator",
                columns: table => new
                {
                    ethicsEvaluatorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyId = table.Column<int>(type: "int", nullable: false),
                    completedEval = table.Column<int>(type: "int", nullable: false),
                    pendingEval = table.Column<int>(type: "int", nullable: false),
                    declinedAssignment = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsEvaluator", x => x.ethicsEvaluatorId);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluator_Faculty_facultyId",
                        column: x => x.facultyId,
                        principalTable: "Faculty",
                        principalColumn: "facultyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secretariat",
                columns: table => new
                {
                    secretariatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facultyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretariat", x => x.secretariatId);
                    table.ForeignKey(
                        name: "FK_Secretariat_Faculty_facultyId",
                        column: x => x.facultyId,
                        principalTable: "Faculty",
                        principalColumn: "facultyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonFundedResearchInfo",
                columns: table => new
                {
                    nonFundedResearchId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    ethicsClearanceId = table.Column<int>(type: "int", nullable: true),
                    completionCertId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    campus = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    college = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    university = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    completion_Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonFundedResearchInfo", x => x.nonFundedResearchId);
                    table.ForeignKey(
                        name: "FK_NonFundedResearchInfo_CompletionCertificate_completionCertId",
                        column: x => x.completionCertId,
                        principalTable: "CompletionCertificate",
                        principalColumn: "completionCertId");
                    table.ForeignKey(
                        name: "FK_NonFundedResearchInfo_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonFundedResearchInfo_EthicsClearance_ethicsClearanceId",
                        column: x => x.ethicsClearanceId,
                        principalTable: "EthicsClearance",
                        principalColumn: "ethicsClearanceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonFundedResearchInfo_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EthicsEvaluation",
                columns: table => new
                {
                    evaluationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    ethicsEvaluatorId = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateOnly>(type: "date", nullable: false),
                    endDate = table.Column<DateOnly>(type: "date", nullable: true),
                    recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    protocolReviewSheet = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    informedConsentForm = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsEvaluation", x => x.evaluationId);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluation_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluation_EthicsEvaluator_ethicsEvaluatorId",
                        column: x => x.ethicsEvaluatorId,
                        principalTable: "EthicsEvaluator",
                        principalColumn: "ethicsEvaluatorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EthicsEvaluatorExpertise",
                columns: table => new
                {
                    ethicsEvaluatorId = table.Column<int>(type: "int", nullable: false),
                    expertiseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthicsEvaluatorExpertise", x => new { x.ethicsEvaluatorId, x.expertiseId });
                    table.ForeignKey(
                        name: "FK_EthicsEvaluatorExpertise_EthicsEvaluator_ethicsEvaluatorId",
                        column: x => x.ethicsEvaluatorId,
                        principalTable: "EthicsEvaluator",
                        principalColumn: "ethicsEvaluatorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EthicsEvaluatorExpertise_Expertise_expertiseId",
                        column: x => x.expertiseId,
                        principalTable: "Expertise",
                        principalColumn: "expertiseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InitialReview",
                columns: table => new
                {
                    initalReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    secretariatId = table.Column<int>(type: "int", nullable: true),
                    chiefId = table.Column<int>(type: "int", nullable: true),
                    urecNo = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    dateReviewed = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialReview", x => x.initalReviewId);
                    table.ForeignKey(
                        name: "FK_InitialReview_Chief_chiefId",
                        column: x => x.chiefId,
                        principalTable: "Chief",
                        principalColumn: "chiefId");
                    table.ForeignKey(
                        name: "FK_InitialReview_EthicsApplication_urecNo",
                        column: x => x.urecNo,
                        principalTable: "EthicsApplication",
                        principalColumn: "urecNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitialReview_Secretariat_secretariatId",
                        column: x => x.secretariatId,
                        principalTable: "Secretariat",
                        principalColumn: "secretariatId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CoProponent",
                columns: table => new
                {
                    coProponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nonFundedResearchId = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    coProponentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    coProponentEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoProponent", x => x.coProponentId);
                    table.ForeignKey(
                        name: "FK_CoProponent_NonFundedResearchInfo_nonFundedResearchId",
                        column: x => x.nonFundedResearchId,
                        principalTable: "NonFundedResearchInfo",
                        principalColumn: "nonFundedResearchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chairperson_facultyId",
                table: "Chairperson",
                column: "facultyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chief_userId",
                table: "Chief",
                column: "userId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompletionReport_urecNo",
                table: "CompletionReport",
                column: "urecNo",
                unique: true,
                filter: "[urecNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CoProponent_nonFundedResearchId",
                table: "CoProponent",
                column: "nonFundedResearchId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplication_userId",
                table: "EthicsApplication",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplicationForms_ethicsFormId",
                table: "EthicsApplicationForms",
                column: "ethicsFormId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplicationForms_urecNo",
                table: "EthicsApplicationForms",
                column: "urecNo");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplicationLog_urecNo",
                table: "EthicsApplicationLog",
                column: "urecNo");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsApplicationLog_userId",
                table: "EthicsApplicationLog",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsClearance_urecNo",
                table: "EthicsClearance",
                column: "urecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluation_ethicsEvaluatorId",
                table: "EthicsEvaluation",
                column: "ethicsEvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluation_urecNo",
                table: "EthicsEvaluation",
                column: "urecNo");

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluator_facultyId",
                table: "EthicsEvaluator",
                column: "facultyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EthicsEvaluatorExpertise_expertiseId",
                table: "EthicsEvaluatorExpertise",
                column: "expertiseId");

            migrationBuilder.CreateIndex(
                name: "IX_Faculty_userId",
                table: "Faculty",
                column: "userId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InitialReview_chiefId",
                table: "InitialReview",
                column: "chiefId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialReview_secretariatId",
                table: "InitialReview",
                column: "secretariatId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialReview_urecNo",
                table: "InitialReview",
                column: "urecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonFundedResearchInfo_completionCertId",
                table: "NonFundedResearchInfo",
                column: "completionCertId",
                unique: true,
                filter: "[completionCertId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NonFundedResearchInfo_ethicsClearanceId",
                table: "NonFundedResearchInfo",
                column: "ethicsClearanceId",
                unique: true,
                filter: "[ethicsClearanceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NonFundedResearchInfo_urecNo",
                table: "NonFundedResearchInfo",
                column: "urecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonFundedResearchInfo_userId",
                table: "NonFundedResearchInfo",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptInfo_urecNo",
                table: "ReceiptInfo",
                column: "urecNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Secretariat_facultyId",
                table: "Secretariat",
                column: "facultyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chairperson");

            migrationBuilder.DropTable(
                name: "CompletionReport");

            migrationBuilder.DropTable(
                name: "CoProponent");

            migrationBuilder.DropTable(
                name: "EthicsApplicationForms");

            migrationBuilder.DropTable(
                name: "EthicsApplicationLog");

            migrationBuilder.DropTable(
                name: "EthicsEvaluation");

            migrationBuilder.DropTable(
                name: "EthicsEvaluatorExpertise");

            migrationBuilder.DropTable(
                name: "InitialReview");

            migrationBuilder.DropTable(
                name: "ReceiptInfo");

            migrationBuilder.DropTable(
                name: "NonFundedResearchInfo");

            migrationBuilder.DropTable(
                name: "EthicsForm");

            migrationBuilder.DropTable(
                name: "EthicsEvaluator");

            migrationBuilder.DropTable(
                name: "Expertise");

            migrationBuilder.DropTable(
                name: "Chief");

            migrationBuilder.DropTable(
                name: "Secretariat");

            migrationBuilder.DropTable(
                name: "CompletionCertificate");

            migrationBuilder.DropTable(
                name: "EthicsClearance");

            migrationBuilder.DropTable(
                name: "Faculty");

            migrationBuilder.DropTable(
                name: "EthicsApplication");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
