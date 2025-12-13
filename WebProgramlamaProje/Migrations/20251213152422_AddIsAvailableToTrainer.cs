using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProgramlamaProje.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAvailableToTrainer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Trainers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7f89e68-7db9-44b5-851b-c49bba96c9bc", "AQAAAAIAAYagAAAAEJNuZz76o1vYxklm0QnQnaCgCl1jyVH1/azMT612323EVQju9GelwdCrCkiuD6RHeg==", "d442e0d6-f95b-40eb-807f-c5611076553d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Trainers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "63d067f8-6290-4760-8a74-3e705e63fcff", "AQAAAAIAAYagAAAAEJKgZVyCSd9AC56KIP3OQ8zUMQF/rRCv15AnNxt8Pty0s1Gt5wqjWUVu6fWO7maVww==", "ecb20661-b371-472f-82df-8e1b66433c14" });
        }
    }
}
