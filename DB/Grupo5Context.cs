using Api_login.Models;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class Grupo5Context : DbContext
    {
        public Grupo5Context(DbContextOptions<Grupo5Context> options)
            : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
        }
    }
}