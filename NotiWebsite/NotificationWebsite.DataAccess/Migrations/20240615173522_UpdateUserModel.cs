﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationWebsite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatIdIdentifier",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatIdIdentifier",
                table: "Users");
        }
    }
}
