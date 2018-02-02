using GenesisVision.DataModel.Models;
using Microsoft.EntityFrameworkCore;

namespace GenesisVision.DataModel
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Participants> Participants { get; set; }
        public DbSet<TradeServers> TradeServers { get; set; }
        public DbSet<TradeAccounts> TradeAccounts { get; set; }
        public DbSet<Trades> Trades { get; set; }
        public DbSet<Charts> Charts { get; set; }
        public DbSet<Tournaments> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Participants>()
                   .HasIndex(x => x.Email)
                   .IsUnique();
            builder.Entity<Participants>()
                   .HasOne(x => x.TradeAccount)
                   .WithOne(x => x.Participant)
                   .HasForeignKey<TradeAccounts>(x => x.ParticipantId);


            builder.Entity<TradeAccounts>()
                   .HasOne(x => x.TradeServer)
                   .WithMany(x => x.TradeAccounts)
                   .HasForeignKey(x => x.TradeServerId);
            builder.Entity<TradeAccounts>()
                   .HasOne(x => x.Tournament)
                   .WithMany(x => x.TradeAccounts)
                   .HasForeignKey(x => x.TournamentId);


            builder.Entity<Trades>()
                   .HasOne(x => x.TradeAccount)
                   .WithMany(x => x.Trades)
                   .HasForeignKey(x => x.TradeAccountId);


            builder.Entity<Charts>()
                   .HasOne(x => x.TradeAccount)
                   .WithMany(x => x.Charts)
                   .HasForeignKey(x => x.TradeAccountId);
        }
    }
}
