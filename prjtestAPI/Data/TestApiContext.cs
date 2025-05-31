using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using prjtestAPI.Models;

namespace prjtestAPI.Data;

public partial class TestApiContext : DbContext
{
    public TestApiContext()
    {
    }

    public TestApiContext(DbContextOptions<TestApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TCompany> TCompanies { get; set; }

    public virtual DbSet<TRefreshToken> TRefreshTokens { get; set; }

    public virtual DbSet<TUser> TUsers { get; set; }

    public virtual DbSet<TUserActionToken> TUserActionTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=testAPI;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__tCompani__2D971CAC2C3D8E2B");

            entity.ToTable("tCompanies");

            entity.Property(e => e.CompanyId).ValueGeneratedNever();
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContractEndAt).HasColumnType("datetime");
            entity.Property(e => e.ContractStartAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TRefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__tRefresh__658FEEEAA3A6AA4E");

            entity.ToTable("tRefreshTokens");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Token).HasMaxLength(512);
            entity.Property(e => e.UserAgent).HasMaxLength(512);

            entity.HasOne(d => d.User).WithMany(p => p.TRefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tUsers__1788CCAC86D93F89");

            entity.ToTable("tUsers");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FailedLoginCount).HasDefaultValue(0);
            entity.Property(e => e.LockoutEndTime).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UserStatus).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Company).WithMany(p => p.TUsers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Companies");
        });

        modelBuilder.Entity<TUserActionToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__tUserAct__658FEEEAD82DA827");

            entity.ToTable("tUserActionTokens");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(100);
            entity.Property(e => e.TokenType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.TUserActionTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActionTokens_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
