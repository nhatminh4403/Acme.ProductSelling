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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<List<Product>> SearchProductsAsync(string query, int maxResults = 3)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Product>();
            }

            var queryable = await _productRepository.GetQueryableAsync();

            var productQuery = queryable
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
                .AsQueryable();

            var rawTerms = query.ToLower().Split((char[])null, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            var laptopKeywords = new[] { "laptop", "notebook", "máy tính xách tay" };
            var gamingKeywords = new[] { "gaming", "game", "chơi game", "gamer" };
            var mouseKeywords = new[] { "mouse", "chuột" };
            var ignoredWords = new[] { "buy", "find", "search", "mua", "tìm", "giá", "product" };

            bool wantsLaptop = rawTerms.Any(t => laptopKeywords.Any(k => t.Contains(k)));
            bool wantsGaming = rawTerms.Any(t => gamingKeywords.Any(k => t.Contains(k)));
            bool wantsMouse = rawTerms.Any(t => mouseKeywords.Any(k => t.Contains(k)));

            bool wantsPad = rawTerms.Any(t => t.Contains("pad") || t.Contains("lót"));

            if (wantsGaming)
            {
                // Must be IsForGaming (if it has specs) 
                // OR a generic gaming item (if category contains 'gaming' - optional backup)
                productQuery = productQuery.Where(p =>
                    (p.LaptopSpecification != null && p.LaptopSpecification.IsForGaming) ||
                    // Fallback for non-laptop gaming items (mouse, etc):
                    (p.LaptopSpecification == null)
                );
            }
            if (wantsLaptop)
            {
                productQuery = productQuery.Where(p => p.Category.Name.ToLower().Contains("laptop")
                                                    || p.LaptopSpecification != null);
            }

            if (wantsMouse && !wantsPad)
            {
                productQuery = productQuery.Where(p =>
                    !p.ProductName.ToLower().Contains("pad") &&
                    !p.ProductName.ToLower().Contains("lót") &&
                    !p.Category.Name.ToLower().Contains("pad")
                );
            }
            var allKeywordsToRemove = laptopKeywords
              .Concat(gamingKeywords)
              .Concat(mouseKeywords)
              .Concat(ignoredWords)
              .ToArray();

            var textSearchTerms = rawTerms
                .Where(term => !allKeywordsToRemove.Any(k => term.Contains(k))) // Remove if term matches a keyword
                .ToList();

            if (textSearchTerms.Count > 0)
            {
                productQuery = productQuery.Where(p => textSearchTerms.Any(term =>
                    p.ProductName.ToLower().Contains(term) ||
                    p.Manufacturer.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) // Also check description!
                ));
            }

            // 7. Execute
            var products = await productQuery
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
                default:
                    return "";
            }
        }
        public string BuildProductContext(List<Product> products, string userRole)
        {
            if (!products.Any()) return string.Empty;

            var sb = new StringBuilder();
            if (userRole != "customer")
            {
                sb.AppendLine($"SYSTEM NOTE: {products.Count} matches found in database.");
            }

            foreach (var p in products)
            {
                // 1. Currency Formatting (VND)
                var priceVal = p.DiscountedPrice ?? p.OriginalPrice;
                var priceString = $"{priceVal:N0} ₫";

                sb.AppendLine($"### {p.ProductName}");

                // 2. Hide Technical ID/Slug for Customers
                if (userRole != "customer")
                {
                    sb.AppendLine($"   - System ID: {p.UrlSlug}");
                }

                // 3. Clarity for Mouse vs Mouse Pad & Laptop Types
                var category = p.Category?.Name ?? "General";
                sb.AppendLine($"   - Type: {category}");
                sb.AppendLine($"   - Brand: {p.Manufacturer?.Name}");
                sb.AppendLine($"   - Price: {priceString}");

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

                string specs = GetTechnicalDetails(p);
                if (!string.IsNullOrWhiteSpace(specs))
                {
                    sb.AppendLine("   - Key Features:");
                    sb.AppendLine(specs); // Pre-formatted
                }

                sb.AppendLine(""); // Spacer
            }

            return sb.ToString();
        }

        public async Task<string> GetSalesSummaryJsonAsync()
        {
            // 1. Setup Time Ranges
            var now = DateTime.UtcNow.Date;
            var yesterdayStart = now.AddDays(-1);
            var yesterdayEnd = now;
            var thirtyDaysAgo = now.AddDays(-30);

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

            var alerts = new List<string>();

            // A. Low Stock Alert (Threshold < 5)
            var lowStockProducts = await productsQuery
                .Where(p => p.IsActive && p.StockCount < 5 && p.StockCount > 0)
                .OrderBy(p => p.StockCount)
                .Take(3)
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

            // ── Laptop ──────────────────────────────────────────────────────────
            if (p.LaptopSpecification != null)
            {
                var s = p.LaptopSpecification;
                var usage = s.IsForGaming ? "[GAMING MACHINE]" : "[OFFICE/BUSINESS]";
                sb.AppendLine($"      * Class: {usage}");
                sb.AppendLine($"      * CPU: {s.CPU}");
                sb.AppendLine($"      * GPU: {s.GraphicsCard}");
                sb.AppendLine($"      * RAM/Storage: {s.RAM} / {s.Storage}");
                sb.AppendLine($"      * Screen: {s.Display}");
                sb.AppendLine($"      * Battery: {s.BatteryLife} | Weight: {s.Weight} | OS: {s.OperatingSystem}");
                sb.AppendLine($"      * Warranty: {s.Warranty}");
            }

            // ── Monitor ─────────────────────────────────────────────────────────
            if (p.MonitorSpecification != null)
            {
                var s = p.MonitorSpecification;
                sb.AppendLine($"      * Spec: {s.ScreenSize}\" {s.PanelType?.Name} Panel");
                sb.AppendLine($"      * Resolution: {s.Resolution} @ {s.RefreshRate}Hz");
                sb.AppendLine($"      * Response: {s.ResponseTimeMs}ms | Brightness: {s.Brightness} nits");
                sb.AppendLine($"      * Gamut: {s.ColorGamut} | VESA: {(s.VesaMount ? "Yes" : "No")}");
            }

            // ── CPU ─────────────────────────────────────────────────────────────
            if (p.CpuSpecification != null)
            {
                var s = p.CpuSpecification;
                sb.AppendLine($"      * Socket: {s.Socket?.Name}");
                sb.AppendLine($"      * Cores: {s.CoreCount}C / {s.ThreadCount}T");
                sb.AppendLine($"      * Clock: Base {s.BaseClock}GHz / Boost {s.BoostClock}GHz");
                sb.AppendLine($"      * L3 Cache: {s.L3Cache}MB | TDP: {s.Tdp}W | iGPU: {(s.HasIntegratedGraphics ? "Yes" : "No")}");
            }

            // ── GPU ─────────────────────────────────────────────────────────────
            if (p.GpuSpecification != null)
            {
                var s = p.GpuSpecification;
                sb.AppendLine($"      * Chipset: {s.Chipset}");
                sb.AppendLine($"      * VRAM: {s.MemorySize}GB {s.MemoryType} | Boost Clock: {s.BoostClock}MHz");
                sb.AppendLine($"      * Interface: {s.Interface} | Rec. PSU: {s.RecommendedPsu}W");
                sb.AppendLine($"      * Length: {s.Length}mm (Check Case clearance)");
            }

            // ── Motherboard ──────────────────────────────────────────────────────
            if (p.MotherboardSpecification != null)
            {
                var s = p.MotherboardSpecification;
                sb.AppendLine($"      * Socket: {s.Socket?.Name} | Chipset: {s.Chipset?.Name}");
                sb.AppendLine($"      * Form Factor: {s.FormFactor?.Name}");
                sb.AppendLine($"      * RAM: {s.RamSlots} slots, max {s.MaxRam}GB ({s.SupportedRamTypes?.Name})");
                sb.AppendLine($"      * Storage: {s.M2Slots}x M.2 | {s.SataPorts}x SATA");
                sb.AppendLine($"      * WiFi: {(s.HasWifi ? "Built-in" : "No")}");
            }

            // ── RAM ─────────────────────────────────────────────────────────────
            if (p.RamSpecification != null)
            {
                var s = p.RamSpecification;
                sb.AppendLine($"      * Type: {s.RamType?.Name} | Form Factor: {s.RamFormFactor}");
                sb.AppendLine($"      * Capacity: {s.Capacity}GB ({s.ModuleCount}x{s.Capacity / (s.ModuleCount > 0 ? s.ModuleCount : 1)}GB)");
                sb.AppendLine($"      * Speed: {s.Speed}MHz | Timing: {s.Timing} | Voltage: {s.Voltage}V");
                sb.AppendLine($"      * RGB: {(s.HasRGB ? "Yes" : "No")}");
            }

            // ── Storage ─────────────────────────────────────────────────────────
            if (p.StorageSpecification != null)
            {
                var s = p.StorageSpecification;
                sb.AppendLine($"      * Type: {s.StorageType} | Form Factor: {s.StorageFormFactor}");
                sb.AppendLine($"      * Capacity: {s.Capacity}GB | Interface: {s.Interface}");
                sb.AppendLine($"      * Read: {s.ReadSpeed}MB/s | Write: {s.WriteSpeed}MB/s");
                if (s.Rpm.HasValue) sb.AppendLine($"      * RPM: {s.Rpm}");
            }

            // ── PSU ─────────────────────────────────────────────────────────────
            if (p.PsuSpecification != null)
            {
                var s = p.PsuSpecification;
                sb.AppendLine($"      * Wattage: {s.Wattage}W | Rating: {s.EfficiencyRating}");
                sb.AppendLine($"      * Form Factor: {s.FormFactor?.Name} | Modular: {s.Modularity}");
            }

            // ── Case ─────────────────────────────────────────────────────────────
            if (p.CaseSpecification != null)
            {
                var s = p.CaseSpecification;
                sb.AppendLine($"      * Form Factor: {s.FormFactor?.Name} | Color: {s.Color}");
                sb.AppendLine($"      * Max GPU: {s.MaxGpuLength}mm | Max CPU Cooler: {s.MaxCpuCoolerHeight}mm");
                sb.AppendLine($"      * Fan Support: {s.FanSupport} | Radiator: {s.RadiatorSupport}");
                sb.AppendLine($"      * Drive Bays: {s.DriveBays} | Front I/O: {s.FrontPanelPorts}");
                sb.AppendLine($"      * Cooling: {s.CoolingSupport}");
            }

            // ── CPU Cooler ───────────────────────────────────────────────────────
            if (p.CpuCoolerSpecification != null)
            {
                var s = p.CpuCoolerSpecification;
                sb.AppendLine($"      * Type: {s.CoolerType} | Fan: {s.FanSize}mm");
                if (s.RadiatorSize.HasValue) sb.AppendLine($"      * Radiator: {s.RadiatorSize}mm");
                if (s.Height.HasValue) sb.AppendLine($"      * Height: {s.Height}mm");
                if (s.TdpSupport.HasValue) sb.AppendLine($"      * TDP Support: {s.TdpSupport}W");
                if (s.NoiseLevel.HasValue) sb.AppendLine($"      * Noise: {s.NoiseLevel}dBA");
                sb.AppendLine($"      * Lighting: {s.LedLighting} | Color: {s.Color}");
            }

            // ── Case Fan ─────────────────────────────────────────────────────────
            if (p.CaseFanSpecification != null)
            {
                var s = p.CaseFanSpecification;
                sb.AppendLine($"      * Size: {s.FanSize}mm | Max RPM: {s.MaxRpm}");
                sb.AppendLine($"      * Airflow: {s.Airflow} CFM | Static Pressure: {s.StaticPressure} mmH2O");
                sb.AppendLine($"      * Noise: {s.NoiseLevel}dB(A) | Connector: {s.Connector}");
                sb.AppendLine($"      * Bearing: {s.BearingType} | RGB: {(s.HasRgb ? "Yes" : "No")}");
            }

            // ── Keyboard ─────────────────────────────────────────────────────────
            if (p.KeyboardSpecification != null)
            {
                var s = p.KeyboardSpecification;
                sb.AppendLine($"      * Type: {s.KeyboardType} ({s.Layout})");
                sb.AppendLine($"      * Switches: {s.SwitchType?.Name}");
                sb.AppendLine($"      * Connectivity: {s.Connectivity}");
            }

            // ── Mouse ─────────────────────────────────────────────────────────────
            if (p.MouseSpecification != null)
            {
                var s = p.MouseSpecification;
                sb.AppendLine($"      * Connectivity: {s.Connectivity} | Buttons: {s.ButtonCount}");
                sb.AppendLine($"      * DPI: {s.Dpi} | Sensor: {s.SensorType} | Polling: {s.PollingRate}Hz");
                sb.AppendLine($"      * Weight: {s.Weight}g | Color: {s.Color} | Backlight: {s.BacklightColor}");
            }

            // ── Mouse Pad ─────────────────────────────────────────────────────────
            if (p.MousepadSpecification != null)
            {
                var s = p.MousepadSpecification;
                sb.AppendLine($"      * Size: {s.Width}x{s.Height}mm | Thickness: {s.Thickness}mm");
                sb.AppendLine($"      * Material: {s.Material} | Surface: {s.SurfaceType}");
                sb.AppendLine($"      * Base: {s.BaseType} | RGB: {(s.HasRgb ? "Yes" : "No")} | Washable: {(s.IsWashable ? "Yes" : "No")}");
            }

            // ── Headset ───────────────────────────────────────────────────────────
            if (p.HeadsetSpecification != null)
            {
                var s = p.HeadsetSpecification;
                sb.AppendLine($"      * Connectivity: {s.Connectivity}");
                sb.AppendLine($"      * Driver: {s.DriverSize}mm | Freq: {s.Frequency} | Impedance: {s.Impedance}Ω");
                sb.AppendLine($"      * Surround: {(s.IsSurroundSound ? "Yes" : "No")} | Noise Cancelling: {(s.IsNoiseCancelling ? "Yes" : "No")}");
                sb.AppendLine($"      * Mic: {(s.HasMicrophone ? $"{s.MicrophoneType}" : "No")} | Sensitivity: {s.Sensitivity}dB");
            }

            // ── Microphone ───────────────────────────────────────────────────────
            if (p.MicrophoneSpecification != null)
            {
                var s = p.MicrophoneSpecification;
                sb.AppendLine($"      * Type: {s.MicrophoneType} | Polar Pattern: {s.PolarPattern}");
                sb.AppendLine($"      * Connection: {s.Connection} ({s.Connectivity})");
                sb.AppendLine($"      * Freq: {s.Frequency} | Sample Rate: {s.SampleRate} | Sensitivity: {s.Sensitivity}");
                sb.AppendLine($"      * Shock Mount: {(s.HasShockMount ? "Yes" : "No")} | Pop Filter: {(s.HasPopFilter ? "Yes" : "No")} | RGB: {(s.HasRgb ? "Yes" : "No")}");
            }

            // ── Chair ─────────────────────────────────────────────────────────────
            if (p.ChairSpecification != null)
            {
                var s = p.ChairSpecification;
                sb.AppendLine($"      * Type: {s.ChairType} | Material: {s.Material}");
                sb.AppendLine($"      * Max Load: {s.MaxWeight}kg | Seat Height: {s.SeatHeight}");
                sb.AppendLine($"      * Armrests: {s.ArmrestType} | Backrest: {s.BackrestAdjustment}");
                sb.AppendLine($"      * Lumbar Support: {(s.HasLumbarSupport ? "Yes" : "No")} | Headrest: {(s.HasHeadrest ? "Yes" : "No")}");
                sb.AppendLine($"      * Base: {s.BaseType} | Wheels: {s.WheelType} | Color: {s.Color}");
            }

            // ── Desk ─────────────────────────────────────────────────────────────
            if (p.DeskSpecification != null)
            {
                var s = p.DeskSpecification;
                sb.AppendLine($"      * Dimensions: {s.Width}x{s.Depth}cm | Height: {s.Height}cm");
                sb.AppendLine($"      * Material: {s.Material} | Surface: {s.SurfaceType} | Max Load: {s.MaxWeight}kg");
                sb.AppendLine($"      * Height Adjustable: {(s.IsHeightAdjustable ? "Yes" : "No")} | Cable Mgmt: {(s.HasCableManagement ? "Yes" : "No")}");
                sb.AppendLine($"      * Extras: Cup Holder {(s.HasCupHolder ? "✓" : "✗")} | Headphone Hook {(s.HasHeadphoneHook ? "✓" : "✗")}");
            }

            // ── Console ───────────────────────────────────────────────────────────
            if (p.ConsoleSpecification != null)
            {
                var s = p.ConsoleSpecification;
                sb.AppendLine($"      * CPU: {s.Processor} | GPU: {s.Graphics}");
                sb.AppendLine($"      * RAM: {s.RAM} | Storage: {s.Storage}");
                sb.AppendLine($"      * Max Resolution: {s.MaxResolution} @ {s.MaxFrameRate}");
                sb.AppendLine($"      * HDR: {(s.HDRSupport ? "Yes" : "No")} | Optical Drive: {s.OpticalDrive}");
                sb.AppendLine($"      * Connectivity: {s.Connectivity} | WiFi: {s.WifiVersion} | BT: {s.BluetoothVersion}");
                sb.AppendLine($"      * Ethernet: {(s.HasEthernet ? "Yes" : "No")}");
            }

            // ── Handheld ──────────────────────────────────────────────────────────
            if (p.HandheldSpecification != null)
            {
                var s = p.HandheldSpecification;
                sb.AppendLine($"      * CPU: {s.Processor} | GPU: {s.Graphics}");
                sb.AppendLine($"      * RAM: {s.RAM} | Storage: {s.Storage}");
                sb.AppendLine($"      * Display: {s.Display} | Battery: {s.BatteryLife}");
                sb.AppendLine($"      * OS: {s.OperatingSystem} | Weight: {s.Weight}g");
                sb.AppendLine($"      * Connectivity: {s.Connectivity} | WiFi: {s.WifiVersion} | BT: {s.BluetoothVersion}");
            }

            // ── Hub ───────────────────────────────────────────────────────────────
            if (p.HubSpecification != null)
            {
                var s = p.HubSpecification;
                sb.AppendLine($"      * Type: {s.HubType} | Total Ports: {s.PortCount}");
                sb.AppendLine($"      * USB-A: {s.UsbAPorts} | USB-C: {s.UsbCPorts}");
                sb.AppendLine($"      * HDMI: {s.HdmiPorts} | DisplayPort: {s.DisplayPorts} | Ethernet: {(s.EthernetPort ? "Yes" : "No")}");
                sb.AppendLine($"      * SD Card: {(s.SdCardReader ? "Yes" : "No")} | Audio Jack: {(s.AudioJack ? "Yes" : "No")}");
                sb.AppendLine($"      * Max Displays: {s.MaxDisplays} @ {s.MaxResolution} | Power Delivery: {s.PowerDelivery}");
            }

            // ── Cable ─────────────────────────────────────────────────────────────
            if (p.CableSpecification != null)
            {
                var s = p.CableSpecification;
                sb.AppendLine($"      * Type: {s.CableType} | Length: {s.Length}m");
                sb.AppendLine($"      * Connectors: {s.Connector1} ↔ {s.Connector2}");
                sb.AppendLine($"      * Max Power: {s.MaxPower} | Data Speed: {s.DataTransferSpeed}");
                sb.AppendLine($"      * Braided: {(s.IsBraided ? "Yes" : "No")} | Color: {s.Color} | Warranty: {s.Warranty}");
            }

            // ── Charger ───────────────────────────────────────────────────────────
            if (p.ChargerSpecification != null)
            {
                var s = p.ChargerSpecification;
                sb.AppendLine($"      * Type: {s.ChargerType} | Total Wattage: {s.TotalWattage}W");
                sb.AppendLine($"      * Ports: {s.PortCount} total ({s.UsbCPorts}x USB-C, {s.UsbAPorts}x USB-A)");
                sb.AppendLine($"      * Max Per Port: {s.MaxOutputPerPort} | Fast Charge: {s.FastChargingProtocols}");
                sb.AppendLine($"      * Tech: {s.Technology} | Foldable Plug: {(s.HasFoldablePlug ? "Yes" : "No")} | Cable Included: {(s.CableIncluded ? "Yes" : "No")}");
            }

            // ── Memory Card ───────────────────────────────────────────────────────
            if (p.MemoryCardSpecification != null)
            {
                var s = p.MemoryCardSpecification;
                sb.AppendLine($"      * Type: {s.CardType} | Capacity: {s.Capacity}GB");
                sb.AppendLine($"      * Speed Class: {s.SpeedClass} | Read: {s.ReadSpeed}MB/s | Write: {s.WriteSpeed}MB/s");
                sb.AppendLine($"      * Waterproof: {(s.Waterproof ? "Yes" : "No")} | Shockproof: {(s.Shockproof ? "Yes" : "No")} | Warranty: {s.Warranty}");
            }

            // ── Webcam ────────────────────────────────────────────────────────────
            if (p.WebcamSpecification != null)
            {
                var s = p.WebcamSpecification;
                sb.AppendLine($"      * Resolution: {s.Resolution} @ {s.FrameRate}fps | FOV: {s.FieldOfView}°");
                sb.AppendLine($"      * Focus: {s.FocusType} | Connectivity: {s.Connectivity} ({s.Connection})");
                sb.AppendLine($"      * Mic: {(s.HasMicrophone ? "Yes" : "No")} | Privacy Shutter: {(s.HasPrivacyShutter ? "Yes" : "No")} | Mount: {s.MountType}");
            }

            // ── Speaker ───────────────────────────────────────────────────────────
            if (p.SpeakerSpecification != null)
            {
                var s = p.SpeakerSpecification;
                sb.AppendLine($"      * Type: {s.SpeakerType} | Wattage: {s.TotalWattage}W");
                sb.AppendLine($"      * Freq: {s.Frequency} | Connectivity: {s.Connectivity}");
                sb.AppendLine($"      * Inputs: {s.InputPorts} | Bluetooth: {(s.HasBluetooth ? "Yes" : "No")} | Remote: {(s.HasRemote ? "Yes" : "No")}");
            }

            // ── Network Hardware ──────────────────────────────────────────────────
            if (p.NetworkHardwareSpecification != null)
            {
                var s = p.NetworkHardwareSpecification;
                sb.AppendLine($"      * Device: {s.DeviceType} | WiFi: {s.WifiStandard}");
                sb.AppendLine($"      * Max Speed: {s.MaxSpeed} | Frequency: {s.Frequency}");
                sb.AppendLine($"      * Ethernet: {s.EthernetPorts} | Antennas: {s.AntennaCount}");
                sb.AppendLine($"      * USB Port: {(s.HasUsb ? "Yes" : "No")} | Security: {s.SecurityProtocol} | Coverage: {s.Coverage}");
            }

            // ── Software ──────────────────────────────────────────────────────────
            if (p.SoftwareSpecification != null)
            {
                var s = p.SoftwareSpecification;
                sb.AppendLine($"      * Type: {s.SoftwareType} | License: {s.LicenseType}");
                sb.AppendLine($"      * Platform: {s.Platform} | Version: {s.Version}");
                sb.AppendLine($"      * Language: {s.Language} | Delivery: {s.DeliveryMethod}");
                sb.AppendLine($"      * Subscription: {(s.IsSubscription ? "Yes" : "No")} | Requirements: {s.SystemRequirements}");
            }

            // ── Power Bank ────────────────────────────────────────────────────────
            if (p.PowerBankSpecification != null)
            {
                var s = p.PowerBankSpecification;
                sb.AppendLine($"      * Capacity: {s.Capacity}mAh | Total Wattage: {s.TotalWattage}W");
                sb.AppendLine($"      * Ports: {s.PortCount} total ({s.UsbCPorts}x USB-C, {s.UsbAPorts}x USB-A)");
                sb.AppendLine($"      * Input: {s.InputPorts} | Max Per Port: {s.MaxOutputPerPort}");
                sb.AppendLine($"      * Fast Charge: {s.FastChargingProtocols} | Recharge Time: {s.RechargingTime}");
                sb.AppendLine($"      * Display: {(s.HasDisplay ? "Yes" : "No")} | Weight: {s.Weight}g | Color: {s.Color}");
            }

            return sb.ToString();
        }
    }
}
