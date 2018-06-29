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
    [RoutePrefix("api/Stat")]
    public class StatController : ApiController
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

        private IStatService _service;

        public IStatService Service
        {
            get
            {
                return _service ?? StatService.Create(Context);
            }
            private set
            {
                _service = value;
            }
        }
        #endregion

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.SuperAdmin + "," + ApplicationRoles.Admin)]
        [Route("GetList")]
        public async Task<IHttpActionResult> GetList(GetListModel model)
        {
            var list = await Service.GetList(model);
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }
         
        [HttpPost]
        [Route("GetUserList")]
        public async Task<IHttpActionResult> GetUserList(GetListModel model)
        {
            model.UserId = User.Identity.GetUserId();
            var list = await Service.GetUserList(model);
            if (list == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("GetUserChart")]
        public async Task<IHttpActionResult> GetUserChart(GetListModel model)
        {
            model.UserId = User.Identity.GetUserId();
            var element = await Service.GetUserChart(model);
            if (element == null || element.Results == null)
            {
                InternalServerError(new Exception("Нет данных"));
            }
            return Ok(element);
        }

        [HttpPost]
        public async Task SaveToPdf(ReportBindingModel model)
        {
            await Service.SaveToPdf(model);
        }
    }
}
