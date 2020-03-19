using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.ViewModels.News;
using Services.CRUD.Interfaces;

namespace NewsWebsite.Controllers
{
    public class NewsController : BaseController
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService, ILogger<NewsController> logger) : base(logger)
        {
            this.newsService = newsService;
        }

        [HttpGet]
        public IActionResult ListAll()
        {
            List<News> newsDbCollection = this.newsService.GetAll().ToList();
            List<DetailsNewsViewModel> detailsNewsViewModels = new List<DetailsNewsViewModel>();

            foreach (News newsDb in newsDbCollection)
            {
                DetailsNewsViewModel listNewsViewModel = new DetailsNewsViewModel
                {
                    Id = newsDb.Id,
                    Title = newsDb.Title,
                    Content = newsDb.Content,
                    ReporterName = newsDb.User.UserName,
                    CreatedAt = newsDb.CreatedAt,
                    UpdatedAt = newsDb.UpdatedAt,
                    ReporterId = newsDb.UserId

                };

                detailsNewsViewModels.Add(listNewsViewModel);
            }

            return View(detailsNewsViewModels);
        }

        [HttpGet]
        public IActionResult GetDetails(string id)
        {
            News newsDb = newsService.GetAll(x => x.Id == id).FirstOrDefault();
            if (newsDb == null)
            {
                ModelState.AddModelError("", "A news with this id doesn't exist!");
                return View("ValidationErrorsWithoutSpecificModel");
            }

            DetailsNewsViewModel detailsNewsViewModel = new DetailsNewsViewModel
            {
                Content = newsDb.Content,
                Title = newsDb.Title,
                ReporterName = newsDb.User.UserName,
                CreatedAt = newsDb.CreatedAt,
                UpdatedAt = newsDb.UpdatedAt,
            };

            return View(detailsNewsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateNewsViewModel createNewsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createNewsViewModel);
            }

            if (this.newsService.GetAll(x => x.Title == createNewsViewModel.Title).FirstOrDefault() != null)
            {
                string titleName = nameof(createNewsViewModel.Title);
                this.ModelState.AddModelError(titleName, string.Format("A news with this {0} already exists!", titleName));
            }

            News newsDb = new News
            {
                Title = createNewsViewModel.Title,
                Content = createNewsViewModel.Content,
                UserId = base.GetCurrentUserId()
            };

            bool isSaved = await newsService.SaveAsync(newsDb);
            if (!isSaved)
            {
                ViewBag.ErrorMessage = "Ooops, something went wrong";
                return View(createNewsViewModel);
            }

            TempData["SuccessMessage"] = "News created successfully!";
            return RedirectToIndexActionInHomeController();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            News newsDb = this.newsService.GetAll(x => x.Id == id).FirstOrDefault();
            if (newsDb == null)
            {
                this.AddValidationErrorsToModelState(new List<string> { "A news with this id doesn't exist" });
                return View("ValidationErrorsWithoutSpecificModel");
            }

            EditNewsViewModel editNewsViewModel = new EditNewsViewModel
            {
                Id = newsDb.Id,
                Title = newsDb.Title,
                Content = newsDb.Content
            };

            return View(editNewsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(EditNewsViewModel editNewsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editNewsViewModel);
            }

            News newsDb = this.newsService.GetAll(x => x.Id == editNewsViewModel.Id).FirstOrDefault();

            if (newsDb == null)
            {
                this.AddValidationErrorsToModelState(new List<string> { "A news with this id doesn't exist" });
                return View("ValidationErrorsWithoutSpecificModel");
            }

            newsDb.Title = editNewsViewModel.Title;
            newsDb.Content = editNewsViewModel.Content;

            bool isSaved = await this.newsService.SaveAsync(newsDb);
            if (!isSaved)
            {
                ViewBag.ErrorMessage = "Ooops, something went wrong";
                return View(editNewsViewModel);
            }

            TempData["SuccessMessage"] = "News edited successfully!";
            return RedirectToIndexActionInHomeController();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            bool isDeleted = await this.newsService.DeleteAsync(id);

            if (isDeleted)
            {
                TempData["SuccessMessage"] = "News deleted successfully!";
                return RedirectToIndexActionInHomeController();
            }

            ModelState.AddModelError("", "Ooops, something went wrong");
            return View("ValidationErrorsWithoutSpecificModel");
        }
    }
}