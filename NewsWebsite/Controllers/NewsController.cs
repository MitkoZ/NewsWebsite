using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.ViewModels.News;
using Services.CRUD.Interfaces;
using NewsWebsite.Utils;
using Services.Transactions.Interfaces;
using NewsWebsite.Auth;
using Microsoft.AspNetCore.Authorization;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;

namespace NewsWebsite.Controllers
{
    public class NewsController : BaseViewsController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly INewsService newsService;

        public NewsController(IUnitOfWork unitOfWork, INewsService newsService)
        {
            this.unitOfWork = unitOfWork;
            this.newsService = newsService;
        }

        [HttpGet]
        public IActionResult Index(string filter, int pageindex = 1, string sortExpression = nameof(DetailsNewsViewModel.Title))
        {
            IQueryable<News> newsDbCollection = this.newsService.GetAll();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                newsDbCollection = newsDbCollection.Where(x => x.Title.Contains(filter));
            }

            List<DetailsNewsViewModel> detailsNewsViewModels = new List<DetailsNewsViewModel>();

            foreach (News newsDb in newsDbCollection.ToList())
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

            PagingList<DetailsNewsViewModel> pagingListViewModels = PagingList.Create(detailsNewsViewModels, 3, pageindex, sortExpression, nameof(DetailsNewsViewModel.Title));
            pagingListViewModels.RouteValue = new RouteValueDictionary {
                { "filter", filter }
            };

            return View(pagingListViewModels);
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
                Id = newsDb.Id,
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
            return View(new CreateNewsViewModel()); // Use the overload with the empty ViewModel, or our Model in the view will be null
        }

        [HttpPost]
        [Authorize]
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
                return View(createNewsViewModel);
            }

            News newsDb = new News
            {
                Title = createNewsViewModel.Title,
                Content = createNewsViewModel.Content,
                UserId = this.GetCurrentUserId()
            };

            newsService.Save(newsDb);
            bool isSaved = await unitOfWork.CommitAsync();

            if (!isSaved)
            {
                TempData["ErrorMessage"] = "Ooops, something went wrong";
                return View(createNewsViewModel);
            }

            TempData["SuccessMessage"] = "News created successfully!";
            return RedirectToListAllActionInCurrentController();
        }

        [HttpGet]
        [ItemOwnerAuthorize(typeof(INewsService), Roles = RoleConstants.Administrator + RoleConstants.Delimiter + RoleConstants.Reporter)]
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
        [ItemOwnerAuthorize(typeof(INewsService), idArgumentName: nameof(EditNewsViewModel.Id), Roles = RoleConstants.Administrator + RoleConstants.Delimiter + RoleConstants.Reporter)]
        public async Task<IActionResult> EditAsync(EditNewsViewModel editNewsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editNewsViewModel);
            }

            News otherNewsDbWithSameTitle = this.newsService.GetAll(x => x.Id != editNewsViewModel.Id && x.Title == editNewsViewModel.Title).FirstOrDefault();
            if (otherNewsDbWithSameTitle != null)
            {
                string titleName = nameof(editNewsViewModel.Title);
                this.ModelState.AddModelError(titleName, string.Format("A news with this {0} already exists!", titleName));
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

            this.newsService.Save(newsDb);
            bool isSaved = await this.unitOfWork.CommitAsync();
            if (!isSaved)
            {
                TempData["ErrorMessage"] = "Ooops, something went wrong";
                return View(editNewsViewModel);
            }

            TempData["SuccessMessage"] = "News edited successfully!";
            return RedirectToListAllActionInCurrentController();
        }

        [HttpPost]
        [ItemOwnerAuthorize(typeof(INewsService), Roles = RoleConstants.Administrator + RoleConstants.Delimiter + RoleConstants.Reporter)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            this.newsService.Delete(id);
            bool isDeleted = await this.unitOfWork.CommitAsync();

            if (isDeleted)
            {
                TempData["SuccessMessage"] = "News deleted successfully!";
                return RedirectToListAllActionInCurrentController();
            }

            ModelState.AddModelError("", "Ooops, something went wrong");
            return View("ValidationErrorsWithoutSpecificModel");
        }
    }
}