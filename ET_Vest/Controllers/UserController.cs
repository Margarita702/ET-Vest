using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ET_Vest.Data;
using ET_Vest.Models;

namespace ET_Vest.Controllers
{
    [Authorize(Roles = "Owner")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public UserController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ApplicationUser currentUser = _userManager.GetUserAsync(User).Result;
            List<ApplicationUser> allUsers = _userManager.Users.ToList();
            List<ApplicationUser> otherUsers = allUsers.Where(u => u.Id != currentUser.Id).ToList();

            return View(otherUsers);
        }


        // Action to show a form to create a new user
        public IActionResult CreateUser()
        {
            // Populate a dropdown with roles (e.g., Admin, Employee)
            ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != Roles.Owner.ToString()).ToList();
            return View();
        }

        // Action to handle form submission to create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add the selected role to the user
                    var role = await _roleManager.FindByIdAsync(model.RoleId);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }

                    return RedirectToAction("Index", "Home"); // Redirect to a suitable location
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If ModelState is not valid, redisplay the form
            ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != Roles.Owner.ToString()).ToList();
            return View(model);
        }

        // Inside UsersController class
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _dbContext.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}