using Blazored.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinarySales.Client.Shared
{
    public class SharedModalOptions
    {
        public static ModalOptions modalOptionsWait = new ModalOptions()
        {
            DisableBackgroundCancel = true,
            HideCloseButton = true,
            HideHeader = true
        };

        public static ModalOptions modalOptionsInfo = new ModalOptions()
        {
            DisableBackgroundCancel = true,
            ContentScrollable = true,
            Animation = ModalAnimation.FadeInOut(0.5)
        };


        public static ModalParameters SetParameterModalInfo(string msj, string cssClass)
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add(nameof(ModalInfo.Msj), msj);
            parameters.Add(nameof(ModalInfo.CssClass), cssClass);

            return parameters;
        }
    }
}
