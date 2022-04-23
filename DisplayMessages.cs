using System;
using TaleWorlds.Core;

namespace CookieLord
{
    public class DisplayMessages
    {
        public void ShowMessage(InformationMessage mes)
        {
            InformationManager.DisplayMessage(mes);
        }

        public InformationMessage CreateFormattedInfoMessage(string stringMes, object[] args)
        {
            return new InformationMessage(String.Format(stringMes, args));
        }

        public InformationMessage CreateBasicInfoMessage(string stringMes)
        {
            return new InformationMessage(stringMes);
        }
    }
}