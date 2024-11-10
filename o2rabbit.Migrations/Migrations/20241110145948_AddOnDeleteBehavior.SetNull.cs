using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace o2rabbit.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddOnDeleteBehaviorSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Processes_ProcessId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Tickets_ParentId",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes",
                column: "ParentId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Processes_ProcessId",
                table: "Tickets",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Tickets_ParentId",
                table: "Tickets",
                column: "ParentId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Processes_ProcessId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Tickets_ParentId",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes",
                column: "ParentId",
                principalTable: "Processes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Processes_ProcessId",
                table: "Tickets",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Tickets_ParentId",
                table: "Tickets",
                column: "ParentId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
