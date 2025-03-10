using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace o2rabbit.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CompleteRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessComment_Processes_ProcessId",
                table: "ProcessComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessComment",
                table: "ProcessComment");

            migrationBuilder.RenameTable(
                name: "ProcessComment",
                newName: "ProcessComments");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessComment_ProcessId",
                table: "ProcessComments",
                newName: "IX_ProcessComments_ProcessId");

            migrationBuilder.AddColumn<long>(
                name: "SpaceId",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessComments",
                table: "ProcessComments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Spaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSpaceMapping",
                columns: table => new
                {
                    ProcessId = table.Column<long>(type: "bigint", nullable: false),
                    SpaceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSpaceMapping", x => new { x.ProcessId, x.SpaceId });
                    table.ForeignKey(
                        name: "FK_ProcessSpace_Processes_AttachableProcessesId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessSpace_Spaces_PossibleSpacesId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpaceComments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SpaceId = table.Column<long>(type: "bigint", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpaceComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpaceComments_Spaces_SpaceId",
                        column: x => x.SpaceId,
                        principalTable: "Spaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SpaceId",
                table: "Tickets",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSpace_PossibleSpacesId",
                table: "ProcessSpaceMapping",
                column: "SpaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SpaceComments_SpaceId",
                table: "SpaceComments",
                column: "SpaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessComments_Processes_ProcessId",
                table: "ProcessComments",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Spaces_SpaceId",
                table: "Tickets",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessComments_Processes_ProcessId",
                table: "ProcessComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Spaces_SpaceId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "ProcessSpaceMapping");

            migrationBuilder.DropTable(
                name: "SpaceComments");

            migrationBuilder.DropTable(
                name: "Spaces");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SpaceId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessComments",
                table: "ProcessComments");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Tickets");

            migrationBuilder.RenameTable(
                name: "ProcessComments",
                newName: "ProcessComment");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessComments_ProcessId",
                table: "ProcessComment",
                newName: "IX_ProcessComment_ProcessId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessComment",
                table: "ProcessComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessComment_Processes_ProcessId",
                table: "ProcessComment",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
