﻿namespace ParserContracts44
{
    public class ClearText
    {
        public static string ClearString(string s)
        {
            var st = s;
            st = st.Replace("ns2:", "");
            st = st.Replace("ns3:", "");
            st = st.Replace("ns4:", "");
            st = st.Replace("ns5:", "");
            st = st.Replace("ns6:", "");
            st = st.Replace("ns7:", "");
            st = st.Replace("ns8:", "");
            st = st.Replace("ns9:", "");
            st = st.Replace("ns10:", "");
            st = st.Replace("ns11:", "");
            st = st.Replace("ns12:", "");
            st = st.Replace("ns1:", "");
            st = st.Replace("oos:", "");
            return st;
        }
    }
}