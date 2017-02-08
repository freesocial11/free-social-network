// #region Author Information
// // SkillWithUsersModel.cs
// // 
// // (c) 2017 Apexol Technologies. All Rights Reserved.
// // 
// #endregion

using System.Collections.Generic;
using mobSocial.WebApi.Configuration.Mvc.Models;
using mobSocial.WebApi.Models.Media;

namespace mobSocial.WebApi.Models.Skills
{
    public class SkillWithUsersModel : RootModel
    {
        public SkillModel Skill { get; set; }

        public string FeaturedMediaImageUrl { get; set; }

        public IList<UserSkillModel> UserSkills { get; set; }

        public int TotalUsers { get; set; }

        public int CurrentPage { get; set; }

        public int UsersPerPage { get; set; }

        public int FollowerCount { get; set; }
    }
}