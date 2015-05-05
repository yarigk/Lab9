using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCViews.Controllers
{
    [LogRequestAttribure()]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Table(string id)
        {
            if (id != null)
            {
                var path = HttpContext.Server.MapPath(String.Format("~/App_Data/{0}.csv", RouteData.Values["id"]));
                var parser = new CsvRecord(path);
                return View(parser);
            }
            return View("Index");
        }
        [ChildActionOnly]
        public ActionResult Csv()
        {
            var path = HttpContext.Server.MapPath("~/App_Data");
            var filePaths = Directory.GetFiles(path, "*.csv");
            for (var i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
            }
            return PartialView(filePaths);
        }
    }
}

public class CsvRecord
{
    public string FilePath { get; set; }
    public List<List<string>> Records { get; set; }
    public CsvRecord(string pathToCsv)
    {
        var sr = new StreamReader(pathToCsv);
        string line;
        Records = new List<List<string>>();
        while ((line = sr.ReadLine()) != null)
        {
            var record = line.Split(new []{';'},StringSplitOptions.None).ToList();
            Records.Add(record);
        }
    }

}

public class LogRequestAttribure : ActionFilterAttribute
{

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        var path =filterContext.HttpContext.Server.MapPath("~/App_Data/Log.csv");
        try
        {
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine("{0};{1}", DateTime.Now.ToLocalTime(), filterContext.HttpContext.Request.Url);
        }
        }
        catch {     }
            base.OnActionExecuted(filterContext);
     
    
    }
}