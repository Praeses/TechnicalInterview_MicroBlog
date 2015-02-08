using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroBlog.Web.Models.Db
{
    public class Follow
    {
        public int Id { get; set; }

        /// Marking both foreign keys as [Required] results in an exception when creating the database
        /// constraints. Sounds like SQL Server doesn't like the potential cycle in cascading delete
        /// scenarios. Options are to either turn off cascading deletes using Fluent, use a trigger
        /// to perform the delete,  or just leave the datamodel slightly imperfect and potentially
        /// allow half-orphened Follows. I'm going with the last option for now.
        /// More info here: http://stackoverflow.com/a/852047/1097016
        /// Error message...
        ///     Introducing FOREIGN KEY constraint 'FK_dbo.Follows_dbo.AspNetUsers_ApplicationUserIdFollowed'
        ///     on table 'Follows' may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION
        ///     or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
        // [Required]
        [Index("IX_ApplicationUserIdAndApplicationUserIdFollowed", 1, IsUnique = true)]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        [InverseProperty("Follows")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Index("IX_ApplicationUserIdAndApplicationUserIdFollowed", 2, IsUnique = true)] // Prevent following the same user more than once
        [ForeignKey("ApplicationUserFollowed")]
        public string ApplicationUserIdFollowed { get; set; }
        [InverseProperty("FollowedBy")]
        public virtual ApplicationUser ApplicationUserFollowed { get; set; }
    }
}