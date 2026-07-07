using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWM_asp.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceId);
                });

            migrationBuilder.CreateTable(
                name: "ArchivedPWDs",
                columns: table => new
                {
                    ArchivedPWDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedPWD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedDataKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedPWDs", x => x.ArchivedPWDId);
                    table.ForeignKey(
                        name: "FK_ArchivedPWDs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchivedPWDs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "SourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedPWDs",
                columns: table => new
                {
                    SavedPWDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedPWD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedDataKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPWDs", x => x.SavedPWDId);
                    table.ForeignKey(
                        name: "FK_SavedPWDs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedPWDs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "SourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedPWDs_SourceId",
                table: "ArchivedPWDs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedPWDs_UserId",
                table: "ArchivedPWDs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPWDs_SourceId",
                table: "SavedPWDs",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPWDs_UserId",
                table: "SavedPWDs",
                column: "UserId");

            migrationBuilder.Sql(@"CREATE TRIGGER trg_SavedPWD_Audit
                ON SavedPWDs
                AFTER UPDATE, DELETE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO ArchivedPWDs
                    (
                        Account,
                        EncryptedPWD,
                        EncryptedDataKey,
                        SourceId,
                        Description,
                        ArchivedAt,
                        ArchivedBy,
                        UserId
                    )
                    SELECT
                        d.Account,
                        d.EncryptedPWD,
                        d.EncryptedDataKey,
                        d.SourceId,
                        d.Description,
                        GETDATE(),
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM inserted WHERE inserted.SavedPWDId = d.SavedPWDId)
                                THEN 'UPDATE'
                            ELSE 'DELETE'
                        END,
                        d.UserId
                    FROM deleted d;
                END;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedPWDs");

            migrationBuilder.DropTable(
                name: "SavedPWDs");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
