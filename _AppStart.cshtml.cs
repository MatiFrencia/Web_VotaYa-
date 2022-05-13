using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VotaYa.Pages
{
    public class _AppStartModel : PageModel
    {
        public void OnGet()
        {
            Redirect("/Login");
        }
    }
}
