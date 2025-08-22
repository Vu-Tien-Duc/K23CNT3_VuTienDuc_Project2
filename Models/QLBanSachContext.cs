using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLBanSachWeb.Models;

public partial class QLBanSachContext : DbContext
{
    public QLBanSachContext()
    {
    }

    public QLBanSachContext(DbContextOptions<QLBanSachContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<CT_DonHang> CT_DonHangs { get; set; }

    public virtual DbSet<CT_GioHang> CT_GioHangs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiSach> LoaiSaches { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.MaAdmin).HasName("PK__Admin__49341E38AB9291CC");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Email, "UQ__Admin__A9D105343D42BB2C").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
        });

        modelBuilder.Entity<CT_DonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDH, e.MaSach }).HasName("PK__CT_DonHa__EC06D123995998A4");

            entity.ToTable("CT_DonHang");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDHNavigation).WithMany(p => p.CT_DonHangs)
                .HasForeignKey(d => d.MaDH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDonHang_DonHang");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.CT_DonHangs)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDonHang_Sach");
        });

        modelBuilder.Entity<CT_GioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaGH, e.MaSach }).HasName("PK__CT_GioHa__EC06F9C761BE354E");

            entity.ToTable("CT_GioHang");

            entity.HasOne(d => d.MaGHNavigation).WithMany(p => p.CT_GioHangs)
                .HasForeignKey(d => d.MaGH)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTGioHang_GioHang");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.CT_GioHangs)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTGioHang_Sach");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDH).HasName("PK__DonHang__27258661A3204EF6");

            entity.ToTable("DonHang");

            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xử lý");

            entity.HasOne(d => d.MaKHNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKH)
                .HasConstraintName("FK_DonHang_KH");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGH).HasName("PK__GioHang__2725AE856B41849E");

            entity.ToTable("GioHang");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaKHNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKH)
                .HasConstraintName("FK_GioHang_KH");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKH).HasName("PK__KhachHan__2725CF1EE0B05D14");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Email, "UQ__KhachHan__A9D105344C900610").IsUnique();

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.SDT).HasMaxLength(15);
        });

        modelBuilder.Entity<LoaiSach>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiSach__730A57595A5FAF81");

            entity.ToTable("LoaiSach");

            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PK__Sach__B235742D7A3A3DD7");

            entity.ToTable("Sach");

            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.NXB).HasMaxLength(100);
            entity.Property(e => e.SoLuong).HasDefaultValue(0);
            entity.Property(e => e.TacGia).HasMaxLength(100);
            entity.Property(e => e.TenSach).HasMaxLength(200);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK_Sach_Loai");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
