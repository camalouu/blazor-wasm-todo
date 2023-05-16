using todo.Shared;
namespace todo.Server.Data;

public static class SeedData
{
    public static void Initialize(TodoContext db)
    {
        var users = new User[]{
             new User{
                Email = "zafar@gmail.com",
                Password = "123"
             },
             new User{
                Email = "kamolov@gmail.com",
                Password = "123"
             },
             new User{
                Email = "zfr@gmail.com",
                Password = "123"
             },
           };

        var todos = new TodoItem[] {
                new TodoItem{
                    Title = "Diploma work",
                    IsDone = false,
                    FileName = null,
                    Id = 1,
                    Owner = "zfr@gmail.com",
                },
                new TodoItem{
                    Title = "Video conferencing application",
                    IsDone = false,
                    FileName = null,
                    Id = 2,
                    Owner = "zfr@gmail.com"
                },
                new TodoItem{
                    Title = "Web scraper for collecting daily data",
                    IsDone = true,
                    FileName = null,
                    Id = 3,
                    Owner = "zafar@gmail.com"
                },
        };
        db.Todos.AddRange(todos);
        db.Users.AddRange(users);
        db.SaveChanges();
    }
}