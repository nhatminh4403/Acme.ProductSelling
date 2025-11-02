using System.ComponentModel;

public enum StorageType
{
    [Description("Hard Disk Drive (HDD)")]
    HDD,

    [Description("SATA Solid-State Drive (SSD)")]
    SataSsd,

    [Description("NVMe M.2 Solid-State Drive (SSD)")]
    NvmeSsd
}