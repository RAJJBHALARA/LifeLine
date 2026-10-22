namespace LifeLine.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<BloodRequest> Requests => Set<BloodRequest>();
    public DbSet<DonorResponse> Responses => Set<DonorResponse>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();

        b.Entity<BloodRequest>()
            .HasOne(r => r.Requester).WithMany(u => u.Requests)
            .HasForeignKey(r => r.RequesterId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<DonorResponse>()
            .HasOne(r => r.Request).WithMany(r => r.Responses)
            .HasForeignKey(r => r.RequestId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<DonorResponse>()
            .HasOne(r => r.Donor).WithMany(u => u.Responses)
            .HasForeignKey(r => r.DonorId).OnDelete(DeleteBehavior.Restrict);

        // A donor can respond to a given request only once.
        b.Entity<DonorResponse>().HasIndex(r => new { r.RequestId, r.DonorId }).IsUnique();
    }
}
