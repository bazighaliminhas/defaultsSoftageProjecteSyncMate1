using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace eSyncMate.Maps
{
    public class Transformations
    {
        public static Dictionary<string, string> purposeMap = new Dictionary<string, string>();
        public static Dictionary<string, string> orderTypeMap = new Dictionary<string, string>();
        public static Dictionary<string, string> paymentMethodMap = new Dictionary<string, string>();
        public static Dictionary<string, string> priceIdentifierMap = new Dictionary<string, string>();
        public static Dictionary<string, string> allowanceChargeMap = new Dictionary<string, string>();

        public static string findTagValue(object tags, string matchIndex, string matchValue, string returnIndex)
        {
            int returnIndexInt = Convert.ToInt32(returnIndex.Trim());
            string[] matchIndexArray = matchIndex.Trim().Split('|');
            string[] matchValueArray = matchValue.Trim().Split('|');
            JArray ediTags = null;

            try
            {
                ediTags = JArray.Parse(JsonConvert.SerializeObject(tags));
            }
            catch (Exception)
            {
                ediTags = new JArray();
                ediTags.Add(JToken.Parse(JsonConvert.SerializeObject(tags)));
            }

            foreach (JObject tag in ediTags)
            {
                try
                {
                    bool isMatch = true;

                    for (int i = 0; i < matchIndexArray.Length && isMatch; i++)
                    {
                        int matchIndexInt = Convert.ToInt32(matchIndexArray[i]);
                        string matchValueString = matchValueArray[i];

                        if (tag["Content"][matchIndexInt]["E"].ToString() != matchValueString)
                        {
                            isMatch = false;
                        }
                    }

                    if (isMatch)
                    {
                        try
                        {
                            return tag["Content"][returnIndexInt]["E"].ToString();
                        }
                        catch (Exception ex)
                        {
                            return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return string.Empty;
        }

        public static string findMapTagValue(string key, string mapMethod)
        {
            string value = string.Empty;

            if (!string.IsNullOrEmpty(key))
            {
                if (mapMethod == "purposeMap")
                {
                    if (purposeMap.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    return string.Empty;

                }
                else if (mapMethod == "orderTypeMap")
                {
                    if (orderTypeMap.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    return string.Empty;
                }
                else if (mapMethod == "priceIdentifierMap")
                {
                    if (priceIdentifierMap.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    return string.Empty;
                }
                else if (mapMethod == "paymentMethodMap")
                {
                    if (paymentMethodMap.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    return string.Empty;
                }
                else if (mapMethod == "allowanceChargeMap")
                {
                    if (allowanceChargeMap.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    return string.Empty;
                }
            }

            return value;
        }

        public static string findLoopTagValue(object tags, string parentTagName, string findTagName, string matchParentIndex, string matchParentValue, string returnIndex)
        {
            int returnIndexInt = Convert.ToInt32(returnIndex.Trim());
            int matchIndexInt = Convert.ToInt32(matchParentIndex.Trim());
            JArray ediTags = null;
            bool isMatch = false;
            JArray parentTagObject = null;

            if (tags == null)
            {
                return string.Empty;
            }

            try
            {
                ediTags = JArray.Parse(JsonConvert.SerializeObject(tags));
            }
            catch (Exception)
            {
                ediTags = new JArray();
                ediTags.Add(JToken.Parse(JsonConvert.SerializeObject(tags)));
            }

            foreach (JObject loopTag in ediTags)
            {
                JArray loopTagContents = (JArray)loopTag["Content"];

                foreach (JObject tag in loopTagContents)
                {
                    if (tag["Name"].ToString() == parentTagName)
                    {
                        if (tag["Content"][matchIndexInt]["E"].ToString() == matchParentValue)
                        {
                            parentTagObject = loopTagContents;
                            break;
                        }
                    }
                }

                if (parentTagObject != null)
                {
                    break;
                }
            }

            if (parentTagObject == null)
            {
                return string.Empty;
            }

            try
            {
                foreach (JObject tag in parentTagObject)
                {
                    if (tag["Name"].ToString() == findTagName)
                    {
                        return tag["Content"][returnIndexInt]["E"].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static string findFirstLoopTagValue(object tags, string findTagName, string matchFindIndex, string matchFindValue, string returnIndex)
        {
            int returnIndexInt = Convert.ToInt32(returnIndex.Trim());
            int matchIndexInt = Convert.ToInt32(matchFindIndex.Trim());
            JArray ediTags = null;
            bool isMatch = false;
            JArray parentTagObject = null;

            if (tags == null)
            {
                return string.Empty;
            }

            try
            {
                ediTags = JArray.Parse(JsonConvert.SerializeObject(tags));
            }
            catch (Exception)
            {
                ediTags = new JArray();
                ediTags.Add(JToken.Parse(JsonConvert.SerializeObject(tags)));
            }

            foreach (JObject loopTag in ediTags)
            {
                JArray loopTagContents = (JArray)loopTag["Content"];

                foreach (JObject tag in loopTagContents)
                {
                    if (tag["Name"].ToString() == findTagName)
                    {
                        if (tag["Content"][matchIndexInt]["E"].ToString() == matchFindValue)
                        {
                            return tag["Content"][returnIndexInt]["E"].ToString();
                        }
                    }
                }

                break;
            }

            return string.Empty;
        }

        public static string formatDate(string value, string format = "")
        {
            var cultureInfo = new CultureInfo("en-US");
            string dateValue = value;
            DateTime date = DateTime.MinValue;

            try
            {
                date = DateTime.ParseExact(dateValue, "yyyyMMdd", cultureInfo);
            }
            catch (Exception)
            {
                try
                {
                    date = DateTime.ParseExact(dateValue, "MM/dd/yyyy", cultureInfo);
                }
                catch (Exception)
                {
                    try
                    {
                        date = DateTime.ParseExact(dateValue, "dd/MM/yyyy", cultureInfo);
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
            }

            return date.ToString(string.IsNullOrEmpty(format) ? "yyyyMMdd" : format);
        }

        public static string formatTotalAmount(string value)
        {
            try
            {
                if(value.Contains("."))
                {
                    string[] nums = value.Split('.');

                    if (nums[1].Length == 1)
                    {
                        value = value.Replace(".", string.Empty) + "0";
                    }
                    else
                    {
                        value = value.Replace(".", string.Empty);
                    }
                }
                else
                {
                    value = value + "00";
                }
            }
            catch (Exception)
            {
            }

            return value;
        }

        public static JArray createSDQArray(object tags)
        {
            JArray ediTags = null;
            JArray locationTags = new JArray();

            if (tags == null)
            {
                return locationTags;
            }

            try
            {
                ediTags = JArray.Parse(JsonConvert.SerializeObject(tags));
            }
            catch (Exception)
            {
                ediTags = new JArray();
                ediTags.Add(JToken.Parse(JsonConvert.SerializeObject(tags)));
            }

            foreach (JObject sdqTag in ediTags)
            {
                JArray sdqTagContents = (JArray)sdqTag["Content"];
                int i = 2;

                while (i < sdqTagContents.Count)
                {
                    JObject locationTag = new JObject();

                    locationTag.Add("locationId", sdqTagContents[i]["E"]);
                    locationTag.Add("quantity", sdqTagContents[i+1]["E"]);

                    locationTags.Add(locationTag);
                    i += 2;
                }
            }

            return locationTags;
        }

        public static string getCurrentTime()
        {
            return DateTime.Now.ToString("HHmm");
        }
    }
}
