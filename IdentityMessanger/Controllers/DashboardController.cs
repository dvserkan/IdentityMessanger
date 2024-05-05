using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace IdentityMessanger.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<WriterUser> _userManager;

        public DashboardController(UserManager<WriterUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.v = ($"{values.Name} {values.Surname}");

			//Weather APi
			string city = "istanbul";
            string api = "67db534b921c0d3eaffb2aefa96c322e";
            string api2 = "60b618984024156d0a007238e4396f6d";

            try
            {
                string connection = ($"http://api.openweathermap.org/data/2.5/weather?q={city}&mode=xml&lang=tr&units=metric&appid={api}");
                XDocument document = XDocument.Load(connection);
                ViewBag.v5 = document.Descendants("temperature").ElementAt(0).Attribute("value").Value;
            }
            catch (Exception)
            {
                return View();
            }

            //statistics
            Context c = new Context();
            ViewBag.v1 = c.WriterMessages.Where(x => x.Receiver == values.Email).Count();
            ViewBag.v3 = c.Users.Count();
			ViewBag.Gelen = c.WriterMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = c.WriterMessages.Where(x => x.Sender == values.Email && x.Rubbish == false).Count();
            ViewBag.Cop = c.WriterMessages.Where(x => x.Rubbish == true).Count();
            return View();
        }

		[HttpGet]
		public async Task<IActionResult> ProfileIndex()
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context c = new Context();
			ViewBag.Gelen = c.WriterMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = c.WriterMessages.Where(x => x.Sender == values.Email && x.Rubbish == false).Count();
            ViewBag.Cop = c.WriterMessages.Where(x => x.Rubbish == true).Count();
            UserEditViewModel model = new UserEditViewModel();
			model.Name = values.Name;
			model.Surname = values.Surname;
			model.PictureURL = values.ImageUrl;
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> ProfileIndex(UserEditViewModel p)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);

			if (p.Picture != null)
			{
				var resource = Directory.GetCurrentDirectory();
				var extansion = Path.GetExtension(p.Picture.FileName);
				var imagename = ($"{Guid.NewGuid()}{extansion}");
				var savelocation = ($"{resource}/wwwroot/userimage/{imagename}");
				var stream = new FileStream(savelocation, FileMode.Create);
				await p.Picture.CopyToAsync(stream);
				user.ImageUrl = imagename;
			}
			user.Name = p.Name;
			user.Surname = p.Surname;
			user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, p.Password);
			var result = await _userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Login");
			}
			return View();
		}


	}
}
