﻿using bd.swseguridad.entidades.Negocio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace bd.swseguridad.datos
{
    public class SwSeguridadDbContext : DbContext
    {

        public SwSeguridadDbContext(DbContextOptions<SwSeguridadDbContext> options)
            : base(options){ }

        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscbdd> Adscbdd { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscexe> Adscexe { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscgrp> Adscgrp { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscmenu> Adscmenu { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscmiem> Adscmiem { get; set; }
        public virtual DbSet<bd.swseguridad.entidades.Negocio.Adscsist> Adscsist { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Adscbdd>(entity =>
            {
                entity.HasKey(e => e.AdbdBdd)
                    .HasName("PK_ADSCBDD");

                entity.ToTable("ADSCBDD");

                entity.Property(e => e.AdbdBdd)
                    .HasColumnName("ADBD_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdbdDescripcion)
                    .HasColumnName("ADBD_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdbdServidor)
                    .HasColumnName("ADBD_SERVIDOR")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Adscexe>(entity =>
            {
                entity.HasKey(e => new { e.AdexBdd, e.AdexGrupo, e.AdexSistema, e.AdexAplicacion })
                    .HasName("PK_ADSCEXE_1");

                entity.ToTable("ADSCEXE");

                entity.Property(e => e.AdexBdd)
                    .HasColumnName("ADEX_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdexGrupo)
                    .HasColumnName("ADEX_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdexSistema)
                    .HasColumnName("ADEX_SISTEMA")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.AdexAplicacion)
                    .HasColumnName("ADEX_APLICACION")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdexSql)
                    .HasColumnName("ADEX_SQL")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Del).HasColumnName("DEL");

                entity.Property(e => e.Ins).HasColumnName("INS");

                entity.Property(e => e.Sel).HasColumnName("SEL");

                entity.Property(e => e.Upd).HasColumnName("UPD");

                entity.HasOne(d => d.Adex)
                    .WithMany(p => p.Adscexe)
                    .HasForeignKey(d => new { d.AdexBdd, d.AdexGrupo })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCEXE_ADSCGRP");

                entity.HasOne(d => d.AdexNavigation)
                    .WithMany(p => p.Adscexe)
                    .HasForeignKey(d => new { d.AdexSistema, d.AdexAplicacion })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCEXE_ADSCMENU");
            });

            modelBuilder.Entity<Adscgrp>(entity =>
            {
                entity.HasKey(e => new { e.AdgrBdd, e.AdgrGrupo })
                    .HasName("PK_ADSCGRP");

                entity.ToTable("ADSCGRP");

                entity.Property(e => e.AdgrBdd)
                    .HasColumnName("ADGR_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdgrGrupo)
                    .HasColumnName("ADGR_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdgrDescripcion)
                    .HasColumnName("ADGR_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdgrNombre)
                    .HasColumnName("ADGR_NOMBRE")
                    .HasColumnType("varchar(32)");

                entity.HasOne(d => d.AdgrBddNavigation)
                    .WithMany(p => p.Adscgrp)
                    .HasForeignKey(d => d.AdgrBdd)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCGRP_ADSCBDD");
            });

            modelBuilder.Entity<Adscmenu>(entity =>
            {
                entity.HasKey(e => new { e.AdmeSistema, e.AdmeAplicacion })
                    .HasName("PK_ADSCMENU");

                entity.ToTable("ADSCMENU");

                entity.Property(e => e.AdmeSistema)
                    .HasColumnName("ADME_SISTEMA")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.AdmeAplicacion)
                    .HasColumnName("ADME_APLICACION")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdmeDescripcion)
                    .HasColumnName("ADME_DESCRIPCION")
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.AdmeElemento)
                    .HasColumnName("ADME_ELEMENTO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmeEnsamblado)
                    .HasColumnName("ADME_ENSAMBLADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmeObjetivo)
                    .HasColumnName("ADME_OBJETIVO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmeOrden).HasColumnName("ADME_ORDEN");

                entity.Property(e => e.AdmePadre)
                    .HasColumnName("ADME_PADRE")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.AdmeTipo)
                    .HasColumnName("ADME_TIPO")
                    .HasColumnType("varchar(1)");

                entity.Property(e => e.AdmeTipoObjeto)
                    .HasColumnName("ADME_TIPO_OBJETO")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.AdmeUrl)
                    .HasColumnName("ADME_URL")
                    .HasColumnType("varchar(150)");

                entity.HasOne(d => d.AdmeSistemaNavigation)
                    .WithMany(p => p.Adscmenu)
                    .HasForeignKey(d => d.AdmeSistema)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCMENU_ADSCSIST");
            });

            modelBuilder.Entity<Adscmiem>(entity =>
            {
                entity.HasKey(e => new { e.AdmiEmpleado, e.AdmiGrupo, e.AdmiBdd })
                    .HasName("PK_ADSCMIEM");

                entity.ToTable("ADSCMIEM");

                entity.Property(e => e.AdmiEmpleado)
                    .HasColumnName("ADMI_EMPLEADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmiGrupo)
                    .HasColumnName("ADMI_GRUPO")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmiBdd)
                    .HasColumnName("ADMI_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdmiCodigoEmpleado)
                    .HasColumnName("ADMI_CODIGO_EMPLEADO")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.AdmiTotal)
                    .HasColumnName("ADMI_TOTAL")
                    .HasColumnType("nchar(3)");

                entity.HasOne(d => d.Admi)
                    .WithMany(p => p.Adscmiem)
                    .HasForeignKey(d => new { d.AdmiBdd, d.AdmiGrupo })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ADSCMIEM_ADSCGRP");
            });

            modelBuilder.Entity<Adscsist>(entity =>
            {
                entity.HasKey(e => e.AdstSistema)
                    .HasName("PK_ADSCSIST");

                entity.ToTable("ADSCSIST");

                entity.Property(e => e.AdstSistema)
                    .HasColumnName("ADST_SISTEMA")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.AdstBdd)
                    .HasColumnName("ADST_BDD")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AdstDescripcion)
                    .HasColumnName("ADST_DESCRIPCION")
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.AdstHost)
                    .HasColumnName("ADST_HOST")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.AdstTipo)
                    .HasColumnName("ADST_TIPO")
                    .HasColumnType("varchar(3)");

                entity.HasOne(d => d.AdstBddNavigation)
                    .WithMany(p => p.Adscsist)
                    .HasForeignKey(d => d.AdstBdd)
                    .HasConstraintName("FK_ADSCSIST_ADSCBDD");
            });
        }



    }


    

}






