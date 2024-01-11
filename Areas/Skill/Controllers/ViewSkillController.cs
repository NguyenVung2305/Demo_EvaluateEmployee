using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.Models.Skill;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppMvc.Net.Areas.Skill.Controllers
{
    [Area("Skill")]
    public class ViewSkillController : Controller
    {
        private readonly ILogger<ViewSkillController> _logger;
        private readonly AppDbContext _context;

        public ViewSkillController(ILogger<ViewSkillController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // /post/
        // /post/{categoryslug?}
        [Route("/skill/{categoryslug?}")]
        public IActionResult Index(string categoryslug, [FromQuery(Name = "p")]int currentPage, int pagesize)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;
            ViewBag.categoryslug = categoryslug;

            CategorySkill category = null;

            if (!string.IsNullOrEmpty(categoryslug))    
            {
                category = _context.CategorySkills.Where(c => c.Slug == categoryslug)
                                    .Include(c => c.CategorySkillChildren)
                                    .FirstOrDefault();

                if (category == null)
                {
                    return NotFound("Không thấy category");
                }                    
            }

            var skills = _context.Skills
                                .Include(p => p.Author)
                                .Include(p => p.SkillCategorySkills)
                                .ThenInclude(p => p.Category)
                                .AsQueryable();

           skills.OrderByDescending(p => p.DateUpdated);

            if (category != null)
            {
                var ids = new List<int>();
                category.ChildCategorySkillIDs(null, ids);
                ids.Add(category.Id);


                skills = skills.Where(p => p.SkillCategorySkills.Where(pc => ids.Contains(pc.CategoryID)).Any());


            }

            int totalSkills = skills.Count();  
            if (pagesize <=0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalSkills / pagesize);
 
            if (currentPage > countPages) currentPage = countPages;     
            if (currentPage < 1) currentPage = 1; 

            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new {
                    p =  pageNumber,
                    pagesize = pagesize
                })
            };

            var skillsInPage = skills.Skip((currentPage - 1) * pagesize)
                             .Take(pagesize);   


            ViewBag.pagingModel = pagingModel;
            ViewBag.totalSkills = totalSkills; 


                 
            ViewBag.category = category;
            return View(skillsInPage.ToList());
        }

        [Route("/skill/{skillslug}.html")]
        public IActionResult Detail(string skillslug)
        {
            var categories = GetCategories();
            ViewBag.categories = categories;

            var skill = _context.Skills.Where(p => p.Slug == skillslug)
                               .Include(p => p.Author)
                               .Include(p => p.SkillCategorySkills)
                               .ThenInclude(pc => pc.Category)
                               .FirstOrDefault();

            if (skill == null)
            {
                return NotFound("Không thấy bài viết");
            }            

            CategorySkill category = skill.SkillCategorySkills.FirstOrDefault()?.Category;
            ViewBag.category = category;

            var otherSkills = _context.Skills.Where(p => p.SkillCategorySkills.Any(c => c.Category.Id == category.Id))
                                            .Where(p => p.SkillId != skill.SkillId)
                                            .OrderByDescending(p => p.DateUpdated)
                                            .Take(5);
            ViewBag.otherSkills = otherSkills;                                

            return View(skill);
        }

        private List<CategorySkill> GetCategories()
        {
            var categories = _context.CategorySkills
                            .Include(c => c.CategorySkillChildren)
                            .AsEnumerable()
                            .Where(c => c.ParentCategorySkill == null)
                            .ToList();
            return categories;                
        }

    }
}