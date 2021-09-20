using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListRepositories.Model;

namespace TodoListRepositories.Data
{
    public interface ITodoListRepository
    {
        Task<IEnumerable<TodoTask>> GetTasks();

        Task<TodoTask> GetTask(string locator);

        Task AddTask(TodoTask task);

        Task UpdateTask(TodoTask task);

        Task DeleteTask(string locator);
    }
}
