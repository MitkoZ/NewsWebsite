using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.News
{
    public class EditNewsViewModel : CreateNewsViewModel
    {
        /// <summary>
        /// Editing is the same as creating, just an id is added to the ViewModel.
        /// Since we are using partial views, we need a model of the same type.
        /// Practice shows that it's better to use inheritance in the ViewModels rather than composition 
        /// (at least for the ViewModels used as parameters of the action methods),
        /// since we may get strange result when some of the properties of a type ViewModel aren't instantiated which causes for example
        /// the removal of the already filled data from the View or not showing up validation errors
        /// when doing "if(!ModelState) {return View(viewModel)}")
        /// Another approach is to use a custom binder, but I think that using inheritance is easier
        /// (with inheritance the EditNewsViewModel is casted to CreateNewsViewModel).
        /// </summary>
        [Required]
        [HiddenInput]
        public string Id { get; set; }
    }
}
