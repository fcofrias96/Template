using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Template.Security.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTokenRevoked = table.Column<bool>(type: "bit", nullable: false),
                    TokenRevokedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_ActionType_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "ActionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(30)", nullable: false, defaultValue: "System"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuItems_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ActionType",
                columns: new[] { "Id", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Create" },
                    { 2, null, null, "Read" },
                    { 3, null, null, "Update" },
                    { 4, null, null, "Delete" },
                    { 5, null, null, "Export" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "Order", "ParentId", "PermissionId", "Title", "Url" },
                values: new object[] { 1, "dashboard-icon", null, null, 1, null, null, "Dashboard", "/dashboard" });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "User" },
                    { 2, null, null, "Role" },
                    { 3, null, null, "Resource" },
                    { 4, null, null, "Permission" },
                    { 5, null, null, "Action" },
                    { 6, null, null, "Configuration" },
                    { 7, null, null, "Category" },
                    { 8, null, null, "Item" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "LastModified", "LastModifiedBy", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "17c61777-49f8-44ff-aaad-eb9b4490c5b6", null, null, null, "User", "USER" },
                    { "5505a98f-78d5-4ef9-b7e3-483f2b4996d3", null, null, null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsTokenRevoked", "LastModified", "LastModifiedBy", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TokenRevokedTime", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "5d3bf744-449f-4305-b935-cc0a3de55059", 0, "46b7993c-5be8-4f5c-b303-844162c48b07", "johnJames@gmail.com", true, "John", false, null, null, "James", false, null, "JOHNJAMES@GMAIL.COM", "JOHNJAMES", "AQAAAAIAAYagAAAAEFAkidU+RequXTS61qMrsqKYMHf2K0SMF5yTfKRWsJB69tSKl6/5BwzM2nBLQR4WsA==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5d15f56d-f061-4383-a009-97d15f7130b1", null, false, "johnJames" },
                    { "e0011881-5740-450f-b1cc-c302e8681abf", 0, "da658be5-7bca-449a-80c1-cf695e4e64dd", "johnDoe@gmail.com", true, "John", false, null, null, "Doe", false, null, "JOHNDOE@GMAIL.COM", "JOHNDOE", "AQAAAAIAAYagAAAAEGcSJktWZuuF0X0VSorw7iel8bpgARpg2K4djOxSEgdCnrJ22lOpGV+DNuiRe9KfYw==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "d41a6e71-735c-4970-a88e-9d17cf97e0d7", null, false, "johnDoe" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionTypeId", "Description", "LastModified", "LastModifiedBy", "Name", "ResourceId" },
                values: new object[,]
                {
                    { 1, 1, "Allows creating users", null, null, "Create User", 1 },
                    { 2, 2, "Allows reading user details", null, null, "Read User", 1 },
                    { 3, 3, "Allows updating users", null, null, "Update User", 1 },
                    { 4, 4, "Allows deleting users", null, null, "Delete User", 1 },
                    { 5, 5, "Allows exporting user data", null, null, "Export User", 1 },
                    { 6, 1, "Allows creating resources", null, null, "Create Resource", 3 },
                    { 7, 2, "Allows reading resources", null, null, "Read Resource", 3 },
                    { 8, 3, "Allows updating resources", null, null, "Update Resource", 3 },
                    { 9, 4, "Allows deleting resources", null, null, "Delete Resource", 3 },
                    { 10, 5, "Allows exporting resource data", null, null, "Export Resource", 3 },
                    { 11, 1, "Allows creating roles", null, null, "Create Role", 2 },
                    { 12, 2, "Allows reading roles", null, null, "Read Role", 2 },
                    { 13, 3, "Allows updating roles", null, null, "Update Role", 2 },
                    { 14, 4, "Allows deleting roles", null, null, "Delete Role", 2 },
                    { 15, 5, "Allows exporting role data", null, null, "Export Role", 2 },
                    { 16, 1, "Allows creating permissions", null, null, "Create Permission", 4 },
                    { 17, 2, "Allows reading permissions", null, null, "Read Permission", 4 },
                    { 18, 3, "Allows updating permissions", null, null, "Update Permission", 4 },
                    { 19, 4, "Allows deleting permissions", null, null, "Delete Permission", 4 },
                    { 20, 5, "Allows exporting permission data", null, null, "Export Permission", 4 },
                    { 21, 2, "Allows reading actions", null, null, "Read Action", 5 },
                    { 22, 1, "Allows creating categories", null, null, "Create Category", 7 },
                    { 23, 2, "Allows reading categories", null, null, "Read Category", 7 },
                    { 24, 3, "Allows updating categories", null, null, "Update Category", 7 },
                    { 25, 4, "Allows deleting categories", null, null, "Delete Category", 7 },
                    { 26, 5, "Allows exporting category data", null, null, "Export Category", 7 },
                    { 27, 1, "Allows creating items", null, null, "Create Item", 8 },
                    { 28, 2, "Allows reading items", null, null, "Read Item", 8 },
                    { 29, 3, "Allows updating items", null, null, "Update Item", 8 },
                    { 30, 4, "Allows deleting items", null, null, "Delete Item", 8 },
                    { 31, 5, "Allows exporting item data", null, null, "Export Item", 8 },
                    { 32, 2, "Allows reading configuration settings", null, null, "Read Configuration", 6 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "Discriminator" },
                values: new object[,]
                {
                    { "17c61777-49f8-44ff-aaad-eb9b4490c5b6", "5d3bf744-449f-4305-b935-cc0a3de55059", "UserRole" },
                    { "17c61777-49f8-44ff-aaad-eb9b4490c5b6", "e0011881-5740-450f-b1cc-c302e8681abf", "UserRole" },
                    { "5505a98f-78d5-4ef9-b7e3-483f2b4996d3", "e0011881-5740-450f-b1cc-c302e8681abf", "UserRole" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "Order", "ParentId", "PermissionId", "Title", "Url" },
                values: new object[,]
                {
                    { 4, "categories-icon", null, null, 3, null, 23, "Categorías", "/categorias" },
                    { 5, "items-icon", null, null, 2, null, 28, "Productos", "/productos" },
                    { 6, "settings-icon", null, null, 4, null, 32, "Configuración", "/configuracion" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 23, "17c61777-49f8-44ff-aaad-eb9b4490c5b6" },
                    { 28, "17c61777-49f8-44ff-aaad-eb9b4490c5b6" },
                    { 1, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 2, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 3, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 4, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 5, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 6, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 7, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 8, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 9, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 10, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 11, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 12, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 13, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 14, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 15, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 16, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 17, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 18, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 19, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 20, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 21, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 22, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 23, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 24, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 25, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 26, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 27, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 28, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 29, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 30, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 31, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" },
                    { 32, "5505a98f-78d5-4ef9-b7e3-483f2b4996d3" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "Order", "ParentId", "PermissionId", "Title", "Url" },
                values: new object[,]
                {
                    { 2, "users-icon", null, null, 2, 6, 2, "Usuarios", "/usuarios" },
                    { 3, "roles-icon", null, null, 1, 6, 12, "Roles", "/roles" },
                    { 7, "permissions-icon", null, null, 3, 6, 17, "Permisos", "/permisos" },
                    { 8, "resources-icon", null, null, 4, 6, 7, "Recursos", "/recursos" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId_Order",
                table: "MenuItems",
                columns: new[] { "ParentId", "Order" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PermissionId",
                table: "MenuItems",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ActionTypeId",
                table: "Permissions",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ResourceId_ActionTypeId",
                table: "Permissions",
                columns: new[] { "ResourceId", "ActionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ActionType");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
