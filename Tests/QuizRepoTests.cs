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

        // 1) CREATE
        [Fact]
        public void AddQuiz_ShouldAddQuizToDatabase()
        {
            var db = GetInMemoryDbContext();
            var service = new QuizRepository(db);

            var quiz = new Quiz
            {
                Title = "Test Quiz",
                Description = "Desc",
                UserID = "test-user",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "Q1",
                        correctString = "Correct",
                        options = new List<Option>
                        {
                            new Option { OptionText = "Wrong" }
                        }
                    }
                }
            };

            service.AddQuiz(quiz);
            service.Save();

            Assert.Single(db.Quizzes);
            Assert.Single(db.Quizzes.First().Questions);
        }

        // 2) CREATE with null questions
        [Fact]
        public void AddQuiz_ShouldAllowNullQuestions()
        {
            var db = GetInMemoryDbContext();
            var service = new QuizRepository(db);

            var quiz = new Quiz
            {
                Title = "Empty quiz",
                Description = "No questions",
                UserID = "test-user",
                Questions = null
            };

            service.AddQuiz(quiz);
            service.Save();

            Assert.Single(db.Quizzes);
        }

        // 3) READ
        [Fact]
        public void GetQuizForEdit_ShouldReturnQuiz_WhenExists()
        {
            var db = GetInMemoryDbContext();

            var quiz = new Quiz
            {
                Title = "Quiz",
                UserID = "test-user",
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

            var service = new QuizRepository(db);

            var loaded = service.GetQuizForEdit(quiz.QuizId);

            Assert.NotNull(loaded);
            Assert.Single(loaded!.Questions);
            Assert.Single(loaded.Questions.First().options);
        }

        // 4) READ negative
        [Fact]
        public void GetQuizForEdit_ShouldReturnNull_WhenNotFound()
        {
            var db = GetInMemoryDbContext();
            var service = new QuizRepository(db);

            var result = service.GetQuizForEdit(999);

            Assert.Null(result);
        }

        // 5) UPDATE
        [Fact]
        public void UpdateQuiz_ShouldModifyTitleAndDescription()
        {
            var db = GetInMemoryDbContext();

            var original = new Quiz
            {
                Title = "Old title",
                Description = "Old desc",
                UserID = "test-user",
                Questions = new List<Question>()
            };

            db.Quizzes.Add(original);
            db.SaveChanges();

            var service = new QuizRepository(db);

            var updated = new Quiz
            {
                QuizId = original.QuizId,
                Title = "New title",
                Description = "New desc",
                UserID = "test-user",
                Questions = new List<Question>()
            };

            service.UpdateQuiz(updated);
            service.Save();

            var loaded = db.Quizzes.First();

            Assert.Equal("New title", loaded.Title);
            Assert.Equal("New desc", loaded.Description);
        }

        // 6) UPDATE negative
        [Fact]
        public void UpdateQuiz_ShouldDoNothing_WhenQuizDoesNotExist()
        {
            var db = GetInMemoryDbContext();
            var service = new QuizRepository(db);

            var updated = new Quiz
            {
                QuizId = 123,
                Title = "Does not exist",
                UserID = "test-user",
                Questions = new List<Question>()
            };

            service.UpdateQuiz(updated);
            service.Save();

            Assert.Empty(db.Quizzes);
        }

        // 7) DELETE
        [Fact]
        public void DeleteQuiz_ShouldRemoveQuiz()
        {
            var db = GetInMemoryDbContext();

            var quiz = new Quiz
            {
                Title = "To delete",
                UserID = "test-user",
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

            var service = new QuizRepository(db);

            service.DeleteQuiz(quiz.QuizId);
            service.Save();

            Assert.Empty(db.Quizzes);
        }

        // 8) DELETE negative
        [Fact]
        public void DeleteQuiz_ShouldNotThrow_WhenQuizDoesNotExist()
        {
            var db = GetInMemoryDbContext();
            var service = new QuizRepository(db);

            service.DeleteQuiz(999);
            service.Save();

            Assert.Empty(db.Quizzes);
        }
    }
}
