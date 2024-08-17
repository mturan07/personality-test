using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("PersonalityTestDb"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/questions", async (AppDbContext context) =>
{
    var questions = await context.Questions.Include(q => q.Answers).ToListAsync();
    return Results.Ok(questions);
});

app.MapGet("/questions/{id:int}", async (int id, AppDbContext context) =>
{
    var question = await context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
    return question != null ? Results.Ok(question) : Results.NotFound();
});

app.MapPost("/questions", async (Question question, AppDbContext context) =>
{
    context.Questions.Add(question);
    await context.SaveChangesAsync();
    return Results.Created($"/questions/{question.Id}", question);
});

app.MapPut("/questions/{id:int}", async (int id, Question updatedQuestion, AppDbContext context) =>
{
    var question = await context.Questions.FindAsync(id);
    if (question == null)
    {
        return Results.NotFound();
    }

    question.Text = updatedQuestion.Text;
    question.Answers = updatedQuestion.Answers;

    await context.SaveChangesAsync();
    return Results.Ok(question);
});

app.MapDelete("/questions/{id:int}", async (int id, AppDbContext context) =>
{
    var question = await context.Questions.FindAsync(id);
    if (question == null)
    {
        return Results.NotFound();
    }

    context.Questions.Remove(question);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
