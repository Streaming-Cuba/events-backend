using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class GroupItemMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GroupItemMetadatas");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "GroupItemMetadatas",
                newName: "ProductorHome");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductorHome",
                table: "GroupItemMetadatas",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GroupItemMetadatas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
