using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZndAlhadedStore.Migrations
{
    /// <inheritdoc />
    public partial class After_Library : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationRoleId",
                table: "RolePermissions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ApplicationRoleId",
                table: "RolePermissions",
                column: "ApplicationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_AspNetRoles_ApplicationRoleId",
                table: "RolePermissions",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_AspNetRoles_ApplicationRoleId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_ApplicationRoleId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
