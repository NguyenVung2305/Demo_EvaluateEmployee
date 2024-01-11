using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Skill;
using Microsoft.AspNetCore.Authorization;
using App.Data;

namespace AppMvc.Net.Areas.Skill.Controllers
{
    [Area("Skill")]
    [Route("admin/categoryskill/category/[action]/{id?}")]
    [Authorize(Roles = RoleName.Administrator)]
    public class CategorySkillController : Controller
    {
        private readonly AppDbContext _context;

        public CategorySkillController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Blog/Category
        public async Task<IActionResult> Index()
        {
            var qr = (from c in _context.CategorySkills select c)
                     .Include(c => c.ParentCategorySkill)
                     .Include(c => c.CategorySkillChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategorySkill == null)
                             .ToList();         

            return View(categories);
        }

        // GET: Blog/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySkills
                .Include(c => c.ParentCategorySkill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        private void CreateSelectItems(List<CategorySkill> source, List<CategorySkill> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("----", level));
            foreach (var category in source)
            {
                // category.Title = prefix + " " + category.Title;
                des.Add(new CategorySkill() {
                    Id = category.Id,
                    Title = prefix + " " + category.Title
                });
                if (category.CategorySkillChildren?.Count > 0)
                {
                    CreateSelectItems(category.CategorySkillChildren.ToList(), des, level +1);
                }
            }
        }
        // GET: Blog/Category/Create
        public async Task<IActionResult> CreateAsync()
        {
            var qr = (from c in _context.CategorySkills select c)
                     .Include(c => c.ParentCategorySkill)
                     .Include(c => c.CategorySkillChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategorySkill == null)
                             .ToList();   
            categories.Insert(0, new CategorySkill(){
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategorySkill>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");


            ViewData["ParentCategoryId"] = selectList;
            return View();
        }

        // POST: Blog/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,ParentCategoryId")] CategorySkill category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentCategorySkillId == -1) category.ParentCategorySkillId  = null;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            var qr = (from c in _context.CategorySkills select c)
                     .Include(c => c.ParentCategorySkill)
                     .Include(c => c.CategorySkillChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategorySkill == null)
                             .ToList();   
            categories.Insert(0, new CategorySkill(){
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategorySkill>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");


            ViewData["ParentCategoryId"] = selectList;
            return View(category);
        }

        // GET: Blog/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySkills.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var qr = (from c in _context.CategorySkills select c)
                     .Include(c => c.ParentCategorySkill)
                     .Include(c => c.CategorySkillChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategorySkill == null)
                             .ToList();   
            categories.Insert(0, new CategorySkill(){
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategorySkill>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");

            ViewData["ParentCategoryId"] = selectList;
            return View(category);
        }


        // POST: Blog/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Slug,ParentCategoryId")] CategorySkill category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            bool canUpdate = true;

            if (category.ParentCategorySkillId  == category.Id)
            {
                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khác");
                canUpdate = false;
            }

            // Kiem tra thiet lap muc cha phu hop
            if (canUpdate && category.ParentCategorySkillId != null)
            { 
            var childCates =  
                        (from c in _context.CategorySkills select c)
                        .Include(c => c.CategorySkillChildren)
                        .ToList()
                        .Where(c => c.ParentCategorySkillId == category.Id);


                // Func check Id 
                Func<List<CategorySkill>, bool> checkCateIds = null;
                checkCateIds = (cates) => 
                    {
                        foreach (var cate in cates)
                        { 
                             Console.WriteLine(cate.Title); 
                            if (cate.Id == category.ParentCategorySkillId)
                            {
                                canUpdate = false;
                                ModelState.AddModelError(string.Empty, "Phải chọn danh mục cha khácXX");
                                return true;
                            }
                            if (cate.CategorySkillChildren!=null)
                                return checkCateIds(cate.CategorySkillChildren.ToList());
                          
                        }
                        return false;
                    };
                // End Func 
                checkCateIds(childCates.ToList()); 
            }




            if (ModelState.IsValid && canUpdate)
            {
                try
                {
                    if (category.ParentCategorySkillId ==  -1)
                        category.ParentCategorySkillId = null;

                    var dtc = _context.CategorySkills.FirstOrDefault(c => c.Id == id);
                    _context.Entry(dtc).State = EntityState.Detached;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var qr = (from c in _context.CategorySkills select c)
                     .Include(c => c.ParentCategorySkill)
                     .Include(c => c.CategorySkillChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategorySkill == null)
                             .ToList();   
                             
            categories.Insert(0, new CategorySkill(){
                Id = -1,
                Title = "Không có danh mục cha"
            });
            var items = new List<CategorySkill>();
            CreateSelectItems(categories, items, 0);
            var selectList = new SelectList(items, "Id", "Title");

            ViewData["ParentCategoryId"] = selectList;


            return View(category);
        }

        // GET: Blog/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.CategorySkills
                .Include(c => c.ParentCategorySkill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Blog/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.CategorySkills
                           .Include(c => c.CategorySkillChildren)
                           .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }   

            foreach (var cCategory in category.CategorySkillChildren)
            {
                cCategory.ParentCategorySkillId = category.ParentCategorySkillId;
            }


            _context.CategorySkills.Remove(category);
            await _context.SaveChangesAsync();

            
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.CategorySkills.Any(e => e.Id == id);
        }
    }
}
