using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class GroupArg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupArg",
                table: "Groups",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupArg",
                table: "Groups");
        }
    }
}
