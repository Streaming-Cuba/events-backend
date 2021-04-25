using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class GroupWithChildGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupArg",
                table: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GroupId",
                table: "Groups",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_GroupId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "GroupArg",
                table: "Groups",
                type: "text",
                nullable: true);
        }
    }
}
