using Microsoft.EntityFrameworkCore;
using FoosballApi.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using System.Text.RegularExpressions;
using System;
using Npgsql.NameTranslation;
using FoosballApi.Models.Leagues;
using FoosballApi.Models.Matches;
using FoosballApi.Models.Goals;
using FoosballApi.Models.Other;
using FoosballApi.Models.SingleLeagueGoals;
using FoosballApi.Models.DoubleLeagueTeams;
using FoosballApi.Models.DoubleLeaguePlayers;
using FoosballApi.Models.DoubleLeagueGoals;
using FoosballApi.Models.DoubleLeagueMatches;

namespace FoosballApi.Data
{
    public class DataContext : DbContext
    {
        private static readonly Regex _keysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        static DataContext()
        => NpgsqlConnection.GlobalTypeMapper.MapEnum<LeagueType>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseSerialColumns();
            FixSnakeCaseNames(modelBuilder);

            // Comment out this function during migration
            // PopulateSingleLeagueMatchesQuery(modelBuilder);
        }

        // Query for fromRawSql() function
        // We do not want to generate new tables with these models
        // When running EF Core migrations
        private void PopulateSingleLeagueMatchesQuery(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SingleLeagueMatchesQuery>(e =>
            {
                e.HasNoKey().ToView(null);
            });

            modelBuilder.Entity<SingleLeagueStandingsMatchesWonAsPlayerOne>(e =>
            {
                e.HasNoKey().ToView(null);
            });

            modelBuilder.Entity<SingleLeagueStandingsMatchesWonAsPlayerTwo>(e =>
            {
                e.HasNoKey().ToView(null);
            });
            modelBuilder.Entity<SingleLeagueStandingsMatchesLostAsPlayerOne>(e =>
            {
                e.HasNoKey().ToView(null);
            });
            modelBuilder.Entity<SingleLeagueStandingsMatchesLostAsPlayerTwo>(e =>
            {
                e.HasNoKey().ToView(null);
            });
        }

        // When running EF Core migrations comment out this line
        // Used for making querying database wiht fromsqlraw easier
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSnakeCaseNamingConvention();

        private void FixSnakeCaseNames(ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var table in modelBuilder.Model.GetEntityTypes())
            {
                ConvertToSnake(mapper, table);
                foreach (var property in table.GetProperties())
                {
                    ConvertToSnake(mapper, property);
                }

                foreach (var primaryKey in table.GetKeys())
                {
                    ConvertToSnake(mapper, primaryKey);
                }

                foreach (var foreignKey in table.GetForeignKeys())
                {
                    ConvertToSnake(mapper, foreignKey);
                }

                foreach (var indexKey in table.GetIndexes())
                {
                    ConvertToSnake(mapper, indexKey);
                }
            }
        }

        private void ConvertToSnake(INpgsqlNameTranslator mapper, object entity)
        {
            switch (entity)
            {
                case IMutableEntityType table:
                    table.SetTableName(ConvertGeneralToSnake(mapper, table.GetTableName()));
                    if (table.GetTableName().StartsWith("asp_net_"))
                    {
                        table.SetTableName(table.GetTableName().Replace("asp_net_", string.Empty));
                        table.SetSchema("identity");
                    }

                    break;
                case IMutableProperty property:
                    // StoreObjectIdentifier soi = new();
                    // property.SetColumnName(ConvertGeneralToSnake(mapper, soi.Name));
                    // Svona var þetta.
                    property.SetColumnName(ConvertGeneralToSnake(mapper, property.GetColumnName()));
                    break;
                case IMutableKey primaryKey:
                    primaryKey.SetName(ConvertKeyToSnake(mapper, primaryKey.GetName()));
                    break;
                case IMutableForeignKey foreignKey:
                    foreignKey.SetConstraintName(ConvertKeyToSnake(mapper, foreignKey.GetConstraintName()));
                    break;
                case IMutableIndex indexKey:
                    indexKey.SetDatabaseName(ConvertKeyToSnake(mapper, indexKey.GetDatabaseName()));
                    break;
                default:
                    throw new NotImplementedException("Unexpected type was provided to snake case converter");
            }
        }

        private string ConvertKeyToSnake(INpgsqlNameTranslator mapper, string keyName) =>
            ConvertGeneralToSnake(mapper, _keysRegex.Replace(keyName, match => match.Value.ToLower()));

        private string ConvertGeneralToSnake(INpgsqlNameTranslator mapper, string entityName) =>
            mapper.TranslateMemberName(ModifyNameBeforeConvertion(mapper, entityName));

        protected virtual string ModifyNameBeforeConvertion(INpgsqlNameTranslator mapper, string entityName) => entityName;

        public DbSet<User> Users { get; set; }

        public DbSet<OrganisationModel> Organisations { get; set; }

        public DbSet<OrganisationListModel> OrganisationList { get; set; }

        public DbSet<VerificationModel> Verifications { get; set; }

        public DbSet<LeagueModel> Leagues { get; set; }

        public DbSet<LeaguePlayersModel> LeaguePlayers { get; set; }

        public DbSet<FreehandMatchModel> FreehandMatches { get; set; }

        public DbSet<FreehandGoalModel> FreehandGoals { get; set; }

        public DbSet<FreehandDoubleMatchModel> FreehandDoubleMatches { get; set; }

        public DbSet<FreehandDoubleGoalModel> FreehandDoubleGoals { get; set; }

        public DbSet<SingleLeagueMatchModel> SingleLeagueMatches { get; set; }

        public DbSet<SingleLeagueGoalModel> SingleLeagueGoals { get; set; }

        public DbSet<DoubleLeagueTeamModel> DoubleLeagueTeams { get; set; }

        public DbSet<DoubleLeaguePlayerModel> DoubleLeaguePlayers { get; set; }

        public DbSet<DoubleLeagueGoalModel> DoubleLeagueGoals { get; set; }

        public DbSet<DoubleLeagueMatchModel> DoubleLeagueMatches { get; set; }
    }
}