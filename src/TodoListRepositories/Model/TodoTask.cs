namespace TodoListRepositories.Model
{
    public class TodoTask
    {
        public virtual string Locator { get; set; }

        public virtual string Title { get; set; }

        public virtual bool Done { get; set; }
    }
}
