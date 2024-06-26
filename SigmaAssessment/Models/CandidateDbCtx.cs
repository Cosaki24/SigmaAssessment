﻿using Microsoft.EntityFrameworkCore;

namespace SigmaAssessment.Models
{
    public class CandidateDbCtx : DbContext
    {
        public CandidateDbCtx(DbContextOptions<CandidateDbCtx> options) : base(options)
        {

        }

        public DbSet<Candidate> CandidatesDb { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>().HasKey(c => c.Email).HasName("PK_Candidate_Email"); // set Email as the primary key
        }
    }
}
