using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Events.API.Migrations.Event
{
    public partial class Added_Url_To_Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItemVote_GroupItems_GroupItemId",
                table: "GroupItemVote");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupItemVote_GroupItemVoteType_TypeId",
                table: "GroupItemVote");

            migrationBuilder.DropTable(
                name: "GroupItemVoteType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupItemVote",
                table: "GroupItemVote");

            migrationBuilder.DropIndex(
                name: "IX_GroupItemVote_TypeId",
                table: "GroupItemVote");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "GroupItemVote");

            migrationBuilder.RenameTable(
                name: "GroupItemVote",
                newName: "GroupItemVotes");

            migrationBuilder.RenameIndex(
                name: "IX_GroupItemVote_GroupItemId",
                table: "GroupItemVotes",
                newName: "IX_GroupItemVotes_GroupItemId");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "GroupItemMetadatas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "GroupItemVotes",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupItemVotes",
                table: "GroupItemVotes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVotes_Type",
                table: "GroupItemVotes",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes",
                column: "GroupItemId",
                principalTable: "GroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItemVotes_GroupItems_GroupItemId",
                table: "GroupItemVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupItemVotes",
                table: "GroupItemVotes");

            migrationBuilder.DropIndex(
                name: "IX_GroupItemVotes_Type",
                table: "GroupItemVotes");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "GroupItemMetadatas");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "GroupItemVotes");

            migrationBuilder.RenameTable(
                name: "GroupItemVotes",
                newName: "GroupItemVote");

            migrationBuilder.RenameIndex(
                name: "IX_GroupItemVotes_GroupItemId",
                table: "GroupItemVote",
                newName: "IX_GroupItemVote_GroupItemId");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "GroupItemVote",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupItemVote",
                table: "GroupItemVote",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GroupItemVoteType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemVoteType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVote_TypeId",
                table: "GroupItemVote",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVoteType_Name",
                table: "GroupItemVoteType",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItemVote_GroupItems_GroupItemId",
                table: "GroupItemVote",
                column: "GroupItemId",
                principalTable: "GroupItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItemVote_GroupItemVoteType_TypeId",
                table: "GroupItemVote",
                column: "TypeId",
                principalTable: "GroupItemVoteType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
