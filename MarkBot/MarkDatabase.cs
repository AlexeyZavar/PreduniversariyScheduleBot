#region

using MarkBot.Parsers.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace MarkBot;

public sealed class MarkDatabase : DbContext
{
    public MarkDatabase(DbContextOptions<MarkDatabase> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Subscriber> Subscribers { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.UseCollation("NOCASE");
    }
}
