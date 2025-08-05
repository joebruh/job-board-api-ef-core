using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtToJobPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "JobPosts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobPosts");
        }
    }
}
