﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestService.BindingModels;
using TestService.ViewModels;

namespace TestService.Interfaces
{
    public interface IStatService
    {
        Task<List<StatViewModel>> GetList(GetListModel model);

        Task<List<StatViewModel>> GetPatternList(int id);

        Task<List<StatViewModel>> GetUserList(GetListModel model);

        Task<StatChartViewModel> GetUserChart(GetListModel model);

        Task SaveToPdf(ReportBindingModel model);

        Task SaveToPdfAdmin(ReportBindingModel model);
    }
}
