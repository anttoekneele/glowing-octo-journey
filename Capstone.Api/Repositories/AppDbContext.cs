using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Communication> Communications => Set<Communication>();
    public DbSet<CommunicationStatusHistory> CommunicationStatusHistories => Set<CommunicationStatusHistory>();
    public DbSet<CommunicationType> CommunicationTypes => Set<CommunicationType>();
    public DbSet<CommunicationTypeStatus> CommunicationTypeStatuses => Set<CommunicationTypeStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Communication>(entity =>
        {
            entity.HasKey(communication => communication.Id);

            entity.Property(communication => communication.Title)
                  .IsRequired();

            entity.Property(communication => communication.TypeCode)
                  .IsRequired();

            entity.Property(communication => communication.CurrentStatus)
                  .IsRequired();

            entity.HasOne(communication => communication.CommunicationType)
                  .WithMany(communicationType => communicationType.Communications)
                  .HasForeignKey(communication => communication.TypeCode);
        });

        modelBuilder.Entity<CommunicationStatusHistory>(entity =>
        {
            entity.HasKey(communicationStatusHistory => communicationStatusHistory.Id);

            entity.Property(communicationStatusHistory => communicationStatusHistory.StatusCode)
                  .IsRequired();

            entity.Property(communicationStatusHistory => communicationStatusHistory.OccurredUtc)
                  .IsRequired();

            entity.HasOne(communicationStatusHistory => communicationStatusHistory.Communication)
                  .WithMany(communication => communication.CommunicationStatusHistories)
                  .HasForeignKey(communicationStatusHistory => communicationStatusHistory.CommunicationId);
        });

        modelBuilder.Entity<CommunicationType>(entity =>
        {
            entity.HasKey(communicationType => communicationType.TypeCode);

            entity.Property(communicationType => communicationType.DisplayName)
                  .IsRequired();
        });

        modelBuilder.Entity<CommunicationTypeStatus>(entity =>
        {
            entity.HasKey(communicationTypeStatus => new { communicationTypeStatus.TypeCode, communicationTypeStatus.StatusCode });

            entity.Property(communicationTypeStatus => communicationTypeStatus.Description)
                  .IsRequired();

            entity.HasOne(communicationTypeStatus => communicationTypeStatus.CommunicationType)
                  .WithMany(communicationType => communicationType.CommunicationTypeStatuses)
                  .HasForeignKey(communicationTypeStatus => communicationTypeStatus.TypeCode);
        });

        // Seed Communication Types
        modelBuilder.Entity<CommunicationType>().HasData(
            new CommunicationType { TypeCode = "EMAIL", DisplayName = "Email" },
            new CommunicationType { TypeCode = "SMS", DisplayName = "SMS" }
        );

        // Seed Communication Type Statuses
        modelBuilder.Entity<CommunicationTypeStatus>().HasData(
            new CommunicationTypeStatus { TypeCode = "EMAIL", StatusCode = "DRAFT", Description = "Draft" },
            new CommunicationTypeStatus { TypeCode = "EMAIL", StatusCode = "SENT", Description = "Sent" },
            new CommunicationTypeStatus { TypeCode = "SMS", StatusCode = "PENDING", Description = "Pending" },
            new CommunicationTypeStatus { TypeCode = "SMS", StatusCode = "DELIVERED", Description = "Delivered" }
        );

        var comm1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var comm2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var seedTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        // Seed Communications
        modelBuilder.Entity<Communication>().HasData(
            new Communication
            {
                Id = comm1Id,
                Title = "Welcome Email",
                TypeCode = "EMAIL",
                CurrentStatus = "DRAFT",
                LastUpdatedUtc = seedTime
            },
            new Communication
            {
                Id = comm2Id,
                Title = "Verification SMS",
                TypeCode = "SMS",
                CurrentStatus = "PENDING",
                LastUpdatedUtc = seedTime
            }
        );

        // Seed Communication Status History
        modelBuilder.Entity<CommunicationStatusHistory>().HasData(
            new CommunicationStatusHistory
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                CommunicationId = comm1Id,
                StatusCode = "DRAFT",
                OccurredUtc = seedTime
            },
            new CommunicationStatusHistory
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                CommunicationId = comm2Id,
                StatusCode = "PENDING",
                OccurredUtc = seedTime
            }
        );
    }
}