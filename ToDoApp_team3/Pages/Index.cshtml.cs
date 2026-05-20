using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToDoApp_team3.Pages
{
    public class IndexModel : PageModel
    {
        public List<(int id, string name)> ItemList { get; set; } = [];

        public DateTime DeadlineAt => DateTime.Now;

        public void OnGet()
        {
            for(int i = 1; i <= 10; i++)
            {
                ItemList.Add((i,$"ID：{i}のタスク"));
            }
        }
    }
}
