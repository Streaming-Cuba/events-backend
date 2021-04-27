using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class Updated_GroupItem_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItems_Groups_GroupId",
                table: "GroupItems");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "GroupItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItems_Groups_GroupId",
                table: "GroupItems",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItems_Groups_GroupId",
                table: "GroupItems");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "GroupItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItems_Groups_GroupId",
                table: "GroupItems",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
