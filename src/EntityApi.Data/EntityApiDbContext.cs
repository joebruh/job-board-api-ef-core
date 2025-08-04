using System;
using EntityApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.Data;

public class EntityApiDbContext(DbContextOptions<EntityApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }


}
