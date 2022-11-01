using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSE.Carrinho.API.Migrations
{
    public partial class AjusteContextCarrinho : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_CarrinhoClientes_ClienteId",
                table: "CarrinhoClientes",
                newName: "IDX_Cliente");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IDX_Cliente",
                table: "CarrinhoClientes",
                newName: "IX_CarrinhoClientes_ClienteId");
        }
    }
}
