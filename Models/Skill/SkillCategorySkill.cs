using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Skill
{
    [Table("SkillCategorySkill")]
    public class SkillCategorySkill
    {
        public int SkillID {set; get;}

        public int CategoryID {set; get;}
        

        [ForeignKey("SkillID")]
        public SkillModel Skill { set; get;}

        [ForeignKey("CategoryID")]
        public CategorySkill Category {set; get;}
    }
}