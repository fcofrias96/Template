using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Template.Security.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    ResourceId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_Permissions_ActionType_ActionId",
                        column: x => x.ActionId,
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
                    ResourceActionId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_MenuItems_Permissions_ResourceActionId",
                        column: x => x.ResourceActionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResourceActionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.ResourceActionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_ResourceActionId",
                        column: x => x.ResourceActionId,
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
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "ParentId", "ResourceActionId", "Title", "Url" },
                values: new object[] { 1, "dashboard-icon", null, null, null, null, "Dashboard", "/dashboard" });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "User" },
                    { 2, null, null, "Role" },
                    { 3, null, null, "Resource" },
                    { 4, null, null, "ResourceAction" },
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
                    { "1e37776f-5ac6-43d6-b8d8-c1ef3ded1ecf", null, null, null, "User", "USER" },
                    { "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1", null, null, null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsTokenRevoked", "LastModified", "LastModifiedBy", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TokenRevokedTime", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "7f017cfb-e8f6-4c91-83f5-6c5864563bc0", 0, "c6aaf7f0-9f30-4841-9787-8d7ad28cacb1", "johnJames@gmail.com", true, "John", false, null, null, "James", false, null, "JOHNJAMES@GMAIL.COM", "JOHNJAMES", "AQAAAAIAAYagAAAAEHt5gorIdu0szsqEtJrqw2ZfTRyOyzLlN9cM7615rfLnMuXYmaYckRakn8Mdxhn+GQ==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0bb810dc-1c04-46f7-8227-19022c0c63a8", null, false, "johnJames" },
                    { "d1900cf8-5095-4d2f-8516-c106f13c35f9", 0, "2ac8be78-25ac-4bec-9ad9-138b3593ee30", "johnDoe@gmail.com", true, "John", false, null, null, "Doe", false, null, "JOHNDOE@GMAIL.COM", "JOHNDOE", "AQAAAAIAAYagAAAAECP9KUf3WENZHewVH+GX5+JX8Fq6m71atBTKpcuRgTl4zwMqZ8fUbIdZTcT6WyzkUg==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "3c2aca22-9a5b-463c-b864-cb7107d38d7f", null, false, "johnDoe" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "ActionId", "LastModified", "LastModifiedBy", "ResourceId" },
                values: new object[,]
                {
                    { 1, 1, null, null, 1 },
                    { 2, 2, null, null, 1 },
                    { 3, 3, null, null, 1 },
                    { 4, 4, null, null, 1 },
                    { 5, 5, null, null, 1 },
                    { 6, 1, null, null, 3 },
                    { 7, 2, null, null, 3 },
                    { 8, 3, null, null, 3 },
                    { 9, 4, null, null, 3 },
                    { 10, 5, null, null, 3 },
                    { 11, 1, null, null, 2 },
                    { 12, 2, null, null, 2 },
                    { 13, 3, null, null, 2 },
                    { 14, 4, null, null, 2 },
                    { 15, 5, null, null, 2 },
                    { 16, 1, null, null, 4 },
                    { 17, 2, null, null, 4 },
                    { 18, 3, null, null, 4 },
                    { 19, 4, null, null, 4 },
                    { 20, 5, null, null, 4 },
                    { 21, 2, null, null, 5 },
                    { 22, 1, null, null, 7 },
                    { 23, 2, null, null, 7 },
                    { 24, 3, null, null, 7 },
                    { 25, 4, null, null, 7 },
                    { 26, 5, null, null, 7 },
                    { 27, 1, null, null, 8 },
                    { 28, 2, null, null, 8 },
                    { 29, 3, null, null, 8 },
                    { 30, 4, null, null, 8 },
                    { 31, 5, null, null, 8 },
                    { 32, 2, null, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "Discriminator" },
                values: new object[,]
                {
                    { "1e37776f-5ac6-43d6-b8d8-c1ef3ded1ecf", "7f017cfb-e8f6-4c91-83f5-6c5864563bc0", "UserRole" },
                    { "1e37776f-5ac6-43d6-b8d8-c1ef3ded1ecf", "d1900cf8-5095-4d2f-8516-c106f13c35f9", "UserRole" },
                    { "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1", "d1900cf8-5095-4d2f-8516-c106f13c35f9", "UserRole" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "ParentId", "ResourceActionId", "Title", "Url" },
                values: new object[,]
                {
                    { 4, "categories-icon", null, null, null, 23, "Categorías", "/categorias" },
                    { 5, "items-icon", null, null, null, 28, "Productos", "/productos" },
                    { 6, "settings-icon", null, null, null, 32, "Configuración", "/configuracion" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "ResourceActionId", "RoleId" },
                values: new object[,]
                {
                    { 23, "1e37776f-5ac6-43d6-b8d8-c1ef3ded1ecf" },
                    { 28, "1e37776f-5ac6-43d6-b8d8-c1ef3ded1ecf" },
                    { 1, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 2, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 3, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 4, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 5, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 6, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 7, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 8, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 9, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 10, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 11, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 12, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 13, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 14, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 15, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 16, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 17, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 18, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 19, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 20, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 21, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 22, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 23, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 24, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 25, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 26, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 27, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 28, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 29, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 30, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 31, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" },
                    { 32, "e630cfc5-d2e2-41d6-aff2-994ed21ee9f1" }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "Icon", "LastModified", "LastModifiedBy", "ParentId", "ResourceActionId", "Title", "Url" },
                values: new object[,]
                {
                    { 2, "users-icon", null, null, 6, 2, "Usuarios", "/usuarios" },
                    { 3, "roles-icon", null, null, 6, 12, "Roles", "/roles" },
                    { 7, "permissions-icon", null, null, 6, 17, "Permisos", "/permisos" },
                    { 8, "resources-icon", null, null, 6, 7, "Recursos", "/recursos" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ResourceActionId",
                table: "MenuItems",
                column: "ResourceActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ActionId",
                table: "Permissions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ResourceId_ActionId",
                table: "Permissions",
                columns: new[] { "ResourceId", "ActionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ResourceActionId",
                table: "RolePermissions",
                column: "ResourceActionId");

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
