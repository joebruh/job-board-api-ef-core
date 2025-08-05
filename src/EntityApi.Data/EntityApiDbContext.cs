using System;
using EntityApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityApi.Data;

public class EntityApiDbContext(DbContextOptions<EntityApiDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<JobPost> JobPosts { get; set; }
    public DbSet<Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.JobPost)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobPostId)
            .OnDelete(DeleteBehavior.Restrict); // prevents multiple cascade paths
    }
}
