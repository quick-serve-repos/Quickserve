using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuickServe.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientNutritions_Ingredients_IngredientId",
                table: "IngredientNutritions");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientNutritions_Nutritions_NutritionId",
                table: "IngredientNutritions");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientProducts_Ingredients_IngredientId",
                table: "IngredientProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientProducts_ProDucts_ProductId",
                table: "IngredientProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_IngredientTypes_IngredientTypeId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientSessions_Ingredients_IngredientId",
                table: "IngredientSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientSessions_Sessions_SessionId",
                table: "IngredientSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientTypeTemplateSteps_IngredientTypes_IngredientTypeId",
                table: "IngredientTypeTemplateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientTypeTemplateSteps_TemplateSteps_TemplateStepId",
                table: "IngredientTypeTemplateSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_Orders_OrderId",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProducts_ProDucts_ProductId",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Account_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Stores_StoreId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProDucts_ProductTemplates_ProductTemplateId",
                table: "ProDucts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplates_Categories_CategoryId",
                table: "ProductTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Orders_OrderId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateSteps_ProductTemplates_ProductTemplateId",
                table: "TemplateSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateSteps",
                table: "TemplateSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stores",
                table: "Stores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTemplates",
                table: "ProductTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProDucts",
                table: "ProDucts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nutritions",
                table: "Nutritions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientTypeTemplateSteps",
                table: "IngredientTypeTemplateSteps");

            migrationBuilder.DropIndex(
                name: "IX_IngredientTypeTemplateSteps_IngredientTypeId",
                table: "IngredientTypeTemplateSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientTypes",
                table: "IngredientTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "TemplateSteps",
                newName: "TemplateStep");

            migrationBuilder.RenameTable(
                name: "Stores",
                newName: "Store");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "Session");

            migrationBuilder.RenameTable(
                name: "ProductTemplates",
                newName: "ProductTemplate");

            migrationBuilder.RenameTable(
                name: "ProDucts",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "OrderProducts",
                newName: "OrderProduct");

            migrationBuilder.RenameTable(
                name: "Nutritions",
                newName: "Nutrition");

            migrationBuilder.RenameTable(
                name: "IngredientTypeTemplateSteps",
                newName: "IngredientType_TemplateStep");

            migrationBuilder.RenameTable(
                name: "IngredientTypes",
                newName: "IngredientType");

            migrationBuilder.RenameTable(
                name: "Ingredients",
                newName: "Ingredient");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "Orders",
                newName: "Store_id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Orders",
                newName: "Customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_StoreId",
                table: "Orders",
                newName: "IX_Orders_Store_id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                newName: "IX_Orders_Customer_id");

            migrationBuilder.RenameColumn(
                name: "ProductTemplateId",
                table: "TemplateStep",
                newName: "ProductTemplate_Id");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateSteps_ProductTemplateId",
                table: "TemplateStep",
                newName: "IX_TemplateStep_ProductTemplate_Id");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Session",
                newName: "Start_Time");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Session",
                newName: "Order_Id");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Session",
                newName: "End_Time");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_OrderId",
                table: "Session",
                newName: "IX_Session_Order_Id");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductTemplate",
                newName: "Image_url");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "ProductTemplate",
                newName: "Category_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTemplates_CategoryId",
                table: "ProductTemplate",
                newName: "IX_ProductTemplate_Category_Id");

            migrationBuilder.RenameColumn(
                name: "ProductTemplateId",
                table: "Product",
                newName: "ProductTemplate_Id");

            migrationBuilder.RenameIndex(
                name: "IX_ProDucts_ProductTemplateId",
                table: "Product",
                newName: "IX_Product_ProductTemplate_Id");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Payment",
                newName: "Payment_type");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderProduct",
                newName: "ProductID");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderProduct",
                newName: "OrderID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProducts_ProductId",
                table: "OrderProduct",
                newName: "IX_OrderProduct_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProducts_OrderId",
                table: "OrderProduct",
                newName: "IX_OrderProduct_OrderID");

            migrationBuilder.RenameColumn(
                name: "TemplateStepId",
                table: "IngredientType_TemplateStep",
                newName: "TemplateStep_Id");

            migrationBuilder.RenameColumn(
                name: "QuantityMin",
                table: "IngredientType_TemplateStep",
                newName: "Quantity_Min");

            migrationBuilder.RenameColumn(
                name: "QuantityMax",
                table: "IngredientType_TemplateStep",
                newName: "Quantity_Max");

            migrationBuilder.RenameColumn(
                name: "IngredientTypeId",
                table: "IngredientType_TemplateStep",
                newName: "IngredientType_Id");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientTypeTemplateSteps_TemplateStepId",
                table: "IngredientType_TemplateStep",
                newName: "IX_IngredientType_TemplateStep_TemplateStep_Id");

            migrationBuilder.RenameColumn(
                name: "IngredientTypeId",
                table: "Ingredient",
                newName: "IngredientType_id");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Ingredient",
                newName: "Image_Url");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_IngredientTypeId",
                table: "Ingredient",
                newName: "IX_Ingredient_IngredientType_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Orders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Account",
                type: "character varying(40)",
                unicode: false,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Account",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Account",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Account",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TemplateStep",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "TemplateStep",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TemplateStep",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Store",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Store",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Store",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Store",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Session",
                type: "character varying(40)",
                unicode: false,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductTemplate",
                type: "character varying(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ProductTemplate",
                type: "numeric(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTemplate",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "ProductTemplate",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "ProductTemplate",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Image_url",
                table: "ProductTemplate",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Product",
                type: "numeric(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Product",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Product",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Product",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<long>(
                name: "RefOrderId",
                table: "Payment",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Payment",
                type: "character varying(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Vitamin",
                table: "Nutrition",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Nutrition",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Nutrition",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Nutrition",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HealthValue",
                table: "Nutrition",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Nutrition",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Nutrition",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "IngredientType_TemplateStep",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IngredientType",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "IngredientType",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "IngredientType",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Ingredient",
                type: "numeric(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ingredient",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Ingredient",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ingredient",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Ingredient",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Image_Url",
                table: "Ingredient",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "character varying(40)",
                unicode: false,
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Category",
                type: "uuid",
                unicode: false,
                maxLength: 40,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Category",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateStep",
                table: "TemplateStep",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Store",
                table: "Store",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Session",
                table: "Session",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTemplate",
                table: "ProductTemplate",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProduct",
                table: "OrderProduct",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nutrition",
                table: "Nutrition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Ingredie__D37B8AD7132C9711",
                table: "IngredientType_TemplateStep",
                columns: new[] { "IngredientType_Id", "TemplateStep_Id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientType",
                table: "IngredientType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ingredient",
                table: "Ingredient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Staff_Account",
                        column: x => x.EmployeeId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staff_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RefOrderId",
                table: "Payment",
                column: "RefOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StoreId",
                table: "Employee",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "ingredient_ingredienttype_id_foreign",
                table: "Ingredient",
                column: "IngredientType_id",
                principalTable: "IngredientType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientNutritions_Ingredient_IngredientId",
                table: "IngredientNutritions",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Ingredien__Nutri__6A30C649",
                table: "IngredientNutritions",
                column: "NutritionId",
                principalTable: "Nutrition",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientProducts_Ingredient_IngredientId",
                table: "IngredientProducts",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientProducts_Product_ProductId",
                table: "IngredientProducts",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientSessions_Ingredient_IngredientId",
                table: "IngredientSessions",
                column: "IngredientId",
                principalTable: "Ingredient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Session__Session__6A30C649",
                table: "IngredientSessions",
                column: "SessionId",
                principalTable: "Session",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "IngredientType_templateStep_ingredienttype_id_foreign",
                table: "IngredientType_TemplateStep",
                column: "IngredientType_Id",
                principalTable: "IngredientType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "ingredienttype_templatestep_templatestep_id_foreign",
                table: "IngredientType_TemplateStep",
                column: "TemplateStep_Id",
                principalTable: "TemplateStep",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Order__2A164134",
                table: "OrderProduct",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Produ__2B0A656D",
                table: "OrderProduct",
                column: "ProductID",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "order_customer_id_foreign",
                table: "Orders",
                column: "Customer_id",
                principalTable: "Account",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "order_store_id_foreign",
                table: "Orders",
                column: "Store_id",
                principalTable: "Store",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "order_payment_method_id_foreign",
                table: "Payment",
                column: "RefOrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplate_ID",
                table: "Product",
                column: "ProductTemplate_Id",
                principalTable: "ProductTemplate",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "producttemplate_categoryid_foreign",
                table: "ProductTemplate",
                column: "Category_Id",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "order_session_id_foreign",
                table: "Session",
                column: "Order_Id",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "templatestep_proucttemplate_id_foreign",
                table: "TemplateStep",
                column: "ProductTemplate_Id",
                principalTable: "ProductTemplate",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ingredient_ingredienttype_id_foreign",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientNutritions_Ingredient_IngredientId",
                table: "IngredientNutritions");

            migrationBuilder.DropForeignKey(
                name: "FK__Ingredien__Nutri__6A30C649",
                table: "IngredientNutritions");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientProducts_Ingredient_IngredientId",
                table: "IngredientProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientProducts_Product_ProductId",
                table: "IngredientProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientSessions_Ingredient_IngredientId",
                table: "IngredientSessions");

            migrationBuilder.DropForeignKey(
                name: "FK__Session__Session__6A30C649",
                table: "IngredientSessions");

            migrationBuilder.DropForeignKey(
                name: "IngredientType_templateStep_ingredienttype_id_foreign",
                table: "IngredientType_TemplateStep");

            migrationBuilder.DropForeignKey(
                name: "ingredienttype_templatestep_templatestep_id_foreign",
                table: "IngredientType_TemplateStep");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Order__2A164134",
                table: "OrderProduct");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Produ__2B0A656D",
                table: "OrderProduct");

            migrationBuilder.DropForeignKey(
                name: "order_customer_id_foreign",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "order_store_id_foreign",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "order_payment_method_id_foreign",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTemplate_ID",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "producttemplate_categoryid_foreign",
                table: "ProductTemplate");

            migrationBuilder.DropForeignKey(
                name: "order_session_id_foreign",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "templatestep_proucttemplate_id_foreign",
                table: "TemplateStep");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateStep",
                table: "TemplateStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Store",
                table: "Store");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Session",
                table: "Session");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTemplate",
                table: "ProductTemplate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_RefOrderId",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProduct",
                table: "OrderProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Nutrition",
                table: "Nutrition");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Ingredie__D37B8AD7132C9711",
                table: "IngredientType_TemplateStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IngredientType",
                table: "IngredientType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ingredient",
                table: "Ingredient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "TemplateStep",
                newName: "TemplateSteps");

            migrationBuilder.RenameTable(
                name: "Store",
                newName: "Stores");

            migrationBuilder.RenameTable(
                name: "Session",
                newName: "Sessions");

            migrationBuilder.RenameTable(
                name: "ProductTemplate",
                newName: "ProductTemplates");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "ProDucts");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "OrderProduct",
                newName: "OrderProducts");

            migrationBuilder.RenameTable(
                name: "Nutrition",
                newName: "Nutritions");

            migrationBuilder.RenameTable(
                name: "IngredientType_TemplateStep",
                newName: "IngredientTypeTemplateSteps");

            migrationBuilder.RenameTable(
                name: "IngredientType",
                newName: "IngredientTypes");

            migrationBuilder.RenameTable(
                name: "Ingredient",
                newName: "Ingredients");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "Store_id",
                table: "Orders",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "Customer_id",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_Store_id",
                table: "Orders",
                newName: "IX_Orders_StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_Customer_id",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameColumn(
                name: "ProductTemplate_Id",
                table: "TemplateSteps",
                newName: "ProductTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_TemplateStep_ProductTemplate_Id",
                table: "TemplateSteps",
                newName: "IX_TemplateSteps_ProductTemplateId");

            migrationBuilder.RenameColumn(
                name: "Start_Time",
                table: "Sessions",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Order_Id",
                table: "Sessions",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "End_Time",
                table: "Sessions",
                newName: "EndTime");

            migrationBuilder.RenameIndex(
                name: "IX_Session_Order_Id",
                table: "Sessions",
                newName: "IX_Sessions_OrderId");

            migrationBuilder.RenameColumn(
                name: "Image_url",
                table: "ProductTemplates",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "Category_Id",
                table: "ProductTemplates",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTemplate_Category_Id",
                table: "ProductTemplates",
                newName: "IX_ProductTemplates_CategoryId");

            migrationBuilder.RenameColumn(
                name: "ProductTemplate_Id",
                table: "ProDucts",
                newName: "ProductTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductTemplate_Id",
                table: "ProDucts",
                newName: "IX_ProDucts_ProductTemplateId");

            migrationBuilder.RenameColumn(
                name: "Payment_type",
                table: "Payments",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "OrderProducts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "OrderProducts",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProduct_ProductID",
                table: "OrderProducts",
                newName: "IX_OrderProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProduct_OrderID",
                table: "OrderProducts",
                newName: "IX_OrderProducts_OrderId");

            migrationBuilder.RenameColumn(
                name: "Quantity_Min",
                table: "IngredientTypeTemplateSteps",
                newName: "QuantityMin");

            migrationBuilder.RenameColumn(
                name: "Quantity_Max",
                table: "IngredientTypeTemplateSteps",
                newName: "QuantityMax");

            migrationBuilder.RenameColumn(
                name: "TemplateStep_Id",
                table: "IngredientTypeTemplateSteps",
                newName: "TemplateStepId");

            migrationBuilder.RenameColumn(
                name: "IngredientType_Id",
                table: "IngredientTypeTemplateSteps",
                newName: "IngredientTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientType_TemplateStep_TemplateStep_Id",
                table: "IngredientTypeTemplateSteps",
                newName: "IX_IngredientTypeTemplateSteps_TemplateStepId");

            migrationBuilder.RenameColumn(
                name: "IngredientType_id",
                table: "Ingredients",
                newName: "IngredientTypeId");

            migrationBuilder.RenameColumn(
                name: "Image_Url",
                table: "Ingredients",
                newName: "ImageUrl");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredient_IngredientType_id",
                table: "Ingredients",
                newName: "IX_Ingredients_IngredientTypeId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Account",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldUnicode: false,
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TemplateSteps",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "TemplateSteps",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TemplateSteps",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Stores",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Stores",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Stores",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldUnicode: false,
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ProductTemplates",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "ProductTemplates",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "ProductTemplates",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ProDucts",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProDucts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "ProDucts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "ProDucts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "RefOrderId",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Payments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Vitamin",
                table: "Nutritions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Nutritions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Nutritions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Nutritions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HealthValue",
                table: "Nutritions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Nutritions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Nutritions",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "IngredientTypeTemplateSteps",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IngredientTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "IngredientTypes",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "IngredientTypes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Ingredients",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(8,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Ingredients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Ingredients",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ingredients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "Ingredients",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Ingredients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldUnicode: false,
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Categories",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldUnicode: false,
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModified",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateSteps",
                table: "TemplateSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stores",
                table: "Stores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTemplates",
                table: "ProductTemplates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProDucts",
                table: "ProDucts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProducts",
                table: "OrderProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Nutritions",
                table: "Nutritions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientTypeTemplateSteps",
                table: "IngredientTypeTemplateSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IngredientTypes",
                table: "IngredientTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ingredients",
                table: "Ingredients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTypeTemplateSteps_IngredientTypeId",
                table: "IngredientTypeTemplateSteps",
                column: "IngredientTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientNutritions_Ingredients_IngredientId",
                table: "IngredientNutritions",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientNutritions_Nutritions_NutritionId",
                table: "IngredientNutritions",
                column: "NutritionId",
                principalTable: "Nutritions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientProducts_Ingredients_IngredientId",
                table: "IngredientProducts",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientProducts_ProDucts_ProductId",
                table: "IngredientProducts",
                column: "ProductId",
                principalTable: "ProDucts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_IngredientTypes_IngredientTypeId",
                table: "Ingredients",
                column: "IngredientTypeId",
                principalTable: "IngredientTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientSessions_Ingredients_IngredientId",
                table: "IngredientSessions",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientSessions_Sessions_SessionId",
                table: "IngredientSessions",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientTypeTemplateSteps_IngredientTypes_IngredientTypeId",
                table: "IngredientTypeTemplateSteps",
                column: "IngredientTypeId",
                principalTable: "IngredientTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientTypeTemplateSteps_TemplateSteps_TemplateStepId",
                table: "IngredientTypeTemplateSteps",
                column: "TemplateStepId",
                principalTable: "TemplateSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_Orders_OrderId",
                table: "OrderProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProducts_ProDucts_ProductId",
                table: "OrderProducts",
                column: "ProductId",
                principalTable: "ProDucts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Account_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Stores_StoreId",
                table: "Orders",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProDucts_ProductTemplates_ProductTemplateId",
                table: "ProDucts",
                column: "ProductTemplateId",
                principalTable: "ProductTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTemplates_Categories_CategoryId",
                table: "ProductTemplates",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Orders_OrderId",
                table: "Sessions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateSteps_ProductTemplates_ProductTemplateId",
                table: "TemplateSteps",
                column: "ProductTemplateId",
                principalTable: "ProductTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
