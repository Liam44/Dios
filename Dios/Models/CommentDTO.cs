using System;

namespace Dios.Models
{
    public class CommentDTO
    {
        public int Id { get; set; } = -1;

        public ErrorReportDTO ErrorReport { get; set; } = new ErrorReportDTO();

        public string Text { get; set; } = string.Empty;

        public DateTime TimeOfComment { get; set; } = DateTime.Now;

        public UserDTO Host { get; set; } = new UserDTO();

        public CommentDTO(Comment comment)
        {
            if (comment != null)
            {
                Id = comment.Id;
                Text = comment.Text;
                TimeOfComment = comment.TimeOfComment;
            }
        }
    }
}
