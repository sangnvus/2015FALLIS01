using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class MatchedSubstring
    {
        public int length { get; set; }
        public int offset { get; set; }
    }

    public class Term
    {
        public int offset { get; set; }
        public string value { get; set; }
    }

    public class Prediction
    {
        public string description { get; set; }
        public string id { get; set; }
        public IList<MatchedSubstring> matched_substrings { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public IList<Term> terms { get; set; }
        public IList<string> types { get; set; }
    }

    public class GoogleAPIQueryAutoCompleteObj
    {
        public IList<Prediction> predictions { get; set; }
        public string status { get; set; }
    }
}
