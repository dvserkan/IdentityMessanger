using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Newtonsoft.Json.Linq;
using X.PagedList;

namespace IdentityMessanger.Controllers
{
	public class MessageController : Controller
	{
		WriterMessageManager c = new WriterMessageManager(new EfWriterMessageDal());

		private readonly UserManager<WriterUser> _userManager;

		public MessageController(UserManager<WriterUser> userManager)
		{
			_userManager = userManager;
		}

		public IActionResult Index()
		{

			return View();
		}

		public async Task<IActionResult> ReceiverMessage(string p, int page = 1)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			p = values.Email;

			ViewBag.Gelen = c.GetListReceiverMessage(p).Where(x => x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = c.GetListSenderMessage(p).Where(x => x.Rubbish == false).Count();
			ViewBag.Cop = c.TGetlist().Where(x => x.Rubbish == true).Count();

			var messagelist = c.GetListReceiverMessage(p).Where(x => x.Rubbish == false).OrderBy(x=> x.Status).OrderByDescending(x=> x.Date).ToPagedList(page, 8);
			return View(messagelist);
		}


		public IActionResult SenderMessageRead(int id)
		{
			var values = c.TGetByID(id);
			values.Status = values.Status == true ? false : true;
			c.TUpdate(values);
			return Redirect("~/Message/ReceiverMessage/");
		}

		public async Task<IActionResult> SenderMessage(string p, int page = 1)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			p = values.Email;

			var messagelist = c.GetListSenderMessage(p).Where(x => x.Rubbish == false).OrderByDescending(x=> x.Date).ToPagedList(page, 8);

			ViewBag.Gelen = c.GetListReceiverMessage(p).Where(x => x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = c.GetListSenderMessage(p).Where(x => x.Rubbish == false).Count();
			ViewBag.Cop = c.TGetlist().Where(x => x.Rubbish == true).Count();

			return View(messagelist);
		}



		[HttpGet]
		public async Task<IActionResult> SenderMessageDetails(int id)
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context ca = new Context();
			ViewBag.Gelen = ca.WriterMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = ca.WriterMessages.Where(x => x.Sender == values.Email && x.Rubbish == false).Count();
			ViewBag.Cop = ca.WriterMessages.Where(x => x.Rubbish == true).Count();

			WriterMessage message = c.TGetByID(id);
			return View(message);
		}


		[HttpGet]
		public async Task<IActionResult> ReceiverMessageDetails(int id)
		{

			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context ca = new Context();
			ViewBag.Gelen = ca.WriterMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = ca.WriterMessages.Where(x => x.Sender == values.Email && x.Rubbish == false).Count();
			ViewBag.Cop = ca.WriterMessages.Where(x => x.Rubbish == true).Count();

			WriterMessage message = c.TGetByID(id);
			return View(message);
		}

		[HttpGet]
		public async Task<IActionResult> SendMessage()
		{
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			Context c = new Context();
			ViewBag.Gelen = c.WriterMessages.Where(x => x.Receiver == values.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = c.WriterMessages.Where(x => x.Sender == values.Email && x.Rubbish == false).Count();
			ViewBag.Cop = c.WriterMessages.Where(x => x.Rubbish == true).Count();

			List<SelectListItem> list = (from x in _userManager.Users
										 select new SelectListItem
										 {
											 Text = x.Email,
											 Value = x.Email.ToString()
										 }).ToList();
			ViewBag.mail = list;

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendMessage(WriterMessage p)
		{

			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			string mail = values.Email;

			string name = ($"{values.Name} {values.Surname}");
			p.Date = Convert.ToDateTime(DateTime.Now);
			p.Sender = mail;
			p.SenderName = name;
			p.Status = false;
			Context ca = new Context();
			var usernamesurname = ca.Users.Where(x => x.Email == p.Receiver).Select(y => y.Name + " " + y.Surname).FirstOrDefault();
			if (usernamesurname == null)
			{
				ModelState.AddModelError("", "Sistemde Kayıtlı Mail Adresi Bulunamadı");
			}
			else
			{
				p.ReceiverName = usernamesurname;
				c.TAdd(p);
                return Redirect("~/Message/SenderMessage/");
            }
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> RubbishMessage(int page = 1)
		{

			var valuese = await _userManager.FindByNameAsync(User.Identity.Name);
			Context ca = new Context();
			ViewBag.Gelen = ca.WriterMessages.Where(x => x.Receiver == valuese.Email && x.Status == false && x.Rubbish == false).Count();
			ViewBag.Giden = ca.WriterMessages.Where(x => x.Sender == valuese.Email && x.Rubbish == false).Count();
			ViewBag.Cop = c.TGetlist().Where(x => x.Rubbish == true).Count();

			var values = c.TGetlist().Where(x => x.Rubbish == true).ToPagedList(page, 8);


			return View(values);
		}


		public async Task<IActionResult> RubbishMessager(int id)
		{
			var valuese = await _userManager.FindByNameAsync(User.Identity.Name);

			var values = c.TGetByID(id);
			values.Rubbish = values.Rubbish == true ? false : true;

            c.TUpdate(values);
			if (values.Sender == valuese.Email)
			{
				return Redirect("~/Message/SenderMessage/");
			}
			else
			{
				return Redirect("~/Message/ReceiverMessage/");
			}
        }
	}
}
