using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShowWeb.Models.ViewModels;

public class RoleManagementVM
{
    public ApplicationUser ApplicationUser { get; set; }
    public IEnumerable<SelectListItem> RolesList { get; set; }
    public IEnumerable<SelectListItem> CompanyList { get; set; }
}