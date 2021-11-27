using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySales.Shared
{
    public class ResponseForList
    {
        public string EntitiesPricipal { get; set; }
        public string EntitiesSecundary { get; set; }
        public int AllRegisters { get; set; }
        public int ActualPage { get; set; }
    }
}
