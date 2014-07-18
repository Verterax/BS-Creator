using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;
using System.IO;
using MSScriptControl;

namespace BranchingStoryCreator
{
    //Remember to make all objects COM visible with [ComVisible(true)] in System.Runtime.InteropServices;

    public class ScriptObject
    {

        public string objName;
        public object obj;

        public ScriptObject(string objName, object obj)
        {
            this.objName = objName;
            this.obj = obj;
        }
    }

    public class GameScript : ScriptControlClass
    {

        #region Variables
        public bool ScriptEnabled {get; set;} //Toggleable for debugging purposes.
        #endregion

        #region Consts

        public static string DIC = "dic";
        public static string TREE = "story";
        public static string BAG = "bag";
        public static string SOUND = "play";

        public static string LANGUAGE = "vbscript";


        #endregion

        #region Init / Load

        public GameScript(List<ScriptObject> scriptObjects)
        {
            Init(scriptObjects);
        }

        private void Init(List<ScriptObject> scriptObjects)
        {
            //Script
            ScriptEnabled = true;

            this.Language = LANGUAGE;
            RegisterObjs(scriptObjects); //Register sound object.
        }

        #endregion


        #region VBScript PreProcessessing.

        public string EvalShorthand(string expression)
        {
            if (ScriptEnabled)
            {
                expression = ReplaceDictionaryShorthand(expression);

                if (expression == "")
                    return "True";

                string resultStr = "";
                object result = null;
                try
                {
                    result = Eval(expression);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid evaluation: " + ex.Message);
                    resultStr = "false";
                }

                if (result is string)
                    resultStr = (string)result;
                else if (result is bool)
                    resultStr = ((bool)result).ToString();
                else if (result is int)
                    resultStr = ((int)result).ToString();
               

                if (expression != "")
                {
                    Console.WriteLine("Eval: {0} ", expression);
                    Console.WriteLine("Result: {0}", result);
                }


                return (resultStr == null) ? "True" : resultStr; //Instead of returning null on an empty expression, return true.  
            }            
            else
                return "True"; 

            //Returning True will ensure that AvailIf expression branches will always be available.
        }

        public void ExecuteStatementShorthand(string statement)
        {
            if (ScriptEnabled)
            {
                string longhand = ReplaceDictionaryShorthand(statement);
                try
                {
                    ExecuteStatement(longhand);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //throw ex;
                }
            }              
        }


        private static string ReplaceDictionaryShorthand(string statement)
        {
            //Replace [anything] with dic("anything")
            statement = statement.Replace("[", DIC + "(\"");
            statement = statement.Replace("]", "\")");
            return statement;
        }

        #endregion

        #region Native Functions

        private void RegisterObjs(List<ScriptObject> scriptObjects)
        {
            if (scriptObjects == null)
                return;

            foreach (ScriptObject item in scriptObjects)
                this.AddObject(item.objName, item.obj, true);
        }


        #endregion

        #region Errors

        public string GetError()
        {
            Error err = this.Error;
            return String.Format("{0} on line {1}, col {2}", err.Description, err.Line, err.Column);
        }

        #endregion

        #region Misc

        #endregion



    }
}
