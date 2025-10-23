using System.ComponentModel;

namespace Acme.ProductSelling.Products.Specs
{
    public enum StorageFormFactor
    {
        // HDD
        [Description("3.5-inch HDD")] // <-- Ví dụ
        Hdd35Inch,
        [Description("2.5-inch HDD")]
        Hdd25Inch,

        // SSD SATA
        [Description("2.5-inch SSD (SATA)")]
        Ssd25InchSata,
        [Description("mSATA SSD")]
        SsdMSata,

        // SSD M.2
        [Description("M.2 2230")]
        SsdM2_2230,
        [Description("M.2 2242")]
        SsdM2_2242,
        [Description("M.2 2280")]
        SsdM2_2280,
        [Description("M.2 22110")]
        SsdM2_22110,

        // SSD Enterprise/Server
        [Description("U.2")]
        SsdU2,
        [Description("PCIe Add-in-Card (AIC)")]
        SsdPcieAic,

        // Bạn có thể giữ nguyên tên hoặc thêm mô tả dễ hiểu hơn nếu muốn
        [Description("EDSFF E1.S")]
        SsdEdsffE1S,
        [Description("EDSFF E1.L")]
        SsdEdsffE1L,
        [Description("EDSFF E3.S")]
        SsdEdsffE3S,
        [Description("EDSFF E3.L")]
        SsdEdsffE3L
    }
}
