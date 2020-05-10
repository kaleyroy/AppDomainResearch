using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;

using CoreLib;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            // Enabling Resource Monitoring
            AppDomain.MonitoringIsEnabled = true;
            // Disabling Process Memory Counter
            //Application.EnabledPerformanceCounter = false;

            // 【关注点】：
            // 研究AppDomain与内存之间关联以及如何通过AppDomain来隔离运行DLL并合理释放内存资源，维持应用进程资源平衡

            // 【特别说明】：
            // 所有DLL运行结果均在AppDomain内部进行二进制编码(序列化)后返回，即所有返回结果都是字节数组对象(bytes[])
            // 外部消费返回结果前，需要进行反序列化操作转换成为常规的.NET对象 (参考：Serialize 和 Deserialize 方法)

            //Dictionary<string, object> outputs = null;
            //var inputs = new Dictionary<string, object>();

            //inputs.Add("message", "Hello Rule");
            //outputs = Application.ExecuteRule("EchoMessageRule", inputs);

            //inputs.Clear(); inputs.Add("count", 1000);
            //outputs = Application.ExecuteRule("GetUserListRule", inputs);

            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules", "textfile.csv");
            //inputs.Clear(); inputs.Add("path", path);
            //outputs = Application.ExecuteRule("ReadTextFileRule", inputs);

            // 运行观察AppDomain实例创建与时间、内存关系测试用例
            // 参数： @instanceNum 为创建的AppDomain数量
            // 描述：比如给定@instanceNum=100，则顺序依次创建100个AppDomain，并记录主进程所占内存量以及每次创建AppDomain的耗时
            //Application.AppDomainCreationMemoryCase(instanceNum: 100);

            // 运行观察AppDomain内执行DLL与时间、内存关系测试用例
            // 参数： @ruleType 为选择运行的DLL场景 , @inputs 为DLL场景的输入参数 , @taskNum 为运行次数
            // 描述：选定一个DLL场景，并设置输入参数，比如给定@taskNum=100,则顺次创建新的AppDomain并运行指定DLL场景100次，每次
            // 运行结束后释放AppDomain，并记录主进程所占内存量以及每次创建AppDomain运行DLL场景直至释放结束后的耗时
            //var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            // 场景： 比如GetUserListRule场景，会在内部随机生成1000个用户对象，并将对象进行JSON序列化后，返回JSON字符串(字节码)
            //Application.AppDomainSingleExecutionMemoryCase(
            //    ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            // 运行观察共享AppDomain内执行DLL与时间、内存关系测试用例
            // 参数： @instanceNum 为共享的AppDomain个数 ,@ruleType 为选择运行的DLL场景 ,@inputs 为DLL场景的输入参数 , @taskNum 为运行次数
            // 描述：预先创建一定数量(比如@instanceNum=3)的AppDomain实例，然后选定一个DLL场景，并设置输入参数，比如给定@taskNum=100,则【随机】
            // 从预先创建AppDomain列表当中选择一个AppDomain运行指定DLL场景，并随机【选择】->【运行】循环100次任务；运行结束后释放所有AppDomain
            // 并记录主进程所占内存量以及每次创建AppDomain运行DLL场景直至释放结束后的耗时
            //var inputs = new Dictionary<string, object>() { { "count", 1000 } };
            // 场景： 比如GetUserListRule场景，会在内部随机生成1000个用户对象，并将对象进行JSON序列化后，返回JSON字符串(字节码)
            //Application.AppDomainSharedExecutionMemoryCase(
            //    instanceNum: 3, ruleType: RuleType.GetUserListRule, inputs: inputs, taskNum: 100);

            Console.Read();
            Application.PrintMemoryUsage();
            Console.ReadLine();
        }
    }







}
