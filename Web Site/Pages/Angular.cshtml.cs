using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SplendidApp.Pages
{
	public class AngularModel : PageModel
	{
		private readonly ILogger<AngularModel> _logger;
		protected bool bDebug = false;

		public AngularModel(ILogger<AngularModel> logger)
		{
			_logger = logger;
#if DEBUG
			bDebug = true;
#endif
		}

		public void OnGet()
		{
			//Console.WriteLine("AngularModel.OnGet");
		}
	}
}
