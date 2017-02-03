using System.Collections.Generic;
using mobSocial.Data.Entity.Skills;

namespace mobSocial.Tests.SampleData
{
    public static class SampleSkills
    {
        private static readonly List<Skill> sampleSkills;

        static SampleSkills()
        {
            sampleSkills = new List<Skill>();
            PopulateSkills();
        }

        public static List<Skill> GetSkills()
        {
            return sampleSkills;
        }

        public static Skill GetSkill(int index)
        {
            return sampleSkills[index];
        }

        static void PopulateSkills()
        {
            sampleSkills.Add(new Skill()
            {
                SkillName = "Skill 1",
                DisplayOrder = 0,
                UserId = 0
            });

            sampleSkills.Add(new Skill() {
                SkillName = "Skill 2",
                DisplayOrder = 1,
                UserId = 0
            });

            sampleSkills.Add(new Skill() {
                SkillName = "Skill 3",
                DisplayOrder = 2,
                UserId = 0
            });
        }
    }
}