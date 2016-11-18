using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Social;
using mobSocial.Data.Entity.Timeline;
using mobSocial.Data.Entity.Users;
using mobSocial.Data.Extensions;
using mobSocial.Services.Users;
using Newtonsoft.Json;

namespace mobSocial.Services.Timeline
{
    public class TimelinePostProcessor : ITimelinePostProcessor
    {
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;

        public TimelinePostProcessor(IUserService userService, UserSettings userSettings)
        {
            _userService = userService;
            _userSettings = userSettings;
        }

        public void ProcessInlineTags(TimelinePost post)
        {
            post.Message = Replace(post.Message, post.InlineTags);
        }

        public void ProcessInlineTags(Comment comment)
        {
            comment.CommentText = Replace(comment.CommentText, comment.InlineTags);
        }

        private string Replace(string message, string inlineTags)
        {
            if(string.IsNullOrEmpty(inlineTags)) return message;
            
            var usersList = JsonConvert.DeserializeObject<List<dynamic>>(inlineTags);
            var userIds = usersList.Select(x => (int) x.Id).ToList();
            var users = _userService.Get(x => userIds.Contains(x.Id)).ToList();
            var userTemplate = _userSettings.UserLinkTemplate;
            if (string.IsNullOrEmpty(userTemplate))
            {
                userTemplate = "<a href='' data-uid='{0}'>{1}</a>";
            }
            foreach (var userId in userIds)
            {
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                    continue;

                var userTag = $"@{user.Name}";
                //first replace if full name is available e.g. @John Smith
                message = message.ReplaceFirst(userTag, string.Format(userTemplate, user.Id, user.Name));

                //then with first name e.g. @John, note we are replacing one at a time, so if two users with same name are being tagged,
                //they should have different links for them
                //then check if first name is available
                userTag = $"@{user.FirstName}";
                message = message.ReplaceFirst(userTag, string.Format(userTemplate, user.Id, user.FirstName));
            }
            return message;
        }
    }
}