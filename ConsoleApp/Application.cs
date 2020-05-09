using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

using CoreLib;

namespace ConsoleApp
{
    public enum RuleType
    {
        EchoMessageRule,
        GetUserListRule,
        ReadTextFileRule
    }
    public class Application
    {
        const string RULE_ASSEMBLY_FOLDER = "Rules";
        const string RULE_IMPLEMENT_TYPENAME = "Implementation";
        const string RULE_EXECUTE_METHOD = "Execute";
        const string REPORT_EXPORT_FOLDER = "Reports";

        public static Dictionary<RuleType, string> RuleTypeAssemblyMap = new Dictionary<RuleType, string>()
        {
            {RuleType.EchoMessageRule,"EchoMessageRule" },
            { RuleType.GetUserListRule,"GetUserListRule"},
            {RuleType.ReadTextFileRule, "ReadTextFileRule"}
        };
        public static bool EnabledPerformanceCounter = true;
        private static readonly PerformanceCounter _processMemoryCounter = !EnabledPerformanceCounter ? null : CreatePerformanceCounter();
        public static Regex GenericTypeRegex = new Regex(@"^(?<name>[\w\+]+(\.[\w|\+]+)*)(\&*)(\**)(`(?<count>\d))?(\[(?<subtypes>.*?)\])(,\s*(?<assembly>[\w\+]+(\.[\w|\+]+)*).*?)?$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

        // ##   User Cases  ##

        public static void AppDomainCreationMemoryCase(int instanceNum = 100)
        {
            var sandboxCreationTimes = new Dictionary<int, double>();
            var sandboxMemoryTraces = new Dictionary<int, double>();

            var parentDirectory = "AppDomainCreationMemoryCase";
            var exportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, parentDirectory);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);

            for (int i = 0; i < instanceNum; i++)
            {
                Console.WriteLine($"Creating sandbox: #{i} ...");
                var id = Guid.NewGuid();
                var stopwatch = Stopwatch.StartNew();

                var sandbox = AssemblySandboxManager.Create(id, id.ToString(), parentDirectory);

                sandboxCreationTimes[i] = MillisecondToSeconds(stopwatch.ElapsedMilliseconds);
                sandboxMemoryTraces[i] = GetProcessMemorySize();

                Console.WriteLine($"Sandbox #{i} -> {sandboxCreationTimes[i]} (s)");
                Console.WriteLine($"Sandbox #{i} -> {sandboxMemoryTraces[i]} (MB)");
            }

            var creationTimesReportFile = GetReportFilePath("appdomain_creation_time.csv");
            using (var sw = new StreamWriter(creationTimesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "time" }));
                foreach (var item in sandboxCreationTimes)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }

            var memoryTracesReportFile = GetReportFilePath("appdomain_creation_memory.csv");
            using (var sw = new StreamWriter(memoryTracesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "memory" }));
                foreach (var item in sandboxMemoryTraces)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }
        }
        public static void AppDomainSingleExecutionMemoryCase(RuleType ruleType, Dictionary<string, object> inputs, int taskNum = 100)
        {
            var sandboxExecuteTimes = new Dictionary<int, double>();
            var sandboxExecuteMemoryTraces = new Dictionary<int, double>();

            var parentDirectory = "AppDomainSingleExecutionMemoryCase";
            var exportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, parentDirectory);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);

            var assemblyName = RuleTypeAssemblyMap[ruleType];
            for (int i = 0; i < taskNum; i++)
            {
                Console.WriteLine($"Creating task: #{i} ...");
                var id = Guid.NewGuid();
                var stopwatch = Stopwatch.StartNew();
                var sandbox = AssemblySandboxManager.Create(id, id.ToString(), parentDirectory);

                // Load assembly into sandbox
                var assemblyBytes = GetAssemblyBytes(assemblyName);
                sandbox.LoadAssembly(assemblyName, assemblyBytes);

                // Execute assembly specified method
                byte[] serializedResult = sandbox.ExecuteMethod(
                    assemblyName, $"{assemblyName}.{RULE_IMPLEMENT_TYPENAME}", RULE_EXECUTE_METHOD,
                    ConstructorTypes: null, ConstructorParameters: null,
                    MethodTypes: new string[] { typeof(Dictionary<string, object>).AssemblyQualifiedName }, MethodParameters: new object[] { inputs });

                Console.WriteLine($"Killing task #{i} ...");
                AppDomain.Unload(sandbox.CurrentDomain);
                sandbox = null; GC.Collect();

                sandboxExecuteTimes[i] = MillisecondToSeconds(stopwatch.ElapsedMilliseconds);
                sandboxExecuteMemoryTraces[i] = GetProcessMemorySize();

                Console.WriteLine($"Task #{i} -> {sandboxExecuteTimes[i]} (s)");
                Console.WriteLine($"Task #{i} -> {sandboxExecuteMemoryTraces[i]} (MB)");
            }

            var executionTimesReportFile = GetReportFilePath($"appdomain_{assemblyName.ToLower()}_single_execution_time.csv");
            using (var sw = new StreamWriter(executionTimesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "time" }));
                foreach (var item in sandboxExecuteTimes)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }

            var executionMemoryTracesReportFile = GetReportFilePath($"appdomain_{assemblyName.ToLower()}_single_execution_memory.csv");
            using (var sw = new StreamWriter(executionMemoryTracesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "memory" }));
                foreach (var item in sandboxExecuteMemoryTraces)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }
        }
        public static void AppDomainSharedExecutionMemoryCase(int instanceNum, RuleType ruleType, Dictionary<string, object> inputs, int taskNum = 100)
        {
            var sandboxExecuteTimes = new Dictionary<int, double>();
            var sandboxExecuteMemoryTraces = new Dictionary<int, double>();
            var taskSandboxTraces = new Dictionary<int, string>();

            var parentDirectory = "AppDomainSharedExecutionMemoryCase";
            var exportDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, parentDirectory);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);

            Console.WriteLine($"Preparing sandboxes: <{instanceNum}> ...");
            var sharedSandboxes = new List<IAssemblySandbox>();
            //for (int i = 0; i < instanceNum; i++)
            //{
            //    var id = Guid.NewGuid();
            //    var sandbox = AssemblySandboxManager.Create(id, id.ToString(), parentDirectory);
            //    sharedSandboxes.Add(sandbox);
            //}

            var rand = new Random(1024);
            var assemblyName = RuleTypeAssemblyMap[ruleType];

            Console.WriteLine($"Runing tasks on sandboxes ...");
            for (int i = 0; i < taskNum; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                //var sandbox = sharedSandboxes[rand.Next(instanceNum)];

                //taskSandboxTraces[i] = sandbox.CurrentDomain.FriendlyName;
                //Console.WriteLine($"Using sandbox: <{taskSandboxTraces[i]}> for task: #{i} ...");

                var id = Guid.NewGuid();
                var sandbox = AssemblySandboxManager.Create(id, id.ToString(), parentDirectory);

                // Load assembly into sandbox
                var assemblyBytes = GetAssemblyBytes(assemblyName);
                sandbox.LoadAssembly(assemblyName, assemblyBytes);

                // Execute assembly specified method
                byte[] serializedResult = sandbox.ExecuteMethod(
                    assemblyName, $"{assemblyName}.{RULE_IMPLEMENT_TYPENAME}", RULE_EXECUTE_METHOD,
                    ConstructorTypes: null, ConstructorParameters: null,
                    MethodTypes: new string[] { typeof(Dictionary<string, object>).AssemblyQualifiedName }, MethodParameters: new object[] { inputs });

                sandboxExecuteTimes[i] = MillisecondToSeconds(stopwatch.ElapsedMilliseconds);
                sandboxExecuteMemoryTraces[i] = GetProcessMemorySize();

                Console.WriteLine($"Task #{i} -> {sandboxExecuteTimes[i]} (s)");
                Console.WriteLine($"Task #{i} -> {sandboxExecuteMemoryTraces[i]} (MB)");
            }

            foreach (var sandbox in sharedSandboxes)
                AppDomain.Unload(sandbox.CurrentDomain);

            GC.Collect(); //Forcing GC
            PrintMemoryUsage();

            var executionTimesReportFile = GetReportFilePath($"appdomain_{assemblyName.ToLower()}_shared_execution_time.csv");
            using (var sw = new StreamWriter(executionTimesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "time" }));
                foreach (var item in sandboxExecuteTimes)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }

            var executionMemoryTracesReportFile = GetReportFilePath($"appdomain_{assemblyName.ToLower()}_shared_execution_memory.csv");
            using (var sw = new StreamWriter(executionMemoryTracesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "memory" }));
                foreach (var item in sandboxExecuteMemoryTraces)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }

            var taskSandboxTracesReportFile = GetReportFilePath($"appdomain_{assemblyName.ToLower()}_shared_execution_task.csv");
            using (var sw = new StreamWriter(taskSandboxTracesReportFile, false))
            {
                sw.WriteLine(string.Join(",", new string[] { "id", "sandbox" }));
                foreach (var item in taskSandboxTraces)
                    sw.WriteLine(string.Join(",", new string[] { item.Key.ToString(), item.Value.ToString() }));

                sw.Flush();
            }
        }


        // ##   Publics   ##

        public static Dictionary<string, object> ExecuteRule(string assemblyName, Dictionary<string, object> inputs)
        {
            // Create assembly sandbox
            var id = Guid.NewGuid();
            var stopwatch = Stopwatch.StartNew();

            var sandbox = AssemblySandboxManager.Create(id);
            PrintMemoryUsage(sandbox.CurrentDomain, "CREATED");

            // Load assembly into sandbox
            var assemblyBytes = GetAssemblyBytes(assemblyName);
            sandbox.LoadAssembly(assemblyName, assemblyBytes);
            PrintMemoryUsage(sandbox.CurrentDomain, "LOADED");

            // Execute assembly specified method
            byte[] serializedResult = sandbox.ExecuteMethod(
                assemblyName, $"{assemblyName}.{RULE_IMPLEMENT_TYPENAME}", RULE_EXECUTE_METHOD,
                ConstructorTypes: null, ConstructorParameters: null,
                MethodTypes: new string[] { typeof(Dictionary<string, object>).AssemblyQualifiedName }, MethodParameters: new object[] { inputs });
            PrintMemoryUsage(sandbox.CurrentDomain, "EXECUTED");

            var result = Deserialize(serializedResult);
            var executionResult = result as Dictionary<string, object>;

            AppDomain.Unload(sandbox.CurrentDomain);
            sandbox = null; GC.Collect();
            PrintMemoryUsage(id, "UNLOADED");

            PrintTotalTime(stopwatch);
            return executionResult;
        }


        // ##   Privates    ##

        private static byte[] GetAssemblyBytes(string assemblyName)
        {
            var assemblyPath = GetRuleAssemblyPath(assemblyName);
            return File.ReadAllBytes(assemblyPath);
        }
        private static string GetRuleAssemblyPath(string assemblyName)
        {
            assemblyName = assemblyName.Split('.').First();
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RULE_ASSEMBLY_FOLDER, $"{assemblyName}.dll");

            return assemblyPath;
        }
        private static string GetReportFilePath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, REPORT_EXPORT_FOLDER, fileName);
        }
        private static PerformanceCounter CreatePerformanceCounter(string process = "ConsoleApp", string category = "Process", string counter = "Working Set - Private")
        {
            var performanceCounter = new PerformanceCounter();

            performanceCounter.CategoryName = category;
            performanceCounter.CounterName = counter;
            performanceCounter.InstanceName = process;

            return performanceCounter;
        }



        #region Serialization & Deserialization

        public static object Deserialize(byte[] byteArray)
        {
            try
            {
                BinaryFormatter binForm = new BinaryFormatter
                {
                    Binder = new BindChanger()
                };
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(byteArray, 0, byteArray.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return binForm.Deserialize(memoryStream);
                }
            }
            catch
            {
                return null;
            }
        }
        public static byte[] Serialize(object objectToSerialize)
        {
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, objectToSerialize);
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
        internal class BindChanger : System.Runtime.Serialization.SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Console.WriteLine($"BindToType: <{typeName} | {assemblyName}>");
                return ReconstructType(string.Format("{0}, {1}", typeName, assemblyName), false);

            }
        }


        private static Type ReconstructType(string typeAssemblyQualifiedName, bool throwOnError = false, params Assembly[] referencedAssemblies)
        {
            Type type = null;

            // If no assemblies were provided, then there wasn't an attempt to reconstruct the type from a specific assembly.
            // Check if the current app domain can be used to resolve the requested type (this should be 99% of calls for resolution).
            if (referencedAssemblies.Count() == 0)
            {
                type = Type.GetType(typeAssemblyQualifiedName, throwOnError);
                if (type != null)
                    return type;

                // If it made it here, populate an array of assemblies in the current app domain.
                referencedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            // If that failed, attempt to resolve the type from the list of supplied assemblies or those in the current app domain.
            foreach (Assembly asm in referencedAssemblies)
            {
                type = asm.GetType(typeAssemblyQualifiedName.Replace($", {asm.FullName}", ""), throwOnError);
                if (type != null)
                    return type;
            }

            // If that failed and the type looks like a generic type with assembly qualified type arguments, proceed with constructing a generic type.
            // TODO: follow the below TODO in ConstructGenericType because this if statement probably isn't accurate enough.
            Match match = GenericTypeRegex.Match(typeAssemblyQualifiedName);
            if (match.Success && !string.IsNullOrEmpty(match.Groups["count"].Value))
            {
                type = ConstructGenericType(typeAssemblyQualifiedName, throwOnError);
                if (type != null)
                    return type;
            }

            // At this point, just returns null;
            return type;
        }
        private static Type ConstructGenericType(string assemblyQualifiedName, bool throwOnError = false, params Assembly[] referencedAssemblies)
        {
            /// Modified the functionality of the regex and type resolution logic when handling cases like:
            ///     1: an assembly-qualified generic type
            ///         A: with only normal type arguments
            ///         B: with only assembly-qualified type arguments
            ///         C: with a mixture of both normal and assembly-qualified type arguments
            ///     2: a generic type
            ///         A: with only normal type arguments
            ///         B: with only assembly-qualified type arguments
            ///         C: with a mixture of both normal and assembly-qualified type arguments
            ///         
            ///     I think it's possible to have a type with normal and assembly-qualified arguments, but I'm not sure.
            ///     I'm also not skilled enough to develop test cases for each of the scenarios addressed here.
            ///     Reference: https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype?view=netframework-3.5
            ///

            Match match = GenericTypeRegex.Match(assemblyQualifiedName);

            if (!match.Success)
                return null;

            string typeName = match.Groups["name"].Value.Trim();
            string typeArguments = match.Groups["subtypes"].Value.Trim();

            // If greater than 0, this is a generic type with this many type arguments.
            int numberOfTypeArguments = -1;
            if (!string.IsNullOrEmpty(match.Groups["count"].Value.Trim()))
            {
                try
                {
                    numberOfTypeArguments = int.Parse(match.Groups["count"].Value.Trim());
                }
                catch { };
            }

            // I guess this attempts to get the default type for a type of typeName for a given numberOfTypeArguments.
            // Seems to work on commonly configured.
            if (numberOfTypeArguments >= 0)
                typeName = typeName + $"`{numberOfTypeArguments}";

            Type genericType = ReconstructType(typeName, throwOnError, referencedAssemblies);
            if (genericType == null)
                return null;

            //List<string> typeNames = new List<string>();
            List<Type> TypeList = new List<Type>();

            int StartOfArgument = 0;
            int offset = 0;
            while (offset < typeArguments.Length)
            {
                // All type arguments are separated by commas.
                // Parsing would be easy, except square brackets introduce scoping.

                // If a left square bracket is encountered, start parsing until the matching right bracket is reached.
                if (typeArguments[offset] == '[')
                {
                    int end = offset;
                    int level = 0;
                    do
                    {
                        switch (typeArguments[end++])
                        {
                            // If the next character is a left square bracket, the beginning of another bracket pair was encountered.
                            case '[':
                                level++;
                                break;

                            // Else if it's a right bracket, the end of a bracket pair was encountered.
                            case ']':
                                level--;
                                break;
                        }
                    } while (level > 0 && end < typeArguments.Length);

                    // 'offset' is still the index of the encountered left square bracket.
                    // 'end' is now the index of the closing right square bracket.
                    // 'level' should be back at zero (meaning all left brackets had closing right brackets). Else there was a formatting error.
                    if (level == 0)
                    {
                        // Adding 1 to the offset and subtracting two from the substring length will get a substring without the brackets.
                        // Check that the substring length, sans the enclosing brackets, would result in a non-empty string.
                        if ((end - offset - 2) > 0)
                        {
                            // If the start of the first type argument was the left square bracket, this argument is an assembly-qualified type.
                            //  Example:    MyGenericType`1[[MyType,MyAssembly]]
                            if (StartOfArgument == offset)
                            {
                                try
                                {
                                    TypeList.Add(ReconstructType(typeArguments.Substring(offset + 1, end - offset - 2).Trim(), throwOnError, referencedAssemblies));
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                            // Else a square bracket was encountered on a generic type argument.
                            //  Example:    MyGenericType`1[AnotherGenericType`2[MyType,AnotherType]]
                            else
                            {
                                try
                                {
                                    TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, end - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
                                }
                                catch
                                {
                                    return null;
                                }
                            }
                        }
                    }

                    // Set the offset and StartOfArgument to the position of the discovered right square bracket (or the end of the string).
                    offset = end;
                    StartOfArgument = offset;

                    // Decrement the number of type arguments 
                    numberOfTypeArguments--;
                }

                // Else if a comma is encountered without hitting a left square bracket, a normal type argument was encountered.
                // I don't know if this will ever happen because these types should always be resolvable, I think.
                else if (typeArguments[offset] == ',')
                {
                    if ((offset - StartOfArgument) > 0)
                    {
                        try
                        {
                            TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, offset - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
                        }
                        catch
                        {
                            return null;
                        }
                    }

                    offset++;
                    StartOfArgument = offset;
                }

                // Essentially adds the character at this offset to any substring produced with the StartOfArgument offset.
                else
                    offset++;
            }

            // 'offset' is out-of-bounds. 'StartOfArgument' may be out-of-bounds. 
            // 'offset-1' should be in-bounds, and if it's greater than 'StartOfArgument', there should be one last type argument to create.
            if ((offset - 1) > StartOfArgument)
            {
                try
                {
                    TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, offset - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
                }
                catch
                {
                    return null;
                }
            }

            // "Should never happen" --original StackOverflow author
            // This should only happen if the number of type arguments supplied in the type string doesn't match with the number of supplied arguments.
            // If it's less than 0, 
            if (numberOfTypeArguments > 0)
                return null;

            try
            {
                return genericType.MakeGenericType(TypeList.ToArray());
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Helpers
        private static double GetProcessMemorySize()
        {
            return _processMemoryCounter.NextValue() / (1024.00 * 1024.00);
        }
        private static void PrintMemoryUsage(AppDomain appDomain, string stageLabel = "CREATED")
        {
            if (EnabledPerformanceCounter)
            {
                var processMemorySize = _processMemoryCounter.NextValue() / (1024.00 * 1024.00);

                Console.WriteLine();
                Console.WriteLine($" ******** {appDomain.FriendlyName.ToUpper()} | {stageLabel} ******** ");
                Console.WriteLine();
                Console.WriteLine($" # Survived Memory:        {BytesToMegaBytes(appDomain.MonitoringSurvivedMemorySize)} MB");
                Console.WriteLine($" # Total Allocated Memory: {BytesToMegaBytes(appDomain.MonitoringTotalAllocatedMemorySize)} MB");
                Console.WriteLine($" # Total Processor Time:   {appDomain.MonitoringTotalProcessorTime.TotalSeconds} s");
                Console.WriteLine($" # Process Memory Size:    {processMemorySize} MB");
                Console.WriteLine();
                Console.WriteLine($" ***************************************************************** ");
                Console.WriteLine();
            }
        }
        private static void PrintMemoryUsage(Guid sandboxId, string stageLabel = "UNLOADED")
        {
            if (EnabledPerformanceCounter)
            {
                var processMemorySize = _processMemoryCounter.NextValue() / (1024.00 * 1024.00);

                Console.WriteLine();
                Console.WriteLine($" ******** {sandboxId.ToString().ToUpper()} | {stageLabel} ******** ");
                Console.WriteLine();
                Console.WriteLine($" # Process Memory Size:    {processMemorySize} MB");
                Console.WriteLine();
                Console.WriteLine($" ***************************************************************** ");
                Console.WriteLine();
            }
        }
        public static void PrintMemoryUsage(string processName = "ConsoleApp", string stageLabel = "CURRENT")
        {
            if (EnabledPerformanceCounter)
            {
                var processMemorySize = _processMemoryCounter.NextValue() / (1024.00 * 1024.00);

                Console.WriteLine();
                Console.WriteLine($" ******** {processName.ToUpper()} | {stageLabel} ******** ");
                Console.WriteLine();
                Console.WriteLine($" # Process Memory Size:    {processMemorySize} MB");
                Console.WriteLine();
            }
        }

        private static void PrintTotalTime(Stopwatch stopwatch)
        {
            var seconds = stopwatch.ElapsedMilliseconds / 1000.00;

            Console.WriteLine();
            Console.WriteLine($">>>>>>>>>>>>>>>>>>> 【TOTAL TIME: {seconds} (s)】 <<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine();
        }

        private static double BytesToMegaBytes(long bytes)
        {
            return bytes / (1024.00 * 1024.00);
        }
        private static double MillisecondToSeconds(long ms)
        {
            return ms / 1000.00;
        }
        #endregion
    }


}
