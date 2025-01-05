using System;
using System.Collections.Generic;

namespace Html_Serializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public HashSet<string> Classes { get; set; }
        public Selector Parent { get; set; } // הפנייה להורה
        public Selector Child { get; set; } // הפנייה לילד

        public Selector(string tagName, string id = null, HashSet<string> classes = null)
        {
            TagName = tagName;
            Id = id;
            Classes = classes ?? new HashSet<string>();
            Parent = null;
            Child = null;
        }

        public void AddChild(Selector child)
        {
            Child = child;
            child.Parent = this; // הגדרת ההורה של הילד
        }

        // פונקציה סטטית הממירה מחרוזת שאילתה לאובייקט Selector
        public static Selector FromQueryString(string queryString)
        {
            var levels = queryString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (levels.Length == 0)
                return null;

            Selector rootSelector = null;
            Selector currentSelector = null;

            foreach (var level in levels)
            {
                string tagName = null;
                string id = null;
                HashSet<string> classes = new HashSet<string>();

                // בדיקת אם יש מזהה
                int hashIndex = level.IndexOf('#');
                string currentLevel = level;

                if (hashIndex != -1)
                {
                    // אם יש מזהה, נשמור את החלק אחרי הסימן #
                    id = level.Substring(hashIndex + 1);
                    // קבע את המחרוזת לפני ה# למשתנה נפרד
                    currentLevel = level.Substring(0, hashIndex);
                }

                // פיצול לפי כיתות
                var classParts = currentLevel.Split('.');
                tagName = classParts[0]; // השם של התגית הראשונה

                for (int i = 1; i < classParts.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(classParts[i]))
                    {
                        classes.Add(classParts[i]); // הוספת שאר הכיתות לרשימה
                    }
                }

                // בדיקה אם המחרוזת לא ריקה למקרה של tagName
                if (string.IsNullOrWhiteSpace(tagName))
                    continue;

                // יצירת אובייקט Selector חדש
                var newSelector = new Selector(tagName, id, classes);

                // אם זה הסלקטור הראשון, הוא שורש
                if (rootSelector == null)
                {
                    rootSelector = newSelector;
                    currentSelector = rootSelector;
                }
                else
                {
                    currentSelector.AddChild(newSelector); // הוספת הילד
                    currentSelector = newSelector; // עדכון לסלקטור הנוכחי
                }
            }

            return rootSelector; // החזרת השורש
        }
    }
}