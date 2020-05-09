//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Security;
//using System.Security.Permissions;
//using System.Text.RegularExpressions;
//using System.Threading;

//using CoreLib;

//namespace ConsoleApp
//{
//    class Program
//    {
//        public static byte[] DllBytes = new byte[] { };// File.ReadAllBytes(@"SharpSploit.dll");

//        public static Regex GenericTypeRegex = new Regex(@"^(?<name>[\w\+]+(\.[\w|\+]+)*)(\&*)(\**)(`(?<count>\d))?(\[(?<subtypes>.*?)\])(,\s*(?<assembly>[\w\+]+(\.[\w|\+]+)*).*?)?$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

//        public static AppDomain GetNewAppDomain(Guid Id)
//        {
//            var applicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Application");
//            var privateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Application", "bin");
//            var shadowCopyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Application", "Work");

//            AppDomainSetup appDomainSetup = new AppDomainSetup
//            {
//                ApplicationBase = applicationBase,
//                //PrivateBinPath = privateBinPath,
//                //ShadowCopyFiles = "true",
//                //ShadowCopyDirectories = shadowCopyDirectory
//            };
//            PermissionSet appDomainPermissions = new PermissionSet(PermissionState.Unrestricted);
//            var appDomain = AppDomain.CreateDomain(Id.ToString(), null, appDomainSetup, appDomainPermissions, null);

//            appDomain.AssemblyLoad += AppDomain_AssemblyLoad;
//            //appDomain.AssemblyResolve += AppDomain_AssemblyResolve;
//            //appDomain.ReflectionOnlyAssemblyResolve += AppDomain_ReflectionOnlyAssemblyResolve;

//            return appDomain;
//        }

//        private static void AppDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
//        {
//            Console.WriteLine($"===========>  {args.LoadedAssembly.FullName}");
//        }
//        private static Assembly AppDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
//        {
//            return Assembly.Load(args.Name);
//        }
//        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
//        {
//            //var libraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries");
//            //var assemblyName = args.Name;
//            //return AppDomain.CurrentDomain.Load(Path.Combine(libraryPath, assemblyName));
//            return Assembly.Load(args.Name);
//        }

//        public static IAssemblySandbox GetAssemeblySandbox(AppDomain appDomain)
//        {
//            Type assmeblySandboxType = typeof(AssemblySandbox);
//            return (IAssemblySandbox)appDomain.CreateInstanceFromAndUnwrap(assmeblySandboxType.Assembly.Location, assmeblySandboxType.FullName);
//        }


//        static void Main()
//        {
//            //Enabling Resource Monitoring
//            AppDomain.MonitoringIsEnabled = true;


//            //PrintMemoryUsage(AppDomain.CurrentDomain);

//            // Creating a lil break here for those following along with a debugger.
//            //Console.WriteLine("Press Return to start.");
//            //Console.ReadLine();

//            // The next three commented lines of code can be uncommented, for debugging purposes.
//            // Check loaded Assemblies before loading SharpSploit into the main AppDomain.
//            //Assembly[] AssembliesBeforeLoad = AppDomain.CurrentDomain.GetAssemblies();

//            // Load the SharpSploit DLL into the main AppDomain, for funsies.
//            //Assembly SharpSploit = Assembly.Load(DllBytes);

//            // Check loaded Assemblies after loading SharpSploit into the main AppDomain.
//            //Assembly[] AssembliesAfterLoad = AppDomain.CurrentDomain.GetAssemblies();

//            // When creating a new AppDomain, a friendly name is required, for later reference. 
//            // GUID's are used to avoid any sort of creative thought or telling name.
//            //Guid SandboxId = Guid.NewGuid();

//            // Create the AppDomain using the above GUID as the friendly name.

//            //while (true)
//            //{
//            //    var appCount = 100;
//            //    var memHistories = new List<Tuple<int, double>>();
//            //    var appDomianHistories = new Dictionary<Guid, AppDomain>();
//            //    for (int i = 0; i < appCount; i++)
//            //    {
//            //        var id = Guid.NewGuid();
//            //        var ad = AssemblySandboxManager.CreateDomain(id, "Application");
//            //        var box = AssemblySandboxManager.BuildSandbox(ad);

//            //        var processMem = GetProcessMemorySize();
//            //        //memHistories.Add(new Tuple<int, double>(i, processMem));
//            //        //appDomianHistories.Add(id, ad);
//            //        Console.WriteLine($"Created: {i}/{processMem} MB");

//            //        DllBytes = File.ReadAllBytes("Libraries/ClassLibrary1.dll");
//            //        box.Load("ClassLibrary1", DllBytes);

//            //        byte[] returnValue = box.ExecuteMethod(
//            //            "ClassLibrary1", "ClassLibrary1.MyClass", "Echo",
//            //            ConstructorTypes: null, ConstructorParameters: null,
//            //            MethodTypes: new string[] { typeof(string).AssemblyQualifiedName }, MethodParameters: new object[] { "HelloWorld" });

//            //        processMem = GetProcessMemorySize();
//            //        Console.WriteLine($"Executed: {i}/{processMem} MB");

//            //        AppDomain.Unload(ad);
//            //        ad = null;
//            //        GC.Collect();

//            //        processMem = GetProcessMemorySize();
//            //        Console.WriteLine($"Unloaded: {i}/{processMem} MB");

//            //        Thread.Sleep(1000);
//            //    }

//            //    //Console.ReadLine();

//            //    //var num = 0;
//            //    //foreach (var item in appDomianHistories.ToList())
//            //    //{
//            //    //    AppDomain.Unload(appDomianHistories[item.Key]);
//            //    //    appDomianHistories[item.Key] = null;
//            //    //    appDomianHistories.Remove(item.Key);
//            //    //    GC.Collect();

//            //    //    ++num;
//            //    //    var processMem = GetProcessMemorySize();
//            //    //    Console.WriteLine($"{num}/{processMem} MB ");

//            //    //    Thread.Sleep(1000);
//            //    //}

//            //    Console.WriteLine(" Press Enter to loop again... ");
//            //    Console.ReadLine();
//            //}
            

//            //AppDomain appDomain2 = SandboxManager.CreateDomain(Guid.NewGuid(), "Application2");
//            //PrintMemoryUsage(appDomain2);

//            // Use this new AppDomain to get an interface to the AssemblySandbox execution class.
//            //IAssemblySandbox assmeblySandbox = GetAssemeblySandbox(appDomain);

//            //AppDomain appDomain = AssemblySandboxManager.CreateDomain(Guid.NewGuid(), "Application1");
//            //PrintMemoryUsage(appDomain);

//            //IAssemblySandbox assmeblySandbox = AssemblySandboxManager.BuildSandbox(appDomain);

//            //// Place a breakpoint in AssemblySandbox.CheckLoadedAssemblies() and uncomment below to verify the Assembly isn't loaded.
//            //var loadedAssemblies = assmeblySandbox.CheckLoadedAssemblies();
//            //PrintMemoryUsage(appDomain);

//            // ## Demo #1: Loading a DLL into the AssemblySandbox, and calling a method from the Assembly.
//            Console.WriteLine("\r\n-- Demo #1 -------------------------------------------------------------------");
//            // We're going to load the SharpSploit DLL and call the non-static method SharpSploit.Credentials.Tokens.WhoAmI().
//            // This isn't the best way to do this (see Demo #3), but it covers how both static and non-static Methods are called.
//            // It's also a product of me developing something for a specific, arbitrary use-case, and I need to rework how calls
//            // to non-static methods are made.

//            // Load SharpSploit into the AssemblySandbox, with name "SharpSploit".
//            //Console.WriteLine("[*] Loading the SharpSploit DLL...");

//            //DllBytes = File.ReadAllBytes("Libraries/ClassLibrary1.dll");
//            //assmeblySandbox.Load("ClassLibrary1", DllBytes);
//            //PrintMemoryUsage(appDomain);

//            //// Place a breakpoint in AssemblySandbox.CheckLoadedAssemblies() and uncomment below to verify the Assembly isn't loaded.
//            //loadedAssemblies = assmeblySandbox.LoadedAssemblies;

//            // Place a breakpoint in AssemblySandbox.CheckLoadedAssemblies() and uncomment below to now verify the Assembly was loaded.
//            //assmeblySandbox.CheckLoadedAssemblies();

//            // Execute the non-static method WhoAmI from the loaded instance of SharpSploit, in the AssemblySandbox.
//            Console.WriteLine("[*] Executing the non-static SharpSploit.Credentials.Tokens.WhoAmI() method from the SharpSploit DLL...");
//            byte[] serializedReturnValueWhoAmI = assmeblySandbox.ExecuteMethod(
//                "ClassLibrary1", "ClassLibrary1.MyClass", "Echo",
//                ConstructorTypes: null, ConstructorParameters: null,
//                MethodTypes: new string[] { typeof(string).AssemblyQualifiedName }, MethodParameters: new object[] { "HelloWorld" });

//            PrintMemoryUsage(appDomain);
//            PrintMemoryUsage(AppDomain.CurrentDomain);

//            // Deserialize the return value. We are assuming the return value is a Type understood in this main AppDomain (i.e. string)
//            var whoAmI = Deserialize(serializedReturnValueWhoAmI);

//            // Print deserialized value, and hit return for next demo.
//            Console.WriteLine("[+] WhoAmI Results: \"{0}\"\r\n", whoAmI);

//            PrintMemoryUsage(appDomain);
//            PrintMemoryUsage(AppDomain.CurrentDomain);

//            //AppDomain.Unload(appDomain);
//            //appDomain = null;
//            //GC.Collect();

//            var readString = File.ReadAllText("log.txt");
//            serializedReturnValueWhoAmI = assmeblySandbox.ExecuteMethod(
//                "ClassLibrary1", "ClassLibrary1.MyClass", "Echo",
//                ConstructorTypes: null, ConstructorParameters: null,
//                MethodTypes: new string[] { typeof(string).AssemblyQualifiedName }, MethodParameters: new object[] { readString });

//            PrintMemoryUsage(appDomain);
//            PrintMemoryUsage(AppDomain.CurrentDomain);

//            AppDomain.Unload(appDomain);
//            appDomain = null;
//            GC.Collect();

//            Console.WriteLine("Press Return to continue...");
//            //PrintMemoryUsage(appDomain);

//            PrintMemoryUsage(AppDomain.CurrentDomain);


//            // ## Demo #2: Passing commonly-Typed variables to a method in the AssemblySandbox.
//            Console.WriteLine("\r\n-- Demo #2 -------------------------------------------------------------------");
//            // We're going to do a simple, static String.Join() on a string-array, akin to String.Join(",", ["a", "b"]) => "a,b"
//            // It's important to understand that this is only possible because we're passing basic Types and calling Methods that exist,
//            // by default, in both application domains; all Types and Methods referenced can be resolved in each domain, given the
//            // assemblies currently loaded.

//            // First, define the types of the specific Method we want to call.
//            List<string> MethodTypes = new List<string>() { "System.String", "System.String[]" };

//            // We want to join the strings "a" and "b" with a delimiter of ",".
//            List<object> MethodParameters = new List<object>() { ",", new string[] { "a", "b" } };

//            // Call the Method on the parameters, and get the serialized result.
//            Console.WriteLine("[*] Executing \'System.String.Join(\",\", new string[] { \"a\", \"b\" })\' within the AssemblySandbox AppDomain...");
//            byte[] serializedReturnValueJoin = assmeblySandbox.ExecuteMethod(null, "System.String", "Join", null, null, MethodTypes.ToArray(), MethodParameters.ToArray());

//            // Deserialize the result into an Object of a Type that we're assuming to be a String.
//            object actualReturnValueJoin = Deserialize(serializedReturnValueJoin);

//            Console.WriteLine("[+] Join result: \"{0}\"", actualReturnValueJoin);
//            Console.WriteLine("Press Return to continue...");
//            Console.ReadLine();



//            // ## Demo #3: Storing and referencing variables and return values in the AssemblySandbox.
//            Console.WriteLine("\r\n-- Demo #3 -------------------------------------------------------------------");
//            // We're going to store a Typed value in the AssemblySandbox's variable Dictionary, call a Method on it, and store
//            // the return value in the AssemblySandbox's variable Dictionary. This is probably the correct way to call a method
//            // on an instance of a potentially unknown class (a class that only exists within the AssemblySandbox).
//            // We're creating a string-array, of size 5, and calling SetValue("asdf", 0), setting the first element to "asdf".

//            // First, define the variable Types of the string-array constructor we want (new string[5]).
//            List<string> ConstructorTypes = new List<string>() { "System.Int32" };

//            // Create a new string-array, of size 5, and store it as a variable named "stringArrayVar" in the AssemblySandbox.
//            // If everything initializes correctly, AssmeblySandbox.ConstructNewObject returns true.
//            Console.WriteLine("[*] Creating a new string[5] and storing it as variable \"stringArrayVar\" inside the AssemblySandbox AppDomain...");
//            bool successfullyCreated = assmeblySandbox.ConstructNewObject("System.String[]", "stringArrayVar", ConstructorTypes.ToArray(), new object[] { 5 });

//            // Execute SetValue on our stored string-array "stringArrayVar", setting the object at index 0 to "asdf", and store
//            // the return value as a new variable named "returnVar" (which will be null because SetValue returns void).
//            // AssmeblySandbox.ExecuteMethodOnVariable returns True if the Method successfully executed on the variable.
//            Console.WriteLine("[*] Executing \'returnVar = stringArrayVar.SetValue(\"asdf\", 0)\' within the AssemblySandbox AppDomain...");
//            bool executeMethodWasSuccessful = assmeblySandbox.ExecuteMethodOnVariable("SetValue", "stringArrayVar", "returnVar", new string[] { "System.Object", "System.Int32" }, new object[] { "asdf", 0 });

//            // Get and deserialize variable "stringArrayVar", which should become a string-array with the Object at index 0 set to "asdf'.
//            string[] stringArrayReturnValue = (string[])Deserialize(assmeblySandbox.GetVariable("stringArrayVar"));
//            Console.WriteLine("stringArrayReturnValue type: {0}", stringArrayReturnValue.GetType());
//            Console.WriteLine("stringArrayReturnValue[0]:  \"{0}\"\r\n", stringArrayReturnValue[0]);

//            // AssmeblySandbox.GetVariableInfo returns a formatted string, displaying info about the stored variables.
//            string variableInfo = assmeblySandbox.GetVariableInfo();
//            Console.WriteLine("Variables Stored in the AssemblySandbox:\r\n========================================");
//            Console.WriteLine(variableInfo);
//            Console.WriteLine();
//            // This is probably how Demo #1 should have been done - creating a new instance of Token, storing it as a variable in
//            // the AssemblySandbox, and calling the WhoAmI Method on the variable.



//            // We're donezo. Unload the AppDomain that the AssemblySandbox was using.
//            Console.WriteLine("Press Return to unload the AssemblySandbox's AppDomain...");
//            Console.ReadLine();

//            AppDomain.Unload(appDomain);

//            // Place a breakpoint in AssemblySandbox.CheckLoadedAssemblies() and uncomment below to verify everything is gone.
//            //assmeblySandbox.CheckLoadedAssemblies();

//            Console.WriteLine("All done. Press Return to exit...");
//            Console.ReadLine();
//        }


//        public static void PrintMemoryUsage(AppDomain appDomain)
//        {
//            var processes = Process.GetProcessesByName("ConsoleApp");
//            var process = processes.Any() ? processes.First() : null;

//            Console.WriteLine();
//            Console.WriteLine($"|===================== {appDomain.FriendlyName} =====================|");
//            Console.WriteLine($"SurvivedMemory: {BytesToMegaBytes(appDomain.MonitoringSurvivedMemorySize)} MB");
//            Console.WriteLine($"TotalAllocatedMemory: {BytesToMegaBytes(appDomain.MonitoringTotalAllocatedMemorySize)} MB");
//            Console.WriteLine($"SurvivedProcessMemory: {BytesToMegaBytes(AppDomain.MonitoringSurvivedProcessMemorySize)} MB");
//            Console.WriteLine($"TotalProcessorTime: {appDomain.MonitoringTotalProcessorTime.TotalSeconds} s");
//            Console.WriteLine($"ProcessPagedMemorySize: {BytesToMegaBytes(process == null ? 0 : process.PagedMemorySize64)} MB");
//            Console.WriteLine($"ProcessMemorySize: {GetProcessMemorySize()} MB");
//            //Console.WriteLine($"ProcessMemorySize: {BytesToMegaBytes(process == null ? 0 : process.PrivateMemorySize64)} MB");
//            Console.WriteLine($"<===================== {appDomain.FriendlyName} =====================>");
//        }

//        public static double GetProcessMemorySize()
//        {
//            //PerformanceCounter PC = new PerformanceCounter();
//            //PC.CategoryName = "Process";
//            //PC.CounterName = "Working Set - Private";
//            //PC.InstanceName = processName;
//            //var memsize = PC.NextValue() / (1024.00 * 1024.00);
//            //PC.Close();
//            //PC.Dispose();
//            //return memsize;

//            var size = performanceCounter.NextValue() / (1024.00 * 1024.00);
//            return size;
//        }

//        public static object Deserialize(byte[] byteArray)
//        {
//            try
//            {
//                BinaryFormatter binForm = new BinaryFormatter
//                {
//                    Binder = new BindChanger()
//                };
//                using (var memoryStream = new MemoryStream())
//                {
//                    memoryStream.Write(byteArray, 0, byteArray.Length);
//                    memoryStream.Seek(0, SeekOrigin.Begin);
//                    return binForm.Deserialize(memoryStream);
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        public static byte[] Serialize(object objectToSerialize)
//        {
//            try
//            {
//                BinaryFormatter serializer = new BinaryFormatter();
//                using (var memoryStream = new MemoryStream())
//                {
//                    serializer.Serialize(memoryStream, objectToSerialize);
//                    return memoryStream.ToArray();
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }


//        public class BindChanger : System.Runtime.Serialization.SerializationBinder
//        {
//            public override Type BindToType(string assemblyName, string typeName)
//            {
//                Console.WriteLine();
//                Console.WriteLine($"============================ BindChanger ============================");
//                Console.WriteLine($">>>>>   {typeName}, {assemblyName}");
//                Console.WriteLine($"============================ BindChanger ============================");
//                return ReconstructType(string.Format("{0}, {1}", typeName, assemblyName), false);

//            }
//        }

//        public static Type ReconstructType(string typeAssemblyQualifiedName, bool throwOnError = false, params Assembly[] referencedAssemblies)
//        {
//            Type type = null;

//            // If no assemblies were provided, then there wasn't an attempt to reconstruct the type from a specific assembly.
//            // Check if the current app domain can be used to resolve the requested type (this should be 99% of calls for resolution).
//            if (referencedAssemblies.Count() == 0)
//            {
//                type = Type.GetType(typeAssemblyQualifiedName, throwOnError);
//                if (type != null)
//                    return type;

//                // If it made it here, populate an array of assemblies in the current app domain.
//                referencedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
//            }

//            // If that failed, attempt to resolve the type from the list of supplied assemblies or those in the current app domain.
//            foreach (Assembly asm in referencedAssemblies)
//            {
//                type = asm.GetType(typeAssemblyQualifiedName.Replace($", {asm.FullName}", ""), throwOnError);
//                if (type != null)
//                    return type;
//            }

//            // If that failed and the type looks like a generic type with assembly qualified type arguments, proceed with constructing a generic type.
//            // TODO: follow the below TODO in ConstructGenericType because this if statement probably isn't accurate enough.
//            Match match = GenericTypeRegex.Match(typeAssemblyQualifiedName);
//            if (match.Success && !string.IsNullOrEmpty(match.Groups["count"].Value))
//            {
//                type = ConstructGenericType(typeAssemblyQualifiedName, throwOnError);
//                if (type != null)
//                    return type;
//            }

//            // At this point, just returns null;
//            return type;
//        }

//        private static Type ConstructGenericType(string assemblyQualifiedName, bool throwOnError = false, params Assembly[] referencedAssemblies)
//        {
//            /// Modified the functionality of the regex and type resolution logic when handling cases like:
//            ///     1: an assembly-qualified generic type
//            ///         A: with only normal type arguments
//            ///         B: with only assembly-qualified type arguments
//            ///         C: with a mixture of both normal and assembly-qualified type arguments
//            ///     2: a generic type
//            ///         A: with only normal type arguments
//            ///         B: with only assembly-qualified type arguments
//            ///         C: with a mixture of both normal and assembly-qualified type arguments
//            ///         
//            ///     I think it's possible to have a type with normal and assembly-qualified arguments, but I'm not sure.
//            ///     I'm also not skilled enough to develop test cases for each of the scenarios addressed here.
//            ///     Reference: https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype?view=netframework-3.5
//            ///

//            Match match = GenericTypeRegex.Match(assemblyQualifiedName);

//            if (!match.Success)
//                return null;

//            string typeName = match.Groups["name"].Value.Trim();
//            string typeArguments = match.Groups["subtypes"].Value.Trim();

//            // If greater than 0, this is a generic type with this many type arguments.
//            int numberOfTypeArguments = -1;
//            if (!string.IsNullOrEmpty(match.Groups["count"].Value.Trim()))
//            {
//                try
//                {
//                    numberOfTypeArguments = int.Parse(match.Groups["count"].Value.Trim());
//                }
//                catch { };
//            }

//            // I guess this attempts to get the default type for a type of typeName for a given numberOfTypeArguments.
//            // Seems to work on commonly configured.
//            if (numberOfTypeArguments >= 0)
//                typeName = typeName + $"`{numberOfTypeArguments}";

//            Type genericType = ReconstructType(typeName, throwOnError, referencedAssemblies);
//            if (genericType == null)
//                return null;

//            //List<string> typeNames = new List<string>();
//            List<Type> TypeList = new List<Type>();

//            int StartOfArgument = 0;
//            int offset = 0;
//            while (offset < typeArguments.Length)
//            {
//                // All type arguments are separated by commas.
//                // Parsing would be easy, except square brackets introduce scoping.

//                // If a left square bracket is encountered, start parsing until the matching right bracket is reached.
//                if (typeArguments[offset] == '[')
//                {
//                    int end = offset;
//                    int level = 0;
//                    do
//                    {
//                        switch (typeArguments[end++])
//                        {
//                            // If the next character is a left square bracket, the beginning of another bracket pair was encountered.
//                            case '[':
//                                level++;
//                                break;

//                            // Else if it's a right bracket, the end of a bracket pair was encountered.
//                            case ']':
//                                level--;
//                                break;
//                        }
//                    } while (level > 0 && end < typeArguments.Length);

//                    // 'offset' is still the index of the encountered left square bracket.
//                    // 'end' is now the index of the closing right square bracket.
//                    // 'level' should be back at zero (meaning all left brackets had closing right brackets). Else there was a formatting error.
//                    if (level == 0)
//                    {
//                        // Adding 1 to the offset and subtracting two from the substring length will get a substring without the brackets.
//                        // Check that the substring length, sans the enclosing brackets, would result in a non-empty string.
//                        if ((end - offset - 2) > 0)
//                        {
//                            // If the start of the first type argument was the left square bracket, this argument is an assembly-qualified type.
//                            //  Example:    MyGenericType`1[[MyType,MyAssembly]]
//                            if (StartOfArgument == offset)
//                            {
//                                try
//                                {
//                                    TypeList.Add(ReconstructType(typeArguments.Substring(offset + 1, end - offset - 2).Trim(), throwOnError, referencedAssemblies));
//                                }
//                                catch
//                                {
//                                    return null;
//                                }
//                            }

//                            // Else a square bracket was encountered on a generic type argument.
//                            //  Example:    MyGenericType`1[AnotherGenericType`2[MyType,AnotherType]]
//                            else
//                            {
//                                try
//                                {
//                                    TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, end - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
//                                }
//                                catch
//                                {
//                                    return null;
//                                }
//                            }
//                        }
//                    }

//                    // Set the offset and StartOfArgument to the position of the discovered right square bracket (or the end of the string).
//                    offset = end;
//                    StartOfArgument = offset;

//                    // Decrement the number of type arguments 
//                    numberOfTypeArguments--;
//                }

//                // Else if a comma is encountered without hitting a left square bracket, a normal type argument was encountered.
//                // I don't know if this will ever happen because these types should always be resolvable, I think.
//                else if (typeArguments[offset] == ',')
//                {
//                    if ((offset - StartOfArgument) > 0)
//                    {
//                        try
//                        {
//                            TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, offset - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
//                        }
//                        catch
//                        {
//                            return null;
//                        }
//                    }

//                    offset++;
//                    StartOfArgument = offset;
//                }

//                // Essentially adds the character at this offset to any substring produced with the StartOfArgument offset.
//                else
//                    offset++;
//            }

//            // 'offset' is out-of-bounds. 'StartOfArgument' may be out-of-bounds. 
//            // 'offset-1' should be in-bounds, and if it's greater than 'StartOfArgument', there should be one last type argument to create.
//            if ((offset - 1) > StartOfArgument)
//            {
//                try
//                {
//                    TypeList.Add(ReconstructType(typeArguments.Substring(StartOfArgument, offset - StartOfArgument).Trim(), throwOnError, referencedAssemblies));
//                }
//                catch
//                {
//                    return null;
//                }
//            }

//            // "Should never happen" --original StackOverflow author
//            // This should only happen if the number of type arguments supplied in the type string doesn't match with the number of supplied arguments.
//            // If it's less than 0, 
//            if (numberOfTypeArguments > 0)
//                return null;

//            try
//            {
//                return genericType.MakeGenericType(TypeList.ToArray());
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        private static double BytesToMegaBytes(long bytes)
//        {
//            return bytes / (1024.00 * 1024.00);
//        }
//    }







//}
