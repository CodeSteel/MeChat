using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeChat.Migrations
{
    /// <inheritdoc />
    public partial class addNameToChatGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ChatGroups",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ChatGroups");
        }
    }
}
