using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsersTableAddedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getDate()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "AspNetUsers");
        }
    }
}
