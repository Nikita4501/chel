using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chel;

namespace UnitTestProject_Register
{
    /// <summary>
    /// Класс тестирования регистрации.
    /// </summary>
    [TestClass]
    public class RegisterTests
    {
        /// <summary>
        /// Позитивные сценарии регистрации.
        /// </summary>
        [TestMethod]
        public void RegisterTestSuccess()
        {
            var registerPage = new RegisterPage();
            string uniqueLogin = $"testuser_{DateTime.Now.Ticks}";

            bool result = registerPage.Register(uniqueLogin, "ValidPass123", "Test User");
            Assert.IsTrue(result);

            // Проверяем, что пользователь действительно появился в БД
            var user = Core.Context.Users.FirstOrDefault(u => u.UserName == uniqueLogin);
            Assert.IsNotNull(user);
            Assert.AreEqual("Test User", user.DisplayName);

            // Очистка после теста
            Core.Context.Users.Remove(user);
            Core.Context.SaveChanges();
        }

        /// <summary>
        /// Негативные сценарии регистрации.
        /// </summary>
        [TestMethod]
        public void RegisterTestFail()
        {
            var registerPage = new RegisterPage();

            // Пустые поля
            Assert.IsFalse(registerPage.Register("", "", ""));
            Assert.IsFalse(registerPage.Register("user", "", "Name"));
            Assert.IsFalse(registerPage.Register("", "pass", "Name"));

            // Существующий логин
            var existingUser = Core.Context.Users.FirstOrDefault();
            if (existingUser != null)
            {
                Assert.IsFalse(registerPage.Register(existingUser.UserName, "any", "any"));
            }

            // Очень длинные строки
            string longStr = new string('a', 200);
            Assert.IsFalse(registerPage.Register(longStr, longStr, longStr));

            // Спецсимволы
            Assert.IsFalse(registerPage.Register("admin'--", "pass", "name"));
        }
    }
}