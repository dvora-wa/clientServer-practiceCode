using Microsoft.EntityFrameworkCore;
using TodoApi;
using ToDoDbContext = TodoApi.ToDoDbContext;
using Task = TodoApi.Item;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.MapGet("/items", async (ToDoDbContext db) =>{
var a= await db.Items.ToListAsync();
return Results.Ok(a);
});

app.MapPost("/items", async ( ToDoDbContext db,Item newTask) => {
    await db.Items.AddAsync(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/item/{newTask.Id}", newTask);
});

app.MapPut("/items/{id}", async (ToDoDbContext db ,int id,bool inputTask) =>
{
    var task = await db.Items.FindAsync(id);

    if (task is null) return Results.NotFound();
    task.IsComplete = !task.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var task = await db.Items.FindAsync(id);

    if (task is null) return Results.NotFound();

    db.Items.Remove(task);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();

