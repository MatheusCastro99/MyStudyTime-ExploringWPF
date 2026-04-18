using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using MyStudyTime.MVVM.Model;

namespace MyStudyTime.Database
{
    public class StudyTimeDbContext : DbContext
    {
        public StudyTimeDbContext() : base("name=StudyTimeConnection")
        {
            try
            {
                Database.CreateIfNotExists();
                System.Diagnostics.Debug.WriteLine("✓ StudyTimeDbContext: Database available");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠ StudyTimeDbContext: Database connection warning: {ex.Message}");
                // Continue anyway - the database might not be available but we can still use the context
            }
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<FlashCard> FlashCards { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Subject configuration
            modelBuilder.Entity<Subject>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Subject>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Subject>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Subject>()
                .Property(s => s.CreatedDate)
                .IsRequired();

            // Note configuration
            modelBuilder.Entity<Note>()
                .HasKey(n => n.Id);

            modelBuilder.Entity<Note>()
                .Property(n => n.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Note>()
                .Property(n => n.Text)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .Property(n => n.Tags)
                .HasMaxLength(1000);

            modelBuilder.Entity<Note>()
                .Property(n => n.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<Note>()
                .HasRequired(n => n.Subject)
                .WithMany(s => s.Notes)
                .HasForeignKey(n => n.SubjectId)
                .WillCascadeOnDelete(true);

            // FlashCard configuration
            modelBuilder.Entity<FlashCard>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.Question)
                .IsRequired();

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.Answer)
                .IsRequired();

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.Difficulty)
                .IsRequired();

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.Tags)
                .HasMaxLength(1000);

            modelBuilder.Entity<FlashCard>()
                .Property(f => f.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<FlashCard>()
                .HasRequired(f => f.Subject)
                .WithMany(s => s.FlashCards)
                .HasForeignKey(f => f.SubjectId)
                .WillCascadeOnDelete(true);
        }
    }
}
