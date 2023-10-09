using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace ToDo.Models;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activity { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=host.docker.internal;port=3306;user=root;password=passw0rd;database=ToDo", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.1.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.Name)
                .HasMaxLength(444)
                .UseCollation("utf8mb4_thai_520_w2");
            entity.Property(e => e.UserId).HasColumnType("int(10) unsigned");
            entity.Property(e => e.When)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Activity)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Activity_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.Password)
                .HasMaxLength(44)
                .UseCollation("utf8mb4_thai_520_w2");
            entity.Property(e => e.Salt)
                .HasMaxLength(24)
                .UseCollation("utf8mb4_thai_520_w2");
            entity.Property(e => e.Username)
                .HasMaxLength(44)
                .UseCollation("utf8mb4_thai_520_w2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
