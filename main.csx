#!/usr/bin/env dotnet-script
#load "nuget: ScriptUnit, 0.2.0"
#load "Configs.csx"
#r "nuget: Bogus, 33.1.1"
#r "nuget: Dapper, 2.0.90"
#r "nuget: FluentAssertions, 4.19.4"
#r "nuget: Microsoft.Data.SqlClient, 3.0.0"
#r "src/TodoListRepositories/bin/Debug/net5.0/TodoListRepository.dll"
using static ScriptUnit;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using TodoListRepositories.Data;
using TodoListRepositories.Model;

return await AddTestsFrom<TodoListRepositoryTests>().Execute();

public class TodoListRepositoryTests
{
    public async Task AddTask_Success()
    {
        using var repo = new TodoListRepository(Configs.Configuration.GetConnectionString("db"));
        await repo.AddTask(new()
        {
            Locator  = "teste",
            Title = "teste",
            Done = false
        });
    }
}
