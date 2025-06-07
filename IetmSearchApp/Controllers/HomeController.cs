using Microsoft.AspNetCore.Mvc;
using ItemSearchApp.Models;
using ItemSearchApp.Helpers;

namespace ItemSearchApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string keyword)
        {
            var path = "/tmp/items.xlsx";
            var items = ExcelReader.LoadItemsFromExcel(path);

            // 価格が空でないアイテムだけを対象にする
            items = items
                .Where(item => !string.IsNullOrWhiteSpace(item.Price))
                .ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string norm = StringHelper.Normalize(keyword);
                items = items
                    .Where(item => StringHelper.Normalize(item.Name).Contains(norm))
                    .ToList();
            }
            else
            {
                items = new List<Item>();
            }

            // 🔽 追加：カテゴリ検出して ViewBag に渡す
            var categoryKeywords = new Dictionary<string, string[]>
   　　　　 {
        　　{ "aircon", new[] { "エアコン", "冷房", "室外機", "室内機" } },
       　　 { "fridge", new[] { "大型冷蔵庫", "冷蔵庫", "5ドア", "6ドア", "ミニ冷蔵庫", "小型冷蔵庫" } },

        { "mini_fridge", new[] { "大型冷蔵庫", "冷蔵庫", "5ドア", "6ドア", "ミニ冷蔵庫", "小型冷蔵庫" } },
       　　 { "tv", new[] { "テレビ", "液晶テレビ", "モニター" } },
       　　 { "metal", new[] { "基板", "レアメタル", "スクラップ" } },
       　　 { "hdd", new[] { "HDD", "ハードディスク" } }
   　　　　 };

            var matchedCategories = new List<string>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string norm = StringHelper.Normalize(keyword);
                foreach (var kvp in categoryKeywords)
                {
                    if (kvp.Value.Any(k => norm.Contains(StringHelper.Normalize(k))))
                    {
                        matchedCategories.Add(kvp.Key);
                    }
                }
            }

            ViewBag.MatchedCategories = matchedCategories;
            ViewBag.Keyword = keyword;

            return View(items);
        }
    }
}
