using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Acme.ProductSelling.Chatbot
{
    public class ChatbotManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        public ChatbotManager(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }
        public async Task<List<Product>> SearchProductsAsync(string query, int maxResults = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            var queryable = await _productRepository.GetQueryableAsync();

            var searchTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Search in product name, description, category, and manufacturer
            var products = await queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.LaptopSpecification)
                .Include(p => p.MonitorSpecification).ThenInclude(m => m.PanelType)
                .Include(p => p.CpuSpecification).ThenInclude(c => c.Socket)
                .Include(p => p.GpuSpecification)
                .Include(p => p.KeyboardSpecification).ThenInclude(k => k.SwitchType)
                .Include(p => p.MouseSpecification)
                .Include(p => p.MotherboardSpecification)
                .Include(p => p.RamSpecification).ThenInclude(r => r.RamType)
                .Include(p => p.StorageSpecification)
                .Include(p => p.CaseSpecification).ThenInclude(c => c.FormFactor)
                .Include(p => p.PsuSpecification).ThenInclude(ps => ps.FormFactor)
                .Include(p => p.HeadsetSpecification)
                .Include(p => p.ChairSpecification)
                .Include(p => p.DeskSpecification)
                .Include(p => p.SpeakerSpecification)
                .Include(p => p.MicrophoneSpecification)
                .Include(p => p.WebcamSpecification)
                .Include(p => p.SoftwareSpecification)
                .Include(p => p.NetworkHardwareSpecification)

                .Where(p => searchTerms.Any(term =>
                    p.ProductName.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term) ||
                    p.Manufacturer.Name.ToLower().Contains(term)
                ))
                .OrderByDescending(p => p.IsActive)
                .ThenByDescending(p => p.StockCount > 0)
                .Take(maxResults)
                .ToListAsync();

            return products;
        }

        public bool IsProductQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            var productKeywords = new[]
            {
                "product", "buy", "price", "cost", "laptop", "mouse", "keyboard",
                "monitor", "cpu", "gpu", "ram", "storage", "available", "stock",
                "purchase", "gaming", "computer", "pc", "specifications", "specs"
            };

            var lowerQuery = query.ToLower();
            return productKeywords.Any(keyword => lowerQuery.Contains(keyword));
        }
        public string GetRoleKnowledgeBase(string userRole)
        {
            switch (userRole)
            {
                case "sales_agent": // For Seller and Cashier
                    return @"
                === EMPLOYEE HANDBOOK: SALES & RETURNS ===
                1. RETURN POLICY: Items can be returned within 30 days if receipt is present. 
                   Exceptions: Opened software and hygiene products cannot be returned.
                2. DISCOUNT AUTHORITY: You are authorized to give 5% off for damage. 
                   You can match competitor prices if verified on a URL.
                3. CASHIER COMMANDS: If a user asks to 'checkout', remind them to use the POS system, not the chatbot.
            ";

                case "warehouse":
                    return @"
                === WAREHOUSE SAFETY PROTOCOLS ===
                1. Heavy items (>10kg) require two-person lift.
                2. Fragile items are marked with Red Tape.
            ";

                case "blogger":
                    return @"
                === BRAND VOICE GUIDELINES ===
                1. Use energetic, tech-savvy language.
                2. Avoid passive voice.
                3. Always include SEO keywords related to the category.
            ";

                default:
                    return "";
            }
        }
        public string BuildProductContext(List<Product> products, string userRole)
        {
            if (!products.Any()) return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"DATABASE SEARCH RESULTS ({products.Count} found):\n");

            foreach (var p in products)
            {
                var price = p.DiscountedPrice ?? p.OriginalPrice;
                sb.AppendLine($"### {p.ProductName}");
                sb.AppendLine($"   - ID: {p.UrlSlug}"); // Used for linking if needed
                sb.AppendLine($"   - Category: {p.Category?.Name} | Manufacturer: {p.Manufacturer?.Name}");
                sb.AppendLine($"   - Price: {price:C}");
                if (p.DiscountPercent > 0) sb.AppendLine($"   - Deal: {p.DiscountPercent}% Off!");

                // 1. Role-based info (Reusing your previous logic)
                if (userRole == "sales_agent" || userRole == "manager")
                {
                    sb.AppendLine($"   - Stock Total: {p.StockCount}");
                    if (p.StoreInventories != null)
                    {
                        var locs = string.Join(", ", p.StoreInventories.Where(i => i.Quantity > 0 && i.IsAvailableForSale).Select(i => $"Store {i.StoreId}"));
                        sb.AppendLine($"   - Locations: {(string.IsNullOrEmpty(locs) ? "Central Warehouse only" : locs)}");
                    }
                }
                else
                {
                    sb.AppendLine($"   - Availability: {(p.IsAvailableForPurchase() ? "In Stock" : "Unavailable")}");
                }

                // 2. TECH SPECS INJECTION
                // Based on your Spec classes
                string specs = GetTechnicalDetails(p);
                if (!string.IsNullOrWhiteSpace(specs))
                {
                    sb.AppendLine("   - TECHNICAL SPECS:");
                    sb.AppendLine(specs); // Pre-formatted
                }

                sb.AppendLine(""); // Spacer
            }

            return sb.ToString();
        }

        public async Task<string> GetSalesSummaryJsonAsync()
        {
            // 1. Setup Time Ranges
            var now = DateTime.Now;
            var yesterdayStart = now.Date.AddDays(-1);
            var yesterdayEnd = now.Date;
            var thirtyDaysAgo = now.Date.AddDays(-30);

            // Get Queryables
            var productsQuery = await _productRepository.GetQueryableAsync();
            var ordersQuery = await _orderRepository.GetQueryableAsync();

            // ---------------------------------------------
            // 2. Metrics Logic
            // ---------------------------------------------

            // A. Active Products Count
            var activeProductsCount = await productsQuery.CountAsync(p => p.IsActive && p.StockCount > 0);

            // B. Revenue Yesterday (Paid Only)
            // We sum TotalAmount for orders placed yesterday that are Paid
            var revenueYesterday = await ordersQuery
                .Where(o => o.OrderDate >= yesterdayStart && o.OrderDate < yesterdayEnd)
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.TotalAmount);

            // C. Current Active Orders (Orders currently being processed)
            var processingOrdersCount = await ordersQuery
                .CountAsync(o => o.OrderStatus == OrderStatus.Processing ||
                                 o.OrderStatus == OrderStatus.Placed ||
                                 o.OrderStatus == OrderStatus.Confirmed);

            // ---------------------------------------------
            // 3. Top Selling Category (The Complex Part)
            // ---------------------------------------------
            // Strategy: 
            // 1. Look at non-cancelled orders from last 30 days.
            // 2. Flatten OrderItems.
            // 3. Join with Products to find the Category.
            // 4. Group by CategoryName and Sum the Quantities.

            var topCategoryStats = await (
                from o in ordersQuery
                where o.OrderDate >= thirtyDaysAgo && o.OrderStatus != OrderStatus.Cancelled
                from oi in o.OrderItems
                join p in productsQuery on oi.ProductId equals p.Id
                group oi.Quantity by p.Category.Name into g
                select new
                {
                    CategoryName = g.Key,
                    TotalUnitsSold = g.Sum()
                })
                .OrderByDescending(x => x.TotalUnitsSold)
                .FirstOrDefaultAsync();

            string topCategoryName = topCategoryStats != null
                ? $"{topCategoryStats.CategoryName} ({topCategoryStats.TotalUnitsSold} units sold last 30 days)"
                : "No sales data recently";

            // ---------------------------------------------
            // 4. Alerts Logic (Fetch-Then-Think Context)
            // ---------------------------------------------
            var alerts = new List<string>();

            // A. Low Stock Alert (Threshold < 5)
            var lowStockProducts = await productsQuery
                .Where(p => p.IsActive && p.StockCount < 5 && p.StockCount > 0)
                .OrderBy(p => p.StockCount)
                .Take(5)
                .Select(p => $"{p.ProductName} ({p.StockCount} left)")
                .ToListAsync();

            if (lowStockProducts.Any())
            {
                alerts.Add($"CRITICAL: Low Stock on: {string.Join(", ", lowStockProducts)}");
            }

            // B. High Pending Orders Alert
            if (processingOrdersCount > 20) // Threshold arbitrary
            {
                alerts.Add($"WARNING: High backlog. {processingOrdersCount} orders currently in process.");
            }

            // C. Recent High Value Orders (Whales)
            var whaleOrder = await ordersQuery
                .Where(o => o.OrderDate >= yesterdayStart)
                .OrderByDescending(o => o.TotalAmount)
                .Select(o => new { o.OrderNumber, o.TotalAmount })
                .FirstOrDefaultAsync();

            if (whaleOrder != null && whaleOrder.TotalAmount > 2000) // Example threshold $2000
            {
                alerts.Add($"OPPORTUNITY: Big order placed yesterday: #{whaleOrder.OrderNumber} (${whaleOrder.TotalAmount:N0})");
            }

            if (!alerts.Any()) alerts.Add("Systems normal. No critical alerts.");

            // ---------------------------------------------
            // 5. Construct & Serialize
            // ---------------------------------------------
            var summary = new
            {
                Date = now.ToString("f"), // Full date/time string
                Metrics = new
                {
                    ProductsInCatalog = activeProductsCount,
                    RevenueYesterday = revenueYesterday,
                    PendingOrdersQueue = processingOrdersCount,
                    TopTrend = topCategoryName
                },
                Alerts = alerts
            };

            return System.Text.Json.JsonSerializer.Serialize(summary);
        }

        private string GetTechnicalDetails(Product p)
        {
            var sb = new StringBuilder();

            // Check Laptop
            if (p.LaptopSpecification != null)
            {
                var s = p.LaptopSpecification;
                sb.AppendLine($"      * CPU: {s.CPU} | GPU: {s.GraphicsCard}");
                sb.AppendLine($"      * RAM: {s.RAM} | Storage: {s.Storage}");
                sb.AppendLine($"      * Screen: {s.Display}");
                sb.AppendLine($"      * Battery: {s.BatteryLife} | Weight: {s.Weight}");
                sb.AppendLine($"      * Gaming Ready: {(s.IsForGaming ? "YES" : "No")}");
            }

            // Check Monitor
            if (p.MonitorSpecification != null)
            {
                var s = p.MonitorSpecification;
                sb.AppendLine($"      * Spec: {s.ScreenSize}\" {s.PanelType?.Name} Panel");
                sb.AppendLine($"      * Resolution: {s.Resolution} @ {s.RefreshRate}Hz");
                sb.AppendLine($"      * Response: {s.ResponseTimeMs}ms");
                sb.AppendLine($"      * Gamut: {s.ColorGamut} | VESA: {(s.VesaMount ? "Yes" : "No")}");
            }

            // Check Components (CPU/GPU/Ram)
            if (p.CpuSpecification != null)
            {
                var s = p.CpuSpecification;
                sb.AppendLine($"      * Socket: {s.Socket?.Name}");
                sb.AppendLine($"      * Cores: {s.CoreCount}C / {s.ThreadCount}T");
                sb.AppendLine($"      * Clock: Base {s.BaseClock}GHz / Boost {s.BoostClock}GHz");
                sb.AppendLine($"      * TDP: {s.Tdp}W | iGPU: {s.HasIntegratedGraphics}");
            }

            if (p.GpuSpecification != null)
            {
                var s = p.GpuSpecification;
                sb.AppendLine($"      * Chipset: {s.Chipset}");
                sb.AppendLine($"      * VRAM: {s.MemorySize}GB {s.MemoryType}");
                sb.AppendLine($"      * Interface: {s.Interface} | Rec. PSU: {s.RecommendedPsu}W");
                sb.AppendLine($"      * Length: {s.Length}mm (Check Case clearance)");
            }

            // Check Peripherals (Keyboard/Mouse/Headset)
            if (p.KeyboardSpecification != null)
            {
                var s = p.KeyboardSpecification;
                sb.AppendLine($"      * Type: {s.KeyboardType} ({s.Layout})");
                sb.AppendLine($"      * Switches: {s.SwitchType?.Name}");
                sb.AppendLine($"      * Connectivity: {s.Connectivity}");
            }

            if (p.MouseSpecification != null)
            {
                var s = p.MouseSpecification;
                sb.AppendLine($"      * DPI: {s.Dpi} | Sensor: {s.SensorType}");
                sb.AppendLine($"      * Weight: {s.Weight}g | Polling: {s.PollingRate}Hz");
            }

            if (p.ChairSpecification != null)
            {
                var s = p.ChairSpecification;
                sb.AppendLine($"      * Material: {s.Material} | Base: {s.BaseType}");
                sb.AppendLine($"      * Load Limit: {s.MaxWeight}kg");
                sb.AppendLine($"      * Adjustments: Armrests {s.ArmrestType}, Backrest {s.BackrestAdjustment}");
            }

            // Generic fallbacks for smaller accessories (using null coalescing extensively)
            if (p.WebcamSpecification != null) sb.AppendLine($"      * Res: {p.WebcamSpecification.Resolution} | FOV: {p.WebcamSpecification.FieldOfView}° | Focus: {p.WebcamSpecification.FocusType}");
            if (p.SpeakerSpecification != null) sb.AppendLine($"      * Type: {p.SpeakerSpecification.SpeakerType} | Watts: {p.SpeakerSpecification.TotalWattage}W | BT: {p.SpeakerSpecification.HasBluetooth}");

            return sb.ToString();
        }
    }
}
