using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Events.API.Migrations.Event
{
    public partial class Changed_MetadataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupItems_GroupItemMetadatas_MetadataId",
                table: "GroupItems");

            migrationBuilder.DropTable(
                name: "GroupItemMetadatas");

            migrationBuilder.DropIndex(
                name: "IX_GroupItems_MetadataId",
                table: "GroupItems");

            migrationBuilder.DropColumn(
                name: "MetadataId",
                table: "GroupItems");

            migrationBuilder.AddColumn<string>(
                name: "MetadataJson",
                table: "GroupItems",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetadataJson",
                table: "GroupItems");

            migrationBuilder.AddColumn<int>(
                name: "MetadataId",
                table: "GroupItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupItemMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Interpreter = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Productor = table.Column<string>(type: "text", nullable: true),
                    ProductorHome = table.Column<string>(type: "text", nullable: true),
                    Support = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemMetadatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupItems_MetadataId",
                table: "GroupItems",
                column: "MetadataId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupItems_GroupItemMetadatas_MetadataId",
                table: "GroupItems",
                column: "MetadataId",
                principalTable: "GroupItemMetadatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
