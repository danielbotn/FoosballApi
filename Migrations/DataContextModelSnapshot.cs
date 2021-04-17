﻿// <auto-generated />
using System;
using FoosballApi.Data;
using FoosballApi.Models.Leagues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FoosballApi.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            modelBuilder.Entity("FoosballApi.Models.Goals.FreehandGoalModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<int>("MatchId")
                        .HasColumnType("integer")
                        .HasColumnName("match_id");

                    b.Property<int>("OponentId")
                        .HasColumnType("integer")
                        .HasColumnName("oponent_id");

                    b.Property<int>("OponentScore")
                        .HasColumnType("integer")
                        .HasColumnName("oponent_score");

                    b.Property<int>("ScoredByScore")
                        .HasColumnType("integer")
                        .HasColumnName("scored_by_score");

                    b.Property<int>("ScoredByUserId")
                        .HasColumnType("integer")
                        .HasColumnName("scored_by_user_id");

                    b.Property<DateTime>("TimeOfGoal")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("time_of_goal");

                    b.Property<bool>("WinnerGoal")
                        .HasColumnType("boolean")
                        .HasColumnName("winner_goal");

                    b.HasKey("Id")
                        .HasName("pk_freehand_goals");

                    b.HasIndex("MatchId")
                        .HasDatabaseName("ix_freehand_goals_match_id");

                    b.HasIndex("OponentId")
                        .HasDatabaseName("ix_freehand_goals_oponent_id");

                    b.HasIndex("ScoredByUserId")
                        .HasDatabaseName("ix_freehand_goals_scored_by_user_id");

                    b.ToTable("freehand_goals");
                });

            modelBuilder.Entity("FoosballApi.Models.Leagues.LeagueModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("OrganisationId")
                        .HasColumnType("integer")
                        .HasColumnName("organisation_id");

                    b.Property<LeagueType>("TypeOfLeague")
                        .HasColumnType("league_type")
                        .HasColumnName("type_of_league");

                    b.Property<int>("UpTo")
                        .HasColumnType("integer")
                        .HasColumnName("up_to");

                    b.HasKey("Id")
                        .HasName("pk_leagues");

                    b.HasIndex("OrganisationId")
                        .HasDatabaseName("ix_leagues_organisation_id");

                    b.ToTable("leagues");
                });

            modelBuilder.Entity("FoosballApi.Models.Leagues.LeaguePlayersModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("LeagueId")
                        .HasColumnType("integer")
                        .HasColumnName("league_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_league_players");

                    b.HasIndex("LeagueId")
                        .HasDatabaseName("ix_league_players_league_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_league_players_user_id");

                    b.ToTable("league_players");
                });

            modelBuilder.Entity("FoosballApi.Models.Matches.FreehandMatchModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_time");

                    b.Property<bool>("GameFinished")
                        .HasColumnType("boolean")
                        .HasColumnName("game_finished");

                    b.Property<bool>("GamePaused")
                        .HasColumnType("boolean")
                        .HasColumnName("game_paused");

                    b.Property<int>("PlayerOneId")
                        .HasColumnType("integer")
                        .HasColumnName("player_one_id");

                    b.Property<int>("PlayerOneScore")
                        .HasColumnType("integer")
                        .HasColumnName("player_one_score");

                    b.Property<int>("PlayerTwoId")
                        .HasColumnType("integer")
                        .HasColumnName("player_two_id");

                    b.Property<int>("PlayerTwoScore")
                        .HasColumnType("integer")
                        .HasColumnName("player_two_score");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_time");

                    b.Property<int>("UpTo")
                        .HasColumnType("integer")
                        .HasColumnName("up_to");

                    b.HasKey("Id")
                        .HasName("pk_freehand_matches");

                    b.HasIndex("PlayerOneId")
                        .HasDatabaseName("ix_freehand_matches_player_one_id");

                    b.HasIndex("PlayerTwoId")
                        .HasDatabaseName("ix_freehand_matches_player_two_id");

                    b.ToTable("freehand_matches");
                });

            modelBuilder.Entity("FoosballApi.Models.OrganisationListModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<int>("OrganisationId")
                        .HasColumnType("integer")
                        .HasColumnName("organisation_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_organisation_list");

                    b.HasIndex("OrganisationId")
                        .HasDatabaseName("ix_organisation_list_organisation_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_organisation_list_user_id");

                    b.ToTable("organisation_list");
                });

            modelBuilder.Entity("FoosballApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users");
                });

            modelBuilder.Entity("FoosballApi.OrganisationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_organisations");

                    b.ToTable("organisations");
                });

            modelBuilder.Entity("FoosballApi.VerificationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<bool>("HasVerified")
                        .HasColumnType("boolean")
                        .HasColumnName("has_verified");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("text")
                        .HasColumnName("password_reset_token");

                    b.Property<DateTime?>("PasswordResetTokenExpires")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("password_reset_token_expires");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("text")
                        .HasColumnName("verification_token");

                    b.HasKey("Id")
                        .HasName("pk_verifications");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_verifications_user_id");

                    b.ToTable("verifications");
                });

            modelBuilder.Entity("FoosballApi.Models.Goals.FreehandGoalModel", b =>
                {
                    b.HasOne("FoosballApi.Models.Matches.FreehandMatchModel", "freehandMatchModel")
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .HasConstraintName("fk_freehand_goals_freehand_matches_match_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoosballApi.Models.User", "oponentId")
                        .WithMany()
                        .HasForeignKey("OponentId")
                        .HasConstraintName("fk_freehand_goals_users_oponent_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoosballApi.Models.User", "scoredByUserId")
                        .WithMany()
                        .HasForeignKey("ScoredByUserId")
                        .HasConstraintName("fk_freehand_goals_users_scored_by_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("freehandMatchModel");

                    b.Navigation("oponentId");

                    b.Navigation("scoredByUserId");
                });

            modelBuilder.Entity("FoosballApi.Models.Leagues.LeagueModel", b =>
                {
                    b.HasOne("FoosballApi.OrganisationModel", "OrganisationModel")
                        .WithMany()
                        .HasForeignKey("OrganisationId")
                        .HasConstraintName("fk_leagues_organisations_organisation_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrganisationModel");
                });

            modelBuilder.Entity("FoosballApi.Models.Leagues.LeaguePlayersModel", b =>
                {
                    b.HasOne("FoosballApi.Models.Leagues.LeagueModel", "League")
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .HasConstraintName("fk_league_players_leagues_league_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoosballApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_league_players_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoosballApi.Models.Matches.FreehandMatchModel", b =>
                {
                    b.HasOne("FoosballApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("PlayerOneId")
                        .HasConstraintName("fk_freehand_matches_users_player_one_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoosballApi.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("PlayerTwoId")
                        .HasConstraintName("fk_freehand_matches_users_player_two_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoosballApi.Models.OrganisationListModel", b =>
                {
                    b.HasOne("FoosballApi.OrganisationModel", "OrganisationModel")
                        .WithMany()
                        .HasForeignKey("OrganisationId")
                        .HasConstraintName("fk_organisation_list_organisations_organisation_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FoosballApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_organisation_list_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrganisationModel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FoosballApi.VerificationModel", b =>
                {
                    b.HasOne("FoosballApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_verifications_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
