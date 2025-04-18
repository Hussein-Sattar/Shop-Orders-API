using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AAU_Task.Migrations
{
    /// <inheritdoc />
    public partial class addnewcoltoorderdetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LineTotal",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineTotal",
                table: "OrderDetails");
        }
    }
}
