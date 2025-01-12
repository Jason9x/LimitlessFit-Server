using Microsoft.EntityFrameworkCore;

namespace LimitlessFit.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options);