using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinarySales.Client.Helpers
{
    public class IJSExtensions
    {
        private readonly IJSRuntime js;

        public IJSExtensions(IJSRuntime js)
        {
            this.js = js;
        }

        public ValueTask<object> SetInLocalStorage(string key, string content)
        {
            return js.InvokeAsync<object>("localStorage.setItem", key, content);
        }

        public ValueTask<string> GetFromLocalStorage(string key)
        {
            return js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public ValueTask<object> RemoveItem(string key)
        {
            return js.InvokeAsync<object>("localStorage.removeItem", key);
        }
    }
}
