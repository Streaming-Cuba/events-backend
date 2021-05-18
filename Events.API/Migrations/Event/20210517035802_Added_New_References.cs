using Microsoft.EntityFrameworkCore.Migrations;

namespace Events.API.Migrations.Event
{
    public partial class Added_New_References : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Groups",
                newName: "GroupParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_GroupId",
                table: "Groups",
                newName: "IX_Groups_GroupParentId");

            migrationBuilder.AlterColumn<int>(
                name: "GroupItemId",
                table: "GroupItemVotes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes",
                column: "GroupItemId",
                principalTable: "GroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_GroupParentId",
                table: "Groups",
                column: "GroupParentId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_GroupParentId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "GroupParentId",
                table: "Groups",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_GroupParentId",
                table: "Groups",
                newName: "IX_Groups_GroupId");

            migrationBuilder.AlterColumn<int>(
                name: "GroupItemId",
                table: "GroupItemVotes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes",
                column: "GroupItemId",
                principalTable: "GroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
