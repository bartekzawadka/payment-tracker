using Microsoft.EntityFrameworkCore.Migrations;

namespace Payment.Tracker.DataLayer.Migrations
{
    public partial class AddedHasInvoicePositionField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasInvoice",
                table: "PaymentPositions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasInvoice",
                table: "PaymentPositions");
        }
    }
}
