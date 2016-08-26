using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Test
{
    public class Arguments
    {
        private string line;
        private int position = 0;
        private const string SEPARATOR = " ";

        public Arguments(string line)
        {
            this.line = line;
        }

        public string Peak()
        {
            string next = "";
            if (position < line.Length - 1)
            {
                int pos = line.Substring(position).IndexOf(SEPARATOR);
                next = pos > 0 ? line.Substring(position, pos) : line.Substring(position);
            }
            return next;
        }

        public string ReadToEnd()
        {
            string next = "";
            if (position < line.Length - 1)
            {
                next = line.Substring(position);
                position = line.Length - 1;
            }
            return next;
        }

        public string Next() 
        {
            string next = "";
            if (position == line.Length - 1)
            {
                return next;
            }
            int pos = line.Substring(position).IndexOf(SEPARATOR);
            if (pos > 0) {
                next = line.Substring(position, pos);
                position = position + pos + 1;
            }
            else
            {
                next = line.Substring(position);
                position = line.Length - 1;
            }
            return next;
        }

        public int NextInt()
        {
            return int.Parse(Next());
        }

        public bool NextBool()
        {
            return bool.Parse(Next());
        }

        public bool IsNextInt()
        {
            int v;
            bool result = int.TryParse(Peak(), out v);
            return result;
        }

        public bool IsNextBool()
        {
            bool v;
            bool result = bool.TryParse(Peak(), out v);
            return result;
        }

        public bool Exit()
        {
            string current = Peak();
            return "x".Equals(current) || "exit".Equals(current);
        }
    }
}
