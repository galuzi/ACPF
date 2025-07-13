using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ACPF.DataAccess;

namespace ACPF.UI
{
    /// <summary>
    /// Classe responsável por inicializar o banco de dados
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Inicializa o banco de dados, criando-o se não existir
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public static async Task InitializeAsync(AppDbContext context)
        {
            try
            {
                // Cria o banco de dados se não existir
                await context.Database.EnsureCreatedAsync();

                // Verifica se já existem categorias
                if (!await context.Categorias.AnyAsync())
                {
                    // As categorias padrão serão inseridas pelo SeedData no AppDbContext
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco de dados: {ex.Message}");
                throw;
            }
        }
    }
} 