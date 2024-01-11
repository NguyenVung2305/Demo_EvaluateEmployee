using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Skill
{

  [Table("CategorySkill")]
  public class CategorySkill
  {

      [Key]
      public int Id { get; set; }



      // Tiều đề CategorySkill
      [Required(ErrorMessage = "Phải có tên danh mục")]
      [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
      [Display(Name = "Tên danh mục")]
      public string Title { get; set; }

      // Nội dung, thông tin chi tiết về CategorySkill
      [DataType(DataType.Text)]
      [Display(Name = "Nội dung danh mục")]
      public string Description { get; set; }

      //chuỗi Url
      [Required(ErrorMessage = "Phải tạo url")]
      [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
      [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
      [Display(Name = "Url hiện thị")]
      public string Slug { set; get; }



      // Các CategorySkill con
      public ICollection<CategorySkill> CategorySkillChildren { get; set; }

      // CategorySkill cha (FKey)
      [Display(Name = "Danh mục cha")]
      public int? ParentCategorySkillId { get; set; }      

      [ForeignKey("ParentCategorySkillId")]
      [Display(Name = "Danh mục cha")]
      public CategorySkill ParentCategorySkill { set; get; }


      public void ChildCategorySkillIDs(ICollection<CategorySkill>  childcates, List<int> lists)
      {
          if (childcates == null)
            childcates = this.CategorySkillChildren;

          foreach (CategorySkill category in childcates)
          {
              lists.Add(category.Id);
              ChildCategorySkillIDs(category.CategorySkillChildren, lists);

          }
      }

      public List<CategorySkill> ListParents()
      {
          List<CategorySkill> li = new List<CategorySkill>();
          var parent = this.ParentCategorySkill;
          while (parent != null)
          {
              li.Add(parent);
              parent = parent.ParentCategorySkill;

          }
          li.Reverse();
          return li;
      }

  }
}