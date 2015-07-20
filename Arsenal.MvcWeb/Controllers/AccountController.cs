﻿using System;
using System.Web.Mvc;
using System.Web.Security;

using Arsenal.MvcWeb.Models;

namespace Arsenal.MvcWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Index

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                object userKey;

                if (MembershipDto.ValidateUser(model.UserName, model.Password, providerUserKey: out userKey))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    UserDto.SetSession(userKey);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        TempData["DataUrl"] = string.Format("data-url={0}", returnUrl);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        TempData["DataUrl"] = "data-url=/";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (MembershipDto.ValidateAcnUser(model.UserName, model.Password))
                {
                    // not in SSO, but in Acn Users
                    // Sync the user info, register SSO and then log in
                    var acnUid = MembershipDto.GetAcnID(model.UserName);

                    if (acnUid > 0)
                    {
                        var user = new MembershipDto();

                        MembershipCreateStatus createStatus;
                        user.CreateAcnUser(acnUid, providerUserKey: out userKey, status: out createStatus);

                        if (createStatus.Equals(MembershipCreateStatus.Success))
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                            UserDto.SetSession(userKey);

                            TempData["DataUrl"] = "data-url=/";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("Warn", ErrorCodeToString(createStatus));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Warn", ErrorCodeToString(MembershipCreateStatus.InvalidUserName));
                    }
                }
                else
                {
                    ModelState.AddModelError("Warn", "用户名或密码不正确");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            TempData["DataUrl"] = "data-url=/";
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var user = new MembershipDto();

                object userKey;
                MembershipCreateStatus createStatus;

                user.CreateUser(model.UserName, model.Password, model.Mobile, model.Email,
                    providerUserKey: out userKey, status: out createStatus);

                if (createStatus.Equals(MembershipCreateStatus.Success))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    UserDto.SetSession(userKey);

                    TempData["DataUrl"] = "data-url=/";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Warn", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;

                try
                {
                    var membership = MembershipDto.GetMembership(username: User.Identity.Name);

                    if (membership != null)
                    {
                        changePasswordSucceeded = MembershipDto.ChangePassword(membership, model.OldPassword, model.NewPassword);
                    }
                    else
                    {
                        changePasswordSucceeded = false;
                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    TempData["DataUrl"] = "data-url=/";
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("Warn", "旧密码不正确或新密码无效");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "此用户名已存在";

                case MembershipCreateStatus.DuplicateEmail:
                    return "此邮箱已被其他用户注册使用";

                case MembershipCreateStatus.InvalidPassword:
                    return "此密码不符合密码规则";

                case MembershipCreateStatus.InvalidEmail:
                    return "此邮箱地址无效";

                case MembershipCreateStatus.InvalidAnswer:
                    return "此密码问题答案无效";

                case MembershipCreateStatus.InvalidQuestion:
                    return "此密码问题无效";

                case MembershipCreateStatus.InvalidUserName:
                    return "此用户名无效";

                case MembershipCreateStatus.ProviderError:
                    return "注册过程出错，请联系管理员";

                case MembershipCreateStatus.UserRejected:
                    return "此申请被取消";

                default:
                    return "发生未知错误，请联系管理员";
            }
        }
        #endregion
    }
}