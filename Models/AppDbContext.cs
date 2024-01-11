
using App.Models.Blog;
using App.Models.Contacts;
using App.Models.Product;
using App.Models.Skill;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace App.Models 
{
    // App.Models.AppDbContext
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<Category>( entity => {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            modelBuilder.Entity<PostCategory>( entity => {
                entity.HasKey( c => new {c.PostID, c.CategoryID});
            });

            modelBuilder.Entity<Post>( entity => {
                entity.HasIndex( p => p.Slug)
                      .IsUnique();
            });



            modelBuilder.Entity<CategoryProduct>( entity => {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            modelBuilder.Entity<ProductCategoryProduct>( entity => {
                entity.HasKey( c => new {c.ProductID, c.CategoryID});
            });

            modelBuilder.Entity<ProductModel>( entity => {
                entity.HasIndex( p => p.Slug)
                      .IsUnique();
            });



            modelBuilder.Entity<CategorySkill>(entity => {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            modelBuilder.Entity<SkillCategorySkill>(entity => {
                entity.HasKey(c => new { c.SkillID, c.CategoryID });
            });

            modelBuilder.Entity<SkillModel>(entity => {
                entity.HasIndex(p => p.Slug)
                      .IsUnique();
            });



        }

        public DbSet<Contact> Contacts { get; set; }


        public DbSet<Category> Categories { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostCategory> PostCategories { get; set; }



        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductModel> Products { get; set;}

        public DbSet<ProductCategoryProduct>  ProductCategoryProducts { get; set; }

        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        public DbSet<CategorySkill> CategorySkills { get; set; }

        public DbSet<SkillModel> Skills { get; set; }

        public DbSet<SkillCategorySkill> SkillCategorySkills{ get; set; }



    }
}
