using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Admin.Pages
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class IndexModel : AbpPageModel
    {
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
        private PagedAndSortedResultRequestDto input { get; set; } = new();
        private readonly IOrderAppService _orderAppService;

        public IndexModel(IOrderAppService orderAppService, IStringLocalizer<ProductSellingResource> stringLocalizer)
        {
            _orderAppService = orderAppService;
            YearlyStatistics = new Dictionary<int, List<MoneyStatistics>>();
            MoneyStatisticsList = new List<MoneyStatistics>();
            _localizer = stringLocalizer;
        }

        public async Task OnGetAsync(int? year = null)
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
