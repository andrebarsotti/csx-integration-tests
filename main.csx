#!/usr/bin/env dotnet-script
#load "nuget: ScriptUnit, 0.2.0"
#load "Configs.csx"
#r "nuget: Bogus, 33.1.1"
#r "nuget: Dapper, 2.0.90"
#r "nuget: FluentAssertions, 4.19.4"
#r "nuget: Microsoft.Data.SqlClient, 3.0.0"
#r "nuget: Microsoft.EntityFrameworkCore.SqlServer, 5.0.10"
#r "src/TodoListRepositories/bin/Debug/net5.0/TodoListRepository.dll"
using static ScriptUnit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Bogus;
using FluentAssertions;
using TodoListRepositories.Data;
using TodoListRepositories.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

return await AddTestsFrom<TodoListRepositoryTests>().Execute();

public class TodoListRepositoryTests : IDisposable
{
    private readonly Faker _faker = new();
    private bool _disposedValue;
    private TodoTaskContext _context;

    public TodoListRepositoryTests()
    {
        _context = new();
        _context.Database.EnsureCreated();
    }

    public async Task AddTask_Success()
    {
        // Setup
        TodoTask task = new()
        {
            Locator = _faker.Random.AlphaNumeric(5),
            Title = _faker.Lorem.Sentence(5),
            Done = _faker.Random.Bool()
        };

        // Execute
        using var repo = new TodoListRepository(Configs.Configuration.GetConnectionString("db"));
        await repo.AddTask(task);

        // Validate
        _context.TodoTask.Any(t => t.Locator == task.Locator).Should().BeTrue();
        _context.TodoTask.FirstOrDefault(t => t.Locator == task.Locator)
                        .Should()
                        .ShouldBeEquivalentTo(task, opt => opt.ExcludingMissingMembers());
    }

    public async Task UpdateTask_Success()
    {
        // Setup
        string locator = _faker.Random.AlphaNumeric(5);
        _context.TodoTask.Add(new TodoTaskEntity{
            Locator = locator,
            Title = _faker.Lorem.Sentence(5),
            Done = _faker.Random.Bool()
        });
        _context.SaveChanges();

        TodoTask task = new()
        {
            Locator = locator,
            Title = _faker.Lorem.Sentence(7),
            Done = true
        };

        //Execute
        using var repo = new TodoListRepository(Configs.Configuration.GetConnectionString("db"));
        await repo.UpdateTask(task);

        //Validate
        _context.TodoTask.FirstOrDefault(t => t.Locator == task.Locator)
                        .Should()
                        .ShouldBeEquivalentTo(task, opt => opt.ExcludingMissingMembers());
    }

    public async Task DeleteTask_Success()
    {
        // Setup
        string locator = _faker.Random.AlphaNumeric(5);
        _context.TodoTask.Add(new TodoTaskEntity{
            Locator = locator,
            Title = _faker.Lorem.Sentence(5),
            Done = _faker.Random.Bool()
        });
        _context.SaveChanges();

        //Execute
        using var repo = new TodoListRepository(Configs.Configuration.GetConnectionString("db"));
        await repo.DeleteTask(locator);

        //Validate
        _context.TodoTask.Any(task => task.Locator == locator)
                        .Should()
                        .BeFalse();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _context = null;
            _disposedValue = true;
        }
    }

    ~TodoListRepositoryTests()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}


public class TodoTaskEntity : TodoTask
{
    public virtual Guid Id { get; set; }
}


public class TodoTaskEntityConfiguration : IEntityTypeConfiguration<TodoTaskEntity>
{
    public void Configure(EntityTypeBuilder<TodoTaskEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Locator).IsUnique();
        builder.Property(e => e.Id).IsRequired().HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(e => e.Locator).IsRequired().HasMaxLength(10);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(256);;
    }
}

public class TodoTaskContext: DbContext
{

    public DbSet<TodoTaskEntity> TodoTask { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(Configs.Configuration.GetConnectionString("db"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoTaskEntityConfiguration());
    }
}
