using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Admin.Pages
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class IndexModel : AbpPageModel
    {
        public decimal TotatOrderPrice { get; set; }
        public long TotalOrderCount { get; set; }
        public decimal TotalMoneyLastMonth { get; set; } = 0;
        public decimal TotalMoneyThisMonth { get; set; } = 0;
        public List<MoneyStatistics> MoneyStatisticsList { get; set; }
        private PagedAndSortedResultRequestDto input { get; set; } = new();
        private readonly IOrderAppService _orderAppService;

        public IndexModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
            MoneyStatisticsList = new List<MoneyStatistics>();
        }

        public async Task OnGetAsync()
        {
            var orders = await _orderAppService.GetListAsync(input);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            var lastYear = DateTime.Now.Year - 1;
            TotalOrderCount = orders.TotalCount;

            // Khởi tạo danh sách 12 tháng với giá trị 0
            InitializeMonthlyListWithYear(lastYear);

            // Lấy thông tin tháng hiện tại
            

            foreach (var order in orders.Items)
            {
                TotatOrderPrice += order.TotalAmount;

                // Tính tiền tháng trước
                if (order.OrderDate.Month == lastMonth && order.OrderDate.Year == lastMonthYear)
                {
                    TotalMoneyLastMonth += order.TotalAmount;
                }

                // Tính tiền tháng này
                if (order.OrderDate.Month == currentMonth && order.OrderDate.Year == currentYear)
                {
                    TotalMoneyThisMonth += order.TotalAmount;
                }

                // Cộng dồn tiền vào tháng tương ứng (chỉ tính năm hiện tại)
                if (order.OrderDate.Year == currentYear)
                {
                    var monthIndex = order.OrderDate.Month - 1; // Index 0-11
                    MoneyStatisticsList[monthIndex].MoneyTotal += order.TotalAmount;
                }
            }
        }

        // Khởi tạo danh sách 12 tháng
        //private void InitializeMonthlyList()
        //{
        //    MoneyStatisticsList = new List<MoneyStatistics>();
        //    var monthNames = new string[]
        //    {
        //    "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
        //    "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
        //    };

        //    for (int i = 0; i < 12; i++)
        //    {
        //        MoneyStatisticsList.Add(new MoneyStatistics
        //        {
        //            Month = monthNames[i],
        //            MoneyTotal = 0
        //        });
        //    }
        //}

        // Phương thức khác: Khởi tạo với tháng và năm
        private void InitializeMonthlyListWithYear(int year)
        {
            MoneyStatisticsList = new List<MoneyStatistics>();
            for (int month = 1; month <= 12; month++)
            {
                MoneyStatisticsList.Add(new MoneyStatistics
                {
                    Month = $"Tháng {month}/{year}",
                    MoneyTotal = 0
                });
            }
        }

        // Phương thức để lấy dữ liệu nhiều năm
        public async Task OnGetAsyncMultiYear()
        {
            var orders = await _orderAppService.GetListAsync(input);
            TotalOrderCount = orders.TotalCount;

            // Lấy các năm từ dữ liệu
            var years = orders.Items.Select(o => o.OrderDate.Year).Distinct().OrderBy(y => y).ToList();

            MoneyStatisticsList = new List<MoneyStatistics>();

            // Tạo danh sách cho tất cả tháng của tất cả năm
            foreach (var year in years)
            {
                for (int month = 1; month <= 12; month++)
                {
                    MoneyStatisticsList.Add(new MoneyStatistics
                    {
                        Month = $"Tháng {month}/{year}",
                        MoneyTotal = 0
                    });
                }
            }

            // Tính tổng tiền cho mỗi tháng
            foreach (var order in orders.Items)
            {
                TotatOrderPrice += order.TotalAmount;

                var monthKey = $"Tháng {order.OrderDate.Month}/{order.OrderDate.Year}";
                var monthStat = MoneyStatisticsList.FirstOrDefault(m => m.Month == monthKey);
                if (monthStat != null)
                {
                    monthStat.MoneyTotal += order.TotalAmount;
                }
            }
        }

        // Phương thức để lấy dữ liệu theo khoảng thời gian với danh sách cố định
        public async Task GetDataByDateRange(DateTime fromDate, DateTime toDate)
        {
            var orders = await _orderAppService.GetListAsync(input);

            MoneyStatisticsList = new List<MoneyStatistics>();

            // Tạo danh sách tháng trong khoảng thời gian
            var current = new DateTime(fromDate.Year, fromDate.Month, 1);
            var end = new DateTime(toDate.Year, toDate.Month, 1);

            while (current <= end)
            {
                MoneyStatisticsList.Add(new MoneyStatistics
                {
                    Month = $"Tháng {current.Month}/{current.Year}",
                    MoneyTotal = 0
                });
                current = current.AddMonths(1);
            }

            // Tính tổng tiền
            var filteredOrders = orders.Items.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate);

            foreach (var order in filteredOrders)
            {
                var monthKey = $"Tháng {order.OrderDate.Month}/{order.OrderDate.Year}";
                var monthStat = MoneyStatisticsList.FirstOrDefault(m => m.Month == monthKey);
                if (monthStat != null)
                {
                    monthStat.MoneyTotal += order.TotalAmount;
                }
            }
        }

        public class MoneyStatistics
        {
            public string Month { get; set; } = string.Empty;
            public decimal MoneyTotal { get; set; }
        }
    }

    // Trong Razor Page, bạn có thể truyền sang JavaScript như sau:
    /*
    @{
        var monthlyData = Json.Serialize(Model.MoneyStatisticsList);
    }

    <script>
        var monthlyStatistics = @Html.Raw(monthlyData);

        // Sử dụng trong Chart.js hoặc thư viện khác
        var chartLabels = monthlyStatistics.map(item => item.Month);
        var chartData = monthlyStatistics.map(item => item.MoneyTotal);

        console.log('Labels:', chartLabels);
        console.log('Data:', chartData);
    </script>
    */
}
