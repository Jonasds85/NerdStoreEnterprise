using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.API.Data.Mappings;
using NSE.Catalogo.API.Models;
using NSE.Core.Data;
using NSE.Core.Messages;

namespace NSE.Catalogo.API.Data
{
    public class CatalogoContext : DbContext, IUnitOfWork
    {
        public CatalogoContext(DbContextOptions<CatalogoContext> opt ) : base( opt ) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();
            //caso vc deixe algum campo string sem especificar o tamanho
            var propertys = modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string)));

            foreach (var property in propertys)
                property.SetColumnType("varchar(100)");            

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProdutoMapping).Assembly);
        }

        public async Task<bool> Commit()
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
