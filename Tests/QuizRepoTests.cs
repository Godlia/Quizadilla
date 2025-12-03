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
        public void AddQuiz_ShouldAddQuizToDatabase()
        {
            var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

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

            repo.AddQuiz(quiz);
            repo.Save();

            Assert.Single(db.Quizzes);
            Assert.Single(db.Quizzes.First().Questions);
        }

        // CREATE with null questions
        [Fact]
        public void AddQuiz_ShouldAllowNullQuestions()
        {
            var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var quiz = new Quiz
            {
                Title = "Empty quiz",
                Description = "No questions",
                UserID = "test-user",
                Questions = null
            };

            repo.AddQuiz(quiz);
            repo.Save();

            Assert.Single(db.Quizzes);
        }

        // READ
        [Fact]
        public void GetQuizForEdit_ShouldReturnQuiz_WhenExists()
        {
            var db = GetInMemoryDbContext();

            var quiz = new Quiz
            {
                Title = "Quizhehesiu",
                UserID = "Akinfenwa",
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "babag-ala-fortnajt",
                        options = new List<Option> { new Option { OptionText = "babag" } }
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

        // READ negative
        [Fact]
        public void GetQuizForEdit_PlsReturnNull_WhenNotExist()
        {
            var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var result = repo.GetQuizForEdit(6767);

            Assert.Null(result);
        }

        // UPDATE
        [Fact]
        public void UpdateQuiz_PleaseUpdateTitlePlusDescription()
        {
            var db = GetInMemoryDbContext();

            var original = new Quiz
            {
                Title = "Please work?",
                Description = "I hope this works",
                UserID = "67",
                Questions = new List<Question>()
            };

            db.Quizzes.Add(original);
            db.SaveChanges();

            var repo = new QuizRepository(db);

            var updated = new Quiz
            {
                QuizId = original.QuizId,
                Title = "Did it work?",
                Description = "Please?",
                UserID = "67",
                Questions = new List<Question>()
            };

            repo.UpdateQuiz(updated);
            repo.Save();

            var loaded = db.Quizzes.First();

            Assert.Equal("Did it work?", loaded.Title);
            Assert.Equal("Please?", loaded.Description);
        }

        // UPDATE negative
        [Fact]
        public void UpdateQuiz_SupposedToDoNothing_WhenQuizNotExisting()
        {
            var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            var updated = new Quiz
            {
                QuizId = 123,
                Title = "Should be non-existent like my social life",
                UserID = "Six-Seven up zero sugar",
                Questions = new List<Question>()
            };

            repo.UpdateQuiz(updated);
            repo.Save();

            Assert.Empty(db.Quizzes);
        }

        // DELETE
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

            var repo = new QuizRepository(db);

            repo.DeleteQuiz(quiz.QuizId);
            repo.Save();

            Assert.Empty(db.Quizzes);
        }

        // DELETE negative
        [Fact]
        public void DeleteQuiz_ShouldNotThrow_WhenQuizDoesNotExist()
        {
            var db = GetInMemoryDbContext();
            var repo = new QuizRepository(db);

            repo.DeleteQuiz(6767);
            repo.Save();

            Assert.Empty(db.Quizzes);
        }
    }
}
