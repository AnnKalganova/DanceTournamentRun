using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using Microsoft.Data.SqlClient;


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

        public virtual DbSet<Dance> Dances { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupsDance> GroupsDances { get; set; }
        public virtual DbSet<Heat> Heats { get; set; }
        public virtual DbSet<Pair> Pairs { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersGroup> UsersGroups { get; set; }
        public virtual DbSet<UsersTournament> UsersTournaments { get; set; }

        public ICollection<Group> GetGroups(long tournId)
        {
            //TODO: заменить на функции внутри бд

            return null;
        }

        public ICollection<Dance> GetDances(long groupId)
        {
            SqlParameter param = new SqlParameter("@groupId", groupId);
            var dances = Dances.FromSqlRaw("EXEC GetDancesByGroupId @groupId", param).ToList();
            return dances;
        }

        public List<Pair> GetPairsByTourn(long tournId)
        {
            SqlParameter param = new SqlParameter("@tournId", tournId);
            var pairs = Pairs.FromSqlRaw("EXEC GetPairsByTournId @tournId", param).ToList();
            return pairs;
        }

        public List<User> GetRefereesByTourn(long tournId)
        {
            SqlParameter param = new SqlParameter("@tournId", tournId);
            var referees = Users.FromSqlRaw("EXEC GetRefereesByTournId @tournId", param).ToList();
            return referees;
        }
        
        public List<User> GetRegistratorsByTournId(long tournId)
        {
            SqlParameter param = new SqlParameter("@tournId", tournId);
            var registrators = Users.FromSqlRaw("EXEC GetRegistratorsByTournId @tournId", param).ToList();
            return registrators;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DanceTournamentRun;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Dance>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.IsCompetitionOn).HasColumnName("isCompetitionOn");

                entity.Property(e => e.IsRegistrationOn)
                    .IsRequired()
                    .HasColumnName("isRegistrationOn")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK_dbo.Groups_dbo.Tournaments_TournamentId");
            });

            modelBuilder.Entity<GroupsDance>(entity =>
            {
                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.GroupsDances)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.GroupsDances_dbo.Dances_DanceId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupsDances)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.GroupsDances_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<Heat>(entity =>
            {
                entity.Property(e => e.Heat1).HasColumnName("Heat");

                entity.HasOne(d => d.Pair)
                    .WithMany(p => p.Heats)
                    .HasForeignKey(d => d.PairId)
                    .HasConstraintName("FK_dbo.Heats_dbo.Pairs_PairId");
            });

            modelBuilder.Entity<Pair>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Pairs)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.Pairs_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<Point>(entity =>
            {
                entity.Property(e => e.Point1).HasColumnName("Point");

                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.Points)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.Points_dbo.Dances_DanceId");

                entity.HasOne(d => d.Pair)
                    .WithMany(p => p.Points)
                    .HasForeignKey(d => d.PairId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Points_dbo.Pairs_PairId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Points)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.Points_dbo.Users_UserId");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasOne(d => d.Pair)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.PairId)
                    .HasConstraintName("FK_dbo.Results_dbo.Pairs_PairId");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.Property(e => e.IsTournamentRun)
                    .HasColumnName("isTournamentRun")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsFirstStepOver)
                   .HasColumnName("isFirstStepOver")
                   .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsSecondStepOver)
                    .HasColumnName("isSecondStepOver")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsTournamentRun)
                    .HasColumnName("isTournamentRun")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Tournament_User");

                

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Login).IsRequired();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<UsersGroup>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.UsersGroups)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.UsersGroups_dbo.Groups_GroupId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersGroups)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.UsersGroups_dbo.Users_UserId");
            });

            modelBuilder.Entity<UsersTournament>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersTournaments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.UsersTournaments_dbo.Users_UserId");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.UsersTournaments)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("[FK_dbo.UsersTournaments_dbo.Tournaments_TournamentId]");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
