using ApiLibertadoresHAS.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiLibertadoresHAS.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Usuario> TB_USUARIOS { get; set; }

        public DbSet<PetwalkerPerfil> TB_PETWALKER_PERFIL { get; set; }

        public DbSet<Pet> TB_PETS { get; set; }

        public DbSet<Avaliacao> TB_AVALIACOES { get; set; }

        public DbSet<Passeio> TB_PASSEIOS { get; set; }

        public DbSet<Transacao> TB_TRANSACOES { get; set; }

        public DbSet<LocalizacaoPasseio> TB_LOCALIZACAO_PASSEIO { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region TB_USUARIOS

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("TB_USUARIOS");

                entity.HasKey(e => e.Cpf);

                entity.Property(e => e.Cpf)
                    .HasMaxLength(11)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.Nome)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Cep)
                    .HasMaxLength(8)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(70)
                    .IsRequired();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Senha)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.TipoUsuario)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Usuario_TipoUsuario",
                    "TipoUsuario IN ('Dono','Petwalker','Ambos')"));

                entity.Property(e => e.StatusUser)
                    .HasMaxLength(25);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Usuario_StatusUser",
                    "StatusUser IN ('Disponivel','Ausente','Nao Perturbar','Invisivel')"));

                entity.Property(e => e.Telefone)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(e => e.Genero)
                    .HasMaxLength(50)
                    .HasDefaultValue("Nao informado");

                entity.Property(e => e.Foto)
                    .HasMaxLength(100);

                entity.Property(e => e.UltimoLogin)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.DataCadastro)
                    .HasColumnType("date")
                    .HasDefaultValueSql("GETDATE()");
            });

            #endregion

            #region TB_PETWALKER_PERFIL

            modelBuilder.Entity<PetwalkerPerfil>(entity =>
            {
                entity.ToTable("TB_PETWALKER_PERFIL");

                entity.HasKey(e => e.Cpf);

                entity.Property(e => e.Cpf)
                    .HasMaxLength(11)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.Disponibilidade)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(e => e.AreaAtendimento)
                    .HasMaxLength(100)
                    .IsRequired();

                // Relacionamento 1:1 com Usuario (chave compartilhada)
                entity.HasOne(e => e.Usuario)
                    .WithOne(e => e.PetwalkerPerfil)
                    .HasForeignKey<PetwalkerPerfil>(e => e.Cpf)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region TB_PETS

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("TB_PETS");

                entity.HasKey(e => e.Rga);

                entity.Property(e => e.Rga)
                    .HasMaxLength(7)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.Descricao)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Nome)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Especie)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Foto)
                    .HasMaxLength(100);

                entity.Property(e => e.Raca)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Peso)
                    .IsRequired();

                entity.Property(e => e.Porte)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Pet_Porte",
                    "Porte IN ('Grande','Medio','Pequeno')"));

                entity.Property(e => e.Sexo)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Pet_Sexo",
                    "Sexo IN ('Macho','Femea')"));

                entity.Property(e => e.CpfDono)
                    .HasMaxLength(11)
                    .IsFixedLength()
                    .IsRequired();

                entity.HasOne(e => e.Dono)
                    .WithMany(e => e.Pets)
                    .HasForeignKey(e => e.CpfDono)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region TB_AVALIACOES

            modelBuilder.Entity<Avaliacao>(entity =>
            {
                entity.ToTable("TB_AVALIACOES");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Comentario)
                    .HasMaxLength(250);

                entity.Property(e => e.Nota)
                    .IsRequired();

                entity.Property(e => e.DataPublicacao)
                    .HasColumnType("date")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Rga)
                    .HasMaxLength(7)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.CpfPetwalker)
                    .HasMaxLength(11)
                    .IsFixedLength()
                    .IsRequired();

                entity.HasOne(e => e.Pet)
                    .WithMany(e => e.Avaliacoes)
                    .HasForeignKey(e => e.Rga)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PetwalkerPerfil)
                    .WithMany(e => e.Avaliacoes)
                    .HasForeignKey(e => e.CpfPetwalker)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region TB_PASSEIOS

            modelBuilder.Entity<Passeio>(entity =>
            {
                entity.ToTable("TB_PASSEIOS");

                entity.HasKey(e => e.IdPasseio);

                entity.Property(e => e.StatusPass)
                    .HasMaxLength(25)
                    .IsRequired();

                entity.Property(e => e.DataPass)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Duracao)
                    .IsRequired();

                entity.Property(e => e.Rga)
                    .HasMaxLength(7)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.CpfPetwalker)
                    .HasMaxLength(11)
                    .IsFixedLength()
                    .IsRequired();

                entity.HasOne(e => e.Pet)
                    .WithMany(e => e.Passeios)
                    .HasForeignKey(e => e.Rga)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PetwalkerPerfil)
                    .WithMany(e => e.Passeios)
                    .HasForeignKey(e => e.CpfPetwalker)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Transacoes)
                    .WithOne(e => e.Passeio)
                    .HasForeignKey(e => e.IdPasseio)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relacionamento 1:1 com LocalizacaoPasseio (chave compartilhada)
                entity.HasOne(e => e.LocalizacaoPasseio)
                    .WithOne(e => e.Passeio)
                    .HasForeignKey<LocalizacaoPasseio>(e => e.IdPasseio)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region TB_TRANSACOES

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.ToTable("TB_TRANSACOES");

                entity.HasKey(e => e.IdTransacao);

                entity.Property(e => e.MtdPgmt)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.StatusPgmt)
                    .HasMaxLength(25)
                    .IsRequired();

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.DataPgmt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.IdPasseio)
                    .IsRequired();
            });

            #endregion

            #region TB_LOCALIZACAO_PASSEIO

            modelBuilder.Entity<LocalizacaoPasseio>(entity =>
            {
                entity.ToTable("TB_LOCALIZACAO_PASSEIO");

                entity.HasKey(e => e.IdPasseio);

                entity.Property(e => e.Latitude)
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                entity.Property(e => e.Longitude)
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                entity.Property(e => e.Cep)
                    .HasMaxLength(8)
                    .IsFixedLength()
                    .IsRequired();

                entity.Property(e => e.Numero)
                    .HasMaxLength(10)
                    .IsRequired();
            });

            #endregion
        }
    }
}