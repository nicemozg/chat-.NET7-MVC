using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Migrations
{
    /// <inheritdoc />
    public partial class ChatNewChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatRoomId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatRoomId",
                table: "AspNetUsers",
                column: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Chats_ChatRoomId",
                table: "AspNetUsers",
                column: "ChatRoomId",
                principalTable: "Chats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Chats_ChatRoomId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatRoomId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "AspNetUsers");
        }
    }
}
