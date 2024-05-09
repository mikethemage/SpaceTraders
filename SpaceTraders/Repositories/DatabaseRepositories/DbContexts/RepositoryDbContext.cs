using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
