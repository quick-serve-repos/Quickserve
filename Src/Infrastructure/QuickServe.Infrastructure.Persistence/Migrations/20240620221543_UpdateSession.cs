using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickServe.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "order_session_id_foreign",
                table: "Session");

            migrationBuilder.RenameColumn(
                name: "Order_Id",
                table: "Session",
                newName: "Store_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Session_Order_Id",
                table: "Session",
                newName: "IX_Session_Store_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Store_Store_Id",
                table: "Session",
                column: "Store_Id",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_Store_Store_Id",
                table: "Session");

            migrationBuilder.RenameColumn(
                name: "Store_Id",
                table: "Session",
                newName: "Order_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Session_Store_Id",
                table: "Session",
                newName: "IX_Session_Order_Id");

            migrationBuilder.AddForeignKey(
                name: "order_session_id_foreign",
                table: "Session",
                column: "Order_Id",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
