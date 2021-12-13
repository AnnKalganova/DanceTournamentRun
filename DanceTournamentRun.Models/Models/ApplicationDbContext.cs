using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DanceTournamentRun.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Club> Clubs { get; set; }
        public virtual DbSet<Coach> Coaches { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DoublePair> DoublePairs { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupsReferee> GroupsReferees { get; set; }
        public virtual DbSet<Pair> Pairs { get; set; }
        public virtual DbSet<Referee> Referees { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DanceTournamentRun;Trusted_Connection=True;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<Club>(entity =>
            {
                entity.HasOne(d => d.City)
                    .WithMany(p => p.Clubs)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Club_City");
            });

            modelBuilder.Entity<Coach>(entity =>
            {
                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Coaches)
                    .HasForeignKey(d => d.ClubId)
                    .HasConstraintName("FK_dbo.Coaches_dbo.Clubs_ClubId");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.BeginTime).HasColumnType("datetime");

                entity.Property(e => e.RegistrationTime).HasColumnType("datetime");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK_dbo.Departments_dbo.Tournaments_TournamentId");
            });

            modelBuilder.Entity<DoublePair>(entity =>
            {
                entity.HasOne(d => d.Pair1)
                    .WithMany(p => p.DoublePairPair1s)
                    .HasForeignKey(d => d.Pair1Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.DoublePairs_dbo.Pairs_Id");

                entity.HasOne(d => d.Pair2)
                    .WithMany(p => p.DoublePairPair2s)
                    .HasForeignKey(d => d.Pair2Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.DoublePairs_dbo.Pairs_Id2");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_dbo.Groups_dbo.Departments_DepartmentId");
            });

            modelBuilder.Entity<GroupsReferee>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupsReferees)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.GroupsReferees_dbo.Groups_GroupId");

                entity.HasOne(d => d.Referee)
                    .WithMany(p => p.GroupsReferees)
                    .HasForeignKey(d => d.RefereeId)
                    .HasConstraintName("FK_dbo.GroupsReferees_dbo.Referees_RefereeId");
            });

            modelBuilder.Entity<Pair>(entity =>
            {
                entity.HasOne(d => d.Coach)
                    .WithMany(p => p.Pairs)
                    .HasForeignKey(d => d.CoachId)
                    .HasConstraintName("FK_dbo.Pairs_dbo.Coaches_CoachId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Pairs)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.Pairs_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Club)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.ClubId)
                    .HasConstraintName("FK_Tournament_Club");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
