namespace todo.Shared;
public class TodoItem
{
    public string? Title { get; set; }
    public bool IsDone { get; set; } = false;
    public int? Id { get; set; }
    public string? Owner { get; set; }
    public string? FileName { get; set; }
}

public class TodoItemShare : TodoItem
{
    public string Receiver { get; set; }
}