﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
    [Authorize]
    [RoutePrefix("api/Pattern")]
    public class PatternController : ApiController
    {

        #region global
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

        private IPatternService _service;

        public IPatternService Service
        {
            get
            {
                return _service ?? PatternService.Create(Context);
            }
            private set
            {
                _service = value;
            }
        }
        #endregion

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.SuperAdmin + "," + ApplicationRoles.Admin)]
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
        public async Task<IHttpActionResult> GetUserList()
        {
            var list = await Service.GetUserList(User.Identity.GetUserId());
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoles.SuperAdmin + "," + ApplicationRoles.Admin)]
        public async Task<IHttpActionResult> GetGroupList(int id)
        {
            var list = await Service.GetGroupList(id);
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpPost]
        public async Task AddElement(PatternBindingModel model)
        {
            await Service.Add(model);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            var element = await Service.Get(id);
            if (element == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }

        [HttpPost]
        public async Task UpdElement(PatternBindingModel model)
        {
            await Service.Upd(model);
        }

        [HttpPost]
        public async Task DelElement(int id)
        {
            await Service.Del(id);
        }

        [HttpGet]
        public async Task<IHttpActionResult> CreateTest(int id)
        {
            var element = await Service.CreateTest(id);
            if (element == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CheakTest(TestResponseModel model)
        {
            model.UserId = User.Identity.GetUserId();
            var element = await Service.CheakTest(model);
            if (element == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }
    }
}
