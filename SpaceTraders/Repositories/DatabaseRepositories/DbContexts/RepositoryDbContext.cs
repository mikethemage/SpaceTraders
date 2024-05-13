using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbContexts;

public class RepositoryDbContext : DbContext
{
    public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options)
    {
    }

    public RepositoryDbContext()
    {
    }

    public DbSet<Token> Tokens { get; set; }
    public DbSet<Agent> Agents { get; set; }
}
