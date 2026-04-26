using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi
{
    public class NameService{
        public string GetName(string name)  // Метод с большой буквы
        {
            if(name == "" || name == null)
            {
                return "Аноним";
            }
            return name;
        }
    }
}