using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToDoApp_team3.Model;

namespace ToDoApp_team3.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string FilterName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedId { get; set; }

        public List<Task> TaskList { get; set; }
        public List<Priorities> PriorityList { get; set; }

        public List<(int id, string name)> ItemList { get; set; } = [];

        public DateTime DeadlineAt => DateTime.Now;

        public void OnGet()
        {
            TaskList = new List<Task>();
            PriorityList = new List<Priorities>();

            for (int i = 1; i <= 10; i++)
            {
                ItemList.Add((i, $"ID：{i}のタスク"));
            }
        }

        public void OnGetFiltering()
        {
            TaskList = new List<Task> ();
        }

        //public IActionResult OnPostDelete()
        //{

        //}

        //public IActionResult OnPostComplete()
        //{

        //}
    }
}
