#region Author Information
// ConversationEntityModel.cs
// 
// (c) 2016 Apexol Technologies. All Rights Reserved.
// 
#endregion

using mobSocial.WebApi.Configuration.Mvc.Models;

namespace mobSocial.WebApi.Models.Conversation
{
    public class ConversationEntityModel : RootEntityModel
    {
        public string ReplyText { get; set; }

        public int ReceiverId { get; set; }

        public bool Group { get; set; }
    }
}