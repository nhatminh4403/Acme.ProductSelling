namespace Acme.ProductSelling.Web.Models
{
    public class BreadcrumbItem
    {
        /// <summary>
        /// The text to display for the breadcrumb link.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The URL the breadcrumb links to. Null or empty for the last item.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Indicates if this is the current, active page.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
