using Dios.Extensions;
using Dios.Models;
using Dios.Repositories;
using Dios.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dios.Controllers
{
    [Authorize(Roles = "User")]
    public class ParameterController : Controller
    {
        private IParametersRepository _parameterRepository;
        IFlatsRepository _flatsRepository;

        public ParameterController(IParametersRepository parameterRepository, IFlatsRepository flatsRepository)
        {
            _parameterRepository = parameterRepository;
            _flatsRepository = flatsRepository;
        }

        [HttpGet]
        [Route("[controller]/[action]/{flatId}")]
        public IActionResult Edit(int flatId)
        {
            var lgh = _flatsRepository.Flat(flatId);

            if (lgh == null)
            {
                return NotFound();
            }

            string userId = User.Id();
            ParameterDTO parameter = _parameterRepository.Get(userId, flatId);

            if (parameter == null)
            {
                return NotFound();
            }

            var model = new ParameterEditVM
            {
                UserId = userId,
                FlatID = flatId,
                IsEmailVisible = parameter.IsEmailVisible,
                IsPhoneNumberVisible = parameter.IsPhoneNumberVisible,
                CanBeContacted = parameter.CanBeContacted
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ParameterEditVM model)
        {
            ParameterDTO parameter = _parameterRepository.Get(model.UserId, model.FlatID);

            if (parameter == null)
            {
                return NotFound();
            }

            parameter.CanBeContacted = model.CanBeContacted;
            parameter.IsEmailVisible = model.IsEmailVisible;
            parameter.IsPhoneNumberVisible = model.IsPhoneNumberVisible;

            _parameterRepository.Edit(parameter);

            return RedirectToAction(nameof(UsersController.Flats), "Users");
        }
    }
}