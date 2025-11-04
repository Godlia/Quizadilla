using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizDbContextConnection"))
);

var app = builder.Build();

// Apply migrations and seed sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    db.Database.Migrate();

    // ✅ Only seed if no quizzes exist
    if (!db.Quizzes.Any())
    {
        var sampleQuizzes = new List<Quiz>
        {
            new Quiz
            {
                Title = "Nordic Capitals",
                Description = "Test your knowledge of the capitals of Nordic countries.",
                Theme = "cheese",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What is the capital of Norway?",
                        correctString = "Oslo",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oslo" },
                            new Option { OptionText = "Stockholm" },
                            new Option { OptionText = "Copenhagen" },
                            new Option { OptionText = "Helsinki" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the capital of Sweden?",
                        correctString = "Stockholm",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oslo" },
                            new Option { OptionText = "Stockholm" },
                            new Option { OptionText = "Copenhagen" },
                            new Option { OptionText = "Helsinki" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the capital of Denmark?",
                        correctString = "Copenhagen",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oslo" },
                            new Option { OptionText = "Reykjavik" },
                            new Option { OptionText = "Copenhagen" },
                            new Option { OptionText = "Helsinki" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the capital of Finland?",
                        correctString = "Helsinki",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oslo" },
                            new Option { OptionText = "Stockholm" },
                            new Option { OptionText = "Copenhagen" },
                            new Option { OptionText = "Helsinki" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the capital of Iceland?",
                        correctString = "Reykjavik",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oslo" },
                            new Option { OptionText = "Stockholm" },
                            new Option { OptionText = "Copenhagen" },
                            new Option { OptionText = "Reykjavik" }
                        }
                    }
                }
            },
            new Quiz
            {
                Title = "Movies Trivia",
                Description = "How well do you know your movies?",
                Theme = "guac",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Who directed 'Inception'?",
                        correctString = "Christopher Nolan",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Steven Spielberg" },
                            new Option { OptionText = "Christopher Nolan" },
                            new Option { OptionText = "James Cameron" },
                            new Option { OptionText = "Quentin Tarantino" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What was the first Star Wars movie to be released?",
                        correctString = "Star Wars: Episode IV - A New Hope",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Star Wars: Episode IV - A New Hope" },
                            new Option { OptionText = "Star Wars: Episode I - The Phantom Menace" },
                            new Option { OptionText = "Rogue One" },
                            new Option { OptionText = "Solo: A Star Wars Story" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "Who directed 'The Room'?",
                        correctString = "Tommy Wiseau",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Tim Burton" },
                            new Option { OptionText = "Ridley Scott" },
                            new Option { OptionText = "Tommy Wiseau" },
                            new Option { OptionText = "Michael Bay" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "Who broke two toes while filming a scene for The Lord of the Rings: The Two Towers?",
                        correctString = "Viggo Mortensen - Aragorn",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Ian McKellen - Gandalf" },
                            new Option { OptionText = "Christopher Lee - Saruman" },
                            new Option { OptionText = "Viggo Mortensen - Aragorn" },
                            new Option { OptionText = "Orlando Bloom - Legolas" }
                        }
                    }
                }
            },
            new Quiz
            {
                Title = "Monuments of the Solar System",
                Description = "How well do you know our solar system?",
                Theme = "onion",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What is the tallest mountain in the solar system?",
                        correctString = "Olympus Mons",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Kilimanjaro" },
                            new Option { OptionText = "Maxwell Montes" },
                            new Option { OptionText = "Olympus Mons" },
                            new Option { OptionText = "Mount Everest" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the biggest asteroid in the solar system?",
                        correctString = "Ceres",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Vesta" },
                            new Option { OptionText = "Psyche" },
                            new Option { OptionText = "Pluto" },
                            new Option { OptionText = "Ceres" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What man made object has traveled the farthest from earth?",
                        correctString = "Voyager 1",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Voyager 1" },
                            new Option { OptionText = "Voyager 2" },
                            new Option { OptionText = "Apollo 13" },
                            new Option { OptionText = "Sputnik 1" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the biggest planet in the solar system?",
                        correctString = "Jupiter",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Saturn" },
                            new Option { OptionText = "Jupiter" },
                            new Option { OptionText = "Uranus" },
                            new Option { OptionText = "Earth" }
                        }
                    }
                }
            },
            new Quiz
            {
                Title = "Science Facts",
                Description = "Test your knowledge of basic science facts.",
                Theme = "salsa",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "What planet is known as the Red Planet?",
                        correctString = "Mars",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Venus" },
                            new Option { OptionText = "Mars" },
                            new Option { OptionText = "Jupiter" },
                            new Option { OptionText = "Saturn" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the chemical symbol for water?",
                        correctString = "H2O",
                        options = new List<Option>
                        {
                            new Option { OptionText = "CO2" },
                            new Option { OptionText = "H2O" },
                            new Option { OptionText = "O2" },
                            new Option { OptionText = "NaCl" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What gas do plants absorb from the atmosphere?",
                        correctString = "Carbon Dioxide",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Oxygen" },
                            new Option { OptionText = "Nitrogen" },
                            new Option { OptionText = "Carbon Dioxide" },
                            new Option { OptionText = "Hydrogen" }
                        }
                    },
                    new Question
                    {
                        QuestionText = "What is the hardest natural substance on Earth?",
                        correctString = "Diamond",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Gold" },
                            new Option { OptionText = "Iron" },
                            new Option { OptionText = "Diamond" },
                            new Option { OptionText = "Silver" }
                        }
                    }
                }
            }
        };

        db.Quizzes.AddRange(sampleQuizzes);
        db.SaveChanges();
        Console.WriteLine("✅ Sample quizzes added to the database.");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
