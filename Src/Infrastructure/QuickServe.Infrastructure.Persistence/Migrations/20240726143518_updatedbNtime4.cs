using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickServe.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updatedbNtime4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Account_Customer_id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Account",
                newName: "Customer_id");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Point = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_id);
                    table.ForeignKey(
                        name: "FK_Customers_Account_Customer_id",
                        column: x => x.Customer_id,
                        principalTable: "Account",
                        principalColumn: "Customer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "order_customer_id_foreign",
                table: "Orders",
                column: "Customer_id",
                principalTable: "Customers",
                principalColumn: "Customer_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "order_customer_id_foreign",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.RenameColumn(
                name: "Customer_id",
                table: "Account",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Account",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Point",
                table: "Account",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Account_Customer_id",
                table: "Orders",
                column: "Customer_id",
                principalTable: "Account",
                principalColumn: "Id");
        }
    }
}
