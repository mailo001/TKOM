using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKOM.Common
{
    public class Getter
    {
        int? _value;

        public Getter()
        {
            _value = null;
        }

        public int GetAndClear() 
        {
            if (_value == null)
                throw new Exception("Try get argument that has not value");
            int v = _value.Value; 
            _value = null; 
            return v; 
        } 

        public bool IsNull() 
        {
            return _value == null; 
        }

        public void Set(int v) 
        { 
            _value = v;
        }

        public void Clear()
        {
            _value = null;
        }

    }
}
