using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class Added_EventGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Groups",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_EventId",
                table: "Groups",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Events_EventId",
                table: "Groups",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Events_EventId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_EventId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Groups");
        }
    }
}
