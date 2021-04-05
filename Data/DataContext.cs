using Microsoft.EntityFrameworkCore;
using FoosballApi.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using System.Text.RegularExpressions;
using System;
using Npgsql.NameTranslation;
using FoosballApi.Models.Leagues;

namespace FoosballApi.Data
{
    public class DataContext: DbContext
    {
        private static readonly Regex _keysRegex = new Regex("^(PK|FK|IX)_", RegexOptions.Compiled);
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseSerialColumns();
            modelBuilder.HasPostgresEnum<LeagueType>();
            FixSnakeCaseNames(modelBuilder);
        }

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
                    property.SetColumnName(ConvertGeneralToSnake(mapper, property.GetColumnName()));
                    break;
                case IMutableKey primaryKey:
                    primaryKey.SetName(ConvertKeyToSnake(mapper, primaryKey.GetName()));
                    break;
                case IMutableForeignKey foreignKey:
                    foreignKey.SetConstraintName(ConvertKeyToSnake(mapper, foreignKey.GetConstraintName()));
                    break;
                case IMutableIndex indexKey:
                    indexKey.SetName(ConvertKeyToSnake(mapper, indexKey.GetName()));
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
    }
}