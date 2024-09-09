using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeChat.Migrations
{
    /// <inheritdoc />
    public partial class addOwnerToChatGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "ChatGroups",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_OwnerId",
                table: "ChatGroups",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroups_AspNetUsers_OwnerId",
                table: "ChatGroups",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroups_AspNetUsers_OwnerId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_OwnerId",
                table: "ChatGroups");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "ChatGroups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
