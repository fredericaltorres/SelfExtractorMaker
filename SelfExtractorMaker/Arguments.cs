using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSugar;

namespace SelfExtractorMaker
{
    class Arguments {

        List<string> _arguments;

        public class ArgumentError {

            public string Message {
                get;
                set;
            }
            public bool Succeeded { 
                get {
                    return this.Message == null;
                }
            }
        }

        public ArgumentError ValidateRequiredParameter(params string [] parameters) {

            var a = new ArgumentError();

            foreach(var p in parameters)
                if(this.GetIndex(p) == -1)
                    a.Message = "parameter {0} is required".format(p);

            return a;
        }
        public int Count {
            get {
                return this._arguments.Count;
            }
        }
        public Arguments(string[] args) {

            _arguments = args.ToList();
        }
        public bool GetArgument(string name, bool defaultValue) {

            var v = this.GetArgument(name);
            return v == null ? defaultValue : Convert.ToBoolean(v);
        }
        public int GetArgument(string name, int defaultValue) {

            var v = this.GetArgument(name);
            return v == null ? defaultValue : Convert.ToInt32(v);
        }
        public string GetArgument(string name) {

            var i = this.GetIndex(name);
            if(i!=-1 && i<this._arguments.Count-1)
                return this._arguments[i+1];

            return null;
        }
        public bool Exists(string name) {

            return this.GetIndex(name) != -1;
        }
        private int GetIndex(string name) {

            for(var i=0; i<this._arguments.Count; i++) 
                if(this._arguments[i]==name)
                    return i;

            return -1;
        }
        public List<string> GetAll(string name) {

            var l = new List<string>();
            var x = this.GetIndex(name);

            if(x!=-1)
                for(var i=x+1; i<this._arguments.Count; i++)
                    l.Add(this._arguments[i]);

            return l;
        }
    }

}
