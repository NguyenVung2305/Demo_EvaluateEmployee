using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Skill;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using AppMvc.Areas.Blog.Models;
using Microsoft.AspNetCore.Identity;
using App.Utilities;
using AppMvc.Areas.Skill.Models;

namespace AppMvc.Areas.Skill.Controllers
{
    [Area("Skill")]
    [Route("admin/skillmanage/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator+  "," + RoleName.Editor)]
    public class SkillManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SkillManageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        // GET: Blog/Post
        public async Task<IActionResult> Index([FromQuery(Name = "p")]int currentPage, int pagesize)
        {
            var posts = _context.Skills
                        .Include(p => p.Author)
                        .OrderByDescending(p => p.DateUpdated);

            int totalPosts = await posts.CountAsync();  
            if (pagesize <=0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalPosts / pagesize);
 
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

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalPosts;

            ViewBag.postIndex = (currentPage - 1) * pagesize;

            var postsInPage = await posts.Skip((currentPage - 1) * pagesize)
                             .Take(pagesize)
                             .Include(p => p.SkillCategorySkills)
                             .ThenInclude(pc  => pc.Category)
                             .ToListAsync();   
                        
            return View(postsInPage);
        }

        // GET: Blog/Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Skills
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.SkillId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Post/Create
        public async Task<IActionResult> CreateAsync()
        {
            var categories = await _context.CategorySkills.ToListAsync();

            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View();
        }

        // POST: Blog/Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs")] CreateSkillModel skill)
        {
            var categories = await _context.CategorySkills.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            if (skill.Slug == null)
            {
                skill.Slug = AppUtilities.GenerateSlug(skill.Title);
            }

            if(await _context.Skills.AnyAsync(p => p.Slug == skill.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(skill);
            }



            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                skill.DateCreated = skill.DateUpdated = DateTime.Now;
                skill.AuthorId = user.Id;
                _context.Add(skill);

                if (skill.CategoryIDs != null)
                {
                    foreach (var CateId in skill.CategoryIDs)
                    {
                        _context.Add(new SkillCategorySkill()
                        {
                            CategoryID = CateId,
                            Skill = skill
                        });
                    }
                }


                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }


            return View(skill);
        }

        // GET: Blog/Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var post = await _context.Skills.FindAsync(id);
            var skill = await _context.Skills.Include(p => p.SkillCategorySkills).FirstOrDefaultAsync(p=> p.SkillId == id);
            if (skill == null)
            {
                return NotFound();
            }

            var postEdit = new CreateSkillModel()
            {
                SkillId = skill.SkillId,
                Title = skill.Title,
                Content = skill.Content,
                Description = skill.Description,
                Slug = skill.Slug,
                Published = skill.Published,
                CategoryIDs  =  skill.SkillCategorySkills.Select(pc => pc.CategoryID).ToArray()
            };

            var categories = await _context.CategorySkills.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");          

            return View(postEdit);
        }

        // POST: Blog/Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SkillID,Title,Description,Slug,Content,Published,CategoryIDs")] CreateSkillModel skill)
        {
            if (id != skill.SkillId)
            {
                return NotFound();
            }
            var categories = await _context.CategorySkills.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");     


            if (skill.Slug == null)
            {
                skill.Slug = AppUtilities.GenerateSlug(skill.Title);
            }

            if(await _context.Skills.AnyAsync(p => p.Slug == skill.Slug && p.SkillId != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(skill);
            }


            if (ModelState.IsValid)
            {
                try
                {

                    var skillUpdate = await _context.Skills.Include(p => p.SkillCategorySkills).FirstOrDefaultAsync(p=> p.SkillId == id);
                    if (skillUpdate == null)
                    {
                        return NotFound();
                    }

                    skillUpdate.Title = skill.Title;
                    skillUpdate.Description = skill.Description;
                    skillUpdate.Content = skill.Content;
                    skillUpdate.Published = skill.Published;
                    skillUpdate.Slug = skill.Slug;
                    skillUpdate.DateUpdated = DateTime.Now;

                    // Update PostCategory
                    if (skill.CategoryIDs == null) skill.CategoryIDs = new int[] {};

                    var oldCateIds = skillUpdate.SkillCategorySkills.Select(c => c.CategoryID).ToArray();
                    var newCateIds = skill.CategoryIDs;

                    var removeCatePosts = from skillCate in skillUpdate.SkillCategorySkills
                                          where (!newCateIds.Contains(skillCate.CategoryID))
                                          select skillCate;
                    _context.SkillCategorySkills.RemoveRange(removeCatePosts);

                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;

                     foreach (var CateId in addCateIds)
                     {
                         _context.SkillCategorySkills.Add(new SkillCategorySkill(){
                             SkillID = id,
                             CategoryID = CateId
                         });
                     }      

                    _context.Update(skillUpdate);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(skill.SkillId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa cập nhật bài viết";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", skill.AuthorId);
            return View(skill);
        }

        // GET: Blog/Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.SkillId == id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Blog/Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            StatusMessage = "Bạn vừa xóa bài viết: "  + skill.Title;

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Skills.Any(e => e.SkillId == id);
        }
    }
}
