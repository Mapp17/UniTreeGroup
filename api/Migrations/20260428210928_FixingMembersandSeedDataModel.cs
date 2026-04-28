using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class FixingMembersandSeedDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniTreeGroupId1",
                table: "Memberships",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UniTreeGroupId1",
                table: "Memberships",
                column: "UniTreeGroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Memberships_UniTreeGroups_UniTreeGroupId1",
                table: "Memberships",
                column: "UniTreeGroupId1",
                principalTable: "UniTreeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Memberships_UniTreeGroups_UniTreeGroupId1",
                table: "Memberships");

            migrationBuilder.DropIndex(
                name: "IX_Memberships_UniTreeGroupId1",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "UniTreeGroupId1",
                table: "Memberships");
        }
    }
}
