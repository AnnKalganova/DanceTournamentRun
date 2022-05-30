using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;


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
        public virtual DbSet<Pair> Pairs { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersGroup> UsersGroups { get; set; }
        public virtual DbSet<UsersTournament> UsersTournaments { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<RefereeProgress> RefereeProgresses { get; set; }
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

        public List<Group> GetGroupsByToken(string token)
        {
            SqlParameter param = new SqlParameter { ParameterName = "@token", Value = token, SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50 };
            var groups = Groups.FromSqlRaw("EXEC GetGroupsByToken @token", param).ToList();
            return groups;
        }

        public int FindSimilarPartner(long grId, long pairId, string lastName, string firstName)
        {
            SqlParameter grIdParam = new SqlParameter("@groupId", grId); 
            SqlParameter pairIdParam = new SqlParameter("@pairId", pairId); 
            SqlParameter lNameParam = new SqlParameter { ParameterName = "@lastName", Value = lastName, SqlDbType = System.Data.SqlDbType.NVarChar, Size = 40 };
            SqlParameter FNameParam = new SqlParameter { ParameterName = "@firstName", Value = firstName, SqlDbType = System.Data.SqlDbType.NVarChar, Size = 40 };
            SqlParameter countParam = new SqlParameter { ParameterName = "@count", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            Database.ExecuteSqlRaw("FindSimilarPartner @groupId, @pairId, @lastName, @firstName, @count OUT", grIdParam, pairIdParam, lNameParam, FNameParam, countParam);
            return (int)countParam.Value;
        }

        public int IsAccessToGroupGranted(long grId, string token)
        {
            SqlParameter grIdParam = new SqlParameter("@groupId", grId);
            SqlParameter tokenParam = new SqlParameter { ParameterName = "@token", Value = token, SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50 };
            SqlParameter resultParam = new SqlParameter { ParameterName = "@result", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            Database.ExecuteSqlRaw("IsAccessToGroupGranted @groupId, @token, @result OUT", grIdParam, tokenParam, resultParam);
            return (int)resultParam.Value;
        }

        public async Task<int> GetCompletedGroupsCount(long tournId)
        {
            SqlParameter tournIdParam = new SqlParameter("@tournId", tournId);
            SqlParameter countParam = new SqlParameter { ParameterName = "@count", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            await Database.ExecuteSqlRawAsync("GetCompletedGroupsCount @tournId, @count OUT", tournIdParam, countParam);
            return (int)countParam.Value;
        }

        public Group GetCurrentGroup(long tournId)
        {
            SqlParameter tournIdParam = new SqlParameter("@tournId", tournId);
            var group = Groups.FromSqlRaw("EXEC GetCurrentGroup @tournId", tournIdParam).ToList();
            return group[0];
        }

        public List<User> GetRefereesByGroupId(long groupId)
        {
            SqlParameter groupIdParam = new SqlParameter("@groupId", groupId);
            var referees = Users.FromSqlRaw("EXEC GetRefereesByGroupId @groupId", groupIdParam).ToList();
            return referees;
        }

        public List<RefereeProgress> GetHeats(long refId, long danceId)
        {
            SqlParameter userIdParam = new SqlParameter("@userId", refId);
            SqlParameter danceIdParam = new SqlParameter("@danceId", danceId);
            var refProgress = RefereeProgresses.FromSqlRaw("EXEC GetHeats @userId, @danceId ", userIdParam, danceIdParam).ToList();
            return refProgress;
        }

        public async Task<int> GetPairScore(long pairId)
        {
            SqlParameter pairIdParam = new SqlParameter("@pairId", pairId);
            SqlParameter scoreParam = new SqlParameter { ParameterName = "@score", SqlDbType = System.Data.SqlDbType.Int, Direction = System.Data.ParameterDirection.Output };
            await Database.ExecuteSqlRawAsync("GetPairScore @pairId, @score OUT", pairIdParam, scoreParam);
            return (int)scoreParam.Value;
        }

        public List<Pair> GetPairsByRefProgress(long refProgressId)
        {
            SqlParameter refPrIdParam = new SqlParameter("@refProgressId", refProgressId);
            var pairs = Pairs.FromSqlRaw("EXEC GetPairsByRefProgress @refProgressId", refPrIdParam).ToList();
            return pairs;
        }

        public List<User> GetAllTournUsers(long tournId)
        {
            SqlParameter tournIdParam = new SqlParameter("@tournId", tournId);
            var users = Users.FromSqlRaw("EXEC GetAllTournUsers @tournId", tournIdParam).ToList();
            return users;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DanceTournamentRun;Trusted_Connection=True;");
                // optionsBuilder.UseSqlServer("Server=(local);Database=DanceTournamentRun;Trusted_Connection=True;");
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
                entity.Property(e => e.CompetitionState).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRegistrationOn)
                    .IsRequired()
                    .HasColumnName("isRegistrationOn")
                    .HasDefaultValueSql("((1))");

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


            modelBuilder.Entity<Pair>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Pairs)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.Pairs_dbo.Groups_GroupId");
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
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.Property(e => e.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

                entity.Property(e => e.Login).IsRequired();

                entity.Property(e => e.Password)
                    .IsRequired();

                entity.Property(e => e.SecurityToken).HasMaxLength(50);

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

            modelBuilder.Entity<Score>(entity =>
            {
                entity.Property(e => e.Score1).HasColumnName("Score");

                entity.HasOne(d => d.RefereeProgress)
                    .WithMany(p => p.Score)
                    .HasForeignKey(d => d.ProgressId)
                    .HasConstraintName("FK_dbo.Scores_dbo.RefereeProgress_ProgressId");

                entity.HasOne(d => d.Pair)
                    .WithMany(p => p.Scores)
                    .HasForeignKey(d => d.PairId)
                    .HasConstraintName("FK_dbo.Scores_dbo.Pairs_PairId");
            });

            modelBuilder.Entity<RefereeProgress>(entity =>
            {
                entity.ToTable("RefereeProgress");

                entity.Property(e => e.IsCompleted)
                    .HasColumnName("isCompleted")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefereeProgress)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.RefereeProgress_dbo.Users_UserId");

                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.RefereeProgress)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.RefereeProgress_dbo.Dances_DanceId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
