using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LimitlessFit.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_name",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_user_id",
                table: "orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_user_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "ix_orders_user_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "orders",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
