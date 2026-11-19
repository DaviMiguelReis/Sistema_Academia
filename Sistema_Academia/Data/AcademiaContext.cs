
using Microsoft.EntityFrameworkCore;
using Sistema_Academia.Models;

namespace Sistema_Academia.Data
{
    public class AcademiaContext : DbContext
    {
        public AcademiaContext(DbContextOptions<AcademiaContext> options)
            : base(options)
        {
        }

        // Tabelas do banco de dados
        public DbSet<Aluno> Alunos => Set<Aluno>();
        public DbSet<Plano> Planos => Set<Plano>();
        public DbSet<Modalidade> Modalidades => Set<Modalidade>();
        public DbSet<Matricula> Matriculas => Set<Matricula>();
        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
        public DbSet<MatriculaModalidade> MatriculaModalidades
            => Set<MatriculaModalidade>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // ALUNO
            // ==========================================

            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Nome)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(a => a.CPF)
                    .HasMaxLength(14)
                    .IsRequired();

                entity.Property(a => a.Email)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(a => a.Telefone)
                    .HasMaxLength(15)
                    .IsRequired();

                entity.Property(a => a.Endereco)
                    .HasMaxLength(200);

                // Impede CPF duplicado
                entity.HasIndex(a => a.CPF)
                    .IsUnique();
            });

            // ==========================================
            // PLANO
            // ==========================================

            modelBuilder.Entity<Plano>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Nome)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.Descricao)
                    .HasMaxLength(300);

                entity.Property(p => p.Valor)
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            // ==========================================
            // MODALIDADE
            // ==========================================

            modelBuilder.Entity<Modalidade>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Nome)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(m => m.Descricao)
                    .HasMaxLength(300);

                entity.Property(m => m.Professor)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(m => m.CapacidadeMaxima)
                    .IsRequired();
            });

            // ==========================================
            // MATRICULA
            // ==========================================

            modelBuilder.Entity<Matricula>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.ValorContratado)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(m => m.Status)
                    .HasMaxLength(30)
                    .IsRequired();

                // Aluno 1:N Matrículas
                entity.HasOne(m => m.Aluno)
                    .WithMany(a => a.Matriculas)
                    .HasForeignKey(m => m.AlunoId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Plano 1:N Matrículas
                entity.HasOne(m => m.Plano)
                    .WithMany(p => p.Matriculas)
                    .HasForeignKey(m => m.PlanoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // PAGAMENTO
            // ==========================================

            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Valor)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(p => p.FormaPagamento)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(p => p.Status)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(p => p.Observacao)
                    .HasMaxLength(500);

                // Matrícula 1:N Pagamentos
                entity.HasOne(p => p.Matricula)
                    .WithMany(m => m.Pagamentos)
                    .HasForeignKey(p => p.MatriculaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // MATRICULA MODALIDADE
            // ==========================================

            modelBuilder.Entity<MatriculaModalidade>(entity =>
            {
                // Chave primária composta
                entity.HasKey(mm => new
                {
                    mm.MatriculaId,
                    mm.ModalidadeId
                });

                // Matrícula 1:N MatrículaModalidade
                entity.HasOne(mm => mm.Matricula)
                    .WithMany(m => m.MatriculaModalidades)
                    .HasForeignKey(mm => mm.MatriculaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Modalidade 1:N MatrículaModalidade
                entity.HasOne(mm => mm.Modalidade)
                    .WithMany(m => m.MatriculaModalidades)
                    .HasForeignKey(mm => mm.ModalidadeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
