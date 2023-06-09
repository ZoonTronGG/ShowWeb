using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowWeb.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageUrlProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://petapixel.com/assets/uploads/2022/01/Dell-Launches-the-Ultra-Modern-XPS-13-Plus-with-OLED-and-Intels-Latest-800x420.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://avatars.mds.yandex.net/get-mpic/5288781/img_id2302440974157776602.jpeg/orig");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://nout.kz/upload/resize_cache/webp/iblock/d16/Screenshot_1.webp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");
        }
    }
}
