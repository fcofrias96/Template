using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Template.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Action",
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
                    table.PrimaryKey("PK_Action", x => x.Id);
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
                        name: "FK_Permissions_Action_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Action",
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
                table: "Action",
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
                table: "Resources",
                columns: new[] { "Id", "LastModified", "LastModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "User" },
                    { 2, null, null, "Role" },
                    { 3, null, null, "Resource" },
                    { 4, null, null, "Permission" },
                    { 5, null, null, "Action" },
                    { 6, null, null, "Category" },
                    { 7, null, null, "Item" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "LastModified", "LastModifiedBy", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e7fa15d-6e45-4c4a-a407-e71ae21441da", null, null, null, "Admin", "ADMIN" },
                    { "9aa0e20c-40a1-46e8-9983-3701c3627a27", null, null, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsTokenRevoked", "LastModified", "LastModifiedBy", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TokenRevokedTime", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "67a9f532-b747-42b8-bda9-e7cd7c2c838c", 0, "84e517d1-e5e0-4e16-8567-f6f3a9d8035b", "johnDoe@gmail.com", true, "John", false, null, null, "Doe", false, null, "JOHNDOE@GMAIL.COM", "JOHNDOE", "AQAAAAIAAYagAAAAEO2M1w9ktRjGBoXjp1xPk/1B5CpB1MU3vihpyCO9iFhyZr9bjBRKzM3OfyeiJ2JK2w==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "8d7fefcf-47fd-40b3-abc7-9347d5bb3448", null, false, "johnDoe" },
                    { "87361a30-cfb7-477b-81a9-0302b879bffd", 0, "d0bcf088-ce0f-40b0-a341-143826ffa09b", "johnJames@gmail.com", true, "John", false, null, null, "James", false, null, "JOHNJAMES.COM", "JOHNJAMES", "AQAAAAIAAYagAAAAEOJhPFJdBxHu/V1VJcG3WNbvyxFH68C3n499wAhr3FYJ/7X7nxNJOSyUOaZbch8RKQ==", null, false, "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "c05f98a9-66a8-4e80-a8dd-8495bf3fc48f", null, false, "johnJames" }
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
                    { 22, 1, null, null, 6 },
                    { 23, 2, null, null, 6 },
                    { 24, 3, null, null, 6 },
                    { 25, 4, null, null, 6 },
                    { 26, 5, null, null, 6 },
                    { 27, 1, null, null, 7 },
                    { 28, 2, null, null, 7 },
                    { 29, 3, null, null, 7 },
                    { 30, 4, null, null, 7 },
                    { 31, 5, null, null, 7 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "Discriminator" },
                values: new object[,]
                {
                    { "1e7fa15d-6e45-4c4a-a407-e71ae21441da", "67a9f532-b747-42b8-bda9-e7cd7c2c838c", "UserRole" },
                    { "9aa0e20c-40a1-46e8-9983-3701c3627a27", "67a9f532-b747-42b8-bda9-e7cd7c2c838c", "UserRole" },
                    { "9aa0e20c-40a1-46e8-9983-3701c3627a27", "87361a30-cfb7-477b-81a9-0302b879bffd", "UserRole" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 2, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 3, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 4, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 5, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 6, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 7, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 8, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 9, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 10, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 11, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 12, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 13, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 14, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 15, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 16, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 17, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 18, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 19, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 20, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 21, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 22, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 23, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 24, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 25, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 26, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 27, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 28, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 29, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 30, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 31, "1e7fa15d-6e45-4c4a-a407-e71ae21441da" },
                    { 23, "9aa0e20c-40a1-46e8-9983-3701c3627a27" },
                    { 28, "9aa0e20c-40a1-46e8-9983-3701c3627a27" }
                });

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
                name: "Action");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}