using System;
using System.Collections.Generic;
using System.Linq;

namespace Html_Serializer
{
    internal class HtmlElements
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public HashSet<KeyValuePair<string, string>> Attributes { get; set; }
        public HashSet<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElements Parent { get; set; }
        public HashSet<HtmlElements> Children { get; set; }

        public HtmlElements(HtmlElements parent, string name)
        {
            Parent = parent;
            Name = name;
            Attributes = new HashSet<KeyValuePair<string, string>>();
            Classes = new HashSet<string>();
            Children = new HashSet<HtmlElements>();
        }

        public IEnumerable<HtmlElements> Descendants()
        {
            foreach (var child in Children)
            {
                yield return child;
                foreach (var descendant in child.Descendants())
                {
                    yield return descendant; // החזר את היורשים
                }
            }
        }

        public HashSet<HtmlElements> FindElements(string selector)
        {
            HashSet<HtmlElements> results = new HashSet<HtmlElements>();
            if (string.IsNullOrEmpty(selector))
            {
                return results;
            }

            string[] selectors = selector.Split('.');
            FindElementsRecursively(this, selectors, results);
            return results;
        }

        private void FindElementsRecursively(HtmlElements currentElement, string[] selectors, HashSet<HtmlElements> results)
        {
            var descendants = currentElement.Descendants().ToList();
            var filtered = descendants.Where(e => e.Name.Equals(selectors[0], StringComparison.OrdinalIgnoreCase) ||
                                                   (selectors.Length > 1 && e.Classes.Contains(selectors[1]))).ToList();

            if (selectors.Length == 1)
            {
                foreach (var element in filtered)
                {
                    results.Add(element);
                }
            }
            else
            {
                foreach (var childSelector in filtered)
                {
                    FindElementsRecursively(childSelector, selectors.Skip(1).ToArray(), results);
                }
            }
        }

        // פונקציית AddChild
        public void AddChild(HtmlElements child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        // פונקציית AddAttribute
        public void AddAttribute(string key, string value)
        {
            Attributes.Add(new KeyValuePair<string, string>(key, value));
        }

        // פונקציית AddClass
        public void AddClass(string className)
        {
            if (!Classes.Contains(className))
            {
                Classes.Add(className);
            }
        }

        // פונקציית Ancestors
        public IEnumerable<HtmlElements> Ancestors()
        {
            HtmlElements current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}