using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tajawul.Models.Domain.Users;

namespace Tajawul.Data;

public partial class Db14639Context : IdentityDbContext<Person>
{
    public Db14639Context()
    {
    }

    public Db14639Context(DbContextOptions<Db14639Context> options)
        : base(options)
    {
    }

    //public virtual DbSet<BusinessManager> BusinessManagers { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<PlaceOwner> PlaceOwners { get; set; }

    //public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = "9c1358a4-5151-4888-ba77-5a71174fabd4",
                Name = "Person",
                NormalizedName = "PERSON"
            },
            new IdentityRole
            {
                Id = "8c1683a7-f52e-4f82-9b88-513ab45fc1c8",
                Name = "CompletedSocialInfo",
                NormalizedName = "COMPLETEDSOCIALINFO"
            },
            new IdentityRole
            {
                Id = "1e84b668-1a0f-4040-a602-3949f371793e",
                Name = "CompletedInterestInfo",
                NormalizedName = "COMPLETEDINTERESTINFO"
            },
            new IdentityRole
            {
                Id = "704a2a66-bc99-4bbe-9ebc-5b74f5df1362",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "251467ac-f694-48c3-922a-46b27ffbbe03",
                Name = "PlaceOwner",
                NormalizedName = "PLACEOWNER"
            },
            new IdentityRole
            {
                Id = "897cb339-f935-4616-ac91-204fbdb49a03",
                Name = "BusinessManager",
                NormalizedName = "BUSINESSMANAGER"
            }
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);


        //modelBuilder.Entity<BusinessManager>(entity =>
        //{
        //    entity.HasKey(e => e.ManagerId).HasName("PK__business__5A6073FC19907B65");

        //    entity.ToTable("business_managers");

        //    entity.Property(e => e.ManagerId).HasColumnName("manager_id");
        //    entity.Property(e => e.PersonId).HasColumnName("Id");

        //    entity.HasOne(d => d.Person).WithMany(p => p.BusinessManagers)
        //        .HasForeignKey(d => d.PersonId)
        //        .HasConstraintName("FK__business___perso__5BE2A6F2");
        //});

        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(e => e.LanguageId).HasName("PK__language__804CF6B33AE2B7B9");

            entity.ToTable("languages");

            entity.Property(e => e.LanguageId).HasColumnName("language_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LanguageCode)
                .HasMaxLength(50)
                .HasColumnName("language_code");
            entity.Property(e => e.LanguageName)
                .HasMaxLength(255)
                .HasColumnName("language_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__persons__543848DFB153A447");

            entity.ToTable("persons");

            entity.HasIndex(e => e.UserName, "UQ__persons__7C9273C45FD361E4").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__persons__AB6E616487425769").IsUnique();

            entity.Property(e => e.Id).HasColumnName("Id");
            //entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            //entity.Property(e => e.City)
            //    .HasMaxLength(100)
            //    .HasColumnName("city");
            //entity.Property(e => e.Country)
            //    .HasMaxLength(100)
            //    .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            //entity.Property(e => e.FirstName)
            //    .HasMaxLength(100)
            //    .HasColumnName("first_name");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            //entity.Property(e => e.LastName)
            //    .HasMaxLength(100)
            //    .HasColumnName("last_name");
            //entity.Property(e => e.Location).HasColumnName("location");
            //entity.Property(e => e.Password)
            //    .HasMaxLength(255)
            //    .HasColumnName("password");
            //entity.Property(e => e.PhoneNumber)
            //    .HasMaxLength(30)
            //    .HasColumnName("phone_number");
            //entity.Property(e => e.PostalCode)
            //    .HasMaxLength(20)
            //    .HasColumnName("postal_code");
            //entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_URL");
            //entity.Property(e => e.Street)
            //    .HasMaxLength(255)
            //    .HasColumnName("street");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("UserName");
        });

        modelBuilder.Entity<PlaceOwner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__place_ow__3C4FBEE4397EA56B");

            entity.ToTable("place_owners");

            entity.HasIndex(e => e.NationalNumber, "UQ__place_ow__4449273AD0EF3137").IsUnique();

            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.DocumentImageUrl).HasColumnName("document_image_URL");
            entity.Property(e => e.NationalNumber)
                .HasMaxLength(50)
                .HasColumnName("national_number");
            entity.Property(e => e.PersonId).HasColumnName("Id");

            entity.HasOne(d => d.Person).WithMany(p => p.PlaceOwners)
                .HasForeignKey(d => d.PersonId)
                .HasConstraintName("FK__place_own__perso__59063A47");
        });

        //modelBuilder.Entity<User>(entity =>
        //{
        //    entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FA5584F82");

        //    entity.ToTable("users");

        //    entity.Property(e => e.UserId).HasColumnName("user_id");
        //    //entity.Property(e => e.Bio).HasColumnName("bio");
        //    //entity.Property(e => e.CompletedSurvey).HasColumnName("completed_survey");
        //    //entity.Property(e => e.Gender).HasColumnName("gender");
        //    //entity.Property(e => e.IsTopTraveler).HasColumnName("is_top_traveler");
        //    entity.Property(e => e.PersonId).HasColumnName("Id");
        //    //entity.Property(e => e.SocialMediaLinks).HasColumnName("social_media_links");

        //    entity.HasOne(d => d.Person).WithMany(p => p.Users)
        //        .HasForeignKey(d => d.PersonId)
        //        .HasConstraintName("FK__users__person_id__5165187F");
        //});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
