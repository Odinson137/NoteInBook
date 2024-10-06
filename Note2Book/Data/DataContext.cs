using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Note2Book.Models;

namespace Note2Book.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        if (Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
        {
            if (!databaseCreator.CanConnect()) databaseCreator.Create();
            if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Folder>()
            .HasMany(c => c.Notes)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<BookComment> BookComments { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Citation> Citations { get; set; }
}