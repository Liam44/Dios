using Dios.Extensions;
using System;
using System.Security.Claims;
using Xunit;

namespace DiosTest.Extensions
{
    public class CurrentUserId
    {
        [Fact]
        public void Id_Null()
        {
            // Arrange
            ClaimsPrincipal user = null;

            // Act

            try
            {
                // Act
                var result = user.Id();
            }
            catch (ArgumentNullException ex)
            {
                // Assert
                Assert.NotNull(ex);
                Assert.Contains(nameof(user), ex.Message);
            }
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
