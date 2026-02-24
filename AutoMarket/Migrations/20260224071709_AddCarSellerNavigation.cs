using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoMarket.Migrations
{
    /// <inheritdoc />
    public partial class AddCarSellerNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SellerId",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_SellerId",
                table: "Cars",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_AspNetUsers_SellerId",
                table: "Cars",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_AspNetUsers_SellerId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_SellerId",
                table: "Cars");

            migrationBuilder.AlterColumn<string>(
                name: "SellerId",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
