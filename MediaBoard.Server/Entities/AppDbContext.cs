using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<ArtistMetric> ArtistMetrics { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<MusicStyle> MusicStyles { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<SearchRanking> SearchRankings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("album_pkey");

            entity.ToTable("album");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.MainId).HasColumnName("main_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasMany(d => d.Artists).WithMany(p => p.Albums)
                .UsingEntity<Dictionary<string, object>>(
                    "AlbumArtist",
                    r => r.HasOne<Artist>().WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_artist_artist_id_fkey"),
                    l => l.HasOne<Album>().WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_artist_album_id_fkey"),
                    j =>
                    {
                        j.HasKey("AlbumId", "ArtistId").HasName("album_artist_pkey");
                        j.ToTable("album_artist");
                        j.HasIndex(new[] { "AlbumId" }, "ix_album_artist_album_id");
                        j.HasIndex(new[] { "ArtistId" }, "ix_album_artist_artist_id");
                        j.IndexerProperty<int>("AlbumId").HasColumnName("album_id");
                        j.IndexerProperty<int>("ArtistId").HasColumnName("artist_id");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Albums)
                .UsingEntity<Dictionary<string, object>>(
                    "AlbumGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_genre_genre_id_fkey"),
                    l => l.HasOne<Album>().WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_genre_album_id_fkey"),
                    j =>
                    {
                        j.HasKey("AlbumId", "GenreId").HasName("album_genre_pkey");
                        j.ToTable("album_genre");
                        j.IndexerProperty<int>("AlbumId").HasColumnName("album_id");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                    });

            entity.HasMany(d => d.Styles).WithMany(p => p.Albums)
                .UsingEntity<Dictionary<string, object>>(
                    "AlbumStyle",
                    r => r.HasOne<MusicStyle>().WithMany()
                        .HasForeignKey("StyleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_style_style_id_fkey"),
                    l => l.HasOne<Album>().WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("album_style_album_id_fkey"),
                    j =>
                    {
                        j.HasKey("AlbumId", "StyleId").HasName("album_style_pkey");
                        j.ToTable("album_style");
                        j.IndexerProperty<int>("AlbumId").HasColumnName("album_id");
                        j.IndexerProperty<int>("StyleId").HasColumnName("style_id");
                    });
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("app_user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.Email, "app_user_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "app_user_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("artist_pkey");

            entity.ToTable("artist");

            entity.HasIndex(e => e.Name, "artist_name_index");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RealName).HasColumnName("real_name");
        });

        modelBuilder.Entity<ArtistMetric>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("artist_metrics_pkey");

            entity.ToTable("artist_metrics");

            entity.Property(e => e.ArtistId)
                .ValueGeneratedNever()
                .HasColumnName("artist_id");
            entity.Property(e => e.TotalClicks)
                .HasDefaultValue(0)
                .HasColumnName("total_clicks");

            entity.HasOne(d => d.Artist).WithOne(p => p.ArtistMetric)
                .HasForeignKey<ArtistMetric>(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("artist_metrics_artist_id_fkey");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genre_pkey");

            entity.ToTable("genre");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<MusicStyle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("music_style_pkey");

            entity.ToTable("music_style");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(r => new { r.UserId, r.MediaId });

            entity.ToTable("rating");

            entity.Property(e => e.MediaId)
                .HasMaxLength(100)
                .HasColumnName("media_id");
            entity.Property(e => e.Score).HasColumnName("rating");
            entity.Property(e => e.Review).HasColumnName("review");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(u => u.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("rating_user_id_fkey");

            entity.HasOne(d => d.Album).WithMany(a => a.Ratings)
                .HasForeignKey(d => d.MediaId)
                .HasConstraintName("rating_media_id_fkey");
        });

        modelBuilder.Entity<SearchRanking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("search_ranking");

            entity.Property(e => e.AlbumCount).HasColumnName("album_count");
            entity.Property(e => e.ArtistId).HasColumnName("artist_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RankScore).HasColumnName("rank_score");
            entity.Property(e => e.TotalClicks).HasColumnName("total_clicks");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
