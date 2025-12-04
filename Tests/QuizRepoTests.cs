using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Quizadilla.Data;
using Quizadilla.Models;
using Xunit;

namespace QuizadillaTests
{
    public class QuizRepositoryTests
    {
        private QuizDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new QuizDbContext(options);
        }


    // CREATE        
        [Fact]
        public void AddQuiz_AddQuizWithQuestionsAndOptionsPrettyPlease()
        {
            using var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var quiz = new Quiz
            {
                Title = "Test Quiz",
                Description = "Very nice quiz just as nice as this test",
                UserID = "Sabrina-Carpenter",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Q1",
                        correctString = "Correct",
                        options = new List<Option>
                        {
                            new Option { OptionText = "A" },
                            new Option { OptionText = "B" }
                        }
                    }
                }
            };

            repo.AddQuiz(quiz);
            repo.Save();

            var stored = db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.options)
                .Single();

            Assert.Equal("Test Quiz", stored.Title);
            Assert.Single(stored.Questions);
            Assert.Equal(2, stored.Questions.First().options.Count);
        }

        // Negative null questions
        [Fact]
        public void AddQuiz_AllowNullQuestionsPwease()
        {
            using var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var quiz = new Quiz
            {
                Title = "Empty quiz",
                Description = "No questions asked",
                UserID = "Kendrinck-Lamar",
                Questions = null
            };

            repo.AddQuiz(quiz);
            repo.Save();

            var stored = db.Quizzes.Single();
            Assert.Equal("Empty quiz", stored.Title);
            Assert.Null(stored.Questions);
        }

    
    // READ
        [Fact]
        public void GetQuizForEdit_ReturnQuizWithBothQuestionsAndOptions_WhenExists()
        {
            using var db = GetInMemoryDbContext();

            var quiz = new Quiz
            {
                Title = "Quiz",
                UserID = "Vince-Masuka",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Q1",
                        options = new List<Option> { new Option { OptionText = "A" } }
                    }
                }
            };

            db.Quizzes.Add(quiz);
            db.SaveChanges();

            var repo = new QuizRepository(db);

            var loaded = repo.GetQuizForEdit(quiz.QuizId);

            Assert.NotNull(loaded);
            Assert.Single(loaded!.Questions);
            Assert.Single(loaded.Questions.First().options);
        }

        [Fact]
        public void GetQuizForEdit_PlsReturnNull_WhenQuizDoesntExist()
        {
            using var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var result = repo.GetQuizForEdit(999);

            Assert.Null(result);
        }

    // UPDATE
        [Fact]
        public void UpdateQuiz_UpdateTitleDescriptionAndQuestionsAndOptions()
        {
            using var db = GetInMemoryDbContext();

            var original = new Quiz
            {
                Title = "Original",
                Description = "Orig desc",
                UserID = "Barack Obama",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Q1",
                        correctString = "A",
                        options = new List<Option>
                        {
                            new Option { OptionText = "A" }, 
                            new Option { OptionText = "B" } 
                        }
                    },
                    new Question
                    {
                        QuestionText = "Q2",
                        correctString = "X",
                        options = new List<Option>
                        {
                            new Option { OptionText = "X" }
                        }
                    }
                }
            };

            db.Quizzes.Add(original);
            db.SaveChanges();

            var repo = new QuizRepository(db);

            // Get w/ keys from db
            var stored = repo.GetQuizForEdit(original.QuizId)!;
            var q1 = stored.Questions.First();
            var q2 = stored.Questions.Skip(1).First();
            var q1OptA = q1.options.First();
            var q1OptB = q1.options.Skip(1).First();

            // Build "updated" object as API (best api itw) does
            var updated = new Quiz
            {
                QuizId = stored.QuizId,
                Title = "Updated",
                Description = "Updated desc",
                UserID = stored.UserID,
                Questions = new List<Question>
                {
                    // Q1 kept, n updated
                    new Question
                    {
                        Id = q1.Id,
                        QuestionText = "Q1 updated",
                        correctString = "NEW",
                        options = new List<Option>
                        {
                            new Option
                            {
                                OptionId = q1OptA.OptionId,
                                OptionText = "A updated"
                            },
                            
                            new Option
                            {
                                OptionId = 0,              
                                OptionText = "C new"
                            }
                        }
                    }
                }
            };

            repo.UpdateQuiz(updated);
            repo.Save();

            var reloaded = repo.GetQuizForEdit(original.QuizId)!;

            Assert.Equal("Updated", reloaded.Title);
            Assert.Equal("Updated desc", reloaded.Description);

            // Q2 removed
            Assert.Single(reloaded.Questions);
            var updatedQ1 = reloaded.Questions.First();
            Assert.Equal("Q1 updated", updatedQ1.QuestionText);
            Assert.Equal("NEW", updatedQ1.correctString);

            // B rem, A upd, C new
            Assert.Equal(2, updatedQ1.options.Count);
            Assert.Contains(updatedQ1.options, o => o.OptionText == "A updated");
            Assert.Contains(updatedQ1.options, o => o.OptionText == "C new");
            Assert.DoesNotContain(updatedQ1.options, o => o.OptionText == "B");
        }

        [Fact]
        public void UpdateQuiz_DoNothingBeLazy_WhenQuizDoesntExist()
        {
            using var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var updated = new Quiz
            {
                QuizId = 12345,
                Title = "Non-existing",
                Description = "Should not be saved",
                UserID = "Ye-Kanye-West",
                Questions = new List<Question>()
            };

            repo.UpdateQuiz(updated);
            repo.Save();

            Assert.Empty(db.Quizzes);
        }

    // DELETE
        [Fact]
        public void DeleteQuiz_RemoveQuizQuestionsAndOptions()
        {
            using var db = GetInMemoryDbContext();

            var quiz = new Quiz
            {
                Title = "To delete",
                UserID = "Justin-Bieber",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Q1",
                        options = new List<Option>
                        {
                            new Option { OptionText = "A" },
                            new Option { OptionText = "B" }
                        }
                    }
                }
            };

            db.Quizzes.Add(quiz);
            db.SaveChanges();

            var repo = new QuizRepository(db);

            repo.DeleteQuiz(quiz.QuizId);
            repo.Save();

            Assert.Empty(db.Quizzes);
            Assert.Empty(db.Questions);
            Assert.Empty(db.Options);
        }

        [Fact]
        public void DeleteQuiz_ShouldNotThrow_WhenQuizDoesntExist()
        {
            using var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            repo.DeleteQuiz(999);
            repo.Save();

            // No exception, db still empty
            Assert.Empty(db.Quizzes);
            Assert.Empty(db.Questions);
            Assert.Empty(db.Options);
        }
    }
}