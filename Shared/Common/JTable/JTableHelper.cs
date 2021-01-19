using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Shared.Common.JTable
{
    public class JTableHelper
    {
        private void RemoveNullNodes(JToken root)
        {
            if (root is JValue)
            {
                if (((JValue) root).Value == null) ((JValue) root).Parent.Remove();
            }
            else if (root is JArray)
            {
                ((JArray) root).ToList().ForEach(n => RemoveNullNodes(n));
                if (!((JArray) root).HasValues) root.Parent.Remove();
            }
            else if (root is JProperty)
            {
                RemoveNullNodes(((JProperty) root).Value);
            }
            else
            {
                var children = ((JObject) root).Properties().ToList();
                children.ForEach(n => RemoveNullNodes(n));

                if (!((JObject) root).HasValues)
                {
                    if (((JObject) root).Parent is JArray)
                    {
                        ((JArray) root.Parent).Where(x => !x.HasValues).ToList().ForEach(n => n.Remove());
                    }
                    else
                    {
                        var propertyParent = ((JObject) root).Parent;
                        while (!(propertyParent is JProperty)) propertyParent = propertyParent.Parent;
                        propertyParent.Remove();
                    }
                }
            }
        }

        //public static JObject JObjectTable(ICollection collection, int draw, int totalRecord, params string[] columns)
        //{
        //    var jobjectTalbe = new JObject();
        //    var aaData = new JArray();
        //    var index = 0;

        //    foreach (var item in collection)
        //    {
        //        index++;
        //        var objtype = item.GetType();
        //        var properties = item.GetType().GetProperties();
        //        var jitem = new JObject(); 
        //        foreach (var pro in properties)
        //        {
        //            if ((from x in columns where x.ToLower().Equals(pro.Name.ToLower()) select x).Any() || !columns.Any())
        //            {
        //                string value = string.Empty;
        //                try
        //                {
        //                    value = pro.GetValue(item, null).ToString();
        //                }
        //                catch (Exception)
        //                { }
        //                jitem.Add(pro.Name, value);
        //            }
        //        }

        //        jitem.Add("_STT", index);
        //        aaData.Add(jitem);
        //    }

        //    jobjectTalbe.Add("draw", draw);
        //    jobjectTalbe.Add("recordsTotal", totalRecord);
        //    jobjectTalbe.Add("recordsFiltered", totalRecord);
        //    jobjectTalbe.Add("data", aaData);

        //    return jobjectTalbe;
        //}

        public static JObject JObjectTable(ICollection collection, int draw, int totalRecord, params string[] columns)
        {
            var jobjectTalbe = new JObject();
            var aaData = new JArray();
            var index = 0;
            var columnStandards = columns.Select(item => item.ToLower()).ToList();
            var isNotExistingColumn = columns.Length == 0;

            foreach (var item in collection)
            {
                index++;
                var objtype = item.GetType();
                var properties = item.GetType().GetProperties();
                var jitem = new JObject();

                foreach (var pro in properties)
                {
                    var proNameStandard = pro.Name.ToLower();
                    var isAddColumn = isNotExistingColumn
                                      || columnStandards.Any(m => m.Equals(proNameStandard));

                    if (isAddColumn)
                    {
                        var value = pro.GetValue(item, null)?.ToString() ?? string.Empty;

                        jitem.Add(pro.Name, value);
                    }
                }

                jitem.Add("_STT", index);
                aaData.Add(jitem);
            }

            jobjectTalbe.Add("draw", draw);
            jobjectTalbe.Add("recordsTotal", totalRecord);
            jobjectTalbe.Add("recordsFiltered", totalRecord);
            jobjectTalbe.Add("data", aaData);

            return jobjectTalbe;
        }

        public static JObject JObjectData(ICollection collection, int draw, int totalRecord, params string[] columns)
        {
            var jobjectTalbe = new JObject();
            var aaData = new JArray();
            var index = 0;

            foreach (var item in collection)
            {
                index++;
                var objtype = item.GetType();
                var properties = item.GetType().GetProperties();
                var jitem = new JObject();

                jitem = JObject.FromObject(item);
                jitem.Add("_STT", index);
                aaData.Add(jitem);
            }

            jobjectTalbe.Add("draw", draw);
            jobjectTalbe.Add("recordsTotal", totalRecord);
            jobjectTalbe.Add("recordsFiltered", totalRecord);
            jobjectTalbe.Add("data", aaData);

            return jobjectTalbe;
        }
    }
}