using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            // טוען את התוכן מהאתר
            var myHtml = await Load("https://www.10dakot.co.il/");
            
            // אין צורך לנקות רווחים מיותרים בצורה זו, כדי לשמור על התוכן
            // השתמש ב-Matches כדי לקבל את התגיות
            var htmlLines = new Regex("<(.*?)>").Matches(myHtml); 
            // יצירת אובייקט השורש 
            HtmlElements root = new HtmlElements(null, "root");
            HtmlElements currentElement = root; // משתנה שיחזיק את האלמנט הנוכחי


            // לולאה על רשימת התגיות
            foreach (Match match in htmlLines)
            {
                var line = match.Groups[1].Value; // קח את התוכן מתוך הדפוס
                var parts = line.Split(new[] { ' ' }, 2); // חותכים את המילה הראשונה
                var tagName = parts[0].ToLower(); // ננמיך אותיות לצורך התאמה

                // אם מדובר בתגית סוגרת
                if (tagName.StartsWith("/"))
                {
                    if (currentElement.Parent != null) // נוסעים לרמה הקודמת בעץ רק אם יש הורה
                    {
                        currentElement = currentElement.Parent; // החזר לאלמנט ההורה
                    }
                }
                else // אם מדובר בתגית HTML
                {
                    // יצירת אובייקט חדש לתגית
                    var newElement = new HtmlElements(currentElement, tagName); // שים לב שההורה כאן

                    // אם יש מרכיבים נוספים אחרי שם התגית
                    if (parts.Length > 1)
                    {
                        var attributesLine = parts[1]; // המשך המחרוזת (ללא המילה הראשונה)
                        var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(attributesLine); // פרוק את ה-attributes

                        foreach (Match attribute in attributes)
                        {
                            if (attribute.Groups.Count > 2)
                            {
                                var attrName = attribute.Groups[1].Value;
                                var attrValue = attribute.Groups[2].Value;

                                if (attrName == "class")
                                {
                                    // אם יש Attribute בשם class - פרקי אותו לחלקים
                                    foreach (var className in attrValue.Split(' '))
                                    {
                                        newElement.AddClass(className); // הוספת כל קלאס
                                    }
                                }
                                else
                                {
                                    newElement.AddAttribute(attrName, attrValue); // הוספת מאפיין
                                    if (attrName == "id")
                                    {
                                        newElement.Id = attrValue; // עדכון המזהה
                                    }
                                }
                            }
                        }
                    }

                    currentElement.AddChild(newElement); // הוסף את התגית לרשימת הילדים

                    // אם התגית לא סוגרת את עצמה
                    if (!tagName.EndsWith("/") && !HtmlHelper.Instance.HtmlVoidTags.Contains(tagName))
                    {
                        currentElement = newElement; // עדכון האלמנט הנוכחי לאלמנט החדש
                    }
                }

            }

            // הדפסת התוצאות
            Console.WriteLine($"Root has {root.Children.Count} children:");
            foreach (var child in root.Children)
            {
                Console.WriteLine(child.Name); // הדפסת שם כל ילד
            }
            Console.WriteLine("***************************");
            var descendantsList = root.Descendants(); // קבל את כל הצאצאים של השורש

            foreach (var descendant in descendantsList)
            {
                Console.WriteLine(descendant.Name); // הדפסת שם כל צאצא
            }
            Console.WriteLine("+++++++++++++");
            //var ancestorsList = someElement.Ancestors(); // קבל את כל האבות של אלמנט מסוים
            var ancestorsList = root.Ancestors(); // קבל את כל הצאצאים של השורש

            foreach (var ancestor in ancestorsList)
            {
                Console.WriteLine(ancestor.Name); // הדפסת שם כל אב
            }

            Console.WriteLine("////////////");

            var result = root.FindElements("a");
            foreach (var element in result)
            {
                Console.WriteLine(element.Name);
            }
            Console.WriteLine(result.Count);                                                                            
            // צריך למחוק אח"כ
            Console.ReadLine();
        }


        // שיטה לטעינת תוכן ה-HTML
        static async Task<string> Load(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync(); // החזרת תוכן ה-HTML
            }
        }
    }

    // הגדר את מחלקת HtmlElements פה
}