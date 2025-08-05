using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToJobPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "JobPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "JobPosts");
        }
    }
}
