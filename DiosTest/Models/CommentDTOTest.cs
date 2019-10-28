using Dios.Models;
using System;
using Xunit;

namespace DiosTest.Models
{
    public class CommentDTOTest
    {
        [Fact]
        public void CommentDTO_Null()
        {
            // Arrange
            Comment comment = null;

            // Act
            CommentDTO commentDTO = new CommentDTO(comment);

            // Assert
            Assert.Equal(-1, commentDTO.Id);
            Assert.Empty(commentDTO.Text);
            Assert.NotEmpty(commentDTO.TimeOfComment.ToString());

            Assert.Equal(-1, commentDTO.ErrorReport.Id);
            Assert.NotNull(commentDTO.ErrorReport);
            Assert.Null(commentDTO.ErrorReport.Seen);
            Assert.Empty(commentDTO.ErrorReport.Description);
            Assert.Empty(commentDTO.ErrorReport.Subject);
            Assert.NotEmpty(commentDTO.ErrorReport.Submitted.ToString());
            Assert.Equal(Status.waiting, commentDTO.ErrorReport.CurrentStatus);
            Assert.Equal(Priority.undefined, commentDTO.ErrorReport.CurrentPriority);
            Assert.Equal(-1, commentDTO.ErrorReport.FlatId);

            Assert.NotNull(commentDTO.Host);
            Assert.Empty(commentDTO.Host.Id);
            Assert.Empty(commentDTO.Host.PersonalNumber);
            Assert.Empty(commentDTO.Host.FirstName);
            Assert.Empty(commentDTO.Host.LastName);
            Assert.Empty(commentDTO.Host.Email);
            Assert.Empty(commentDTO.Host.PhoneNumber);
            Assert.Empty(commentDTO.Host.PhoneNumber2);
            Assert.Empty(commentDTO.Host.NormalizedEmail);
        }

        [Fact]
        public void CommentDTO_NotNull()
        {
            // Arrange
            int commentId = 1;
            string text = "someText";
            DateTime timeOfComment = DateTime.Now;

            Comment comment = new Comment
            {
                Id = commentId,
                Text = text,
                TimeOfComment = timeOfComment
            };

            // Act
            CommentDTO commentDTO = new CommentDTO(comment);

            // Assert
            Assert.Equal(commentId, commentDTO.Id);
            Assert.Equal(text, commentDTO.Text);
            Assert.Equal(timeOfComment, commentDTO.TimeOfComment);

            Assert.Equal(-1, commentDTO.ErrorReport.Id);
            Assert.NotNull(commentDTO.ErrorReport);
            Assert.Null(commentDTO.ErrorReport.Seen);
            Assert.Empty(commentDTO.ErrorReport.Description);
            Assert.Empty(commentDTO.ErrorReport.Subject);
            Assert.NotEmpty(commentDTO.ErrorReport.Submitted.ToString());
            Assert.Equal(Status.waiting, commentDTO.ErrorReport.CurrentStatus);
            Assert.Equal(Priority.undefined, commentDTO.ErrorReport.CurrentPriority);
            Assert.Equal(-1, commentDTO.ErrorReport.FlatId);

            Assert.NotNull(commentDTO.Host);
            Assert.Empty(commentDTO.Host.Id);
            Assert.Empty(commentDTO.Host.PersonalNumber);
            Assert.Empty(commentDTO.Host.FirstName);
            Assert.Empty(commentDTO.Host.LastName);
            Assert.Empty(commentDTO.Host.Email);
            Assert.Empty(commentDTO.Host.PhoneNumber);
            Assert.Empty(commentDTO.Host.PhoneNumber2);
            Assert.Empty(commentDTO.Host.NormalizedEmail);
        }
    }
}
