using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeChat.Migrations
{
    /// <inheritdoc />
    public partial class setPostedByNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_PostedById",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostedById",
                table: "Chats",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_PostedById",
                table: "Chats",
                column: "PostedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_PostedById",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostedById",
                table: "Chats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_PostedById",
                table: "Chats",
                column: "PostedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
