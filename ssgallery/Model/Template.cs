using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssgallery.Model
{
    class Template
    {
        public string RawHtml
        {
            get;
            set;
        }

        private readonly Dictionary<string, string> Values;

        public List<TemplateItem> Items;

        public Template()
        {
            Values = new Dictionary<string, string>();
            Items = new List<TemplateItem>();
        }

        public void AddValue(string name, string value)
        {
            if( Values.ContainsKey(name) )
            {
                Values[name] = value;
            }
            else
            {
                Values.Add(name, value);
            }
        }

        public void AddValues(Dictionary<string, string> values)
        {
            foreach (var key in values.Keys)
            {
                AddValue(key, values[key]);
            }
        }

        public void RenderHtml(string filename)
        {
            var rendered = RenderItems();

            foreach (var key in Values.Keys)
            {
                var fmt = string.Format("%{0}%", key);
                rendered = rendered.Replace(fmt, Values[key]);
            }

            if (File.Exists(filename))
            {
                var md5 = System.Security.Cryptography.MD5.Create();
                var renderedBytes = Encoding.ASCII.GetBytes(rendered);
                var renderedHash = md5.ComputeHash(renderedBytes);
                var existingBytes = File.ReadAllBytes(filename);
                var existingHash = md5.ComputeHash(existingBytes);
                
                if (renderedHash.SequenceEqual(existingHash))
                {
                    Console.WriteLine("Skipping {0}, existing file is up to date", filename);
                    return;
                }
            }

            Console.WriteLine(string.Format("Writing {0}", filename));

            File.WriteAllText(filename, rendered);
        }

        private string RenderItems()
        {
            if (!Items.Any())
                return RawHtml;

            var rendered = RawHtml;
            var itemInfo = Items[0];

            var startTag = string.Format("<!-- %{0}_START% -->", itemInfo.Tag);
            var endTag = string.Format("<!-- %{0}_END% -->", itemInfo.Tag);

            var startindex = rendered.IndexOf(startTag);

            if (startindex < 0)
            {
                throw new Exception(string.Format("Could not find item start tag {0}", startTag));
            }

            var endindex = rendered.IndexOf(endTag);

            if (endindex < 0)
            {
                throw new Exception(string.Format("Could not find item end tag {0}", endTag));
            }

            var itemTemplate = rendered.Substring(startindex, endindex - startindex);

            var aboveItems = rendered.Substring(0, startindex);

            var belowItems = rendered.Substring(endindex, (rendered.Length - endindex));

            string itemsHtml = "";

            foreach (var item in Items)
            {
                var curItemHtml = itemTemplate;
                foreach (var key in item.Values.Keys)
                {
                    var fmt = string.Format("%{0}%", key);
                    curItemHtml = curItemHtml.Replace(fmt, item.Values[key]);
                }

                itemsHtml += curItemHtml + Environment.NewLine;
            }

            rendered = aboveItems + itemsHtml + belowItems;

            return rendered;
        }
    }

    class TemplateItem
    {
        public string Tag;
        internal Dictionary<string, string> Values;

        public TemplateItem()
        {
            Values = new Dictionary<string, string>();
        }

        public void AddValue(string name, string value)
        {
            if( Values.ContainsKey(name) )
            {
                Values[name] = value;
            }
            else
            {
                Values.Add(name, value);
            }
        }
    }
}
