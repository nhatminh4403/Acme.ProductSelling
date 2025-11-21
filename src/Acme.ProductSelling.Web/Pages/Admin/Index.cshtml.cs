using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Web.Pages.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Admin.Pages
{
    [Authorize]
    public class IndexModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 24;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        public decimal TotatOrderPrice { get; set; }
        public long TotalOrderCount { get; set; }
        public decimal TotalMoneyLastMonth { get; set; } = 0;
        public decimal TotalMoneyThisMonth { get; set; } = 0;
        public decimal TotalMoneyLastYear { get; set; } = 0;
        public List<MoneyStatistics> MoneyStatisticsList { get; set; }
        public Dictionary<int, List<MoneyStatistics>> YearlyStatistics { get; set; }
        public int SelectedYear { get; set; }
        public List<int> AvailableYears { get; set; } = new();
        private GetOrderListInput input { get; set; } = new();
        private readonly IOrderAppService _orderAppService;
        private readonly IBlogAppService _blogAppService;

        public PagedResultDto<BlogDto> Blogs { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        private readonly ICurrentUser _currentUser;
        public IndexModel(IOrderAppService orderAppService, IStringLocalizer<ProductSellingResource> stringLocalizer, ICurrentUser currentUser, IBlogAppService blogAppService)
        {
            _orderAppService = orderAppService;
            YearlyStatistics = new Dictionary<int, List<MoneyStatistics>>();
            MoneyStatisticsList = new List<MoneyStatistics>();
            _localizer = stringLocalizer;
            _currentUser = currentUser;
            _blogAppService = blogAppService;
        }

        public async Task OnGetAsync(int? year = null)
        {

            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}");
            }
            if (!_currentUser.IsInRole(IdentityRoleConsts.Blogger))
            {
                var orders = await _orderAppService.GetListAsync(input);
                var totalProfit = await _orderAppService.GetProfitReportAsync(input);
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                var lastYear = DateTime.Now.Year - 1;
                TotalOrderCount = orders.TotalCount;
                SelectedYear = year ?? currentYear;

                AvailableYears = Enumerable.Range(currentYear - 2, 3).ToList();
                InitializeThreeYearsData(currentYear);

                foreach (var order in totalProfit.Items)
                {
                    TotatOrderPrice += order.TotalAmount;

                    if (order.OrderDate.Month == lastMonth && order.OrderDate.Year == lastMonthYear)
                    {
                        TotalMoneyLastMonth += order.TotalAmount;
                    }

                    if (order.OrderDate.Month == currentMonth && order.OrderDate.Year == currentYear)
                    {
                        TotalMoneyThisMonth += order.TotalAmount;
                    }
                    if (order.OrderDate.Year == currentYear)
                    {
                        TotalMoneyLastYear += order.TotalAmount;
                    }
                    var orderYear = order.OrderDate.Year;
                    if (YearlyStatistics.ContainsKey(orderYear))
                    {
                        var monthIndex = order.OrderDate.Month - 1;
                        YearlyStatistics[orderYear][monthIndex].MoneyTotal += order.TotalAmount;
                    }
                }
                MoneyStatisticsList = YearlyStatistics.ContainsKey(SelectedYear)
                   ? YearlyStatistics[SelectedYear]
                   : new List<MoneyStatistics>();
            }
            else
            {
                var input = new PagedAndSortedResultRequestDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = "CreationTime DESC"
                };
                var blogs = await _blogAppService.GetListAsync(input);

                Blogs = new PagedResultDto<BlogDto>
                {
                    TotalCount = blogs.TotalCount,
                    Items = blogs.Items
                };
            }
        }
        private void InitializeThreeYearsData(int currentYear)
        {
            YearlyStatistics.Clear();
            for (int year = currentYear - 2; year <= currentYear; year++)
            {
                var monthlyData = new List<MoneyStatistics>();

                for (int month = 1; month <= 12; month++)
                {
                    var monthLocalizer = _localizer["Admin:Months:" + month];

                    monthlyData.Add(new MoneyStatistics
                    {
                        Month = $"{monthLocalizer}/{year}",
                        Year = year,
                        MonthNumber = month,
                        MoneyTotal = 0
                    });
                }

                YearlyStatistics[year] = monthlyData;
            }
        }

        public decimal GetTotalForYear(int year)
        {
            if (!YearlyStatistics.ContainsKey(year))
                return 0;

            return YearlyStatistics[year].Sum(m => m.MoneyTotal);
        }

        public class MoneyStatistics
        {
            public string Month { get; set; } = string.Empty;
            public int Year { get; set; }
            public int MonthNumber { get; set; }
            public decimal MoneyTotal { get; set; }
        }
    }
}
