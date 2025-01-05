using System.Text.Json;
using System;
using System.IO;

// מחלקת HtmlHelper: אחראית לטעינת תגיות HTML ותגיות ריקות.
// משתמשת בדפוס Singleton על מנת לאפשר גישה למופע יחיד בלבד.
internal class HtmlHelper
{
    private static HtmlHelper instance; // מופע סטטי של HtmlHelper
    private string[] htmlTags; // מערך לתגיות HTML
    public string[] HtmlVoidTags { get; private set; } // מערך לתגיות ריקות (שנה ל-public)

    // בנאי פרטי - מונע יצירת מופעים חדשים מחוץ למחלקה
    private HtmlHelper()
    {
        LoadHtmlTags(); // טוען את תגיות ה-HTML
        LoadHtmlVoidTags(); // טוען את תגיות ה-void
    }

    // שיטה סטטית לקבלת המופע היחיד של HtmlHelper
    public static HtmlHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new HtmlHelper(); // יצירת מופע חדש אם לא קיים
            }
            return instance; // מחזיר את המופע הקיים
        }
    }

    // שיטה לטעינת תגיות HTML מקובץ JSON
    private void LoadHtmlTags()
    {
        try
        {
            var content = File.ReadAllText("JSONFiles/HtmlTags.json");
            htmlTags = JsonSerializer.Deserialize<string[]>(content); // המרת המידע למערך
        }
        catch (Exception ex)
        {
            Console.WriteLine("שגיאה בקריאת תגיות HTML: " + ex.Message); // הצגת השגיאה לקונסול
        }
    }

    // שיטה לטעינת תגיות ריקות מקובץ JSON
    private void LoadHtmlVoidTags()
    {
        try
        {
            var content = File.ReadAllText("JSONFiles/HtmlVoidTags.json");
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(content); // המרת המידע למערך
        }
        catch (Exception ex)
        {
            Console.WriteLine("שגיאה בקריאת תגיות ריקות: " + ex.Message); // הצגת השגיאה לקונסול
        }
    }

    // שיטה להדפסת תגיות ה-HTML והתגיות הריקות לקונסול
    public void PrintTags()
    {
        Console.WriteLine("תגיות HTML:");
        foreach (var tag in htmlTags)
        {
            Console.WriteLine(tag); // הדפסת כל תגית HTML
        }

        Console.WriteLine("תגיות ריקות:");
        foreach (var voidTag in HtmlVoidTags)
        {
            Console.WriteLine(voidTag); // הדפסת כל תגית ריקה
        }
    }


}