using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{

    /// <summary>
    /// Proxy interface for AssmeblyLoader.
    /// </summary>
    public interface IAssemblySandbox
    {
        void LoadAssembly(string name, byte[] bytes);
        byte[] ExecuteMethod(string assemblyName, string typeName, string methodName, string[] ConstructorTypes = null, object[] ConstructorParameters = null, string[] MethodTypes = null, object[] MethodParameters = null);


        bool ExecuteMethodAndStoreResults(string assemblyName, string assemblyQualifiedTypeName, string methodName, string variableName, string[] ConstructorTypes = null, object[] ConstructorParameters = null, string[] MethodTypes = null, object[] MethodParameters = null);
        bool ExecuteMethodOnVariable(string methodName, string targetVariableName, string returnVariableName, string[] MethodTypes = null, object[] MethodParameters = null);
        bool ConstructNewObject(string assemblyQualifiedTypeName, string variableName, string[] ConstructorTypes = null, object[] ConstructorParameters = null);
        bool SetVariable(string variableName, string assemblyQualifiedTypeName = "", byte[] serializedObject = null);
        byte[] GetVariable(string variableName);
        string GetVariableInfo(string variableName = "");

        
        //Helpers
        AppDomain CurrentDomain { get; }
        bool IsBusy { get; }
        Assembly[] LoadedAssemblies {get;}
        bool ExistsAssembly(string assemblyName);
    }
}
