using System.ComponentModel.DataAnnotations;
using App.Models.Skill;

namespace AppMvc.Areas.Skill.Models {
    public class CreateSkillModel : SkillModel {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; }
    }
}