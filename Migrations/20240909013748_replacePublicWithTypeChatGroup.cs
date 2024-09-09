using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeChat.Migrations
{
    /// <inheritdoc />
    public partial class replacePublicWithTypeChatGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Public",
                table: "ChatGroups");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChatGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChatGroups");

            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "ChatGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
