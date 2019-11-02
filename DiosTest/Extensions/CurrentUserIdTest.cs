using Dios.Extensions;
using System;
using System.Security.Claims;
using Xunit;

namespace DiosTest.Extensions
{
    public sealed class CurrentUserIdTest
    {
        [Fact]
        public void Id_Null()
        {
            // Arrange
            ClaimsPrincipal user = null;

            // Act

            // Act
            Exception ex = Assert.Throws<ArgumentNullException>(() => user.Id());

            // Assert
            Assert.Contains(nameof(user), ex.Message);
        }

        [Fact]
        public void Id_NotNull()
        {
            // Arrange
            string userId = "someUserId";
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                                                                          userId));

            // Act
            var result = user.Id();

            // Assert
            Assert.Equal(userId, result);
        }
    }
}
