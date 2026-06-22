using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentSummeryChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentSummery_Year",
                table: "Orders",
                newName: "PaymentSummery_ExpYear");

            migrationBuilder.AddColumn<string>(
                name: "BuyerEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerEmail",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PaymentSummery_ExpYear",
                table: "Orders",
                newName: "PaymentSummery_Year");
        }
    }
}
