using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LimitlessFit.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationAdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "additional_data",
                table: "notifications",
                type: "varchar(4000)",
                maxLength: 4000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additional_data",
                table: "notifications");
        }
    }
}
