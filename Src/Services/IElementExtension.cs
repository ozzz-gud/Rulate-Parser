using AngleSharp.Dom;
using System.Collections.Generic;
using System.Linq;

namespace Src.Services
{
    public static class IElementExtension
    {
        public static List<IElement> GetFollowingSiblingSequence(this INode element, string tag = "p")
        {
            var _element = element as IElement;

            var list = new List<IElement>();
            var nextElem = _element.NextElementSibling;
            while (true)
            {
                if (nextElem != null && nextElem.TagName == tag.ToUpper())
                {
                    list.Add(nextElem);
                    nextElem = nextElem.NextElementSibling;
                }
                else
                    break;
            }
            return list;
        }

        public static List<string> GetFollowingSiblingSequenceText(this INode element, string tag = "p")
        {
            var sequence = element.GetFollowingSiblingSequence(tag);
            return sequence.Select((elem, _) => elem.TextContent).ToList();
        }
    }
}
