using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace o2rabbit.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ProcessIntroduceParentChildRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "Processes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processes_ParentId",
                table: "Processes",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes",
                column: "ParentId",
                principalTable: "Processes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Processes_ParentId",
                table: "Processes");

            migrationBuilder.DropIndex(
                name: "IX_Processes_ParentId",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Processes");
        }
    }
}
