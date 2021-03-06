﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using Mhotivo.Implement.Services;
using Mhotivo.Interface.Interfaces;
using Mhotivo.ParentSite.Authorization;
using Mhotivo.ParentSite.Models;

namespace Mhotivo.ParentSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly ISessionManagementService _sessionManagementService;
        private readonly ITutorRepository _tutorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordGenerationService _passwordGenerationService;

        public AccountController(ISessionManagementService sessionManagementService, ITutorRepository tutorRepository, IUserRepository userRepository, IPasswordGenerationService passwordGenerationService)
        {
            _sessionManagementService = sessionManagementService;
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
            _passwordGenerationService = passwordGenerationService;
        }

        // GET: /Account/
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogIn(TutorLoginModel model, string returnUrl)
        {
            var tutor = model.Email.Contains("@") ? _tutorRepository.Filter(y => y.User.Email == model.Email).FirstOrDefault()
                : _tutorRepository.Filter(y => y.IdNumber == model.Email).FirstOrDefault();

            if (tutor != null)
            {
                if (_sessionManagementService.LogIn(model.Email, model.Password))
                {
                    if (tutor.User.IsUsingDefaultPassword)
                    {
                        return RedirectToAction("ChangePassword");
                    }
                    return tutor.User.Email.Equals("") ? RedirectToAction("ConfirmEmail") : RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
                return View(model);
            }
            ModelState.AddModelError("", "El usuario ingresado no es un tutor");
            return View(model);
        }

        public ActionResult LogOut()
        {
            _sessionManagementService.LogOut();
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeNewUser]
        public ActionResult ConfirmEmail()
        {
            return View("EmailConfirmation");
        }

        [HttpPost]
        [AuthorizeNewUser]
        public ActionResult UpdateEmail(UpdateTutorMailModel model)
        {
            var userId = Convert.ToInt64(_sessionManagementService.GetUserLoggedId());
            var tutorUser = _tutorRepository.Filter(x => x.User.Id == userId).Include(x => x.User).FirstOrDefault();
            
            if (tutorUser != null)
            {
                var user = tutorUser.User;
                user.Email = model.Email;
                _userRepository.Update(user);
                return RedirectToAction("Index", "Notification");
            }

            return RedirectToAction("ConfirmEmail");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var userId = Convert.ToInt32(_sessionManagementService.GetUserLoggedId());
            var user = _userRepository.GetById(userId);
            user.Password = model.NewPassword;
            user.HashPassword();
            user.DefaultPassword = null;
            user.IsUsingDefaultPassword = false;
            _userRepository.Update(user);
            return RedirectToAction("Index", "Home");
        }

        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RecoverPassword()
        {
            return View(new LostPasswordModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecoverPassword(LostPasswordModel model)
        {
            var user = _userRepository.Filter(x => x.Email.Equals(model.Email)).FirstOrDefault();
            if (user == null) return RedirectToAction("Index", "Home");
            var password = _passwordGenerationService.GenerateTemporaryPassword();
            user.Password = password;
            user.HashPassword();
            user.DefaultPassword = user.Password;
            user.IsUsingDefaultPassword = true;
            _userRepository.Update(user);
            MailgunEmailService.SendEmailToUser(user,MessageService.ChangePasswordMessage(password));
            return RedirectToAction("LogIn", "Account");
        }
    }
}
