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

return await AddTestsFrom<TodoListRepositoryTests>().Execute();

public class TodoListRepositoryTests
{
    private readonly Faker _faker = new();

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

        // Check
        using TodoTaskContext context = new();
        context.TodoTask.Any(t => t.Locator == task.Locator).Should().BeTrue();
        context.TodoTask.FirstOrDefault(t => t.Locator == task.Locator)
                        .Should()
                        .ShouldBeEquivalentTo(task, opt => opt.ExcludingMissingMembers());
    }

    public async Task UpdateTask_Success()
    {
        // Setup
        using TodoTaskContext context = new();
        string locator = _faker.Random.AlphaNumeric(5);
        context.TodoTask.Add(new TodoTaskEntity{
            Locator = locator,
            Title = _faker.Lorem.Sentence(5),
            Done = _faker.Random.Bool()
        });
        context.SaveChanges();

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
        context.TodoTask.FirstOrDefault(t => t.Locator == task.Locator)
                        .Should()
                        .ShouldBeEquivalentTo(task, opt => opt.ExcludingMissingMembers());
    }

    public async Task DeleteTask_Success()
    {
        // Setup
        using TodoTaskContext context = new();
        string locator = _faker.Random.AlphaNumeric(5);
        context.TodoTask.Add(new TodoTaskEntity{
            Locator = locator,
            Title = _faker.Lorem.Sentence(5),
            Done = _faker.Random.Bool()
        });
        context.SaveChanges();

        //Execute
        using var repo = new TodoListRepository(Configs.Configuration.GetConnectionString("db"));
        await repo.DeleteTask(locator);

        //Validate
        context.TodoTask.Any(task => task.Locator == locator)
                        .Should()
                        .BeFalse();
    }
}


[Table("TodoTask")]
public class TodoTaskEntity : TodoTask
{
    [Key]
    public Guid Id { get; set; }
}


public class TodoTaskContext: DbContext
{

    public DbSet<TodoTaskEntity> TodoTask { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(Configs.Configuration.GetConnectionString("db"));
}
