using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Events.API.Migrations.Event
{
    public partial class Added_Vote_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupItemSocialSocials");

            migrationBuilder.DropColumn(
                name: "Votes",
                table: "GroupItems");

            migrationBuilder.CreateTable(
                name: "GroupItemSocials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    SocialId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemSocials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupItemSocials_GroupItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GroupItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupItemSocials_Socials_SocialId",
                        column: x => x.SocialId,
                        principalTable: "Socials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupItemVoteType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemVoteType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupItemVote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: true),
                    GroupItemId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupItemVote_GroupItems_GroupItemId",
                        column: x => x.GroupItemId,
                        principalTable: "GroupItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupItemVote_GroupItemVoteType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "GroupItemVoteType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemTypes_Name",
                table: "GroupItemTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemSocials_ItemId_SocialId",
                table: "GroupItemSocials",
                columns: new[] { "ItemId", "SocialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemSocials_SocialId",
                table: "GroupItemSocials",
                column: "SocialId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVote_GroupItemId",
                table: "GroupItemVote",
                column: "GroupItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVote_TypeId",
                table: "GroupItemVote",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemVoteType_Name",
                table: "GroupItemVoteType",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupItemSocials");

            migrationBuilder.DropTable(
                name: "GroupItemVote");

            migrationBuilder.DropTable(
                name: "GroupItemVoteType");

            migrationBuilder.DropIndex(
                name: "IX_GroupItemTypes_Name",
                table: "GroupItemTypes");

            migrationBuilder.AddColumn<long>(
                name: "Votes",
                table: "GroupItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "GroupItemSocialSocials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    SocialId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupItemSocialSocials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupItemSocialSocials_GroupItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GroupItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupItemSocialSocials_Socials_SocialId",
                        column: x => x.SocialId,
                        principalTable: "Socials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemSocialSocials_ItemId_SocialId",
                table: "GroupItemSocialSocials",
                columns: new[] { "ItemId", "SocialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupItemSocialSocials_SocialId",
                table: "GroupItemSocialSocials",
                column: "SocialId");
        }
    }
}
