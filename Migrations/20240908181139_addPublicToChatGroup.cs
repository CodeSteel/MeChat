using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeChat.Migrations
{
    /// <inheritdoc />
    public partial class addPublicToChatGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "ChatGroups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Public",
                table: "ChatGroups");
        }
    }
}
