using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using chel;

namespace UnitTestProject_Auth
{
    /// <summary>
    /// Класс тестирования авторизации.
    /// </summary>
    [TestClass]
    public class AuthTests
    {
        /// <summary>
        /// Проверяет, что метод <see cref="LoginPage.Auth"/> возвращает <c>false</c> для заведомо неверных учётных данных.
        /// </summary>
        [TestMethod]
        public void AuthTest()
        {
            bool result = new LoginPage().Auth("wrong", "wrong");
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Проверяет успешную авторизацию для всех пользователей, существующих в базе данных.
        /// </summary>
        [TestMethod]
        public void AuthTestSuccess()
        {
            var users = Core.Context.Users.ToList();
            foreach (var user in users)
            {
                bool result = new LoginPage().Auth(user.UserName, user.PasswordHash);
                Assert.IsTrue(result, $"Пользователь {user.UserName} не прошёл авторизацию");
            }
        }

        /// <summary>
        /// Проверяет негативные сценарии авторизации.
        /// </summary>
        [TestMethod]
        public void AuthTestFail()
        {
            var loginPage = new LoginPage();

            // Пустые поля
            Assert.IsFalse(loginPage.Auth("", ""));

            var anyUser = Core.Context.Users.FirstOrDefault();
            if (anyUser != null)
            {
                // Неверный логин
                Assert.IsFalse(loginPage.Auth("NotExist", anyUser.PasswordHash));
                // Неверный пароль
                Assert.IsFalse(loginPage.Auth(anyUser.UserName, "WrongPass123"));
            }

            // Логин с пробелами
            Assert.IsFalse(loginPage.Auth("   ", "123"));
            // Длинные строки
            Assert.IsFalse(loginPage.Auth(new string('a', 100), new string('b', 100)));
        }
    }
}