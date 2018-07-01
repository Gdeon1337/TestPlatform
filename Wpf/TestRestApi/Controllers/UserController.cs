﻿using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TestService;
using TestService.BindingModels;
using TestService.Implementations;
using TestService.Interfaces;

namespace TestRestApi.Controllers
{
    [Authorize(Roles = ApplicationRoles.SuperAdmin + "," + ApplicationRoles.Admin)]
    public class UserController : ApiController
    {
        #region global

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationDbContext _context;

        public ApplicationDbContext Context
        {
            get
            {
                return _context ?? HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _context = value;
            }
        }

        private IUserService _service;

        public IUserService Service
        {
            get
            {
                return _service ?? UserService.Create(Context, UserManager);
            }
            private set
            {
                _service = value;
            }
        }
        #endregion

        [HttpPost]
        public async Task AddElement(UserBindingModel model)
        {
            await Service.AddElement(model);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetList()
        {
            var list = await Service.GetList();
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            var element = await Service.Get(id);
            if (element == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }

        [HttpPost]
        public async Task UpdElement(UserBindingModel model)
        {
            await Service.UpdElement(model);
        }

        [HttpPost]
        public async Task DelElement(string id)
        {
            await Service.DelElement(id);
        }

        [HttpPost]
        public async Task SetGroup(UserBindingModel model)
        {
            await Service.SetGroup(model);
        }
    }
}
