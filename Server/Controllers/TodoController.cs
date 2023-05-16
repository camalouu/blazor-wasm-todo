using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using todo.Server.Data;
using todo.Shared;
using Microsoft.AspNetCore.Authorization;

namespace BlazorApp.Controllers;

[Route("todos")]
[ApiController]
[Authorize]
public class TodosController : Controller
{
    private readonly TodoContext _db;

    public TodosController(TodoContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetTodos()
    {
        string user = User.Identity.Name;
        return (await _db.Todos.Where(td => td.Owner == user).ToListAsync());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TodoItem>> DeleteTodo(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();

        return Ok("todo deleted");
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItem todo)
    {
        await _db.Todos.AddAsync(todo);
        await _db.SaveChangesAsync();

        return Ok("todo created");
    }

    [HttpPost]
    [Route("share")]
    public async Task<ActionResult<TodoItem>> Share(TodoItemShare todo)
    {
        TodoItem newTodo = new();
        newTodo.Title = todo.Title;
        newTodo.IsDone = todo.IsDone;
        newTodo.Owner = todo.Receiver;
        newTodo.Id = todo.Id;
        newTodo.FileName = todo.FileName;
        await _db.Todos.AddAsync(newTodo);
        await _db.SaveChangesAsync();
        
        return Ok("shared with the receiver");
    }

    [HttpPost]
    [Route("upload")]
    public async Task<ActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null)
            return BadRequest("File is required");

        var fileName = file.FileName;
        var extension = Path.GetExtension(fileName);
        var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{Guid.NewGuid().ToString()}{extension}";
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data");
        var fullPath = Path.Combine(directoryPath, newFileName);
        Directory.CreateDirectory(directoryPath);
        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(fileStream);
        }

        return Ok(newFileName);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> UpdateTodo(int id, TodoItem updatedTodo)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        todo.Title = updatedTodo.Title;
        todo.IsDone = updatedTodo.IsDone;
        _db.Todos.Update(todo);
        await _db.SaveChangesAsync();

        return Ok("todo updated");
    }
}