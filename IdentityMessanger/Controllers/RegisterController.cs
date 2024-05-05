using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMessanger.Controllers
{
	public class RegisterController : Controller
	{
		private readonly UserManager<WriterUser> _userManager;

		public RegisterController(UserManager<WriterUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View(new UserRegisterViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> Index(UserRegisterViewModel p)
		{

			var resource = Directory.GetCurrentDirectory();
			var extansion = Path.GetExtension(p.Picture.FileName);
			var imagename = ($"{Guid.NewGuid()}{extansion}");
			var savelocation = ($"{resource}/wwwroot/userimage/{imagename}");
			var stream = new FileStream(savelocation, FileMode.Create);
            await p.Picture.CopyToAsync(stream);

            

            WriterUser w = new WriterUser
			{
				Name = p.Name,
				Surname = p.Surname,
				Email = p.Mail,
				UserName = p.UserName,
				ImageUrl = imagename
			};

			if (p.Password == p.ConfirmPassword)
			{
				var result = await _userManager.CreateAsync(w, p.Password);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Login");
				}
				else
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError("", item.Description);
					}
				}
			}
			return RedirectToAction("Index","Login");

		}
	}
}
