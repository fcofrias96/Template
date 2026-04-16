using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Template.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(25)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(13,2)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Category",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "LastModifiedAt", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Clothes" },
                    { 2, null, null, "HouseholdAppliance" },
                    { 3, null, null, "Fruit" },
                    { 4, null, null, "Tool" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
