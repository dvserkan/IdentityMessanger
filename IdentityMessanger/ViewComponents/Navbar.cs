using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMessanger.ViewComponents
{
	public class Navbar : ViewComponent
	{
		private readonly UserManager<WriterUser> _userManager;

		public Navbar(UserManager<WriterUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.v = values.ImageUrl;

			return View();
		}
	}
}
